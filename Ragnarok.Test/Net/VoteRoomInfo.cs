using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

using Ragnarok;
using Ragnarok.ObjectModel;

namespace Ragnarok.Test.Net
{
    /// <summary>
    /// 将棋投票中の部屋情報です。
    /// </summary>
    [DataContract()]
    [Serializable()]
    public class VoteRoomInfo : IModel, IEquatable<VoteRoomInfo>
    {
        private int id = -1;
        private string name = string.Empty;
        private int ownerNo = -1;
        private bool hasPassword = false;
        private string password;
        private DateTime baseTimeNtp;
        private TimeSpan totalVoteSpan;
        private TimeSpan voteSpan;
        private VoteParticipantInfo[] participantList =
            new VoteParticipantInfo[0];
        private DateTime createTime = DateTime.Now;

        /// <summary>
        /// プロパティの変更時に呼ばれるイベントです。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティの変更通知を出します。
        /// </summary>
        public void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                Util.SafeCall(() =>
                    handler(this, e));
            }
        }

        /// <summary>
        /// 部屋IDを取得します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                if (this.id != value)
                {
                    this.id = value;

                    this.RaisePropertyChanged("Id");
                }
            }
        }

        /// <summary>
        /// 名前を取得または設定します。
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (this.name != value)
                {
                    this.name = value;

                    this.RaisePropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// 投票ルームのオーナーの番号を取得します。
        /// </summary>
        [DataMember(Order = 3, IsRequired = true)]
        public int OwnerNo
        {
            get
            {
                return this.ownerNo;
            }
            set
            {
                if (this.ownerNo != value)
                {
                    this.ownerNo = value;

                    this.RaisePropertyChanged("OwnerNo");
                }
            }
        }

        /// <summary>
        /// パスワードを使用するかどうかを取得または設定します。
        /// </summary>
        [DataMember(Order = 4, IsRequired = true)]
        public bool HasPassword
        {
            get
            {
                return this.hasPassword;
            }
            set
            {
                if (this.hasPassword != value)
                {
                    this.hasPassword = value;

                    this.RaisePropertyChanged("HasPassword");
                }
            }
        }

        /// <summary>
        /// パスワードを取得または設定します。
        /// </summary>
        [DataMember(Order = 5, IsRequired = false)]
        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                if (this.password != value)
                {
                    this.password = value;

                    this.RaisePropertyChanged("Password");
                }
            }
        }

        /// <summary>
        /// <see cref="VoteSpan"/>の基準時刻を取得または設定です。
        /// </summary>
        [DataMember(Order = 8, IsRequired = true)]
        public DateTime BaseTimeNtp
        {
            get
            {
                return this.baseTimeNtp;
            }
            set
            {
                if (this.baseTimeNtp != value)
                {
                    this.baseTimeNtp = value;

                    this.RaisePropertyChanged("BaseTimeNtp");
                }
            }
        }

        /// <summary>
        /// 投票が終了するまでの全残り時間を取得または設定します。
        /// 投票中の場合は投票開始時からの時間です。
        /// </summary>
        [DataMember(Order = 9, IsRequired = true)]
        public TimeSpan TotalVoteSpan
        {
            get
            {
                return this.totalVoteSpan;
            }
            set
            {
                if (this.totalVoteSpan != value)
                {
                    this.totalVoteSpan = value;

                    this.RaisePropertyChanged("TotalVoteSpan");
                }
            }
        }

        /// <summary>
        /// 今の投票が終了するまでの時間間隔を取得または設定します。
        /// 投票中の場合は投票開始時からの時間です。
        /// </summary>
        [DataMember(Order = 10, IsRequired = true)]
        public TimeSpan VoteSpan
        {
            get
            {
                return this.voteSpan;
            }
            set
            {
                if (this.voteSpan != value)
                {
                    this.voteSpan = value;

                    this.RaisePropertyChanged("VoteSpan");
                }
            }
        }

        /// <summary>
        /// その部屋に接続している参加者の情報を取得または設定します。
        /// </summary>
        /// <remarks>
        /// Listのみ更新したい場合があるので、privateにしてはいけません。
        /// </remarks>
        [DataMember(Order = 11, IsRequired = true)]
        public VoteParticipantInfo[] ParticipantList
        {
            get
            {
                return this.participantList;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (this.participantList != value)
                {
                    this.participantList = value;

                    this.RaisePropertyChanged("ParticipantList");
                }
            }
        }

        /// <summary>
        /// 部屋の作成された日時を取得します。
        /// </summary>
        [DataMember(Order = 12, IsRequired = true)]
        public DateTime CreateTime
        {
            get
            {
                return this.createTime;
            }
            private set
            {
                if (this.createTime != value)
                {
                    this.createTime = value;

                    this.RaisePropertyChanged("CreateTime");
                }
            }
        }

        /// <summary>
        /// オブジェクトの各プロパティが正しく設定されているか調べます。
        /// </summary>
        public bool Validate()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                return false;
            }

            if (this.ParticipantList == null)
            {
                return false;
            }

            if (!this.ParticipantList.All(part => part.Validate()))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 等値性を判断します。
        /// </summary>
        public override bool Equals(object obj)
        {
            var other = obj as VoteRoomInfo;

            return Equals(other);
        }

        /// <summary>
        /// 等値性を判断します。
        /// </summary>
        public bool Equals(VoteRoomInfo other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return (this.Id == other.Id);
        }

        /// <summary>
        /// == 演算子を実装します。
        /// </summary>
        public static bool operator ==(VoteRoomInfo lhs, VoteRoomInfo rhs)
        {
            return Util.GenericClassEquals(lhs, rhs);
        }

        /// <summary>
        /// != 演算子を実装します。
        /// </summary>
        public static bool operator !=(VoteRoomInfo lhs, VoteRoomInfo rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// ハッシュコードを取得します。
        /// </summary>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        /// <summary>
        /// デシリアライズ後に呼ばれます。
        /// </summary>
        [OnDeserialized()]
        private void OnDeserialized(StreamingContext context)
        {
            // リストは要素数が０だとnullになることがあります。
            if (this.participantList == null)
            {
                this.participantList = new VoteParticipantInfo[0];
            }
        }
    }
}
