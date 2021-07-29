using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Go
{
    /// <summary>
    /// 碁の手を管理します。
    /// </summary>
    public static class Move
    {
        /// <summary>
        /// SGF形式の文字列から打ち手を作成します。
        /// </summary>
        public static Tuple<Square, Stone> ParseSgf(string sgf, int boardSize)
        {
            if (string.IsNullOrEmpty(sgf))
            {
                throw new ArgumentNullException("sgf");
            }

            if (boardSize < 3 || boardSize > 27)
            {
                throw new ArgumentOutOfRangeException("boardSize");
            }

            if ((boardSize & 1) == 0)
            {
                throw new ArgumentException(
                    "boardSize must be odd.");
            }

            var stoneC = sgf[0];
            var stone = (
                stoneC == 'B' ? Stone.Black :
                stoneC == 'W' ? Stone.White :
                Stone.Empty);
            if (stone == Stone.Empty)
            {
                return null;
            }

            if (sgf[1] != '[' || sgf[4] != ']')
            {
                return null;
            }

            var col = sgf[2] - 'a';
            var row = sgf[3] - 'a';
            if (col < 0 || col >= boardSize || row < 0 || row >= boardSize)
            {
                return null;
            }

            return Tuple.Create(Square.Create(col, row, boardSize), stone);
        }

        /// <summary>
        /// 石をアルファベットに変換します。
        /// </summary>
        private static string StoneText(Stone stone)
        {
            return (
                stone == Stone.Black ? "B" :
                stone == Stone.White ? "W" :
                stone == Stone.Error ? "E" : "?");
        }

        /// <summary>
        /// 打ち手の文字列表現をSGF形式(B[aa], W[rd], PASSなど)で取得します。
        /// </summary>
        public static string ToSgf(Square sq, Stone stone)
        {
            if (sq.IsEmpty)
            {
                return "";
            }
            else if (sq.IsPass)
            {
                return "[]";
            }
            else
            {
                return $"{StoneText(stone)}{sq.ToSgf()}";
            }
        }

        /// <summary>
        /// 打ち手の文字列表現を日本形式(B1-1, W19-7, PASSなど)で取得します。
        /// </summary>
        public static string ToJstr(Square sq, Stone stone)
        {
            if (sq.IsEmpty)
            {
                return "";
            }
            else if (sq.IsPass)
            {
                return "PASS";
            }
            else
            {
                return $"{StoneText(stone)}{sq.ToJstr()}";
            }
        }
    }
}
