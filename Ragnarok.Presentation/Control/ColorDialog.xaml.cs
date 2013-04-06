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
    /// ColorDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ColorDialog : Window
    {
        /// <summary>
        /// 選択された色を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
                "SelectedColor", typeof(Color), typeof(ColorDialog),
                new FrameworkPropertyMetadata(Colors.Black,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 選択された色を取得または設定します。
        /// </summary>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        static ColorDialog()
        {
            TopmostProperty.OverrideMetadata(
                typeof(ColorDialog),
                new FrameworkPropertyMetadata(true));
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ColorDialog(Color? color = null)
        {
            InitializeComponent();

            if (color != null)
            {
                SelectedColor = color.Value;
            }
        }
    }
}
