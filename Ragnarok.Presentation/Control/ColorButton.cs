using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Ragnarok.Presentation.Control
{
    using Command;

    /// <summary>
    /// ColorButton.xaml の相互作用ロジック
    /// </summary>
    [TemplatePart(Type = typeof(Button), Name = "Part_Button")]
    public class ColorButton : ButtonBase
    {
        /// <summary>
        /// ボタンのコントロール名。
        /// </summary>
        private const string ButtonName = "Part_Button";
        /// <summary>
        /// 色ブラシのコントロール名。
        /// </summary>
        private const string ColorBrushName = "Part_ColorBrush";

        private Button button;
        private SolidColorBrush colorBrush;

        /// <summary>
        /// 色変更時に呼ばれるイベントです。
        /// </summary>
        public static readonly RoutedEvent ColorChangedEvent =
            EventManager.RegisterRoutedEvent(
                "ColorChanged", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<Color>),
                typeof(ColorButton));

        /// <summary>
        /// 色変更時に呼ばれるイベントです。
        /// </summary>
        public event RoutedPropertyChangedEventHandler<Color> ColorChanged
        {
            add { AddHandler(ColorChangedEvent, value); }
            remove { RemoveHandler(ColorChangedEvent, value); }
        }

        /// <summary>
        /// 色を示す依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                "Color", typeof(Color), typeof(ColorButton),
                new FrameworkPropertyMetadata(Colors.Black,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnColorChanged));

        /// <summary>
        /// 色を取得または設定します。
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (ColorButton)d;

            self.UpdateColor((Color)e.OldValue, (Color)e.NewValue);
        }

        /// <summary>
        /// テンプレートが変わったときに呼ばれます。
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.button = GetTemplateChild(ButtonName) as Button;
            this.colorBrush = null;

            if (this.button != null)
            {
                // 色選択コマンドを設定します。
                this.button.Command = new RelayCommand(SelectColor);

                this.colorBrush = this.button.FindResource(ColorBrushName) as SolidColorBrush;
            }

            UpdateColor(Color, Color);
        }

        /// <summary>
        /// 色更新時などに呼ばれます。
        /// </summary>
        private void UpdateColor(Color oldValue, Color newValue)
        {
            if (this.colorBrush != null)
            {
                this.colorBrush.Color = newValue;
            }

            if (oldValue != newValue)
            {
                var e = new RoutedPropertyChangedEventArgs<Color>(
                    oldValue, newValue, ColorChangedEvent);

                RaiseEvent(e);
            }
        }

        /// <summary>
        /// 色選択を行います。
        /// </summary>
        private void SelectColor(object sender, ExecuteRelayEventArgs e)
        {
            try
            {
                var dialog = new ColorDialog
                {
                    SelectedColor = Color,
                };

                // 色選択ダイアログを出します。
                var result = dialog.ShowDialogCenterMouse();
                if (result == true)
                {
                    Color = dialog.SelectedColor;
                }
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);

                DialogUtil.ShowError(ex,
                    "色の選択に失敗しました (ToT)");
            }
        }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static ColorButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ColorButton),
                new FrameworkPropertyMetadata(typeof(ColorButton)));
        }
    }
}
