using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace Ragnarok.Forms
{
    /// <summary>
    /// WinForms関係の便利メソッドを定義します。
    /// </summary>
    public static class FormsUtil
    {
        /// <summary>
        /// UIProcessで動作させるためのコントロールです。
        /// </summary>
        public static Control Synchronizer 
        {
            get;
            private set;
        }

        /// <summary>
        /// WPFを使うための初期化処理を行います。
        /// </summary>
        public static void Initialize()
        {
            Initializer.Initialize();

            Synchronizer = new Control();

            Util.SetPropertyChangedCaller(CallPropertyChanged);
            Util.SetColletionChangedCaller(CallCollectionChanged);
            Util.SetEventCaller(UIProcess);
        }

        /// <summary>
        /// コマンドの状態をすべて更新します。
        /// </summary>
        public static void InvalidateCommand()
        {
            UIProcess(() =>
                Input.CommandManager.InvalidateRequerySuggested());
        }

        /// <summary>
        /// UIProcess時に別スレッドでの実行が必要になるか調べます。
        /// </summary>
        public static bool InvokeRequired()
        {
            if (Synchronizer == null)
            {
                throw new InvalidOperationException(
                    "Ragnarok.Formsは初期化されていません。");
            }

            return Synchronizer.InvokeRequired;
        }

        /// <summary>
        /// UIThread上でメソッドを実行します。
        /// </summary>
        public static void UIProcess(Action func)
        {
            if (Synchronizer.InvokeRequired)
            {
                Synchronizer.BeginInvoke(func);
            }
            else
            {
                func();
            }
        }

        /// <summary>
        /// UIThread上でメソッドを実行します。
        /// </summary>
        public static void UIProcess(this Control control, Action func)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(func);
            }
            else
            {
                func();
            }
        }

        /// <summary>
        /// 必要ならGUIスレッド上でPropertyChangedを呼び出します。
        /// </summary>
        public static void CallPropertyChanged(PropertyChangedEventHandler handler,
                                               object sender,
                                               PropertyChangedEventArgs e)
        {
            if (handler == null)
            {
                return;
            }

            // 個々のDelegate単位で呼び出すスレッドを変更します。
            foreach (PropertyChangedEventHandler child in
                     handler.GetInvocationList())
            {
                var target = child.Target as Control;

                try
                {
                    // 必要があれば指定のスレッド上で実行します。
                    if (target != null && target.InvokeRequired)
                    {
                        target.BeginInvoke(child, sender, e);
                    }
                    else
                    {
                        child(sender, e);
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorException(ex,
                        "PropertyChangedの呼び出しに失敗しました。");
                }
            }
        }

        /// <summary>
        /// 必要ならGUIスレッド上でCollectionChangedを呼び出します。
        /// </summary>
        public static void CallCollectionChanged(NotifyCollectionChangedEventHandler handler,
                                                 object sender,
                                                 NotifyCollectionChangedEventArgs e)
        {
            if (handler == null)
            {
                return;
            }

            // 個々のDelegate単位で呼び出すスレッドを変更します。
            foreach (NotifyCollectionChangedEventHandler child in
                     handler.GetInvocationList())
            {
                var target = child.Target as Control;

                try
                {
                    // 必要があれば指定のスレッド上で実行します。
                    if (target != null && target.InvokeRequired)
                    {
                        // コレクションの状態が変わる前に変更通知を出す
                        // 必要があるため、Invokeを使っています。
                        target.Invoke(child, sender, e);
                    }
                    else
                    {
                        child(sender, e);
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorException(ex,
                        "CollectionChangedの呼び出しに失敗しました。");
                }
            }
        }
    }
}
