using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Ragnarok.NicoNico
{
    /// <summary>
    /// XML情報の共通データを持ちます。
    /// </summary>
    /// <remarks>
    /// ただの便利クラスです。
    /// </remarks>
    public class XmlInfomationBase
    {
        /// <summary>
        /// 取得時刻を取得します。
        /// </summary>
        public DateTime Time
        {
            get;
            private set;
        }

        /// <summary>
        /// ルートノードを取得します。
        /// </summary>
        public XmlNode RootNode
        {
            get;
            private set;
        }

        /// <summary>
        /// 対象オブジェクトのIDを取得します。
        /// </summary>
        public virtual string IdString
        {
            get;
            protected set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected XmlInfomationBase()
        {
            this.Time = DateTime.Now;
            this.IdString = "";
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected XmlInfomationBase(XmlNode node, string rootName,
                                    string idString)
             : this()
        {
            var root = node.SelectSingleNode("/" + rootName);
            if (root == null)
            {
                throw new NicoException(
                    "XMLがパースできません。", idString);
            }

            // 情報が正しく取得できたか調べます。
            var status = root.Attributes["status"];
            if (status == null)
            {
                throw new NicoException(
                    "情報が正しく取得できませんでした。", idString);
            }

            // 放送情報の取得に失敗した場合、その理由を返します。
            if (status.Value != "ok")
            {
                var code = root.SelectSingleNode("error/code");
                if (code != null)
                {
                    throw new NicoException(
                        NicoStatusCodeUtil.GetCode(code.InnerText), idString);
                }
                else
                {
                    throw new NicoException(
                        "不明なエラーが発生しました。", idString);
                }
            }

            // 取得時刻です。
            var time = root.Attributes["time"];
            if (time != null)
            {
                this.Time = StrUtil.ToDateTime(time.Value);
            }

            this.RootNode = root;
            this.IdString = idString;
        }
    }
}
