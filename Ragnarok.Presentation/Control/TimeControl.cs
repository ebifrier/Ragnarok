using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ragnarok.Presentation.Control
{
    /// <summary>
    /// TimeControl.xaml の相互作用ロジック
    /// </summary>
    [TemplatePart(Name = "Part_ValueText0", Type = typeof(DecoratedText))]
    [TemplatePart(Name = "Part_ValueText1", Type = typeof(DecoratedText))]
    [TemplatePart(Name = "Part_ValueText2", Type = typeof(DecoratedText))]
    [TemplatePart(Name = "Part_ValueText3", Type = typeof(DecoratedText))]
    [TemplatePart(Name = "Part_ValueText4", Type = typeof(DecoratedText))]
    [TemplatePart(Name = "Part_ValueText5", Type = typeof(DecoratedText))]
    [TemplatePart(Name = "Part_ColonText0", Type = typeof(DecoratedText))]
    [TemplatePart(Name = "Part_ColonText1", Type = typeof(DecoratedText))]
    public partial class TimeControl : UserControl
    {
        private DecoratedText[] valueTexts = new DecoratedText[6];
        private DecoratedText[] colonTexts = new DecoratedText[2];

        private static string Part_ValueTextName(int n)
        {
            return string.Format("Part_ValueText{0}", n);
        }

        private static string Part_ColonTextName(int n)
        {
            return string.Format("Part_ColonText{0}", n);
        }

        /// <summary>
        /// 表示する時間を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty TimeSpanProperty =
            DependencyProperty.Register(
                "TimeSpan", typeof(TimeSpan), typeof(TimeControl),
                new FrameworkPropertyMetadata(
                    TimeSpan.Zero, OnTimeSpanChanged));

        /// <summary>
        /// 表示する時間を取得または設定します。
        /// </summary>
        public TimeSpan TimeSpan
        {
            get { return (TimeSpan)GetValue(TimeSpanProperty); }
            set { SetValue(TimeSpanProperty, value); }
        }

        static void OnTimeSpanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (TimeControl)d;

            self.TimeSpanUpdated((TimeSpan)e.NewValue);
        }

        /// <summary>
        /// 時間更新時に呼ばれ、表示を更新します。
        /// </summary>
        void TimeSpanUpdated(TimeSpan span)
        {
            for (var i = 0; i < this.valueTexts.Length; ++i)
            {
                var control = this.valueTexts[i];
                if (control == null)
                {
                    continue;
                }

                var n = 0;
                switch (i)
                {
                    case 0: n = span.Seconds; break;
                    case 1: n = span.Seconds / 10; break;
                    case 2: n = span.Minutes; break;
                    case 3: n = span.Minutes / 10; break;
                    case 4: n = span.Hours; break;
                    case 5: n = span.Hours / 10; break;
                }

                control.Text = (n % 10).ToString();
            }


            // ':'記号の不透明度。
            var millis = /*(span.Milliseconds < 500 ?
                500 - span.Milliseconds :
                span.Milliseconds - 500);*/
                span.Milliseconds;
            var opacity = MathEx.InterpLiner(0.0, 1.0, millis * 0.001);

            // ':'記号の不透明度を変更します。
            for (var i = 0; i < this.colonTexts.Length; ++i)
            {
                var control = this.colonTexts[i];
                if (control == null)
                {
                    continue;
                }

                //control.Opacity = opacity;
            }
        }

        /// <summary>
        /// xamlファイル適用時に呼ばれます。
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            for (var i = 0; i < this.valueTexts.Length; ++i)
            {
                this.valueTexts[i] = (DecoratedText)GetTemplateChild(Part_ValueTextName(i));
            }

            for (var i = 0; i < this.colonTexts.Length; ++i)
            {
                this.colonTexts[i] = (DecoratedText)GetTemplateChild(Part_ColonTextName(i));
            }

            TimeSpanUpdated(TimeSpan);
        }
        
        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static TimeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TimeControl),
                new FrameworkPropertyMetadata(typeof(TimeControl)));
            /*ActualWidthProperty.OverrideMetadata(
                typeof(TimeControl),
                new FrameworkPropertyMetadata(0, OnWidthChanged));*/
        }

        /*static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (TimeControl)d;
            var width = (double)e.NewValue;

            if (!double.IsNaN(width))
            {
                self.valueTexts
                    .Concat(self.colonTexts)
                    .ForEachWithIndex((v, i) =>
                    {
                        if (v != null) v.Width = width / 8;
                    });
            }
        }*/

        public TimeControl()
        {
            //EventManager.RegisterClassHandler(
        }
    }
}
