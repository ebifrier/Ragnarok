using System;
using System.Collections.Generic;
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
using System.Globalization;

namespace Ragnarok.Presentation.Control
{
    /// <summary>
    /// DecoratedText.xaml の相互作用ロジック
    /// </summary>
    public abstract class DecoratedTextBase : System.Windows.Controls.Control
    {        
        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static DecoratedTextBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DecoratedTextBase),
                new FrameworkPropertyMetadata(typeof(DecoratedTextBase)));
        }

        /// <summary>
        /// 文字の縁取りを示す依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(
                "Stroke", typeof(Brush), typeof(DecoratedTextBase),
                new FrameworkPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// 文字の縁を塗るブラシを取得または設定します。
        /// </summary>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        /// 文字の縁取り幅を示す依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(
                "StrokeThickness", typeof(double), typeof(DecoratedTextBase),
                new FrameworkPropertyMetadata(1.0));

        /// <summary>
        /// 文字の縁の太さを取得または設定します。
        /// </summary>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
    }
}
