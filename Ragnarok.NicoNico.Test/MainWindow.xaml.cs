﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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

using Ragnarok.CookieGetter;
using Ragnarok.NicoNico;
using Ragnarok.ObjectModel;
using Ragnarok.Presentation;
using Ragnarok.Presentation.NicoNico;

namespace Ragnarok.NicoNico.Test
{
    using Login;

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window, IModel
    {
        private LoginData loginData = new LoginData();
        private string liveUrl;
        private string comment;
        private NicoClient nicoClient = new NicoClient();
        private Live.CommentClient commentClient = new Live.CommentClient();

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged(this, e);
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
        /// コメント文字列を取得または設定します。
        /// </summary>
        public string Comment
        {
            get { return this.comment; }
            set
            {
                if (this.comment != value)
                {
                    this.comment = value;

                    this.RaisePropertyChanged("Comment");
                }
            }
        }

        public NicoClient NicoClient
        {
            get
            {
                return this.nicoClient;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            /*System.Diagnostics.Trace.Listeners.Add(
                new System.Diagnostics.ConsoleTraceListener());*/

            this.nicoClient.PropertyChanged +=
                (sender, e) => NotifyPropertyChanged(e);

            this.commentClient.CommentReceived += (sender, e) =>
                Console.WriteLine(e.Comment.OriginalXml);

            this.commentClient.ConnectedRoom += (sender, e) =>
                Console.WriteLine("Connected to {0} ({1}).", e.RoomLabel, e.RoomIndex);
            this.commentClient.Disconnected += (sender, e) =>
                Console.WriteLine("Disconnected");

            var loginData = new LoginData()
            {
                LoginMethod = Login.LoginMethod.WithBrowser,
                BrowserType = BrowserType.GoogleChrome,
            };
            this.nicoClient.LoginAsync(loginData);
            this.layoutBase.DataContext = this;
        }

        /// <summary>
        /// ログイン処理を行います。
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
                return;
            }
        }

        /// <summary>
        /// 放送に接続します。
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

                /*Live.LiveInfo.Create(
                    "lv203771397",
                    this.nicoClient.CookieContainer);*/

                this.commentClient.Connect(
                    LiveUrl,
                    this.nicoClient.CookieContainer);
                this.commentClient.StartReceiveMessage(1000);
            }
            catch (Exception ex)
            {
                DialogUtil.ShowError(ex,
                    "放送への接続に失敗しました。");
                return;
            }

            DialogUtil.Show(
                "接続に成功しました。",
                "ＯＫ",
                MessageBoxButton.OK);
        }

        /// <summary>
        /// コメントを投稿します。
        /// </summary>
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            this.commentClient.BroadcastComment(
                Comment,
                "184",
                DateTime.Now);

            Comment = string.Empty;
        }

        private void CommentTestButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.commentClient.IsConnected)
            {
                MessageBox.Show("放送に接続していません。");
                return;
            }

            this.commentClient.CommentSent += commentClient_CommentSent;

            /*for (var i = 0; i < 200; ++i)
            {
                this.commentClient.BroadcastComment(
                    "テスト(^o^)d",
                    "184 big",
                    start);
            }*/

            /*for (var i = 0; i < 200; ++i)
            {
                this.commentClient.SendOwnerComment(
                    "11金",
                    "");

                this.commentClient.SendOwnerComment(
                    "55玉",
                    "");

                System.Threading.Thread.Sleep(1000);
            }*/

            commentClient_CommentSent(this.commentClient, null);
        }

        void commentClient_CommentSent(object sender, Live.CommentRoomSentEventArgs e)
        {
            var client = (Live.CommentClient)sender;

            if (client.LeaveCommentCount == 0)
            {
                /*this.commentClient.BroadcastComment(
                    "テスト(^o^)d",
                    "184",
                    DateTime.Now);*/
            }
        }

        private void StreamInfoTestButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var times = 10;

                var start = DateTime.Now;
                for (var i = 0; i < times; ++i)
                {
                    if (Live.LiveUtil.GetLiveStreamInfo(
                            LiveUrl,
                            this.nicoClient.CookieContainer) == null)
                    {
                        MessageBox.Show("非同期取得テストに失敗しました。");
                    }
                }
                var ellapsedAsync = DateTime.Now - start;

                start = DateTime.Now;
                for (var i = 0; i < times; ++i)
                {
                    if (Live.LiveUtil.GetLiveStreamInfoSync(
                        LiveUrl,
                        this.nicoClient.CookieContainer) == null)
                    {
                        MessageBox.Show("同期取得テストに失敗しました。");
                    }
                }
                var ellapsedSync = DateTime.Now - start;

                MessageBox.Show(
                    "非同期取得: " + ellapsedAsync.TotalMilliseconds + "[ms]\n" +
                    "　同期取得: " + ellapsedSync.TotalMilliseconds + "[ms]");
            }
            catch (Exception)
            {
                //Wpf.MessageUtil.ErrorMessage(ex, "");
            }
        }
    }
}
