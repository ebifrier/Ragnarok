using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 駒の位置を保存します。
    /// </summary>
    [DataContract()]
    [TypeConverter(typeof(SquareConverter))]
    public class Square : IEquatable<Square>
    {
        /// <summary>
        /// 列を取得または設定します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public int File
        {
            get;
            set;
        }

        /// <summary>
        /// 段を取得または設定します。
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        public int Rank
        {
            get;
            set;
        }

        /// <summary>
        /// オブジェクトのコピーを作成します。
        /// </summary>
        public Square Clone()
        {
            return (Square)MemberwiseClone();
        }

        /// <summary>
        /// 位置の先後を入れ替えたものを作成します。
        /// </summary>
        public Square Flip()
        {
            return new Square(
                (Board.BoardSize + 1) - File,
                (Board.BoardSize + 1) - Rank);
        }

        /// <summary>
        /// 文字列化します。
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}{1}", File, Rank);
        }

        /// <summary>
        /// 正しい位置にあるか確かめます。
        /// </summary>
        public bool Validate()
        {
            if (File < 1 || Board.BoardSize < File ||
                Rank < 1 || Board.BoardSize < Rank)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// オブジェクトの等値性を判断します。
        /// </summary>
        public override bool Equals(object obj)
        {
            var result = this.PreEquals(obj);
            if (result.HasValue)
            {
                return result.Value;
            }

            return Equals(obj as Square);
        }

        /// <summary>
        /// オブジェクトの等値性を判断します。
        /// </summary>
        public bool Equals(Square other)
        {
            if ((object)other == null)
            {
                return false;
            }

            if (File != other.File ||
                Rank != other.Rank)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// ハッシュ値を取得します。
        /// </summary>
        public override int GetHashCode()
        {
            return (
                this.File.GetHashCode() ^
                this.Rank.GetHashCode());
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public static bool operator==(Square lhs, Square rhs)
        {
            return Ragnarok.Util.GenericEquals(lhs, rhs);
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public static bool operator !=(Square lhs, Square rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Square(int file, int rank)
        {
            File = file;
            Rank = rank;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Square()
        {
        }
    }
}
