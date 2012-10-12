using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Ragnarok.Presentation.Control.ColorPicker
{
    /// <summary>
    /// Hue値を色に変換します。
    /// </summary>
    [ValueConversion(typeof(double), typeof(Color))]
    public class HueToColorConverter : IValueConverter
    {
        /// <summary>
        /// 変換メソッド
        /// </summary>
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            var doubleValue = (double)value;

            return ColorUtils.ConvertHsvToRgb(doubleValue, 1, 1);
        }

        /// <summary>
        /// 逆変換メソッド
        /// </summary>
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// HSVのスペクトルを表示するコントロールです。
    /// </summary>
    [TemplatePart(Type = typeof(Thumb), Name = "PART_Thumb")]
    public class HsvControl : System.Windows.Controls.Control
    {
        private const string ThumbName = "PART_Thumb";

        private readonly TranslateTransform m_thumbTransform =
            new TranslateTransform();
        private Thumb m_thumb;
        private bool m_withinUpdate = false;

        /// <summary>
        /// Hue値を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register(
                "Hue",
                typeof(double),
                typeof(HsvControl),
                new UIPropertyMetadata((double)0.0, OnHueChanged));

        /// <summary>
        /// Sat値を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register(
                "Saturation",
                typeof(double),
                typeof(HsvControl),
                new UIPropertyMetadata((double)0.0, OnSaturationChanged));

        /// <summary>
        /// Value値を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(double),
                typeof(HsvControl),
                new UIPropertyMetadata((double)0, OnValueChanged));

        /// <summary>
        /// 選択された色を取得または設定します。
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
                "SelectedColor",
                typeof(Color),
                typeof(HsvControl),
                new UIPropertyMetadata(Colors.Transparent, OnSelectedColorChanged));

        /// <summary>
        /// 選択された色が変更されたときに呼ばれるイベントです。
        /// </summary>
        public static readonly RoutedEvent SelectedColorChangedEvent =
            EventManager.RegisterRoutedEvent(
                "SelectedColorChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<Color>),
                typeof(HsvControl));        

        /// <summary>
        /// 色変更時に呼ばれるイベントを変更します。
        /// </summary>
        public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged
        {
            add { AddHandler(SelectedColorChangedEvent, value); }
            remove { RemoveHandler(SelectedColorChangedEvent, value); }
        }

        /// <summary>
        /// Hue値を取得または設定します。
        /// </summary>
        public double Hue
        {
            get { return (double)GetValue(HueProperty); }
            set { SetValue(HueProperty, value); }
        }

        /// <summary>
        /// Sat値を取得または設定します。
        /// </summary>
        public double Saturation
        {
            get { return (double)GetValue(SaturationProperty); }
            set { SetValue(SaturationProperty, value); }
        }

        /// <summary>
        /// Value値を取得または設定します。
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 選択された色を取得または設定します。
        /// </summary>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        #region Event Handlers
        /// <summary>
        /// Hueが更新されたときに呼ばれます。
        /// </summary>
        private static void OnHueChanged(
            DependencyObject relatedObject, DependencyPropertyChangedEventArgs e)
        {
            var hsvControl = relatedObject as HsvControl;
            if (hsvControl != null && !hsvControl.m_withinUpdate)
            {
                hsvControl.UpdateSelectedColor();
            }
        }

        /// <summary>
        /// Satが更新されたときに呼ばれます。
        /// </summary>
        private static void OnSaturationChanged(
            DependencyObject relatedObject, DependencyPropertyChangedEventArgs e)
        {
            var hsvControl = relatedObject as HsvControl;
            if (hsvControl != null && !hsvControl.m_withinUpdate)
            {
                hsvControl.UpdateThumbPosition();
            }
        }

        /// <summary>
        /// Valueが更新されたときに呼ばれます。
        /// </summary>
        private static void OnValueChanged(
            DependencyObject relatedObject, DependencyPropertyChangedEventArgs e)
        {
            var hsvControl = relatedObject as HsvControl;
            if (hsvControl != null && !hsvControl.m_withinUpdate)
            {
                hsvControl.UpdateThumbPosition();
            }
        }

        /// <summary>
        /// 色が更新されたときに呼ばれます。
        /// </summary>
        private static void OnSelectedColorChanged(
            DependencyObject relatedObject, DependencyPropertyChangedEventArgs e)
        {
            var hsvControl = relatedObject as HsvControl;
            if (hsvControl != null)
            {
                // 色の変更を通知します。
                ColorUtils.FireSelectedColorChangedEvent(
                    hsvControl, SelectedColorChangedEvent,
                    (Color)e.OldValue,
                    (Color)e.NewValue);
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// サムネイルが移動したときに呼ばれます。
        /// </summary>
        private static void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            var hsvControl = sender as HsvControl;

            if (hsvControl != null)
            {
                hsvControl.OnThumbDragDelta(e);
            }
        }

        /// <summary>
        /// サムネイルが移動したときに呼ばれます。
        /// </summary>
        private void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            double offsetX = m_thumbTransform.X + e.HorizontalChange;
            double offsetY = m_thumbTransform.Y + e.VerticalChange;

            UpdatePositionAndSaturationAndValue(offsetX, offsetY);
        }

        /// <summary>
        /// マウスの左ボタンが押されたときに呼ばれます。
        /// </summary>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (m_thumb != null)
            {
                Point position = e.GetPosition(this);

                UpdatePositionAndSaturationAndValue(position.X, position.Y);

                // ドラッグ処理を開始するためにイベントを呼びます。
                m_thumb.RaiseEvent(e);
            }

            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// サイズが更新されたときに呼ばれます。
        /// </summary>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            UpdateThumbPosition();

            base.OnRenderSizeChanged(sizeInfo);
        }

        /// <summary>
        /// Template適用時に呼ばれます。
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            m_thumb = GetTemplateChild(ThumbName) as Thumb;
            if (m_thumb != null)
            {
                UpdateThumbPosition();
                m_thumb.RenderTransform = m_thumbTransform;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 色の更新処理を行います。
        /// </summary>
        private void UpdateSelectedColor()
        {
            SelectedColor = ColorUtils.ConvertHsvToRgb(Hue, Saturation, Value);
        }

        /// <summary>
        /// 値を (0,max] の範囲に収めます。
        /// </summary>
        private double LimitValue(double value, double max)
        {
            return Math.Max(0, Math.Min(value, max));
        }

        /// <summary>
        /// HSVコントロールで選択された色を示す○の位置から色を更新します。
        /// </summary>
        private void UpdatePositionAndSaturationAndValue(double positionX, double positionY)
        {
            positionX = LimitValue(positionX, ActualWidth);
            positionY = LimitValue(positionY, ActualHeight);

            m_thumbTransform.X = positionX;
            m_thumbTransform.Y = positionY;

            Saturation = positionX / ActualWidth;
            Value      = 1 - positionY / ActualHeight;

            UpdateSelectedColor();
        }

        /// <summary>
        /// HSVコントロールで選択された色を示す○の位置を更新します。
        /// </summary>
        private void UpdateThumbPosition()
        {
            m_thumbTransform.X = Saturation * ActualWidth;
            m_thumbTransform.Y = (1 - Value) * ActualHeight;

            UpdateSelectedColor();
        }

        #endregion

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static HsvControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(HsvControl),
                new FrameworkPropertyMetadata(typeof(HsvControl)));

            // Register Event Handler for the Thumb 
            EventManager.RegisterClassHandler(
                typeof(HsvControl),
                Thumb.DragDeltaEvent,
                new DragDeltaEventHandler(HsvControl.OnThumbDragDelta));
        }
    }
}
