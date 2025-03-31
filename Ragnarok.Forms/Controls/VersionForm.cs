﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Ragnarok.Utility;

namespace Ragnarok.Forms.Controls
{
    public partial class VersionForm : Form
    {
        private AssemblyAccessor assemblyAccessor;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public VersionForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// アセンブリ情報を取得または設定します。
        /// </summary>
        public AssemblyAccessor AssemblyAccessor
        {
            get { return this.assemblyAccessor; }
            set
            {
                if (this.assemblyAccessor != value)
                {
                    this.assemblyAccessor = value;

                    OnAssemblyAccessorChanged();
                }
            }
        }

        private void OnAssemblyAccessorChanged()
        {
            FormsUtil.UIProcess(() =>
            {
                this.appNameLabel.Text = AssemblyAccessor?.Title;
                this.versionLabel.Text = "Version " + AssemblyAccessor?.Version;
                this.copyrightLabel.Text = AssemblyAccessor?.Copyright;
            });
        }
    }
}
