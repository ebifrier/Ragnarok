using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Runtime.Serialization;

using Ragnarok;
using Ragnarok.Utility;

namespace Ragnarok.Net.ProtoBuf
{
    /// <summary>
    /// proto-bufが送受信するデータです。
    /// </summary>
    /// <remarks>
    /// <para>
    /// データ長など送受信に関する情報は<see cref="PbPacketHeader"/>クラスが持ち、
    /// データ内容やデータの種類などはこのオブジェクトが持ちます。
    /// </para>
    /// 
    /// <para>
    /// <see cref="ContentTypeName"/>には<see cref="Content"/>のクラス名が
    /// 含まれています。これは受信側がこれをデシリアライズするときに必要な
    /// 情報になります。
    /// ただし、アセンブリ名までが含まれるFullQualifiedNameは使いません。
    /// なぜなら、FullQualifiedNameにはアセンブリのバージョン番号までもが
    /// 含まれており値が少しでも違うと別クラスとして扱われてしまうためです。
    /// 
    /// そのため、このライブラリでは少し曖昧になりますが、バージョン番号の
    /// チェックは行わずネームスペースまでを含んだクラス名を使いクラスの
    /// デシリアライズを行っています。
    /// 仮に全く同じ名前があった場合は、上手く動かないので注意してください。
    /// </para>
    /// </remarks>
    [DataContract()]
    internal class PbPayload2
    {
        /// <summary>
        /// 送信データＩＤを取得または設定します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// レスポンスかどうかを取得または設定します。
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        public bool IsResponse
        {
            get;
            set;
        }

        /// <summary>
        /// シリアライズされたデータのクラス名を取得します。
        /// </summary>
        [DataMember(Order = 3, IsRequired = true)]
        public string ContentTypeName
        {
            get;
            set;
        }

        /// <summary>
        /// シリアライズされたデータを取得または設定します。
        /// </summary>
        [DataMember(Order = 4, IsRequired = true)]
        public byte[] Content
        {
            get;
            set;
        }

        /// <summary>
        /// シリアライズされたデータの長さを取得します。
        /// </summary>
        public int ContentLength
        {
            get
            {
                if (Content == null)
                {
                    return 0;
                }

                return Content.Length;
            }
        }

        /// <summary>
        /// シリアライズ時に使います。
        /// </summary>
        public PbPayload2()
        {
        }
    }
}
