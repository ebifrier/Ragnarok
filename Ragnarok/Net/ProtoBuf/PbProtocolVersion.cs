using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Ragnarok.Net.ProtoBuf
{
    /// <summary>
    /// C/S間のプロトコルのバージョン情報です。
    /// </summary>
    /// <remarks>
    /// メジャー/マイナー/リビジョンの各値はそれぞれ999が
    /// 最大値になっています。
    /// 
    /// protobufでシリアライズ/デシリアライズするために必要です。
    /// </remarks>
    [DataContract()]
    [Serializable()]
    public class PbProtocolVersion :
        IComparable<PbProtocolVersion>, IEquatable<PbProtocolVersion>
    {
        /// <summary>
        /// メジャーバージョンを取得します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public int MajorVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// マイナーバージョンを取得します。
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        public int MinorVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// リビジョンを取得します。
        /// </summary>
        [DataMember(Order = 3, IsRequired = true)]
        public int Revision
        {
            get;
            private set;
        }

        /// <summary>
        /// バージョン番号を取得します。
        /// </summary>
        /// <remarks>
        /// バージョン番号は
        /// <see cref="MajorVersion"/> * 1000000 +
        /// <see cref="MinorVersion"/> * 1000 +
        /// <see cref="Revision"/>
        /// となります。
        /// </remarks>
        public int Version
        {
            get
            {
                return (
                    this.MajorVersion * 1000000 +
                    this.MinorVersion * 1000 +
                    this.Revision);
            }
        }

        /// <summary>
        /// オブジェクトのプロパティが正しいか調べます。
        /// </summary>
        public bool Validate()
        {
            if (MajorVersion < 0 || 1000 <= MajorVersion)
            {
                return false;
            }

            if (MinorVersion < 0 || 1000 <= MinorVersion)
            {
                return false;
            }

            if (Revision < 0 || 1000 <= Revision)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// バージョンの比較を行います。
        /// </summary>
        public int CompareTo(PbProtocolVersion other)
        {
            if (this.MajorVersion != other.MajorVersion)
            {
                return (this.MajorVersion - other.MajorVersion);
            }

            if (this.MinorVersion != other.MinorVersion)
            {
                return (this.MinorVersion - other.MinorVersion);
            }

            if (this.Revision != other.Revision)
            {
                return (this.Revision - other.Revision);
            }

            return 0;
        }

        /// <summary>
        /// バージョンの等値性を判断します。
        /// </summary>
        public override bool Equals(object obj)
        {
            var other = obj as PbProtocolVersion;

            return Equals(other);
        }

        /// <summary>
        /// バージョンの等値性を判断します。
        /// </summary>
        public bool Equals(PbProtocolVersion other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return (
                this.MajorVersion == other.MajorVersion &&
                this.MinorVersion == other.MinorVersion &&
                this.Revision == other.Revision);
        }

        /// <summary>
        /// ハッシュコードを取得します。
        /// </summary>
        public override int GetHashCode()
        {
            return Version.GetHashCode();
        }

        /// <summary>
        /// == 演算子を実装します。
        /// </summary>
        public static bool operator ==(PbProtocolVersion lhs, PbProtocolVersion rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// != 演算子を実装します。
        /// </summary>
        public static bool operator !=(PbProtocolVersion lhs, PbProtocolVersion rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// &lt; 演算子を実装します。
        /// </summary>
        public static bool operator <(PbProtocolVersion lhs, PbProtocolVersion rhs)
        {
            return (lhs.CompareTo(rhs) < 0);
        }

        /// <summary>
        /// &gt; 演算子を実装します。
        /// </summary>
        public static bool operator >(PbProtocolVersion lhs, PbProtocolVersion rhs)
        {
            return (rhs < lhs);
        }

        /// <summary>
        /// &lt;= 演算子を実装します。
        /// </summary>
        public static bool operator <=(PbProtocolVersion lhs, PbProtocolVersion rhs)
        {
            return !(rhs < lhs);
        }

        /// <summary>
        /// &gt;= 演算子を実装します。
        /// </summary>
        public static bool operator >=(PbProtocolVersion lhs, PbProtocolVersion rhs)
        {
            return !(lhs < rhs);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PbProtocolVersion(int major, int minor, int revision)
        {
            this.MajorVersion = major;
            this.MinorVersion = minor;
            this.Revision = revision;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PbProtocolVersion(int major, int minor)
            : this(major, minor, 0)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PbProtocolVersion(int major)
            : this(major, 0, 0)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PbProtocolVersion()
            : this(0, 0, 0)
        {
        }
    }
}
