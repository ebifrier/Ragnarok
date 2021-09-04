using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ragnarok.Presentation.Debug
{
    /// <summary>
    /// SendLogDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class SendLogDialog : Window
    {
        private readonly ReportDialogModel model;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SendLogDialog(ReportDialogModel model)
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

            this.model = model;
            this.DataContext = model;
        }

        /// <summary>
        /// ログを送信します。
        /// </summary>
        private void ExecuteOK(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.model != null)
            {
                // 失敗・キャンセル時はダイアログを閉じません。
                if (this.model.SendReport())
                {
                    DialogResult = true;
                }
            }
        }

        /// <summary>
        /// ログ送信をキャンセルします。
        /// </summary>
        private void ExecuteCancel(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
