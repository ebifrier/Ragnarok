using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// マスを管理します。
    /// </summary>
    [DataContract()]
    public enum Square
    {
        Empty = 0
    }

    /// <summary>
    /// マスの操作用メソッドを提供します。
    /// </summary>
    public static class SquareUtil
    {
        /// <summary>
        /// 筋・段からマスを作成します。
        /// </summary>
        public static Square Create(int file, int rank)
        {
            return (Square)((file - 1) * Board.BoardSize + (rank - 1)) + 1;
        }

        /// <summary>
        /// マスのある筋を取得します。
        /// </summary>
        public static int GetFile(this Square sq)
        {
            return (((int)sq - 1) / Board.BoardSize) + 1;
        }

        /// <summary>
        /// マスのある段を取得します。
        /// </summary>
        public static int GetRank(this Square sq)
        {
            return (((int)sq - 1) % Board.BoardSize) + 1;
        }

        /// <summary>
        /// 位置の先後を入れ替えたものを作成します。
        /// </summary>
        public static bool IsEmpty(this Square sq)
        {
            return (sq == Square.Empty);
        }

        /// <summary>
        /// 位置の先後を入れ替えたものを作成します。
        /// </summary>
        public static Square Flip(this Square sq)
        {
            return Create(
                (Board.BoardSize + 1) - sq.GetFile(),
                (Board.BoardSize + 1) - sq.GetRank());
        }

        /// <summary>
        /// 文字列化します。
        /// </summary>
        public static string ToString(this Square sq)
        {
            return $"{sq.GetFile()}{sq.GetRank()}";
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
            var square = Create(file, rank);

            if (!square.IsEmpty() && !square.Validate())
            {
                throw new FormatException(
                    source + ": 正しくないSquare型です。");
            }

            return square;
        }

        /// <summary>
        /// マスが正しい位置にあるか確かめます。
        /// </summary>
        public static bool Validate(this Square sq)
        {
            return Validate(sq.GetFile(), sq.GetRank());
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
    }
}
