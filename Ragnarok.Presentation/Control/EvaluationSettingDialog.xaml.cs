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

using Ragnarok.ObjectModel;
using Ragnarok.Presentation.Utility;

namespace Ragnarok.Presentation.Control
{
    /// <summary>
    /// EvaluationSettingDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class EvaluationSettingDialog : Window
    {
        private ViewModelProxy model;
        private EvaluationControl control;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EvaluationSettingDialog(EvaluationControl control)
        {
            InitializeComponent();
            CommandBindings.Add(
                new CommandBinding(
                    RagnarokCommands.Cancel,
                    ExecuteCancel));

            this.model = new ViewModelProxy(control);
            this.control = control;

            DataContext = model;
        }

        /// <summary>
        /// Cancel
        /// </summary>
        private void ExecuteCancel(object sender, ExecutedRoutedEventArgs e)
        {
            this.model.RollbackViewModel();

            DialogResult = false;
        }
    }
}
