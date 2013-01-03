using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 各プレイヤーの情報を保持します。
    /// </summary>
    [Serializable()]
    [DataContract()]
    public class ShogiPlayer : IEquatable<ShogiPlayer>
    {
        /// <summary>
        /// 解析に使われた文字列を取得または設定します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = false)]
        public string OriginalText
        {
            get;
            set;
        }

        /// <summary>
        /// プレイヤーＩＤを取得または設定します。
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        public string PlayerId
        {
            get;
            set;
        }

        /// <summary>
        /// ユーザーのニックネームを取得または設定します。
        /// </summary>
        /// <remarks>
        /// ニックネームは無い場合もあります。
        /// </remarks>
        [DataMember(Order = 3, IsRequired = false)]
        public string Nickname
        {
            get;
            set;
        }

        /// <summary>
        /// 棋力を取得または設定します。
        /// </summary>
        [DataMember(Order = 4, IsRequired = true)]
        public SkillLevel SkillLevel
        {
            get;
            set;
        }

        /// <summary>
        /// オブジェクトの各プロパティが正しく設定されているか調べます。
        /// </summary>
        public bool Validate()
        {
            if (string.IsNullOrEmpty(PlayerId))
            {
                return false;
            }

            if (SkillLevel == null || !SkillLevel.Validate())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 文字列化します。
        /// </summary>
        public override string ToString()
        {
            return Stringizer.ToString(this);
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

            return Equals(other as ShogiPlayer);
        }

        /// <summary>
        /// オブジェクトの等値性を判断します。
        /// </summary>
        public bool Equals(ShogiPlayer other)
        {
            if ((object)other == null)
            {
                return false;
            }

            if (PlayerId != other.PlayerId)
            {
                return false;
            }

            // 棋力では判断しません。
            /*if (!this.SkillLevel.Equals(other.SkillLevel))
            {
                return false;
            }*/

            return true;
        }

        /// <summary>
        /// ハッシュコードを取得します。
        /// </summary>
        public override int GetHashCode()
        {
            return (
                PlayerId.GetHashCode() /*^
                SkillLevel.GetHashCode()*/);
        }

        /// <summary>
        /// == 演算子を実装します。
        /// </summary>
        public static bool operator ==(ShogiPlayer lhs, ShogiPlayer rhs)
        {
            return Ragnarok.Util.GenericEquals(lhs, rhs);
        }

        /// <summary>
        /// != 演算子を実装します。
        /// </summary>
        public static bool operator !=(ShogiPlayer lhs, ShogiPlayer rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ShogiPlayer()
        {
            PlayerId = string.Empty;
            Nickname = string.Empty;
            SkillLevel = new SkillLevel();
        }
    }
}
