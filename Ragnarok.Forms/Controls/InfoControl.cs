using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Ragnarok.Utility;

namespace Ragnarok.Forms.Controls
{
    /// <summary>
    /// InfoBaseクラスの情報を表示するためのコントロールです。
    /// </summary>
    public partial class InfoControl : UserControl
    {
        private readonly Label[] itemNameLabels;
        private readonly Label[] itemValueLabels;
        private InfoBase info;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InfoControl()
        {
            InitializeComponent();

            this.itemNameLabels = new Label[]
            {
                this.itemNameLabel1,
                this.itemNameLabel2,
                this.itemNameLabel3,
            };
            this.itemValueLabels = new Label[3];
        }

        /// <summary>
        /// Infoプロパティの変更通知を出します。
        /// </summary>
        public event EventHandler InfoChanged;

        /// <summary>
        /// 情報を表示するInfoBaseオブジェクトを取得します。
        /// </summary>
        public InfoBase Info
        {
            get { return this.info; }
            set
            {
                if (this.info != value)
                {
                    this.info = value;

                    InfoChanged.SafeRaiseEvent(this, EventArgs.Empty);
                    InfoUpdated();
                }
            }
        }

        /// <summary>
        /// Infoプロパティが変わった時に呼ばれます。
        /// </summary>
        private void InfoUpdated()
        {
            try
            {
                // いったんレイアウトをすべて停止します。
                this.tableLayoutPanel.SuspendLayout();

                if (Info == null)
                {
                    return;
                }

                // 名前 or 作者名
                if (string.IsNullOrEmpty(Info.AuthorName))
                {
                    this.nameLabel.Text = "名前：";
                    this.nameValueLabel.Text = Info.Title;
                }
                else
                {
                    this.nameLabel.Text = "制作者：";
                    this.nameValueLabel.Text = Info.AuthorName;
                }

                var infoItems = GetInfoItems().ToList();
                for (var i = 0; i < 3; ++i)
                {
                    // 前に作ったラベルは消しておきます。
                    if (this.itemValueLabels[i] != null)
                    {
                        this.tableLayoutPanel.Controls.Remove(this.itemValueLabels[i]);
                        this.itemValueLabels[i] = null;
                    }

                    if (i < infoItems.Count())
                    {
                        var infoItem = infoItems[i];
                        this.itemNameLabels[i].Text = infoItem.Item1 + "：";

                        // 値の追加
                        var valueLabel = CreateLabel(infoItem.Item2, infoItem.Item3);
                        this.tableLayoutPanel.Controls.Add(valueLabel, 1, 2 + i * 2);
                        this.itemValueLabels[i] = valueLabel;
                    }
                    else
                    {
                        this.itemNameLabels[i].Text = string.Empty;
                    }
                }
            }
            finally
            {
                // レイアウトの状態は必ず元に戻します。
                this.tableLayoutPanel.ResumeLayout(false);
                this.tableLayoutPanel.PerformLayout();
            }
        }

        /// <summary>
        /// コントロールに表示する情報一覧を取得します。
        /// </summary>
        private IEnumerable<Tuple<string, string, string>> GetInfoItems()
        {
            if (Info.PixivId > 0)
            {
                yield return Tuple.Create("pixiv", Info.PixivId.ToString(), info.PixivUrl);
            }

            if (!string.IsNullOrEmpty(info.NicoCommunity))
            {
                yield return Tuple.Create("nicommunity", Info.NicoCommunity, info.NicoCommunityUrl);
            }

            if (!string.IsNullOrEmpty(info.TwitterId))
            {
                yield return Tuple.Create("twitter", Info.TwitterId, info.TwitterUrl);
            }

            if (!string.IsNullOrEmpty(info.HomepageUrl))
            {
                yield return Tuple.Create("website", Info.HomepageUrl, info.HomepageUrl);
            }

            if (!string.IsNullOrEmpty(info.BlogUrl))
            {
                yield return Tuple.Create("blog", Info.BlogUrl, info.BlogUrl);
            }

            if (!string.IsNullOrEmpty(info.MailAddress))
            {
                yield return Tuple.Create("mail", Info.MailAddress, string.Empty);
            }

            if (!string.IsNullOrEmpty(Info.Comment))
            {
                yield return Tuple.Create("comment", Info.Comment, string.Empty);
            }
        }

#if false
        /// <summary>
        /// 指定のラベル名を持つラベルをレイアウトパネルに追加します。
        /// </summary>
        private void AddItem(Tuple<string, string, string> infoItem, int row)
        {
            var nameLabel = CreateLabel(infoItem.Item1);
            var valueLabel = CreateLabel(infoItem.Item2, infoItem.Item3);

            this.tableLayoutPanel.Controls.Add(nameLabel, 0, row);
            this.tableLayoutPanel.Controls.Add(valueLabel, 1, row);
        }
#endif

        /// <summary>
        /// ラベルかリンク付きラベルを作成します。
        /// </summary>
        private Label CreateLabel(string text, string link = "")
        {
            if (string.IsNullOrEmpty(link))
            {
                return new Label
                {
                    Text = text,
                    Dock = DockStyle.Fill,
                };
            }
            else
            {
                var linkLabel = new LinkLabel
                {
                    Text = text,
                    Dock = DockStyle.Fill,
                };

                linkLabel.Links.Add(0, text.Length, link);
                linkLabel.LinkClicked += OnHttpLink;
                return linkLabel;
            }
        }

        /// <summary>
        /// リンクがクリックされたときにリンク先に移動します。
        /// </summary>
        private void OnHttpLink(object sender, LinkLabelLinkClickedEventArgs e)
        {
            e.Link.Visited = true;
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
    }
}
