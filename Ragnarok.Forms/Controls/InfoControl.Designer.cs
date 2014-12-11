namespace Ragnarok.Forms.Controls
{
    partial class InfoControl
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.nameLabel = new System.Windows.Forms.Label();
            this.itemNameLabel1 = new System.Windows.Forms.Label();
            this.itemNameLabel2 = new System.Windows.Forms.Label();
            this.itemNameLabel3 = new System.Windows.Forms.Label();
            this.nameValueLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.nameLabel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.itemNameLabel1, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.itemNameLabel2, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.itemNameLabel3, 0, 6);
            this.tableLayoutPanel.Controls.Add(this.nameValueLabel, 1, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 7;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(266, 91);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.nameLabel.Location = new System.Drawing.Point(3, 0);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(89, 18);
            this.nameLabel.TabIndex = 4;
            this.nameLabel.Text = "作者名 or 名前：";
            // 
            // itemNameLabel1
            // 
            this.itemNameLabel1.AutoSize = true;
            this.itemNameLabel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.itemNameLabel1.Location = new System.Drawing.Point(30, 24);
            this.itemNameLabel1.Name = "itemNameLabel1";
            this.itemNameLabel1.Size = new System.Drawing.Size(62, 18);
            this.itemNameLabel1.TabIndex = 3;
            this.itemNameLabel1.Text = "ItemName1";
            // 
            // itemNameLabel2
            // 
            this.itemNameLabel2.AutoSize = true;
            this.itemNameLabel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.itemNameLabel2.Location = new System.Drawing.Point(30, 48);
            this.itemNameLabel2.Name = "itemNameLabel2";
            this.itemNameLabel2.Size = new System.Drawing.Size(62, 18);
            this.itemNameLabel2.TabIndex = 1;
            this.itemNameLabel2.Text = "ItemName2";
            // 
            // itemNameLabel3
            // 
            this.itemNameLabel3.AutoSize = true;
            this.itemNameLabel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.itemNameLabel3.Location = new System.Drawing.Point(30, 72);
            this.itemNameLabel3.Name = "itemNameLabel3";
            this.itemNameLabel3.Size = new System.Drawing.Size(62, 19);
            this.itemNameLabel3.TabIndex = 2;
            this.itemNameLabel3.Text = "ItemName3";
            // 
            // nameValueLabel
            // 
            this.nameValueLabel.AutoEllipsis = true;
            this.nameValueLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nameValueLabel.Location = new System.Drawing.Point(98, 0);
            this.nameValueLabel.Name = "nameValueLabel";
            this.nameValueLabel.Size = new System.Drawing.Size(165, 18);
            this.nameValueLabel.TabIndex = 5;
            this.nameValueLabel.Text = "名前 or 作者名";
            this.nameValueLabel.UseMnemonic = false;
            // 
            // InfoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "InfoControl";
            this.Size = new System.Drawing.Size(266, 91);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label itemNameLabel1;
        private System.Windows.Forms.Label itemNameLabel2;
        private System.Windows.Forms.Label itemNameLabel3;
        private System.Windows.Forms.Label nameValueLabel;
    }
}
