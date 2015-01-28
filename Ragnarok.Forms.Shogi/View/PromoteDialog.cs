using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ragnarok.Forms.Shogi.View
{
    /// <summary>
    /// 成り・成らず用のダイアログです。
    /// </summary>
    public partial class PromoteDialog : Form
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PromoteDialog()
        {
            InitializeComponent();
        }

        private void PromoteDialog_Load(object sender, EventArgs e)
        {

        }

        private void yesButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
