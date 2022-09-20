using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using Ragnarok.MathEx;

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
            Synchronizer.CreateControl();

            Util.SetPropertyChangedCaller(CallPropertyChanged);
            Util.SetColletionChangedCaller(CallCollectionChanged);
            Util.SetEventCaller(_ => UIProcess(_));
        }

        /// <summary>
        /// デザインモード中かどうかを判定します。
        /// </summary>
        /// <remarks>
        /// Control.DesignModeではコンストラクタで使えない or 正しく値を返さないことがある
        /// 等のバグがあるため新しい判定用関数を作っています。
        /// </remarks>
        public static bool IsDesignMode(Control control)
        {
            // コンストラクタでは正しく判定されません。
            if (control.Site?.DesignMode == true)
            {
                return true;
            }

            // .NET Frameworkでは動きますが、.NET Coreでは正しく判定されないことがあります。
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return true;
            }

            // 親コントロールにデザインモードのコントロールがあるか
            for (Control parent = control; parent != null; parent = parent.Parent)
            {
                if (parent.Site?.DesignMode == true)
                {
                    return true;
                }
            }

            // 実行ファイル名に VisualStudio が含まれていれば、デザインモードであると思われます。
            // ただのハックです。
            var asm = Assembly.GetExecutingAssembly();
            if (asm.Location.Contains("VisualStudio", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // プロセス名に devenv が含まれていればデザインモードであると思われます。
            var proc = System.Diagnostics.Process.GetCurrentProcess();
            if (string.Equals(proc.ProcessName, "devenv", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// コマンドの状態をすべて更新します。
        /// </summary>
        public static void InvalidateCommand()
        {
            UIProcess(() =>
            {
                Input.CommandManager.InvalidateRequerySuggested();
                Application.DoEvents();
            });
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
        public static void UIProcess(this Control control, Action func)
        {
            if (control?.InvokeRequired == true)
            {
                control.BeginInvoke(func);
            }
            else
            {
                func?.Invoke();
            }
        }

        /// <summary>
        /// UIThread上でメソッドを実行するようにメソッドを登録します。
        /// </summary>
        public static void UIProcess(Action func)
        {
            if (Synchronizer == null)
            {
                throw new InvalidOperationException(
                    "Ragnarok.Formsは初期化されていません。");
            }

            Synchronizer.UIProcess(func);
        }

        /// <summary>
        /// UIThread上でメソッドを実行します。
        /// </summary>
        public static void UIProcessSync(this Control control, Action func)
        {
            if (control?.InvokeRequired == true)
            {
                control.Invoke(func);
            }
            else
            {
                func?.Invoke();
            }
        }

        /// <summary>
        /// UIThread上でメソッドを実行します。
        /// </summary>
        public static void UIProcessSync(Action func)
        {
            if (Synchronizer == null)
            {
                throw new InvalidOperationException(
                    "Ragnarok.Formsは初期化されていません。");
            }

            Synchronizer.UIProcessSync(func);
        }

        /// <summary>
        /// UIThread上でメソッドを実行するようにメソッドを登録します。
        /// </summary>
        public static void UIDispatch(this Control control, Action func)
        {
            if (control != null)
            {
                control.BeginInvoke(func);
            }
            else
            {
                func?.Invoke();
            }
        }

        /// <summary>
        /// UIThread上でメソッドを実行するようにメソッドを登録します。
        /// </summary>
        public static void UIDispatch(Action func)
        {
            if (Synchronizer == null)
            {
                throw new InvalidOperationException(
                    "Ragnarok.Formsは初期化されていません。");
            }

            Synchronizer.UIDispatch(func);
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

            try
            {
                UIProcess(() => handler(sender, e));
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);

                Log.ErrorException(ex,
                    "PropertyChangedの呼び出しに失敗しました。");
            }

            // 個々のDelegate単位で呼び出すスレッドを変更します。
            /*foreach (PropertyChangedEventHandler child in
                     handler.GetInvocationList())
            {
                try
                {
                    var target = child.Target as Component;

                    // 必要があれば指定のスレッド上で実行します。
                    if (target?.InvokeRequired == true)
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
            }*/
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

            try
            {
                UIProcessSync(() => handler(sender, e));
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);

                Log.ErrorException(ex,
                    "PropertyChangedの呼び出しに失敗しました。");
            }

            // 個々のDelegate単位で呼び出すスレッドを変更します。
            /*foreach (NotifyCollectionChangedEventHandler child in
                     handler.GetInvocationList())
            {
                try
                {
                    var target = child.Target as Component;

                    // 必要があれば指定のスレッド上で実行します。
                    if (target?.InvokeRequired == true)
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
            }*/
        }

        /// <summary>
        /// 変換を行います。
        /// </summary>
        public static Pointd ToPointd(this PointF p)
        {
            return new Pointd(p.X, p.Y);
        }

        /// <summary>
        /// 変換を行います。
        /// </summary>
        public static Pointd ToPointd(this Point p)
        {
            return new Pointd(p.X, p.Y);
        }

        /// <summary>
        /// 変換を行います。
        /// </summary>
        public static PointF ToPointF(this Pointd p)
        {
            return new PointF(Convert.ToSingle(p.X), Convert.ToSingle(p.Y));
        }

        /// <summary>
        /// 変換を行います。
        /// </summary>
        public static Point ToPoint(this Pointd p)
        {
            return new Point(Convert.ToInt32(p.X), Convert.ToInt32(p.Y));
        }

        private static Cursor TransparentCursor = MakeTransparentCursor();

        /// <summary>
        /// 透明なカーソルを作成します。
        /// </summary>
        private static Cursor MakeTransparentCursor()
        {
            using (var bitmap = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.FillRectangle(Brushes.Black, g.VisibleClipBounds);
                }
                bitmap.MakeTransparent();

                // 作成した画像でCursorのインスタンスを作成
                var handle = bitmap.GetHicon();
                var icon = Icon.FromHandle(handle);
                return new Cursor(icon.Handle);
            }
        }

        /// <summary>
        /// マウスカーソルをデフォルト表示に戻します。
        /// </summary>
        public static void ShowCursor(this Form form)
        {
            if (form == null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            form.Cursor = Cursors.Default;
        }

        /// <summary>
        /// マウスカーソルに透明な画像を設定します。
        /// </summary>
        public static void HideCursor(this Form form)
        {
            if (form == null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            form.Cursor = TransparentCursor;
        }
    }
}
