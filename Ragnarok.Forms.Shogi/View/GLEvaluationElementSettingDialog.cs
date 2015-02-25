using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Ragnarok.Forms;
using Ragnarok.Utility;
using B = Ragnarok.Forms.Bindings;

namespace Ragnarok.Forms.Shogi.View
{
    /// <summary>
    /// 評価値用エレメントの設定ダイアログです。
    /// </summary>
    public partial class GLEvaluationElementSettingDialog : Form
    {
        private B.BindingsCollection bindings;
        private GLEvaluationElement targetElement;

        private ImageSetInfo oldImageSet;
        private bool oldIsVisibleValue;
        private bool oldIsValueFullWidth;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GLEvaluationElementSettingDialog(GLEvaluationElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            InitializeComponent();
            this.bindings = new B.BindingsCollection(this);
            this.targetElement = element;

            // 古い値の保存
            this.oldImageSet = element.ImageSet;
            this.oldIsVisibleValue = element.IsVisibleValue;
            this.oldIsValueFullWidth = element.IsValueFullWidth;

            // コンボボックスに画像セットの一覧を設定します。
            this.imageSetListComboBox.DataSource = element.ImageSetList;
            
            // コンボボックスの選択アイテムと、評価値エレメントの選択画像セットを
            // バインディングさせます。
            this.bindings.Add(
                this.imageSetListComboBox, "SelectedItem",
                element, "ImageSet",
                OnImageSetChanged);
            this.bindings.Add(
                this.visibleValueCheckBox, "Checked",
                element, "IsVisibleValue");
            this.bindings.Add(
                this.valueFullWidthCheckBox, "Checked",
                element, "IsValueFullWidth");
        }

        /// <summary>
        /// 作者名やTwitterIdなどの表示情報を更新します。
        /// </summary>
        private void OnImageSetChanged(object sender, B.BindingPropertyChangedEventArgs e)
        {
            this.infoControl.Info = this.targetElement.ImageSet;
        }

        /// <summary>
        /// OKボタンが押された場合
        /// </summary>
        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// キャンセルボタンが押された場合
        /// </summary>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.targetElement.ImageSet = this.oldImageSet;
            this.targetElement.IsVisibleValue = this.oldIsVisibleValue;
            this.targetElement.IsValueFullWidth = this.oldIsValueFullWidth;
        }
    }
}
