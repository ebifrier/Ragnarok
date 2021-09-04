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

using Ragnarok.Utility;

namespace Ragnarok.Presentation.Control
{
    /// <summary>
    /// DateTimeの変更に使うコントロールです。
    /// </summary>
    [TemplatePart(Type = typeof(Grid), Name = "Part_Grid")]
    [TemplatePart(Type = typeof(NumericUpDown), Name = "Part_Years")]
    [TemplatePart(Type = typeof(NumericUpDown), Name = "Part_Monthes")]
    public partial class DateTimeEditControl : System.Windows.Controls.Control
    {
        private const string GridPartName = "Part_Grid";
        private const string YearsPartName = "Part_Years";
        private const string MonthesPartName = "Part_Monthes";

        private readonly ReentrancyLock syncLock = new ReentrancyLock();
        private Grid gridPart;
        private NumericUpDown yearsPart;
        private NumericUpDown monthesPart;
        
        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static DateTimeEditControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DateTimeEditControl),
                new FrameworkPropertyMetadata(typeof(DateTimeEditControl)));
        }

        /// <summary>
        /// DateTimeを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", typeof(DateTime), typeof(DateTimeEditControl),
                new FrameworkPropertyMetadata(DateTime.MinValue,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnValueChanged));

        /// <summary>
        /// DateTimeを取得または設定します。
        /// </summary>
        public DateTime Value
        {
            get { return (DateTime)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// DateTimeが変更されたときに呼ばれます。
        /// </summary>
        static void OnValueChanged(DependencyObject d,
                                   DependencyPropertyChangedEventArgs e)
        {
            var self = (DateTimeEditControl)d;
            var span = (DateTime)e.NewValue;

            self.SyncValueToControl(span);
        }

        private void SyncValueToControl(DateTime span)
        {
            using (var result = this.syncLock.Lock())
            {
                if (result == null) return;

                if (this.yearsPart != null)
                {
                    this.yearsPart.Value = span.Year;
                }

                if (this.monthesPart != null)
                {
                    this.monthesPart.Value = span.Month;
                }
            }
        }

        private void SyncValueFromControl()
        {
            using (var result = this.syncLock.Lock())
            {
                if (result == null) return;

                var years = this.yearsPart != null ?
                    Convert.ToInt32(this.yearsPart.Value) :
                    Value.Year;

                var monthes = this.monthesPart != null ?
                    Convert.ToInt32(this.monthesPart.Value) :
                    Value.Month;

                Value = new DateTime(years, monthes, 1);
            }
        }

        void ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
        {
            SyncValueFromControl();
        }
        
        /// <summary>
        /// テンプレートが変わったときに呼ばれます。
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.yearsPart != null)
            {
                this.yearsPart.ValueChanged -= ValueChanged;
            }
            if (this.monthesPart != null)
            {
                this.monthesPart.ValueChanged -= ValueChanged;
            }

            this.gridPart = GetTemplateChild(GridPartName) as Grid;
            this.yearsPart = GetTemplateChild(YearsPartName) as NumericUpDown;
            this.monthesPart = GetTemplateChild(MonthesPartName) as NumericUpDown;

            if (this.yearsPart != null)
            {
                this.yearsPart.ValueChanged += ValueChanged;
            }
            if (this.monthesPart != null)
            {
                this.monthesPart.ValueChanged += ValueChanged;
            }

            SyncValueToControl(Value);
        }
    }
}
