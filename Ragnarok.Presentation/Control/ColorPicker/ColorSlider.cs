using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ragnarok.Presentation.Control.ColorPicker
{
    /// <summary>
    /// 色を設定するためのスライダーです。
    /// </summary>
    public class ColorSlider : Slider
    {
        /// <summary>
        /// 左端の色を示す依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty LeftColorProperty =
            DependencyProperty.Register(
                "LeftColor",
                typeof(Color),
                typeof(ColorSlider),
                new UIPropertyMetadata(Colors.Black));

        /// <summary>
        /// 右端の色を示す依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty RightColorProperty =
            DependencyProperty.Register(
                "RightColor",
                typeof(Color),
                typeof(ColorSlider),
                new UIPropertyMetadata(Colors.White));

        /// <summary>
        /// 左端の色を取得または設定します。
        /// </summary>
        public Color LeftColor
        {
            get { return (Color)GetValue(LeftColorProperty); }
            set { SetValue(LeftColorProperty, value); }
        }
        
        /// <summary>
        /// 右端の色を取得または設定します。
        /// </summary>
        public Color RightColor
        {
            get { return (Color)GetValue(RightColorProperty); }
            set { SetValue(RightColorProperty, value); }
        }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static ColorSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ColorSlider),
                new FrameworkPropertyMetadata(typeof(ColorSlider)));
        }
    }
}
