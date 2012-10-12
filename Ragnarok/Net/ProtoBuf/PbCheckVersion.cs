using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Ragnarok.Utility;

namespace Ragnarok.Net.ProtoBuf
{
    /// <summary>
    /// プロトコルのバージョンチェックをしたその結果です。
    /// </summary>
    public enum PbVersionCheckResult
    {
        /// <summary>
        /// OK
        /// </summary>
        [LabelDescription(Description = "バージョンはOKです。")]
        Ok,

        /// <summary>
        /// 不明なエラー
        /// </summary>
        [LabelDescription(Description = "不明なエラーです。")]
        Unknown,

        /// <summary>
        /// 値が正しくありません。
        /// </summary>
        [LabelDescription(Description = "バージョン値が正しくありません。")]
        InvalidValue,

        /// <summary>
        /// プロトコルのバージョンが高すぎます。
        /// </summary>
        [LabelDescription(Description = "バージョンが高すぎます。")]
        TooUpper,

        /// <summary>
        /// プロトコルのバージョンが低すぎます。
        /// </summary>
        [LabelDescription(Description = "バージョンが低すぎます。")]
        TooLower,

        /// <summary>
        /// タイムアウトしました。
        /// </summary>
        [LabelDescription(Description = "タイムアウトしました。")]
        Timeout,
    }

    /// <summary>
    /// プロトコルのバージョンチェックを行います。
    /// </summary>
    [DataContract()]
    [Serializable()]
    public class PbCheckProtocolVersionRequest
    {
        /// <summary>
        /// プロトコルのバージョンを取得または設定します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public PbProtocolVersion ProtocolVersion
        {
            get;
            set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PbCheckProtocolVersionRequest(PbProtocolVersion version)
        {
            ProtocolVersion = version;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PbCheckProtocolVersionRequest()
        {
        }
    }

    /// <summary>
    /// プロトコルのバージョンチェックを行います。
    /// </summary>
    [DataContract()]
    [Serializable()]
    public class PbCheckProtocolVersionResponse
    {
        /// <summary>
        /// バージョンチェックの結果を取得または設定します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public PbVersionCheckResult Result
        {
            get;
            set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PbCheckProtocolVersionResponse(PbVersionCheckResult result)
        {
            Result = result;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PbCheckProtocolVersionResponse()
        {
        }
    }
}
