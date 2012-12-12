using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Ragnarok.Test.Net
{
    /// <summary>
    /// 投票に参加しているユーザーの情報です。
    /// </summary>
    [DataContract()]
    [Serializable()]
    public class VoteParticipantInfo
    {
        /// <summary>
        /// GUIDに変換可能な識別子を取得または設定します。
        /// </summary>
        [DataMember(Order = 10, IsRequired = true)]
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// 参加者の一意な番号を取得または設定します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public int No
        {
            get;
            set;
        }

        /// <summary>
        /// ユーザー名を取得します。
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 省略されたユーザー名を取得します。
        /// </summary>
        public string ShortName
        {
            get
            {
                var name = Ragnarok.Util.HankakuSubstring(Name, 10);
                if (name == Name)
                {
                    return name;
                }

                return name + "...";
            }
        }

        /// <summary>
        /// ユーザー画像へのURLを取得します。
        /// </summary>
        [DataMember(Order = 3, IsRequired = true)]
        public string ImageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// オブジェクトをニコ生のコメンターとして使うかどうかを
        /// 取得または設定します
        /// </summary>
        [DataMember(Order = 4, IsRequired = true)]
        public bool IsUseAsNicoCommenter
        {
            get;
            set;
        }

        /// <summary>
        /// 放送主としてやっている放送URLを取得または設定します。
        /// </summary>
        [DataMember(Order = 6, IsRequired = false)]
        public LiveData[] LiveDataList
        {
            get;
            set;
        }

        /// <summary>
        /// 一言メッセージを取得または設定します。
        /// </summary>
        [DataMember(Order = 7, IsRequired = false)]
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 短い一言メッセージを取得します。
        /// </summary>
        public string ShortMessage
        {
            get
            {
                var message = Ragnarok.Util.HankakuSubstring(Message, 10);
                if (Message == message)
                {
                    return message;
                }

                return message + "...";
            }
        }

        /// <summary>
        /// オブジェクトのプロパティ値が正しいかどうか判断します。
        /// </summary>
        public bool Validate()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return false;
            }

            if (LiveDataList == null)
            {
                return false;
            }

            // 放送データはなくてもかまいませんが、もし放送データが
            // 存在する場合、その内容が間違っていれば不正な値とします。
            if (LiveDataList.Any(_ => !_.Validate()))
            {
                return false;
            }

            // 画像URLは無くてもOKとします。
            return true;
        }

        /// <summary>
        /// デシリアライズ後に呼ばれます。
        /// </summary>
        [OnDeserialized()]
        private void AfterDeserialized(StreamingContext context)
        {
            // protobufは、配列の要素数が０だと、
            // nullになることがあります。
            if (LiveDataList == null)
            {
                LiveDataList = new LiveData[0];
            }
        }
    }
}
