using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

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
                throw new ArgumentNullException("control");
            }

            var screen = Screen.FromHandle(control.Handle);
            var bounds = screen.Bounds;

            control.Left = Math.Max(control.Left, bounds.Left);
            var right = control.Left + control.Width;
            control.Left = Math.Min(right, bounds.Right) - control.Width;

            control.Top = Math.Max(control.Top, bounds.Top);
            var bottom = control.Top + control.Height;
            control.Top = Math.Min(bottom, bounds.Bottom) - control.Height;
        }

        /// <summary>
        /// マウス位置を中心にダイアログを開きます。
        /// </summary>
        public static DialogResult ShowDialogCenterMouse(this Form dialog)
        {
            if (dialog == null)
            {
                throw new ArgumentNullException("dialog");
            }

            var screenPos = Cursor.Position;

            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Left = screenPos.X - (dialog.Width / 2);
            dialog.Top = screenPos.Y - (dialog.Height / 2);
            dialog.AdjustInDisplay();

            return dialog.ShowDialog();
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
            return MessageBox.Show(owner, message, title, buttons);
        }
        
        /// <summary>
        /// 一般的なダイアログ表示用メソッドです。
        /// </summary>
        public static DialogResult Show(string message, string title,
                                        MessageBoxButtons buttons)
        {
            return MessageBox.Show(message, title, buttons);
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
                "{1}{0}{0} 詳細: {2}",
                Environment.NewLine,
                message, ex.Message);

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
            var dialog = new FontDialog()
            {
                Font = defaultInfo,
            };

            var result = dialog.ShowDialog(owner);
            if (result != DialogResult.OK)
            {
                return null;
            }

            return dialog.Font;
        }
        #endregion

        #region 色選択
        /// <summary>
        /// 色選択ダイアログを表示します。
        /// </summary>
        public static Color? ShowColorDialog(Color? defaultColor, IWin32Window owner = null)
        {
            var dialog = new ColorDialog();

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
        #endregion
    }
}
