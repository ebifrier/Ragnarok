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
using System.Windows.Shapes;
using System.ComponentModel;

using Ragnarok;
using Ragnarok.Presentation;
using Ragnarok.ObjectModel;
using Ragnarok.Utility;

namespace Ragnarok.Presentation.Control
{
    /// <summary>
    /// TimeSpanWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TimeSpanWindow : Window
    {
        /// <summary>
        /// TimeSpanを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", typeof(TimeSpan), typeof(TimeSpanWindow),
                new FrameworkPropertyMetadata(TimeSpan.Zero,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// TimeSpanを取得または設定します。
        /// </summary>
        public TimeSpan Value
        {
            get { return (TimeSpan)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TimeSpanWindow(TimeSpan timeSpan)
        {
            InitializeComponent();

            Activated += delegate { this.buttons.Focus(); };
            DataContext = this;

            Value = timeSpan;
        }
    }
}
