namespace Ragnarok.Forms.Shogi.View
{
    partial class GLEvaluationElementSettingDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.imageSetListComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.visibleValueCheckBox = new System.Windows.Forms.CheckBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.infoControl = new Ragnarok.Forms.Controls.InfoControl();
            this.valueFullWidthCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.infoControl);
            this.groupBox2.Location = new System.Drawing.Point(12, 50);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(301, 101);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "基本情報";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "選択された評価値画像";
            // 
            // imageSetListComboBox
            // 
            this.imageSetListComboBox.DisplayMember = "Title";
            this.imageSetListComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.imageSetListComboBox.FormattingEnabled = true;
            this.imageSetListComboBox.Location = new System.Drawing.Point(12, 24);
            this.imageSetListComboBox.Name = "imageSetListComboBox";
            this.imageSetListComboBox.Size = new System.Drawing.Size(301, 20);
            this.imageSetListComboBox.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.valueFullWidthCheckBox);
            this.groupBox1.Controls.Add(this.visibleValueCheckBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 157);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(301, 67);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "設定";
            // 
            // visibleValueCheckBox
            // 
            this.visibleValueCheckBox.AutoSize = true;
            this.visibleValueCheckBox.Location = new System.Drawing.Point(15, 18);
            this.visibleValueCheckBox.Name = "visibleValueCheckBox";
            this.visibleValueCheckBox.Size = new System.Drawing.Size(110, 16);
            this.visibleValueCheckBox.TabIndex = 0;
            this.visibleValueCheckBox.Text = "数字をで表示する";
            this.visibleValueCheckBox.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(238, 241);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "キャンセル";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(157, 241);
            this.okButton.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 8;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // infoControl
            // 
            this.infoControl.AutoSize = true;
            this.infoControl.Info = null;
            this.infoControl.Location = new System.Drawing.Point(6, 18);
            this.infoControl.Name = "infoControl";
            this.infoControl.Size = new System.Drawing.Size(289, 77);
            this.infoControl.TabIndex = 0;
            // 
            // valueFullWidthCheckBox
            // 
            this.valueFullWidthCheckBox.AutoSize = true;
            this.valueFullWidthCheckBox.Location = new System.Drawing.Point(15, 40);
            this.valueFullWidthCheckBox.Name = "valueFullWidthCheckBox";
            this.valueFullWidthCheckBox.Size = new System.Drawing.Size(235, 16);
            this.valueFullWidthCheckBox.TabIndex = 1;
            this.valueFullWidthCheckBox.Text = "数字を全角数字で表示する（横幅を伸ばす）";
            this.valueFullWidthCheckBox.UseVisualStyleBackColor = true;
            // 
            // GLEvaluationElementSettingDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 273);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.imageSetListComboBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GLEvaluationElementSettingDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "評価値画像の設定";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox imageSetListComboBox;
        private Controls.InfoControl infoControl;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.CheckBox visibleValueCheckBox;
        private System.Windows.Forms.CheckBox valueFullWidthCheckBox;

    }
}