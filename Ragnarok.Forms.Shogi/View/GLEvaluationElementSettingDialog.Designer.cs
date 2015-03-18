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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.valueFullWidthCheckBox = new System.Windows.Forms.CheckBox();
            this.visibleValueCheckBox = new System.Windows.Forms.CheckBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.infoControl = new Ragnarok.Forms.Controls.InfoControl();
            this.imageSetListComboBox = new System.Windows.Forms.ComboBox();
            this.okButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.cancelButton, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.okButton, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(3);
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(301, 277);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 3);
            this.groupBox1.Controls.Add(this.valueFullWidthCheckBox);
            this.groupBox1.Controls.Add(this.visibleValueCheckBox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(6, 168);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(289, 67);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "設定";
            // 
            // valueFullWidthCheckBox
            // 
            this.valueFullWidthCheckBox.AutoSize = true;
            this.valueFullWidthCheckBox.Location = new System.Drawing.Point(15, 40);
            this.valueFullWidthCheckBox.Name = "valueFullWidthCheckBox";
            this.valueFullWidthCheckBox.Size = new System.Drawing.Size(223, 16);
            this.valueFullWidthCheckBox.TabIndex = 1;
            this.valueFullWidthCheckBox.Text = "値を全角数字で表示する（横幅を伸ばす）";
            this.valueFullWidthCheckBox.UseVisualStyleBackColor = true;
            // 
            // visibleValueCheckBox
            // 
            this.visibleValueCheckBox.AutoSize = true;
            this.visibleValueCheckBox.Location = new System.Drawing.Point(15, 18);
            this.visibleValueCheckBox.Name = "visibleValueCheckBox";
            this.visibleValueCheckBox.Size = new System.Drawing.Size(88, 16);
            this.visibleValueCheckBox.TabIndex = 0;
            this.visibleValueCheckBox.Text = "値を表示する";
            this.visibleValueCheckBox.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(220, 246);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(3, 3, 3, 5);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "キャンセル";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // groupBox3
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox3, 3);
            this.groupBox3.Controls.Add(this.groupBox2);
            this.groupBox3.Controls.Add(this.imageSetListComboBox);
            this.groupBox3.Location = new System.Drawing.Point(6, 10);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 7, 3, 7);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.groupBox3.Size = new System.Drawing.Size(289, 148);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "評価値画像";
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Controls.Add(this.infoControl);
            this.groupBox2.Location = new System.Drawing.Point(10, 52);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.groupBox2.Size = new System.Drawing.Size(269, 87);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "基本情報";
            // 
            // infoControl
            // 
            this.infoControl.AutoSize = true;
            this.infoControl.Info = null;
            this.infoControl.Location = new System.Drawing.Point(6, 22);
            this.infoControl.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.infoControl.Name = "infoControl";
            this.infoControl.Size = new System.Drawing.Size(257, 61);
            this.infoControl.TabIndex = 0;
            // 
            // imageSetListComboBox
            // 
            this.imageSetListComboBox.DisplayMember = "Title";
            this.imageSetListComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.imageSetListComboBox.FormattingEnabled = true;
            this.imageSetListComboBox.Location = new System.Drawing.Point(10, 22);
            this.imageSetListComboBox.Margin = new System.Windows.Forms.Padding(7);
            this.imageSetListComboBox.Name = "imageSetListComboBox";
            this.imageSetListComboBox.Size = new System.Drawing.Size(269, 20);
            this.imageSetListComboBox.TabIndex = 4;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(139, 246);
            this.okButton.Margin = new System.Windows.Forms.Padding(3, 3, 3, 5);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 9;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // GLEvaluationElementSettingDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(301, 277);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GLEvaluationElementSettingDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "評価値画像の設定";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox valueFullWidthCheckBox;
        private System.Windows.Forms.CheckBox visibleValueCheckBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox imageSetListComboBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private Controls.InfoControl infoControl;

    }
}