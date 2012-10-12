using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Runtime.InteropServices;

namespace Ragnarok.Presentation
{
    /// <summary>
    /// ユーティリティクラスです。
    /// </summary>
    public static class WpfUtil
    {
        #region PInvoke
        [StructLayout(LayoutKind.Sequential)]
        private struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(ref Win32Point pt);
        #endregion

        /// <summary>
        /// デザインモードかどうかを取得します。
        /// (running in Blend or Visual Studio).
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
                var prop = DesignerProperties.IsInDesignModeProperty;
                var desc = DependencyPropertyDescriptor.FromProperty(
                    prop, typeof(FrameworkElement));

                return (bool)desc.Metadata.DefaultValue;
            }
        }

        /// <summary>
        /// UIスレッドに関連づけられたディスパッチャーを取得します。
        /// </summary>
        public static Dispatcher UIDispatcher
        {
            get
            {
                if (Application.Current == null)
                {
                    return null;
                }

                return Application.Current.Dispatcher;
            }
        }

        /// <summary>
        /// 与えられた手続きをUIスレッド上で実行します。
        /// </summary>
        public static void UIProcess(Action func)
        {
            var dispatcher = WpfUtil.UIDispatcher;

            if (dispatcher == null || dispatcher.CheckAccess())
            {
                func();
            }
            else
            {
                dispatcher.BeginInvoke(func);
            }
        }

        /// <summary>
        /// コマンドバインダをすべて更新します。
        /// </summary>
        public static void InvalidateCommand()
        {
            UIProcess(CommandManager.InvalidateRequerySuggested);
        }

        /// <summary>
        /// Mouse.GetPositionにはバグがあるので、P/Invokeでマウス座標を取ります。
        /// </summary>
        /// <remarks>
        /// マウスのDragAndDrop中やキャプチャ中に座標を取ろうとすると
        /// 間違った値を返してきます。
        /// 
        /// 参考: http://www.switchonthecode.com/tutorials/wpf-snippet-reliably-getting-the-mouse-position
        /// </remarks>
        public static Point GetMousePosition(Visual relativeTo)
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return relativeTo.PointFromScreen(new Point(w32Mouse.X, w32Mouse.Y));
        }

        /// <summary>
        /// 透明度だけを変えた色を作成します。
        /// </summary>
        public static Color MakeColor(byte a, Color baseColor)
        {
            return Color.FromArgb(
                a,
                baseColor.R,
                baseColor.G,
                baseColor.B);
        }

        /// <summary>
        /// コントロールから指定の型の子要素を検索します。
        /// </summary>
        public static TChild FindVisualChild<TChild>(this DependencyObject parent)
            where TChild : DependencyObject
        {
            if ((object)parent == null)
            {
                return null;
            }

            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < count; ++i)
            {
                var childDep = VisualTreeHelper.GetChild(parent, i);
                var child = childDep as TChild;

                if ((object)child != null)
                {
                    return child;
                }
                else
                {
                    // 子コントロールのさらに子コントロールを検索します。
                    var childOfChild = FindVisualChild<TChild>(childDep);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }

            return null;
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
                var target = child.Target as DispatcherObject;

                try
                {
                    // 必要があれば指定のスレッド上で実行します。
                    if (target != null && !target.Dispatcher.CheckAccess())
                    {
                        target.Dispatcher.BeginInvoke(
                            child,
                            sender, e);
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
                var target = child.Target as DispatcherObject;

                try
                {
                    // 必要があれば指定のスレッド上で実行します。
                    if (target != null && !target.Dispatcher.CheckAccess())
                    {
                        // コレクションの状態が変わる前に変更通知を出す
                        // 必要があるため、Invokeを使っています。
                        target.Dispatcher.Invoke(
                            child,
                            sender, e);
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
