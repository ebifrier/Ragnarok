using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Go
{
    /// <summary>
    /// 盤上の各交点を示すオブジェクトです。
    /// </summary>
    public struct Square : IEquatable<Square>
    {
        /// <summary>
        /// 碁盤すべての位置を返します。
        /// </summary>
        public static Square[] All(int boardSize)
        {
            return (from r in Enumerable.Range(0, boardSize)
                    from f in Enumerable.Range(0, boardSize)
                    select Create(f, r, boardSize))
                   .ToArray();
        }

        /// <summary>
        /// 位置がない場合のオブジェクト
        /// </summary>
        public static Square Empty()
        {
            return new Square();
        }

        /// <summary>
        /// パス用のオブジェクト
        /// </summary>
        public static Square Pass()
        {
            return new Square(-1, 19);
        }

        /// <summary>
        /// 段列を持った石の位置オブジェクトを作成します。
        /// </summary>
        public static Square Create(int col, int row, int boardSize)
        {
            if (boardSize < 3 || boardSize > 27)
            {
                throw new ArgumentOutOfRangeException("boardSize");
            }

            if ((boardSize & 1) == 0)
            {
                throw new ArgumentException(
                    "boardSize must be odd.");
            }

            if (col < 0 || col >= boardSize)
            {
                throw new ArgumentOutOfRangeException("file");
            }

            if (row < 0 || row >= boardSize)
            {
                throw new ArgumentOutOfRangeException("rank");
            }

            return new Square(
                (row + 1) * (boardSize + 2) + (col + 1), boardSize);
        }

        /// <summary>
        /// 段列を持った石の位置オブジェクトを作成します。
        /// </summary>
        public static Square FromIndex(int index, int boardSize)
        {
            if (boardSize < 3 || boardSize > 27)
            {
                throw new ArgumentOutOfRangeException(nameof(boardSize));
            }

            if ((boardSize & 1) == 0)
            {
                throw new ArgumentException(
                    "boardSize must be odd.");
            }

            if (index < 0 || index >= (boardSize + 2) * (boardSize + 2))
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return new Square(index, boardSize);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private Square(int index, int boardSize)
        {
            Index = index;
            BoardSize = boardSize;
        }

        /// <summary>
        /// クローン
        /// </summary>
        public Square Clone()
        {
            return (Square)MemberwiseClone();
        }

        /// <summary>
        /// 列段を示すインデックスを取得します。
        /// </summary>
        public int Index
        {
            get;
            private set;
        }

        /// <summary>
        /// 列を取得します。
        /// </summary>
        public int Col
        {
            get { return (BoardSize == 0 ? -1 : Index % BoardSize2 - 1); }
        }

        /// <summary>
        /// 段を取得します。
        /// </summary>
        public int Row
        {
            get { return (BoardSize == 0 ? -1 : Index / BoardSize2 - 1); }
        }

        /// <summary>
        /// 碁盤のサイズを取得します。
        /// </summary>
        public int BoardSize
        {
            get;
            private set;
        }

        /// <summary>
        /// BoardSize + 2を取得します。
        /// </summary>
        public int BoardSize2
        {
            get { return BoardSize + 2; }
        }

        /// <summary>
        /// 空かどうかを取得します。
        /// </summary>
        public bool IsEmpty
        {
            get { return (BoardSize == 0 && Index == 0); }
        }

        /// <summary>
        /// パスかどうかを取得します。
        /// </summary>
        public bool IsPass
        {
            get { return (Index < 0); }
        }

        /// <summary>
        /// 位置を90 * <paramref name="times90" />度反転したオブジェクトを返します。
        /// </summary>
        public Square Rotate(int times90)
        {
            if (IsEmpty || IsPass) return this;
            times90 = (times90 + 4) % 4;

            switch (times90)
            {
                case 0:
                    return Create(Col, Row, BoardSize);
                case 1:
                    return Create(BoardSize - Row - 1, Col, BoardSize);
                case 2:
                    return Create(BoardSize - Col - 1, BoardSize - Row - 1, BoardSize);
                case 3:
                    return Create(Row, BoardSize - Col - 1, BoardSize);
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// 位置を180度反転したオブジェクトを返します。
        /// </summary>
        public Square Inv()
        {
            return Rotate(2);
        }

        /// <summary>
        /// オブジェクトの状態が正しいか確認します。
        /// </summary>
        public bool IsOk()
        {
            if (IsPass)
            {
                return true;
            }

            if (BoardSize < 3 || BoardSize > 27)
            {
                return false;
            }

            if ((BoardSize & 1) == 0)
            {
                return false;
            }

            if (Col < 0 || Col >= BoardSize)
            {
                return false;
            }

            if (Row < 0 || Row >= BoardSize)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 交点をSGF文字列に変換します。
        /// </summary>
        public string ToSgf()
        {
            if (IsEmpty) return "";
            if (IsPass) return "[]";

            var colc = (char)('a' + Col);
            var rowc = (char)('a' + Row);
            return $"[{colc}{rowc}]";
        }

        /// <summary>
        /// 交点を日本用の文字列に変換します。
        /// </summary>
        public string ToJstr()
        {
            if (IsEmpty) return "";
            if (IsPass) return "PASS";

            return $"{Col+1}-{Row+1}";
        }

        /// <summary>
        /// 交点をヨーロッパ形式の文字列に変換します。
        /// </summary>
        public string ToEstr()
        {
            if (IsEmpty) return string.Empty;
            if (IsPass) return "pass";

            var col = (char)((int)'A' + Col + (Col >= 8 ? 1 : 0));
            var row = BoardSize - Row;
            return $"{col}{row}";
        }

        /// <summary>
        /// ヨーロッパ形式の交点をパースします。
        /// </summary>
        public static Square ParseEstr(string europe, int boardSize)
        {
            if (europe.Length < 2)
            {
                throw new ArgumentException($"'{europe}' is invalid format");
            }

            if (!int.TryParse(europe.Substring(1), out int rawRow))
            {
                throw new ArgumentException($"'{europe}' is invalid format");
            }

            var col = (europe[0] - 'A') - (europe[0] >= 'I' ? 1 : 0);
            var row = boardSize - rawRow;
            return Create(col, row, boardSize);
        }

        /// <summary>
        /// 文字列化します。
        /// </summary>
        public override string ToString()
        {
            return ToSgf();
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public override bool Equals(object obj)
        {
            var status = this.PreEquals(obj);
            if (status != null)
            {
                return status.Value;
            }

            return Equals((Square)obj);
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public bool Equals(Square other)
        {
            if (IsEmpty && other.IsEmpty)
            {
                return true;
            }

            if (IsPass && other.IsPass)
            {
                return true;
            }

            if (BoardSize != other.BoardSize)
            {
                return false;
            }

            if (Index != other.Index)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// ハッシュ値を計算します。
        /// </summary>
        public override int GetHashCode()
        {
            return (
                BoardSize * BoardSize * 10000 +
                (IsPass ? 1000 : 0) +
                Index);
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public static bool operator ==(Square x, Square y)
        {
            return Util.GenericEquals(x, y);
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public static bool operator !=(Square x, Square y)
        {
            return !(x == y);
        }

        public static Square operator +(Square x, int offset)
        {
            return Square.FromIndex(x.Index + offset, x.BoardSize);
        }

        public static Square operator -(Square x, int offset)
        {
            return Square.FromIndex(x.Index - offset, x.BoardSize);
        }
    }
}
