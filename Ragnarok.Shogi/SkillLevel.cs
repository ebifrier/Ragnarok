using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 棋力の種別です。
    /// </summary>
    [DataContract()]
    public enum SkillKind
    {
        /// <summary>
        /// 不明。
        /// </summary>
        [EnumMember()]
        Unknown,
        /// <summary>
        /// 級
        /// </summary>
        [EnumMember()]
        Kyu,
        /// <summary>
        /// 段
        /// </summary>
        [EnumMember()]
        Dan,
    }

    /// <summary>
    /// 棋力を表します。
    /// </summary>
    [Serializable()]
    [DataContract()]
    public class SkillLevel : IEquatable<SkillLevel>
    {
        /// <summary>
        /// 棋力不明。
        /// </summary>
        public static SkillLevel Unknown
        {
            get;
        } = new SkillLevel(SkillKind.Unknown, 0);

        /// <summary>
        /// 棋力の種別を取得または設定します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public SkillKind Kind
        {
            get;
            set;
        }

        /// <summary>
        /// 級位や段位を取得または設定します。
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        public int Grade
        {
            get;
            set;
        }

        /// <summary>
        /// 棋力登録時のオリジナル文字列を取得または設定します。
        /// </summary>
        [DataMember(Order = 3, IsRequired = true)]
        public string OriginalText
        {
            get;
            set;
        }

        /// <summary>
        /// オブジェクトの各プロパティが正しく設定されているか調べます。
        /// </summary>
        public bool Validate()
        {
            switch (Kind)
            {
                case SkillKind.Dan:
                    if (Grade < 1 || 10 < Grade)
                    {
                        return false;
                    }
                    break;
                case SkillKind.Kyu:
                    if (Grade < 1 || 20 < Grade)
                    {
                        return false;
                    }
                    break;
                case SkillKind.Unknown:
                    break;
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// オブジェクトの等値性を判断します。
        /// </summary>
        public override bool Equals(object other)
        {
            var result = this.PreEquals(other);
            if (result.HasValue)
            {
                return result.Value;
            }

            return Equals(other as SkillLevel);
        }

        /// <summary>
        /// オブジェクトの等値性を判断します。
        /// </summary>
        public bool Equals(SkillLevel other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return (
                Kind == other.Kind &&
                Grade == other.Grade);
        }

        /// <summary>
        /// ハッシュコードを取得します。
        /// </summary>
        public override int GetHashCode()
        {
            return (
                Kind.GetHashCode() ^
                Grade.GetHashCode());
        }

        /// <summary>
        /// 文字列化します。
        /// </summary>
        public override string ToString()
        {
            return Stringizer.ToString(this);
        }

        /// <summary>
        /// == 演算子を実装します。
        /// </summary>
        public static bool operator ==(SkillLevel lhs, SkillLevel rhs)
        {
            return Ragnarok.Util.GenericEquals(lhs, rhs);
        }

        /// <summary>
        /// != 演算子を実装します。
        /// </summary>
        public static bool operator !=(SkillLevel lhs, SkillLevel rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SkillLevel(SkillKind kind, int grade)
        {
            this.Kind = kind;
            this.Grade = grade;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SkillLevel()
        {
        }
    }
}
