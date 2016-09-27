using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Ragnarok.NicoNico.Live
{
    /// <summary>
    /// 生放送情報の共通データを持ちます。
    /// </summary>
    /// <remarks>
    /// ただの便利クラスです。
    /// </remarks>
    public class LiveInfomationBase
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
        /// 対象オブジェクトのIDを取得します。(主にデバッグ用)
        /// </summary>
        public string IdString
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected LiveInfomationBase()
        {
            this.Time = DateTime.Now;
            this.IdString = "";
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected LiveInfomationBase(XmlNode node, string rootName,
                                     string idString)
             : this()
        {
            var root = node.SelectSingleNode("/" + rootName);
            if (root == null)
            {
                throw new NicoLiveException(
                    NicoStatusCode.XmlParseError, idString);
            }

            // 放送情報が正しく取得できたか調べます。
            var status = root.Attributes["status"];
            if (status == null)
            {
                throw new NicoLiveException(
                    NicoStatusCode.XmlParseError, idString);
            }

            // 放送情報の取得に失敗した場合、その理由を返します。
            if (status.Value != "ok")
            {
                var code = root.SelectSingleNode("error/code");
                if (code != null)
                {
                    throw new NicoLiveException(
                        NicoStatusCodeUtil.GetCode(code.InnerText), idString);
                }
                else
                {
                    throw new NicoLiveException(
                        NicoStatusCode.UnknownError, idString);
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
