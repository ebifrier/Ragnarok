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
    /// TimeSpanの変更に使うコントロールです。
    /// </summary>
    [TemplatePart(Type = typeof(Grid), Name = "Part_Grid")]
    [TemplatePart(Type = typeof(NumericUpDown), Name = "Part_Hours")]
    [TemplatePart(Type = typeof(NumericUpDown), Name = "Part_Minutes")]
    [TemplatePart(Type = typeof(NumericUpDown), Name = "Part_Seconds")]
    public partial class TimeSpanEditControl : System.Windows.Controls.Control
    {
        private const string GridPartName = "Part_Grid";
        private const string HoursPartName = "Part_Hours";
        private const string MinutesPartName = "Part_Minutes";
        private const string SecondsPartName = "Part_Seconds";

        private readonly ReentrancyLock syncLock = new ReentrancyLock();
        private Grid gridPart;
        private NumericUpDown hoursPart;
        private NumericUpDown minutesPart;
        private NumericUpDown secondsPart;
        
        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static TimeSpanEditControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(TimeSpanEditControl),
                new FrameworkPropertyMetadata(typeof(TimeSpanEditControl)));
        }

        /// <summary>
        /// TimeSpanを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", typeof(TimeSpan), typeof(TimeSpanEditControl),
                new FrameworkPropertyMetadata(TimeSpan.Zero,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnValueChanged, CoerceValue));

        /// <summary>
        /// TimeSpanを取得または設定します。
        /// </summary>
        public TimeSpan Value
        {
            get { return (TimeSpan)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// TimeSpanが変更されたときに呼ばれます。
        /// </summary>
        static object CoerceValue(DependencyObject d, object oldValue)
        {
            var self = (TimeSpanEditControl)d;
            var span = (TimeSpan)oldValue;

            if (span == TimeSpan.MinValue || span == TimeSpan.MaxValue)
            {
                return oldValue;
            }

            // ミリ秒は必ず０にします。
            // というのもコントロールから値を変更すると必ずミリ秒が０に
            // なるため、どうせなら必ず０を保証しようということです。
            if (span.Milliseconds != 0)
            {
                return new TimeSpan(
                    (int)span.TotalHours,
                    span.Minutes,
                    span.Seconds);
            }

            return oldValue;
        }

        /// <summary>
        /// TimeSpanが変更されたときに呼ばれます。
        /// </summary>
        static void OnValueChanged(DependencyObject d,
                                      DependencyPropertyChangedEventArgs e)
        {
            var self = (TimeSpanEditControl)d;
            var span = (TimeSpan)e.NewValue;

            self.SyncValueToControl(span);
        }

        private void SyncValueToControl(TimeSpan span)
        {
            using (var result = this.syncLock.Lock())
            {
                if (result == null) return;

                if (this.gridPart != null)
                {
                    if (span == TimeSpan.MinValue || span == TimeSpan.MaxValue)
                    {
                        this.gridPart.IsEnabled = false;
                        return;
                    }
                    else
                    {
                        this.gridPart.IsEnabled = true;
                    }
                }

                if (this.hoursPart != null)
                {
                    this.hoursPart.Value = (int)span.TotalHours;
                }

                if (this.minutesPart != null)
                {
                    this.minutesPart.Value = span.Minutes;
                }

                if (this.secondsPart != null)
                {
                    this.secondsPart.Value = span.Seconds;
                }
            }
        }

        private void SyncValueFromControl()
        {
            using (var result = this.syncLock.Lock())
            {
                if (result == null) return;

                if (this.gridPart != null && !this.gridPart.IsEnabled)
                {
                    if (Value != TimeSpan.MinValue && Value != TimeSpan.MaxValue)
                    {
                        Value = TimeSpan.MinValue;
                    }
                    
                    return;
                }

                var hours = this.hoursPart != null ?
                    Convert.ToInt32(this.hoursPart.Value) :
                    (int)Value.TotalHours;

                var minutes = this.minutesPart != null ?
                    Convert.ToInt32(this.minutesPart.Value) :
                    Value.Minutes;

                var seconds = this.secondsPart != null ?
                    Convert.ToInt32(this.secondsPart.Value) :
                    Value.Seconds;

                Value = new TimeSpan(hours, minutes, seconds);
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

            if (this.hoursPart != null)
            {
                this.hoursPart.ValueChanged -= ValueChanged;
            }
            if (this.minutesPart != null)
            {
                this.minutesPart.ValueChanged -= ValueChanged;
            }
            if (this.secondsPart != null)
            {
                this.secondsPart.ValueChanged -= ValueChanged;
            }

            this.gridPart = GetTemplateChild(GridPartName) as Grid;
            this.hoursPart = GetTemplateChild(HoursPartName) as NumericUpDown;
            this.minutesPart = GetTemplateChild(MinutesPartName) as NumericUpDown;
            this.secondsPart = GetTemplateChild(SecondsPartName) as NumericUpDown;

            if (this.hoursPart != null)
            {
                this.hoursPart.ValueChanged += ValueChanged;
            }
            if (this.minutesPart != null)
            {
                this.minutesPart.ValueChanged += ValueChanged;
            }
            if (this.secondsPart != null)
            {
                this.secondsPart.ValueChanged += ValueChanged;
            }

            SyncValueToControl(Value);
        }
    }
}
