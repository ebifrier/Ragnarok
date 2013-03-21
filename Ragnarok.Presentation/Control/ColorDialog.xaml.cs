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
        /// 選択された色を取得または設定します。
        /// </summary>
        public Color SelectedColor
        {
            get
            {
                return this.colorPicker.SelectedColor;
            }
            set
            {
                this.colorPicker.SelectedColor = value;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ColorDialog()
        {
            InitializeComponent();

            CommandBindings.Add(
                new CommandBinding(
                    RagnarokCommands.OK,
                    ExecuteOK));
            CommandBindings.Add(
                new CommandBinding(
                    RagnarokCommands.Cancel,
                    ExecuteCancel));
        }

        /// <summary>
        /// OKボタン
        /// </summary>
        private void ExecuteOK(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// キャンセルボタン
        /// </summary>
        private void ExecuteCancel(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
