using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Ragnarok;
using Ragnarok.Net.CookieGetter;
using Ragnarok.NicoNico;
using Ragnarok.NicoNico.Login;
using Ragnarok.NicoNico.Live;
using Ragnarok.ObjectModel;
using Ragnarok.Utility;
using Ragnarok.Presentation;
using Ragnarok.Presentation.NicoNico;

namespace CommentGetter
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window, IModel
    {
        private readonly static DateTime StartTime = new DateTime(2014, 2, 26, 23, 0, 0);
        private readonly static DateTime EndTime = new DateTime(2014, 2, 26, 16, 45, 0);

        private LoginData loginData = new LoginData();
        private NicoClient nicoClient = new NicoClient();
        private CommentClient commentClient = new CommentClient();
        private string liveUrl;

        private List<List<Comment>> commentRoomList = new List<List<Comment>>();
        private DateTime lastCommentTime = DateTime.Now;
        private int commentCount = 0;
        private Timer timer;
        private ReentrancyLock timerLock = new ReentrancyLock();

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged(this, e);
        }

        public MainWindow()
        {
            InitializeComponent();

            /*System.Diagnostics.Trace.Listeners.Add(
                new System.Diagnostics.ConsoleTraceListener());*/

            this.commentClient.CommentReceived += (sender, e) =>
                Console.Write(".");
            this.commentClient.CommentReceived +=
                commentClient_CommentReceived;

            this.commentClient.ConnectedRoom += (sender, e) =>
                Log.Info("Connected to {0}.", e.RoomIndex);
            this.commentClient.DisconnectedRoom += (sender, e) =>
                Log.Info("Disconnected from {0}.", e.RoomIndex);

            var loginData = new LoginData()
            {
                LoginMethod = LoginMethod.WithBrowser,
                BrowserType = BrowserType.GoogleChrome,
            };
            this.nicoClient.LoginAsync(loginData);

            DataContext = this;

            this.timer = new Timer(TimeCallback, null, 1000, 1000);
        }

        /// <summary>
        /// クライアントオブジェクトを取得または設定します。
        /// </summary>
        public NicoClient NicoClient
        {
            get
            {
                return this.nicoClient;
            }
        }

        /// <summary>
        /// ログインデータを取得または設定します。
        /// </summary>
        public LoginData LoginData
        {
            get { return this.loginData; }
            set
            {
                if (this.loginData != value)
                {
                    this.loginData = value;

                    this.RaisePropertyChanged("LoginData");
                }
            }
        }

        /// <summary>
        /// 放送URLを取得または設定します。
        /// </summary>
        public string LiveUrl
        {
            get { return this.liveUrl; }
            set
            {
                if (this.liveUrl != value)
                {
                    this.liveUrl = value;

                    this.RaisePropertyChanged("LiveUrl");
                }
            }
        }

        /// <summary>
        /// ログイン処理
        /// </summary>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new LoginWindow(this.nicoClient);

                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message + "\n" +
                    ex.StackTrace,
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 接続処理
        /// </summary>
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!this.nicoClient.IsLogin)
                {
                    MessageBox.Show(
                        "ログインしていません。",
                        "エラー",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                this.commentClient.Connect(
                    LiveUrl,
                    this.nicoClient.CookieContainer);
            }
            catch (Exception ex)
            {
                DialogUtil.ShowError(ex,
                    "放送への接続に失敗しました。");
                return;
            }
        }

        /*private long CompareComment(Comment x, Comment y)
        {
            var ticksX = x.Date.Ticks;
            var ticksY = y.Date.Ticks;

            if (ticksX != ticksY)
            {
                return (ticksX - ticksY);
            }

            return (x.Date.Millisecond - y.Date.Millisecond);
        }*/

        /// <summary>
        /// コメント受信時のコールバック
        /// </summary>
        void commentClient_CommentReceived(object sender, CommentRoomReceivedEventArgs e)
        {
            var index = e.RoomIndex;

            lock (this.commentRoomList)
            {
                if (index >= this.commentRoomList.Count())
                {
                    for (var i = this.commentRoomList.Count(); i <= index; ++i)
                    {
                        this.commentRoomList.Add(new List<Comment>());
                    }
                }
            }

            // 挿入ソートでコメントを時系列順に並べます。
            var commentList = this.commentRoomList[index];
            var j = 0;
            for (; j < commentList.Count(); ++j)
            {
                if (commentList[j].Date >= e.Comment.Date) break;
            }

            for (var k = j; k < commentList.Count(); ++k)
            {
                if (commentList[k].Date > e.Comment.Date) break;
                if (e.Comment.Text == commentList[k].Text) return;
            }

            commentList.Insert(j, e.Comment);
            this.lastCommentTime = DateTime.Now;
            this.commentCount += 1;
        }

        void TimeCallback(object state)
        {
            try
            {
                using (var result = this.timerLock.Lock())
                {
                    if (result == null) return;

                    if (!this.nicoClient.IsLogin || string.IsNullOrEmpty(LiveUrl))
                    {
                        return;
                    }

                    var now = DateTime.Now;
                    if (now < this.lastCommentTime + TimeSpan.FromSeconds(3))
                    {
                        return;
                    }

                    var when = GetCommentStartTime();
                    if (when != StartTime && this.commentCount == 0)
                    {
                        if (this.timer != null)
                        {
                            this.timer.Dispose();
                            this.timer = null;
                        }

                        SaveComment();
                        this.commentClient.Disconnect();
                        return;
                    }

                    Log.Info("Connect {0}.", when);

                    this.lastCommentTime = DateTime.Now;
                    this.commentCount = 0;

                    this.commentClient.Connect(
                        LiveUrl, NicoClient.CookieContainer);
                    this.commentClient.StartReceiveMessage(
                        1000, 10 * 1000, when);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "タイマーコールバックで例外が発生しました。");

                LiveUrl = null;
                this.commentClient.Disconnect();
            }
        }

        private DateTime GetCommentStartTime()
        {
            lock (this.commentRoomList)
            {
                if (!this.commentRoomList.Any())
                {
                    return StartTime;
                }

                var lastDate = this.commentRoomList
                    .Where(_ => _.Any())
                    .Max(_ => _.First().Date);

                return (lastDate - TimeSpan.FromMinutes(1));
            }
        }

        private void SaveComment()
        {
            for (var i = 0; i < this.commentRoomList.Count(); ++i)
            {
                var commentList = this.commentRoomList[i];
                var room = this.commentClient.GetRoomInfo(i);
                var roomLabel = (room != null ? room.RoomLabel : string.Empty);
                var filename = string.Format("comment_{0}_{1}.txt", i, roomLabel);

                using (var stream = new FileStream(filename, FileMode.Create))
                using (var writer = new StreamWriter(stream))
                {
                    commentList.ForEach(_ =>
                        writer.WriteLine(_.OriginalXml));
                }
            }
        }
    }
}
