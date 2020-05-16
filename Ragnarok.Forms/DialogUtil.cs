using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;

namespace Ragnarok.Forms
{
    /// <summary>
    /// 確認メッセージなどのダイアログを作成します。
    /// </summary>
    public static class DialogUtil
    {
        /// <summary>
        /// ウィンドウが画面からはみ出している場合、画面内に収まるように調節します。
        /// </summary>
        public static void AdjustInDisplay(this Control control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            // control.LocationはScreen.FromHandle後に変わってしまう
            // ことがあるので、先に値だけ取得しておきます。
            var pos = control.Location;
            var bounds = Screen.GetBounds(control);

            var left = Math.Max(pos.X, bounds.Left);
            var right = left + control.Width;
            control.Left = Math.Min(right, bounds.Right) - control.Width;

            var top = Math.Max(pos.Y, bounds.Top);
            var bottom = top + control.Height;
            control.Top = Math.Min(bottom, bounds.Bottom) - control.Height;
        }

        /// <summary>
        /// マウス位置を中心にダイアログを開きます。
        /// </summary>
        public static void SetCenterMouse(this Form form)
        {
            if (form == null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            var screenPos = Cursor.Position;

            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(
                screenPos.X - (form.Width / 2),
                screenPos.Y - (form.Height / 2));
            form.AdjustInDisplay();
        }

        /// <summary>
        /// 親ウィンドウの中心にダイアログを開きます。
        /// </summary>
        public static void SetCenterParent(this Form form)
        {
            if (form == null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            form.StartPosition = FormStartPosition.Manual;

            // フォームの親はShowするときに設定されるため、
            // 表示後の親を基準に値を設定する。
            form.Load += (_, e) =>
            {
                var parent = form.Owner;
                form.Location = new Point(
                    parent.Location.X + (parent.Width - form.Width) / 2,
                    parent.Location.Y + (parent.Height - form.Height) / 2);
                form.AdjustInDisplay();
            };
        }

        /// <summary>
        /// 一般的なダイアログ表示用メソッドです。
        /// </summary>
        public static DialogResult Show(IWin32Window owner, string message,
                                        string title,
                                        MessageBoxButtons buttons,
                                        MessageBoxDefaultButton defaultButton)
        {
            return MessageBox.Show(
                owner, message, title, buttons,
                MessageBoxIcon.None, defaultButton);
        }

        /// <summary>
        /// 一般的なダイアログ表示用メソッドです。
        /// </summary>
        public static DialogResult Show(IWin32Window owner, string message,
                                        string title,
                                        MessageBoxButtons buttons)
        {
            return MessageBox.Show(
                owner, message, title, buttons);
        }
        
        /// <summary>
        /// 一般的なダイアログ表示用メソッドです。
        /// </summary>
        public static DialogResult Show(string message, string title,
                                        MessageBoxButtons buttons)
        {
            return MessageBox.Show(
                message, title, buttons);
        }

        /// <summary>
        /// 一般的なダイアログ表示用メソッドです。
        /// </summary>
        public static DialogResult Show(string message)
        {
            return Show(message, "メッセージ", MessageBoxButtons.OK);
        }

        /// <summary>
        /// エラーメッセージの表示用メソッドです。
        /// </summary>
        public static void ShowError(IWin32Window owner, Exception ex,
                                     string message)
        {
            var text = string.Format(
                CultureInfo.CurrentCulture,
                "{1}{0}{0} 詳細: {2}",
                Environment.NewLine,
                message, ex?.Message);

            // ログにも出力します。
            Log.ErrorException(ex, message);

            MessageBox.Show(
                owner, text, "エラー",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// エラーメッセージの表示用メソッドです。
        /// </summary>
        public static void ShowError(IWin32Window owner, string message)
        {
            // ログにも出力します。
            Log.Error(message);

            MessageBox.Show(
                owner, message, "エラー",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// エラーメッセージの表示用メソッドです。
        /// </summary>
        public static void ShowError(Exception ex, string message)
        {
            ShowError(null, ex, message);
        }

        /// <summary>
        /// エラーメッセージの表示用メソッドです。
        /// </summary>
        public static void ShowError(string message)
        {
            ShowError((IWin32Window)null, message);
        }

        #region フォント
        /// <summary>
        /// フォントダイアログを開き、フォントの設定を行います。
        /// </summary>
        public static Font ShowFontDialog(Font defaultInfo, IWin32Window owner = null)
        {
            using (var dialog = new FontDialog()
            {
                Font = defaultInfo,
            })
            {
                var result = dialog.ShowDialog(owner);
                if (result != DialogResult.OK)
                {
                    return null;
                }

                return dialog.Font;
            }
        }
        #endregion

        #region 色選択
        /// <summary>
        /// 色選択ダイアログを表示します。
        /// </summary>
        public static Color? ShowColorDialog(Color? defaultColor, IWin32Window owner = null)
        {
            using (var dialog = new ColorDialog())
            {
                // 必要ならデフォルト色を設定します。
                if (defaultColor != null)
                {
                    dialog.Color = defaultColor.Value;
                }

                // OKボタンが押されたら、その色を返します。
                var result = dialog.ShowDialog(owner);
                if (result != DialogResult.OK)
                {
                    return null;
                }

                return dialog.Color;
            }
        }
        #endregion
    }
}
