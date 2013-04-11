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

namespace Ragnarok.Presentation.Update
{
    /// <summary>
    /// UpdateWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class UpdateWindow : Window
    {
        /// <summary>
        /// ファイルのダウンロードが完了しているかどうかを示す依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty IsDownloadedProperty =
            DependencyProperty.Register(
                "IsDownloaded",
                typeof(bool),
                typeof(UpdateWindow),
                new UIPropertyMetadata(false));

        /// <summary>
        /// ファイルのダウンロードが完了しているかどうかを取得または設定します。
        /// </summary>
        public bool IsDownloaded
        {
            get { return (bool)GetValue(IsDownloadedProperty); }
            set { SetValue(IsDownloadedProperty, value); }
        }
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UpdateWindow()
        {
            InitializeComponent();
        }
    }
}
