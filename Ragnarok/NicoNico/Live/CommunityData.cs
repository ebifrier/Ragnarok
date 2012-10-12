using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Ragnarok.NicoNico.Live
{
    /// <summary>
    /// 放送提供者の情報です。
    /// </summary>
    [DataContract()]
    public class ProviderData
    {
        /// <summary>
        /// 提供者のタイプを取得または設定します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public ProviderType ProviderType
        {
            get;
            set;
        }

        /// <summary>
        /// ＩＤを取得または設定します。
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// ＩＤ文字列を取得します。
        /// </summary>
        public string IdString
        {
            get
            {
                switch (this.ProviderType)
                {
                    case ProviderType.Unknown:
                        return "unknown";
                    case Live.ProviderType.Community:
                        return string.Format("co{0}", Id);
                    case Live.ProviderType.Channel:
                        return string.Format("ch{0}", Id);
                    case Live.ProviderType.Official:
                        return string.Format("official");
                }

                throw new InvalidOperationException(
                    string.Format(
                        "不明な提供者タイプです。({0})",
                        (int)ProviderType));
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ProviderData(ProviderType type, int id)
        {
            this.ProviderType = type;
            this.Id = id;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        [Obsolete("protobuf用のコンストラクタです。")]
        protected ProviderData()
        {
        }
    }
}
