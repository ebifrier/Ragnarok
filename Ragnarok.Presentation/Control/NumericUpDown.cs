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
    /// <summary>
    /// NumericUpDown.xaml の相互作用ロジック
    /// </summary>
    [TemplatePart(Type = typeof(TextBox), Name = "TextBoxPart")]
    [TemplatePart(Type = typeof(RepeatButton), Name = "UpButtonPart")]
    [TemplatePart(Type = typeof(RepeatButton), Name = "DownButtonPart")]
    public partial class NumericUpDown : System.Windows.Controls.Control
    {
        /// <summary>
        /// テキストボックスのコントロール名。
        /// </summary>
        private const string ElementTextName = "TextBoxPart";

        /// <summary>
        /// Upボタンのコントロール名。
        /// </summary>
        private const string ElementUpButtonName = "UpButtonPart";

        /// <summary>
        /// Downボタンのコントロール名。
        /// </summary>
        private const string ElementDownButtonName = "DownButtonPart";

        private TextBox textBox;
        private RepeatButton upButton;
        private RepeatButton downButton;
        private bool isSyncingTextAndValue = false;

        #region イベント
        /// <summary>
        /// 値変更時に呼ばれるイベントです。
        /// </summary>
        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(
                "ValueChanged", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<decimal>),
                typeof(NumericUpDown));

        /// <summary>
        /// 値変更時に呼ばれるイベントです。
        /// </summary>
        public event RoutedPropertyChangedEventHandler<decimal> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }
        #endregion

        #region プロパティ
        /// <summary>
        /// テキストボックス上で数値を変更可能かどうかを示す依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register(
                "IsEditable", typeof(bool), typeof(NumericUpDown),
                new PropertyMetadata(true));

        /// <summary>
        /// 数値データを文字列として扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text", typeof(string), typeof(NumericUpDown),
                new FrameworkPropertyMetadata("0", OnTextChanged),
                ValidateText);

        /// <summary>
        /// 数値データのフォーマットを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty TextFormatProperty =
            DependencyProperty.Register(
                "TextFormat", typeof(string), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(string.Empty, OnTextFormatChanged));

        /// <summary>
        /// 数値データの依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", typeof(decimal), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(
                    default(decimal),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnValueChanged, OnCoerceValue));

        /// <summary>
        /// 移動ステップ値を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register(
                "Step", typeof(decimal), typeof(NumericUpDown),
                new FrameworkPropertyMetadata((decimal)1));

        /// <summary>
        /// 最小値を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum", typeof(decimal), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(
                    (decimal)0, OnMinimumChanged, OnCoerceMinimum));

        /// <summary>
        /// 最大値を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum", typeof(decimal), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(
                    (decimal)100, OnMaximumChanged, OnCoerceMaximum));

        /// <summary>
        /// テキストボックス上で数値を変更可能かどうかを取得または設定します。
        /// </summary>
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        /// <summary>
        /// 値の文字列データを取得または設定します。
        /// </summary>
        [Bindable(true)]
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// 値の文字列フォーマットを取得または設定します。
        /// </summary>
        [Browsable(true)]
        [Bindable(true)]
        public string TextFormat
        {
            get { return (string)GetValue(TextFormatProperty); }
            set { SetValue(TextFormatProperty, value); }
        }

        /// <summary>
        /// 値を取得または設定します。
        /// </summary>
        [Browsable(true)]
        [Bindable(true)]
        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 移動ステップ値を取得または設定します。
        /// </summary>
        [Browsable(true)]
        [Bindable(true)]
        public decimal Step
        {
            get { return (decimal)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        /// <summary>
        /// 設定できる最小値を取得または設定します。
        /// </summary>
        [Browsable(true)]
        [Bindable(true)]
        public decimal Minimum
        {
            get { return (decimal)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// 設定できる最大値を取得または設定します。
        /// </summary>
        [Browsable(true)]
        [Bindable(true)]
        public decimal Maximum
        {
            get { return (decimal)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        #endregion

        #region プロパティメソッド
        /// <summary>
        /// テキストと値の同期を取ります。
        /// </summary>
        private void SyncValueAndText(bool changeValue, decimal value)
        {
            if (this.isSyncingTextAndValue)
            {
                return;
            }

            this.isSyncingTextAndValue = true;

            if (changeValue)
            {
                Value = value;
            }
            else
            {
                Text = (!string.IsNullOrEmpty(TextFormat)
                    ? string.Format(TextFormat, value)
                    : value.ToString());
            }

            this.isSyncingTextAndValue = false;
        }

        /// <summary>
        /// 値文字列が変更されたときに呼ばれます。
        /// </summary>
        private static void OnTextChanged(DependencyObject d,
                                          DependencyPropertyChangedEventArgs e)
        {
            var self = (NumericUpDown)d;
            var text = (string)e.NewValue;

            self.SyncValueAndText(true, decimal.Parse(text));
        }

        /// <summary>
        /// 値の表示フォーマットが変更されたときに呼ばれます。
        /// </summary>
        private static void OnTextFormatChanged(DependencyObject d,
                                                DependencyPropertyChangedEventArgs e)
        {
            var self = (NumericUpDown)d;

            self.SyncValueAndText(true, self.Value);
        }

        /// <summary>
        /// 値文字列の修正を行います。(054 -> 54 など)
        /// </summary>
        private static object CoerceValueString(DependencyObject d,
                                                object baseValue)
        {
            var text = (string)baseValue;

            /*if (string.IsNullOrEmpty(text))
            {
                return "0";
            }*/

            var value = decimal.Parse(text);
            return value.ToString();
        }

        /// <summary>
        /// 値文字列が有効か確認をします。
        /// </summary>
        private static bool ValidateText(object value)
        {
            var text = (string)value;

            decimal result;
            return decimal.TryParse(text, out result);
        }

        /// <summary>
        /// 値の変更時に呼ばれます。
        /// </summary>
        private static void OnValueChanged(DependencyObject d,
                                           DependencyPropertyChangedEventArgs e)
        {
            var self = (NumericUpDown)d;

            self.SyncValueAndText(false, self.Value);

            // 値変更イベントを発生させます。
            var args = new RoutedPropertyChangedEventArgs<decimal>(
                (decimal)e.OldValue, self.Value)
            {
                RoutedEvent = ValueChangedEvent,
            };
            self.RaiseEvent(args);
        }

        /// <summary>
        /// 値を[MinValue, MaxValue]の間に納めます。
        /// </summary>
        private static object OnCoerceValue(DependencyObject d,
                                            object baseValue)
        {
            var self = (NumericUpDown)d;
            var value = (decimal)baseValue;

            return Math.Min(self.Maximum, Math.Max(value, self.Minimum));
        }

        /// <summary>
        /// 最小値が変わったときに呼ばれます。
        /// </summary>
        private static void OnMinimumChanged(DependencyObject d,
                                            DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(MaximumProperty);
            d.CoerceValue(ValueProperty);
        }

        /// <summary>
        /// 必要なら設定された最小値を修正します。
        /// </summary>
        private static object OnCoerceMinimum(DependencyObject d,
                                              object baseValue)
        {
            var self = (NumericUpDown)d;
            var value = (decimal)baseValue;

            // 当然、最大値よりも小さくないとダメです。
            return Math.Min(value, self.Maximum);
        }

        /// <summary>
        /// 最大値が変わったときに呼ばれます。
        /// </summary>
        private static void OnMaximumChanged(DependencyObject d,
                                             DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(MinimumProperty);
            d.CoerceValue(ValueProperty);
        }

        /// <summary>
        /// 必要なら設定された最大値を修正します。
        /// </summary>
        private static object OnCoerceMaximum(DependencyObject d,
                                              object baseValue)
        {
            var self = (NumericUpDown)d;
            var value = (decimal)baseValue;

            // 当然、最小値よりも大きくないとダメです。
            return Math.Max(value, self.Minimum);
        }
        #endregion

        #region イベント/オーバーライド
        /// <summary>
        /// アクセラレータキーが押されたときに呼ばれます。
        /// </summary>
        protected override void OnAccessKey(AccessKeyEventArgs e)
        {
            base.OnAccessKey(e);

            if (this.textBox != null)
            {
                this.textBox.Focus();
            }
        }

        /// <summary>
        /// テンプレートが変わったときに呼ばれます。
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.upButton != null)
            {
                this.upButton.Click -= upButton_Click;
            }

            if (this.downButton != null)
            {
                this.downButton.Click -= downButton_Click;
            }

            this.textBox = GetTemplateChild(ElementTextName) as TextBox;
            this.upButton = GetTemplateChild(ElementUpButtonName) as RepeatButton;
            this.downButton = GetTemplateChild(ElementDownButtonName) as RepeatButton;

            if (this.upButton != null)
            {
                this.upButton.Click += upButton_Click;
            }

            if (this.downButton != null)
            {
                this.downButton.Click += downButton_Click;
            }
        }

        /// <summary>
        /// 値を増やします。
        /// </summary>
        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            OnIncrement();
        }

        /// <summary>
        /// 値を減らします。
        /// </summary>
        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            OnDecrement();
        }

        /// <summary>
        /// コントロールがフォーカスを受け取ったときは
        /// テキストボックスをフォーカスします。
        /// </summary>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (this.textBox != null)
            {
                this.textBox.Focus();
            }
        }

        /// <summary>
        /// キーが押されたときに呼ばれます。
        /// </summary>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            switch (e.Key)
            {
                case Key.Up:
                    OnIncrement();
                    e.Handled = true;
                    break;
                case Key.Down:
                    OnDecrement();
                    e.Handled = true;
                    break;
                case Key.Enter:
                    if (IsEditable)
                    {
                        var binding = BindingOperations.GetBindingExpression(
                            this.textBox, TextBox.TextProperty);

                        if (binding != null)
                        {
                            binding.UpdateSource();
                        }
                    }
                    break;
            }
        }
        #endregion

        #region アクション
        /// <summary>
        /// 値を増やします。
        /// </summary>
        private void OnIncrement()
        {
            Value += Step;
        }

        /// <summary>
        /// 値を減らします。
        /// </summary>
        private void OnDecrement()
        {
            Value -= Step;
        }
        #endregion

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(NumericUpDown),
                new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }
    }
}
