using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ragnarok.Presentation.Control
{
    /// <summary>
    /// ウォーターマーク付きのテキストボックスクラスです。
    /// </summary>
    public class WatermarkTextBox : TextBox
    {
        /// <summary>
        /// フォーカスを得た時、テキストを全選択するかどうかを決める依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty SelectAllOnGotFocusProperty =
            DependencyProperty.Register(
                "SelectAllOnGotFocus", typeof(bool),
                typeof(WatermarkTextBox),
                new PropertyMetadata(false));

        /// <summary>
        /// フォーカスを得た時、テキストを全選択するかどうかを取得または設定します。
        /// </summary>
        public bool SelectAllOnGotFocus
        {
            get { return (bool)GetValue(SelectAllOnGotFocusProperty); }
            set { SetValue(SelectAllOnGotFocusProperty, value); }
        }

        /// <summary>
        /// ウォーターマークを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register(
            "Watermark", typeof(object),
            typeof(WatermarkTextBox),
            new UIPropertyMetadata(null));

        /// <summary>
        /// ウォーターマークを取得または設定します。
        /// </summary>
        public object Watermark
        {
            get { return (object)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        /// <summary>
        /// ウォーターマークのテンプレートを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty WatermarkTemplateProperty =
            DependencyProperty.Register(
            "WatermarkTemplate", typeof(DataTemplate),
            typeof(WatermarkTextBox),
            new UIPropertyMetadata(null));

        /// <summary>
        /// ウォーターマークのテンプレートを取得または設定します。
        /// </summary>
        public DataTemplate WatermarkTemplate
        {
            get { return (DataTemplate)GetValue(WatermarkTemplateProperty); }
            set { SetValue(WatermarkTemplateProperty, value); }
        }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static WatermarkTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(WatermarkTextBox),
                new FrameworkPropertyMetadata(typeof(WatermarkTextBox)));
        }

        #region オーバーライド

        /// <summary>
        /// キーフォーカスを得た時に呼ばれます。
        /// </summary>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            if (SelectAllOnGotFocus)
            {
                SelectAll();
            }
            else
            {
                SelectionLength = 0;
            }
        }

        /// <summary>
        /// マウスの左ボタンイベントを事前に処理します。
        /// </summary>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!IsKeyboardFocused && SelectAllOnGotFocus)
            {
                Focus();
                e.Handled = true;
            }

            base.OnPreviewMouseLeftButtonDown(e);
        }

        #endregion
    }
}
