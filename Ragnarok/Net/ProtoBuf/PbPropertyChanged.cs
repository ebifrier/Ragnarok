using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Ragnarok.Net.ProtoBuf
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract()]
    [Serializable()]
    public class PbPropertyChanged
    {
        /// <summary>
        /// 対象となるオブジェクトのIDを取得します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public string ObjectId
        {
            get;
            set;
        }

        /// <summary>
        /// 変更されたプロパティ名を取得または設定します。
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        public string PropertyName
        {
            get;
            set;
        }

        /// <summary>
        /// 変更されたプロパティの型を取得または設定します。
        /// </summary>
        public Type PropertyType
        {
            get;
            set;
        }

        /// <summary>
        /// プロパティの型名を扱います。(protobuf用)
        /// </summary>
        [DataMember(Order = 3, IsRequired = true)]
        private string PropertyTypeName
        {
            get
            {
                if (PropertyType == null)
                {
                    return null;
                }

                return PropertyType.FullName;
            }
            set
            {
                PropertyType =
                    (string.IsNullOrEmpty(value)
                         ? null
                         : Util.FindTypeFromCurrentDomain(value));
            }
        }

        /// <summary>
        /// 新しいプロパティの値を取得または設定します。
        /// </summary>
        public object PropertyValue
        {
            get;
            set;
        }

        /// <summary>
        /// 新しいプロパティ値のバイナリデータを扱います。(protobuf用)
        /// </summary>
        [DataMember(Order = 4, IsRequired = true)]
        private byte[] PropertyData
        {
            get;
            set;
        }

        /// <summary>
        /// プロパティの更新時刻を取得します。
        /// </summary>
        [DataMember(Order = 5, IsRequired = true)]
        public DateTime Timestamp
        {
            get;
            private set;
        }

        /// <summary>
        /// データシリアライズ前にプロパティのバイナリデータを設定します。
        /// </summary>
        /// <remarks>
        /// 設定されたプロパティの値からバイナリデータを作成します。
        /// </remarks>
        public void SerializePropertyValue()
        {
            if (PropertyType == null)
            {
                throw new InvalidOperationException(
                    "プロパティデータが設定されていません。");
            }

            PropertyData = PbUtil.Serialize(PropertyValue, PropertyType);
            Timestamp = NtpClient.GetTime();
        }

        /// <summary>
        /// データデシリアライズ後にプロパティの値を設定します。
        /// </summary>
        /// <remarks>
        /// プロパティのバイナリデータからオブジェクトの値を復元します。
        /// </remarks>
        [OnDeserialized()]
        private void OnAfterDeserializing(StreamingContext context)
        {
            if (PropertyType == null)
            {
                throw new InvalidOperationException(
                    "プロパティデータが設定されていません。");
            }

            PropertyValue = PbUtil.Deserialize(PropertyData, PropertyType);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PbPropertyChanged()
        {
            Timestamp = NtpClient.GetTime();
        }
    }
}
