using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ragnarok.Presentation.Control
{
    /// <summary>
    /// ProgressDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ProgressDialog : Window
    {
        /// <summary>
        /// キャンセル時に呼ばれるイベントを定義します。
        /// </summary>
        public static RoutedEvent CancelEvent =
            EventManager.RegisterRoutedEvent(
                "Cancel", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ProgressDialog));

        /// <summary>
        /// 操作完了時に呼ばれるイベントを定義します。
        /// </summary>
        public static RoutedEvent CompletedEvent =
            EventManager.RegisterRoutedEvent(
                "Completed", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ProgressDialog));

        /// <summary>
        /// 操作がキャンセルされたかどうかを示す依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty IsCanceledProperty =
            DependencyProperty.Register(
                "IsCanceled",
                typeof(bool),
                typeof(ProgressDialog),
                new PropertyMetadata(false));

        /// <summary>
        /// 操作が失敗したかどうかを示す依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty IsFaultedProperty =
            DependencyProperty.Register(
                "IsFaulted",
                typeof(bool),
                typeof(ProgressDialog),
                new PropertyMetadata(false));

        /// <summary>
        /// 操作が失敗したときの例外を示す依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ExceptionProperty =
            DependencyProperty.Register(
                "Exception",
                typeof(Exception),
                typeof(ProgressDialog),
                new PropertyMetadata((Exception)null));

        /// <summary>
        /// キャンセル時に呼ばれるイベントを追加または削除します。
        /// </summary>
        public event RoutedEventHandler Cancel
        {
            add
            {
                AddHandler(CancelEvent, value);
            }
            remove
            {
                RemoveHandler(CancelEvent, value);
            }
        }

        /// <summary>
        /// 操作完了時に呼ばれるイベントを追加または削除します。
        /// </summary>
        public event RoutedEventHandler Completed
        {
            add
            {
                AddHandler(CompletedEvent, value);
            }
            remove
            {
                RemoveHandler(CompletedEvent, value);
            }
        }

        /// <summary>
        /// 操作がキャンセルされたかどうかを取得します。
        /// </summary>
        public bool IsCanceled
        {
            get
            {
                return (bool)GetValue(IsCanceledProperty);
            }
            private set
            {
                SetValue(IsCanceledProperty, value);
            }
        }

        /// <summary>
        /// 操作が失敗したかどうかを取得します。
        /// </summary>
        public bool IsFaulted
        {
            get
            {
                return (bool)GetValue(IsFaultedProperty);
            }
            private set
            {
                SetValue(IsFaultedProperty, value);
            }
        }

        /// <summary>
        /// 失敗時の例外を取得します。
        /// </summary>
        public Exception Exception
        {
            get
            {
                return (Exception)GetValue(ExceptionProperty);
            }
            private set
            {
                SetValue(ExceptionProperty, value);
            }
        }

        /// <summary>
        /// キャンセルイベントを発火します。
        /// </summary>
        private void RaiseCancelEvent()
        {
            var args = new RoutedEventArgs(CancelEvent);

            Util.SafeCall(() =>
                RaiseEvent(args));
        }

        /// <summary>
        /// 完了時のイベントを発火します。
        /// </summary>
        private void RaiseCompletedEvent()
        {
            var args = new RoutedEventArgs(CompletedEvent);

            Util.SafeCall(() =>
                RaiseEvent(args));
        }

        private readonly Action action;
        private readonly CancellationTokenSource cancelTokenSource;
        private Task task;

        /// <summary>
        /// キャンセルボタンクリック時に呼ばれます。
        /// </summary>
        private void DoCancel(bool close)
        {
            if (!this.task.IsCompleted && !this.task.IsCanceled)
            {
                this.cancelTokenSource.Cancel();
                IsCanceled = true;

                RaiseCancelEvent();

                // ダイアログは自動で閉じます。
                if (close)
                {
                    Close();
                }
            }
        }

        /// <summary>
        /// 操作完了時に呼ばれます。
        /// </summary>
        private void TaskCompleted(Task state)
        {
            WPFUtil.UIProcess(
                () =>
                {
                    if (this.task.Exception != null)
                    {
                        IsFaulted = true;
                        Exception = this.task.Exception;

                        Log.ErrorException(
                            Exception,
                            "Taskが途中で失敗しました。");
                    }

                    RaiseCompletedEvent();

                    // ダイアログは自動で閉じます。
                    Close();
                });
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ProgressDialog(Action action)
        {
            InitializeComponent();

            Loaded += ProgressDialog_Loaded;
            Closed += ProgressDialog_Closed;

            this.action = action;
            this.cancelTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// キャンセルボタンクリック時に呼ばれます。
        /// </summary>
        private void CancelBUtton_Click(object sender, RoutedEventArgs e)
        {
            DoCancel(true);
        }

        void ProgressDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.task = Task.Factory.StartNew(
                this.action,
                this.cancelTokenSource.Token);

            this.task.ContinueWith(TaskCompleted);
        }

        void ProgressDialog_Closed(object sender, EventArgs e)
        {
            DoCancel(false);
        }
    }
}
