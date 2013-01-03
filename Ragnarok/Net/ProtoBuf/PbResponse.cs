using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ProtoBuf;

namespace Ragnarok.Net.ProtoBuf
{
    /// <summary>
    /// エラー時に使うレスポンス型です。
    /// </summary>
    [DataContract()]
    [Serializable()]
    internal sealed class PbDummy
    {
    }

    /// <summary>
    /// レスポンスを扱います。
    /// </summary>
    internal interface IPbResponse
    {
        /// <summary>
        /// レスポンスの値を取得または設定します。
        /// </summary>
        object Response
        {
            get;
            set;
        }

        /// <summary>
        /// エラーコードを取得または設定します。
        /// </summary>
        int ErrorCode
        {
            get;
            set;
        }
    }

    /// <summary>
    /// レスポンス型が指定された<see cref="IPbResponse"/>のクラスです。
    /// </summary>
    [DataContract()]
    [Serializable()]
    internal sealed class PbResponse<TRes> : IPbResponse
        where TRes: class
    {
        /// <summary>
        /// エラーコードを取得または設定します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public int ErrorCode
        {
            get;
            set;
        }

        /// <summary>
        /// レスポンスの値を取得または設定します。
        /// </summary>
        public object Response
        {
            get { return TypedResponse; }
            set { TypedResponse = (TRes)value; }
        }

        /// <summary>
        /// 型付けされたレスポンスをシリアライズ用に保持します。
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        private TRes TypedResponse
        {
            get;
            set;
        }
    }
}
