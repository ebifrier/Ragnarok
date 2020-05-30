using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 駒の位置を保存します。
    /// </summary>
    [DataContract()]
    [TypeConverter(typeof(SquareConverter))]
    public readonly struct Square : IEquatable<Square>
    {
        /// <summary>
        /// 空のマスを取得します。
        /// </summary>
        public static readonly Square Empty = new Square();

        /// <summary>
        /// 列を取得または設定します。
        /// </summary>
        [DataMember(Order = 1, IsRequired = true)]
        public int File
        {
            get;
        }

        /// <summary>
        /// 段を取得または設定します。
        /// </summary>
        [DataMember(Order = 2, IsRequired = true)]
        public int Rank
        {
            get;
        }

        /// <summary>
        /// File-Rankを0～80までの整数にして取得します。
        /// </summary>
        public int Index
        {
            get { return ((File - 1) * 9 + (Rank - 1)); }
        }

        /// <summary>
        /// 空のマスかどうかを調べます。
        /// </summary>
        public bool IsEmpty
        {
            get { return (this == Empty); }
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
            return $"{File}{Rank}";
        }

        /// <summary>
        /// 文字列からSquareオブジェクトを作成します。
        /// </summary>
        public static Square Parse(string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var trimmedSource = source.Trim();
            if (trimmedSource.Length != 2)
            {
                throw new FormatException(
                    $"{source}: Square型への変換に失敗しました。");
            }

            var file = int.Parse(
                trimmedSource.Substring(0, 1),
                CultureInfo.InvariantCulture);
            var rank = int.Parse(
                trimmedSource.Substring(1, 1),
                CultureInfo.InvariantCulture);
            var square = new Square(file, rank);

            if (!square.IsEmpty && !square.Validate())
            {
                throw new FormatException(
                    source + ": 正しくないSquare型です。");
            }

            return square;
        }

        /// <summary>
        /// 正しい位置にあるか確かめます。
        /// </summary>
        public bool Validate()
        {
            return Validate(File, Rank);
        }

        /// <summary>
        /// マスが正しい位置にあるか確かめます。
        /// </summary>
        public static bool Validate(int file, int rank)
        {
            if (file < 1 || Board.BoardSize < file ||
                rank < 1 || Board.BoardSize < rank)
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

            return Equals((Square)obj);
        }

        /// <summary>
        /// オブジェクトの等値性を判断します。
        /// </summary>
        public bool Equals(Square other)
        {
            if (File != other.File || Rank != other.Rank)
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
            return (File.GetHashCode() ^ Rank.GetHashCode());
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public static bool operator==(Square lhs, Square rhs)
        {
            return Util.GenericEquals(lhs, rhs);
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public static bool operator !=(Square lhs, Square rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// オブジェクトを比較します。(nullとの比較用)
        /// </summary>
        public static bool operator ==(Square lhs, Square? rhs)
        {
            if (rhs == null)
            {
                return lhs.IsEmpty;
            }

            return Util.GenericEquals(lhs, rhs.Value);
        }

        /// <summary>
        /// オブジェクトを比較します。(nullとの比較用)
        /// </summary>
        public static bool operator !=(Square lhs, Square? rhs)
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
        /// 0～80までの整数を行と列に変換します。
        /// </summary>
        public Square(int index)
        {
            if (index < 0 || 81 <= index)
            {
                throw new IndexOutOfRangeException();
            }

            File = (index % 9) + 1;
            Rank = (index / 9) + 1;
        }
    }
}
