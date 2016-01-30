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

using Ragnarok;
using Ragnarok.ObjectModel;
using Ragnarok.Presentation;

namespace Ragnarok.Presentation.Control
{
    /// <summary>
    /// EvaluationSettingDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class EvaluationSettingDialog : Window
    {
        //private ViewModelProxy model;
        private EvaluationControl control;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EvaluationSettingDialog(EvaluationControl control)
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

            //this.model = new ViewModelProxy(control);
            this.control = control;

            //DataContext = model;
        }

        /// <summary>
        /// OK
        /// </summary>
        private void ExecuteOK(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                this.control.Connect();

                DialogResult = false;
            }
            catch (Exception ex)
            {
                DialogUtil.ShowError(ex,
                    "評価値サーバーへの接続に失敗しました。");
            }
        }

        /// <summary>
        /// Cancel
        /// </summary>
        private void ExecuteCancel(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                //this.model.RollbackViewModel();
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "プロパティ値のロールバックに失敗しました。");
            }

            DialogResult = false;
        }
    }
}
