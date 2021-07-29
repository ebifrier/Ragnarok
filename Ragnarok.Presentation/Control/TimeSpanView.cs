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
    [TemplatePart(Name = "Part_ValueText", Type = typeof(DecoratedText))]
    [TemplatePart(Name = "Part_SpecialContainer", Type = typeof(UIElement))]
    [TemplatePart(Name = "Part_StringText", Type = typeof(DecoratedText))]
    public partial class TimeSpanView : DecoratedTextBase
    {
        private readonly string NormalContainerName = "Part_NormalContainer";
        private readonly string ValueTextName = "Part_ValueText";
        private readonly string SpecialContainerName = "Part_SpecialContainer";
        private readonly string StringTextName = "Part_StringText";

        private UIElement normalContainer;
        private DecoratedText valueText;
        private UIElement specialContainer;
        private DecoratedText stringText;

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static TimeSpanView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TimeSpanView),
                new FrameworkPropertyMetadata(typeof(TimeSpanView)));
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
        /// 表示する時間のフォーマットを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ValueFormatProperty =
            DependencyProperty.Register(
                "ValueFormat", typeof(string), typeof(TimeSpanView),
                new FrameworkPropertyMetadata(@"hh\:mm\:ss", OnValueChanged));

        /// <summary>
        /// 表示する時間のフォーマットを取得または設定します。
        /// </summary>
        public string ValueFormat
        {
            get { return (string)GetValue(ValueFormatProperty); }
            set { SetValue(ValueFormatProperty, value); }
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
            if (this.valueText == null)
            {
                return;
            }

            this.valueText.Text = span.ToString(ValueFormat);
        }

        /// <summary>
        /// xamlファイル適用時に呼ばれます。
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.normalContainer = (UIElement)GetTemplateChild(NormalContainerName);
            this.valueText = (DecoratedText)GetTemplateChild(ValueTextName);
            this.specialContainer = (UIElement)GetTemplateChild(SpecialContainerName);
            this.stringText = (DecoratedText)GetTemplateChild(StringTextName);

            ValueUpdated(Value);
        }
    }
}
