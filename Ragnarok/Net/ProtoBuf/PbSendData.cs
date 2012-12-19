using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

using Ragnarok.Utility;

namespace Ragnarok.Net.ProtoBuf
{
    /// <summary>
    /// protobufの送信用データを保持します。
    /// </summary>
    public sealed class PbSendData
    {
        /// <summary>
        /// 送信データを取得または設定します。
        /// </summary>
        public object Data
        {
            get;
            private set;
        }

        /// <summary>
        /// シリアライズされた送信データを取得します。
        /// </summary>
        public byte[] SerializedData
        {
            get;
            private set;
        }

        /// <summary>
        /// 送信データの型名を取得します。
        /// </summary>
        public string TypeName
        {
            get;
            private set;
        }

        /// <summary>
        /// エンコードされた送信データの型名を取得します。
        /// </summary>
        public string EncodedTypeName
        {
            get;
            private set;
        }

        /// <summary>
        /// 送信データのエンコードされたデータを取得します。
        /// </summary>
        public byte[] EncodedTypeData
        {
            get;
            private set;
        }

        /// <summary>
        /// 準備したデータをシリアライズします。
        /// </summary>
        public void Serialize()
        {
            if (Data == null)
            {
                throw new PbException("Dataがnullです。");
            }

            SerializedData = PbUtil.Serialize(Data, Data.GetType());

            // 型名はPbConnectionでエンコードします。
            TypeName = TypeSerializer.Serialize(Data.GetType());
            EncodedTypeName = PbConnection.EncodeTypeName(TypeName);
            EncodedTypeData = Encoding.UTF8.GetBytes(EncodedTypeName);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PbSendData(object data)
        {
            Data = data;
        }
    }
}
