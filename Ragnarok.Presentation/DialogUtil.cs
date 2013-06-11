using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using Ragnarok;
using Ragnarok.Presentation.Control;

namespace Ragnarok.Presentation
{
    using Drawing = System.Drawing;
    using Forms = System.Windows.Forms;

    /// <summary>
    /// フォントにまつわる情報をまとめて保持します。
    /// </summary>
    public class FontInfo
    {
        /// <summary>
        /// フォントファミリを取得または設定します。
        /// </summary>
        public FontFamily Family;

        /// <summary>
        /// フォントサイズを取得または設定します。
        /// </summary>
        public double Size;

        /// <summary>
        /// フォントスタイルを取得または設定します。
        /// </summary>
        public FontStyle Style;

        /// <summary>
        /// フォントの太さを取得または設定します。
        /// </summary>
        public FontWeight Weight;
    }

    /// <summary>
    /// 確認メッセージなどのダイアログを作成します。
    /// </summary>
    public static class DialogUtil
    {
        /// <summary>
        /// 'はい'のみを出すダイアログを作成します。
        /// </summary>
        private static GenericDialog CreateOKDialog()
        {
            return new GenericDialog()
            {
                Title = "確認ダイアログ",
                Message = "",
                Button1Text = null,
                Button1Kind = MessageBoxResult.None,
                Button2Text = "OK",
                Button2Kind = MessageBoxResult.OK,
            };
        }

        /// <summary>
        /// 'はい/キャンセル'を出すダイアログを作成します。
        /// </summary>
        private static GenericDialog CreateOKCancelDialog()
        {
            return new GenericDialog()
            {
                Title = "確認ダイアログ",
                Message = "",
                Button1Text = "OK",
                Button1Kind = MessageBoxResult.OK,
                Button2Text = "キャンセル",
                Button2Kind = MessageBoxResult.Cancel,
            };
        }
        
        /// <summary>
        /// はい/いいえを出すダイアログを作成します。
        /// </summary>
        private static GenericDialog CreateYesNoDialog()
        {
            return new GenericDialog()
            {
                Title = "質問ダイアログ",
                Message = "",
                Button1Text = "はい",
                Button1Kind = MessageBoxResult.Yes,
                Button2Text = "いいえ",
                Button2Kind = MessageBoxResult.No,
            };
        }

        /// <summary>
        /// ボタン種別からダイアログを作成します。
        /// </summary>
        public static GenericDialog CreateDialog(MessageBoxButton button)
        {
            if (button == MessageBoxButton.OK)
            {
                return CreateOKDialog();
            }
            else if (button == MessageBoxButton.OKCancel)
            {
                return CreateOKCancelDialog();
            }
            else if (button == MessageBoxButton.YesNo)
            {
                return CreateYesNoDialog();
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// 一般的なダイアログ表示用メソッドです。
        /// </summary>
        public static GenericDialog CreateDialog(Window owner,
                                                 string message, string title,
                                                 MessageBoxButton button)
        {
            return CreateDialog(
                owner, message, title, button,
                MessageBoxResult.No);
        }

        /// <summary>
        /// 一般的なダイアログ表示用メソッドです。
        /// </summary>
        public static GenericDialog CreateDialog(Window owner,
                                                 string message, string title,
                                                 MessageBoxButton button,
                                                 MessageBoxResult focused)
        {
            var dialog = CreateDialog(button);

            dialog.Message = message;
            dialog.Title = title;

            if (owner != null)
            {
                dialog.Owner = owner;
                dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            // 必要なら表示時にフォーカスのあるボタンを設定します。
            if (focused == MessageBoxResult.None)
            {
                dialog.SetFocusButtonNum(1);
            }
            else
            {
                dialog.SetFocusButton(focused);
            }

            return dialog;
        }

        /// <summary>
        /// 一般的なダイアログ表示用メソッドです。
        /// </summary>
        public static GenericDialog CreateDialog(string message,
                                                 string title,
                                                 MessageBoxButton button)
        {
            return CreateDialog(null, message, title, button, MessageBoxResult.None);
        }

        /// <summary>
        /// ウィンドウが画面からはみ出している場合、画面内に収まるように調節します。
        /// </summary>
        public static void AdjustInDisplay(this Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            var wih = new WindowInteropHelper(window);
            var screen = Forms.Screen.FromHandle(wih.Handle);
            var bounds = screen.Bounds;

            window.Left = Math.Max(window.Left, bounds.Left);
            var right = window.Left + window.Width;
            window.Left = Math.Min(right, bounds.Right) - window.Width;

            window.Top = Math.Max(window.Top, bounds.Top);
            var bottom = window.Top + window.Height;
            window.Top = Math.Min(bottom, bounds.Bottom) - window.Height;
        }

        /// <summary>
        /// マウス位置を中心にダイアログを開きます。
        /// </summary>
        public static bool? ShowDialogCenterMouse(this Window dialog)
        {
            if (dialog == null)
            {
                throw new ArgumentNullException("dialog");
            }

            // WPFではコントロールのサイズは実際にそれを開いた後に決まります。
            dialog.Loaded += (sender, e) =>
            {
                var p = WPFUtil.GetMousePosition(dialog);
                var screenPos = dialog.PointToScreen(p);

                dialog.WindowStartupLocation = WindowStartupLocation.Manual;
                dialog.Left = screenPos.X - (dialog.ActualWidth / 2);
                dialog.Top = screenPos.Y - (dialog.ActualHeight / 2);
                dialog.AdjustInDisplay();
            };

            return dialog.ShowDialog();
        }

        /// <summary>
        /// 一般的なダイアログ表示用メソッドです。
        /// </summary>
        public static MessageBoxResult Show(Window owner, string message, string title,
                                            MessageBoxButton button, MessageBoxResult focused)
        {
            var dialog = CreateDialog(owner, message, title, button, focused);

            dialog.ShowDialog();
            return dialog.ResultButton;
        }

        /// <summary>
        /// 一般的なダイアログ表示用メソッドです。
        /// </summary>
        public static MessageBoxResult Show(Window owner, string message, string title,
                                            MessageBoxButton button)
        {
            var dialog = CreateDialog(owner, message, title, button, MessageBoxResult.None);

            dialog.ShowDialog();
            return dialog.ResultButton;
        }
        
        /// <summary>
        /// 一般的なダイアログ表示用メソッドです。
        /// </summary>
        public static MessageBoxResult Show(string message, string title,
                                            MessageBoxButton button)
        {
            var dialog = CreateDialog(message, title, button);

            dialog.ShowDialog();
            return dialog.ResultButton;
        }

        /// <summary>
        /// エラーメッセージの表示用メソッドです。
        /// </summary>
        private static void ShowErrorInternal(string message)
        {
            var dialog = CreateDialog(
                message, "エラー", MessageBoxButton.OK);

            dialog.Topmost = true;
            dialog.ShowDialogCenterMouse();
        }

        /// <summary>
        /// エラーメッセージの表示用メソッドです。
        /// </summary>
        public static void ShowError(string message)
        {
            // ログにも出力します。
            Log.Error(message);

            ShowErrorInternal(message);
        }

        /// <summary>
        /// エラーメッセージの表示用メソッドです。
        /// </summary>
        public static void ShowError(Exception ex, string message)
        {
            var text = string.Format(
                "{1}{0}{0} 詳細: {2}",
                Environment.NewLine,
                message, ex.Message);

            // ログにも出力します。
            Log.ErrorException(ex, message);

            ShowErrorInternal(text);
        }

        #region TimeSpan
        /// <summary>
        /// ダイアログから時間間隔を取得します。
        /// </summary>
        public static TimeSpan? ShowTimeSpanDialog(TimeSpan defaultValue)
        {
            // 時間間隔をウィンドウから取得します。
            var window = new TimeSpanWindow(defaultValue);
            var result = window.ShowDialogCenterMouse();
            if (result == null || !result.Value)
            {
                return null;
            }

            return window.Value;
        }

        /// <summary>
        /// ダイアログから時間間隔を取得します。
        /// </summary>
        public static TimeSpan? ShowTimeSpanDialog()
        {
            return ShowTimeSpanDialog(TimeSpan.Zero);
        }
        #endregion

        #region フォント
        /// <summary>
        /// Drawing.FontからFontInfoに変換します。
        /// </summary>
        public static FontInfo ConvertFont(Drawing.Font font)
        {
            return new FontInfo()
            {
                Family = new FontFamily(font.FontFamily.Name),
                Size = font.SizeInPoints / 72.0 * 96.0,
                Style = ((font.Style & Drawing.FontStyle.Italic) == 0 ?
                          FontStyles.Normal : FontStyles.Italic),
                Weight = ((font.Style & Drawing.FontStyle.Bold) == 0 ?
                           FontWeights.Normal : FontWeights.Bold),
            };
        }

        /// <summary>
        /// FontInfoからDrawing.Fontに変換します。
        /// </summary>
        public static Drawing.Font ConvertFont(FontInfo fontInfo)
        {
            return new Drawing.Font(
                fontInfo.Family.Source,
                (float)(fontInfo.Size * 96.0 / 72.0),
                Drawing.FontStyle.Regular |
                (fontInfo.Style == FontStyles.Italic ?
                    Drawing.FontStyle.Italic :
                    Drawing.FontStyle.Regular) |
                (fontInfo.Weight >= FontWeights.Bold ?
                    Drawing.FontStyle.Bold :
                    Drawing.FontStyle.Regular));
        }

        /// <summary>
        /// フォントダイアログを開き、フォントの設定を行います。
        /// </summary>
        public static FontInfo ShowFontDialog(FontInfo defaultInfo)
        {
            var dialog = new System.Windows.Forms.FontDialog()
            {
                Font = ConvertFont(defaultInfo),
            };

            var result = dialog.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.OK)
            {
                return null;
            }

            return ConvertFont(dialog.Font);
        }
        #endregion

        #region 色選択
        /// <summary>
        /// 色選択ダイアログを表示します。
        /// </summary>
        public static Color? ShowColorDialog(Color? defaultColor, Window owner = null)
        {
            var dialog = new ColorDialog();
            if (owner != null)
            {
                dialog.Owner = owner;
            }

            // 必要ならデフォルト色を設定します。
            if (defaultColor != null)
            {
                dialog.SelectedColor = defaultColor.Value;
            }

            // OKボタンが押されたら、その色を返します。
            var result = dialog.ShowDialog();
            if (result != null && result.Value)
            {
                return dialog.SelectedColor;
            }

            return null;
        }
        #endregion
    }
}
