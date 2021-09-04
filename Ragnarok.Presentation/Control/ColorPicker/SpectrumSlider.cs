using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Ragnarok.Presentation.Control.ColorPicker
{
    /// <summary>
    /// スペクトルを移動するためのスライダーです。
    /// </summary>
    public class SpectrumSlider : Slider
    {
        /// <summary>
        /// スペクトル背景の色の数です。
        /// </summary>
        private const int SpectrumColorCount = 40;

        /// <summary>
        /// Hueの値を示す依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register(
                "Hue",
                typeof(double),
                typeof(SpectrumSlider),
                new UIPropertyMetadata((double)0.0, OnHuePropertyChanged));

        private bool m_withinChanging = false;

        /// <summary>
        /// Hue値の変更時に呼ばれます。
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> HueChanged;

        /// <summary>
        /// Hue値を取得または設定します。
        /// </summary>
        public double Hue
        {
            get { return (double)GetValue(HueProperty); }
            set { SetValue(HueProperty, value); }
        }

        /// <summary>
        /// Hueプロパティが変更されたときに呼ばれます。
        /// </summary>
        private static void OnHuePropertyChanged(
            DependencyObject relatedObject, DependencyPropertyChangedEventArgs e)
        {
            var spectrumSlider = relatedObject as SpectrumSlider;
            if (spectrumSlider != null)
            {
                spectrumSlider.OnHuePropertyChanged(e);
            }
        }

        /// <summary>
        /// Hueプロパティが変更されたときに呼ばれます。
        /// </summary>
        private void OnHuePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            // OnValueChangedと無限ループにならないように。
            if (!m_withinChanging)
            {
                m_withinChanging = true;
                var hue = (double)e.NewValue;
                Value = 360.0 - hue;
                m_withinChanging = false;
            }

            // 変更通知イベントを呼びます。
            var handler = HueChanged;
            if (handler != null)
            {
                handler(this,
                    new RoutedPropertyChangedEventArgs<double>(
                        (double)e.OldValue,
                        (double)e.NewValue));
            }
        }

        /// <summary>
        /// スライダーの値が変更されたときに呼ばれます。
        /// </summary>
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);

            // OnHuePropertyChangedと無限ループにならないように。
            if (!m_withinChanging &&
                !BindingOperations.IsDataBound(this, HueProperty))
            {
                m_withinChanging = true;
                Hue = 360.0 - newValue;
                m_withinChanging = false;
            }
        }

        /// <summary>
        /// スペクトルの背景色を設定します。
        /// </summary>
        private void SetBackground()
        {
            var backgroundBrush = new LinearGradientBrush()
            {
                StartPoint = new Point(0.5, 0),
                EndPoint = new Point(0.5, 1),
            };

            var spectrumColors = ColorUtils.GetSpectrumColors(SpectrumColorCount);
            for (var i = 0; i < SpectrumColorCount; ++i)
            {
                var offset = i * 1.0 / SpectrumColorCount;
                var gradientStop = new GradientStop(spectrumColors[i], offset);

                backgroundBrush.GradientStops.Add(gradientStop);
            }

            backgroundBrush.Freeze();
            Background = backgroundBrush;
        }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static SpectrumSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(SpectrumSlider),
                new FrameworkPropertyMetadata(typeof(SpectrumSlider)));
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SpectrumSlider()
        {
            SetBackground();
        }
    }
}
