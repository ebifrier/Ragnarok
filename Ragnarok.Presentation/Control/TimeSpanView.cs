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
    /// TimeSpanオブジェクトの表示を行います。
    /// </summary>
    /// <remarks>
    /// 普通に文字列化すると数字によってフォントサイズが微妙に違うらしく
    /// 文字位置がずれてしまいます。
    /// </remarks>
    [TemplatePart(Name = "Part_NormalContainer", Type = typeof(UIElement))]
    [TemplatePart(Name = "Part_ValueText0", Type = typeof(DecoratedText))]
    [TemplatePart(Name = "Part_ValueText1", Type = typeof(DecoratedText))]
    [TemplatePart(Name = "Part_ValueText2", Type = typeof(DecoratedText))]
    [TemplatePart(Name = "Part_ValueText3", Type = typeof(DecoratedText))]
    [TemplatePart(Name = "Part_ValueText4", Type = typeof(DecoratedText))]
    [TemplatePart(Name = "Part_ValueText5", Type = typeof(DecoratedText))]
    [TemplatePart(Name = "Part_ColonText0", Type = typeof(DecoratedText))]
    [TemplatePart(Name = "Part_ColonText1", Type = typeof(DecoratedText))]
    [TemplatePart(Name = "Part_SpecialContainer", Type = typeof(UIElement))]
    [TemplatePart(Name = "Part_StringText", Type = typeof(DecoratedText))]
    public partial class TimeSpanView : DecoratedTextBase
    {
        private readonly string NormalContainerName = "Part_NormalContainer";
        private readonly string SpecialContainerName = "Part_SpecialContainer";
        private readonly string StringTextName = "Part_StringText";

        private UIElement normalContainer;
        private DecoratedText[] valueTexts = new DecoratedText[6];
        private DecoratedText[] colonTexts = new DecoratedText[2];
        private UIElement specialContainer;
        private DecoratedText stringText;

        private static string Part_ValueTextName(int n)
        {
            return string.Format("Part_ValueText{0}", n);
        }

        private static string Part_ColonTextName(int n)
        {
            return string.Format("Part_ColonText{0}", n);
        }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static TimeSpanView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TimeSpanView),
                new FrameworkPropertyMetadata(typeof(TimeSpanView)));
            FontSizeProperty.OverrideMetadata(
                typeof(TimeSpanView),
                new FrameworkPropertyMetadata(OnFontSizeChanged));
        }

        private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (TimeSpanView)d;

            self.NumberWidth =
                self.FontSize / 2.0 +
                Math.Max(1.0, self.FontSize / 10);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TimeSpanView()
        {
            OnFontSizeChanged(this,
                new DependencyPropertyChangedEventArgs(
                    FontSizeProperty, FontSize, FontSize));
        }

        /// <summary>
        /// 表示する時間を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", typeof(TimeSpan), typeof(TimeSpanView),
                new FrameworkPropertyMetadata(TimeSpan.Zero, OnValueChanged));

        /// <summary>
        /// 表示する時間を取得または設定します。
        /// </summary>
        public TimeSpan Value
        {
            get { return (TimeSpan)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 各数字の表示幅を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty NumberWidthProperty =
            DependencyProperty.Register(
                "NumberWidth", typeof(double), typeof(TimeSpanView),
                new FrameworkPropertyMetadata(7.0));

        /// <summary>
        /// 各数字の表示幅を取得または設定します。
        /// </summary>
        public double NumberWidth
        {
            get { return (double)GetValue(NumberWidthProperty); }
            set { SetValue(NumberWidthProperty, value); }
        }

        /// <summary>
        /// TimeSpanがMinValueの時に表示される文字列を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty MinValueTextProperty =
            DependencyProperty.Register(
                "MinValueText", typeof(string), typeof(TimeSpanView),
                new FrameworkPropertyMetadata("MinValue", OnValueChanged));

        /// <summary>
        /// TimeSpanがMinValueの時に表示される文字列を取得または設定します。
        /// </summary>
        public string MinValueText
        {
            get { return (string)GetValue(MinValueTextProperty); }
            set { SetValue(MinValueTextProperty, value); }
        }

        /// <summary>
        /// TimeSpanがMaxValueの時に表示される文字列を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty MaxValueTextProperty =
            DependencyProperty.Register(
                "MaxValueText", typeof(string), typeof(TimeSpanView),
                new FrameworkPropertyMetadata("MaxValue", OnValueChanged));

        /// <summary>
        /// TimeSpanがMaxValueの時に表示される文字列を取得または設定します。
        /// </summary>
        public string MaxValueText
        {
            get { return (string)GetValue(MaxValueTextProperty); }
            set { SetValue(MaxValueTextProperty, value); }
        }

        static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (TimeSpanView)d;

            self.ValueUpdated(self.Value);
        }

        /// <summary>
        /// 時間更新時に呼ばれ、表示を更新します。
        /// </summary>
        void ValueUpdated(TimeSpan span)
        {
            if (span == TimeSpan.MinValue || span == TimeSpan.MaxValue)
            {
                if (this.normalContainer != null)
                {
                    this.normalContainer.Visibility = Visibility.Collapsed;
                }

                if (this.specialContainer != null)
                {
                    this.specialContainer.Visibility = Visibility.Visible;
                }

                if (this.stringText != null)
                {
                    this.stringText.Text = (span == TimeSpan.MinValue ?
                        MinValueText : MaxValueText);
                }
            }
            else
            {
                if (this.normalContainer != null)
                {
                    this.normalContainer.Visibility = Visibility.Visible;
                }

                if (this.specialContainer != null)
                {
                    this.specialContainer.Visibility = Visibility.Collapsed;
                }

                UpdateValueTexts(span);
            }
        }

        /// <summary>
        /// 数字部分を更新します。
        /// </summary>
        private void UpdateValueTexts(TimeSpan span)
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
            // 静止状態では表示したいため、ミリ秒が０の時は
            // 不透明度を1.0にする必要があります。
            /*var millis = (span.Milliseconds < 500 ?
                span.Milliseconds :
                1000 - span.Milliseconds);
            var opacity = MathEx.InterpLiner(1.0, 0.0, millis * 0.002);
            Log.Info("opacity: {0}, {1}", span.Milliseconds, opacity);*/

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

            this.normalContainer = (UIElement)GetTemplateChild(NormalContainerName);
            this.specialContainer = (UIElement)GetTemplateChild(SpecialContainerName);
            this.stringText = (DecoratedText)GetTemplateChild(StringTextName);

            for (var i = 0; i < this.valueTexts.Length; ++i)
            {
                this.valueTexts[i] = (DecoratedText)GetTemplateChild(Part_ValueTextName(i));
            }

            for (var i = 0; i < this.colonTexts.Length; ++i)
            {
                this.colonTexts[i] = (DecoratedText)GetTemplateChild(Part_ColonTextName(i));
            }

            ValueUpdated(Value);
        }
    }
}
