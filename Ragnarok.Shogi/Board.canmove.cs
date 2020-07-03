using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Shogi
{
    public partial class Board
    {
        private static readonly int[][] MoveTableGyoku = new int[][]
        {
            new int[] { 0, -1 },
            new int[] { 0, +1 },
            new int[] { -1, 0 },
            new int[] { +1, 0 },
            new int[] { -1, -1 },
            new int[] { +1, -1 },
            new int[] { -1, +1 },
            new int[] { +1, +1 },
        };

        private static readonly int[][] MoveTableKin = new int[][]
        {
            new int[] { 0, -1 },
            new int[] { 0, +1 },
            new int[] { -1, -1 },
            new int[] { +1, -1 },
            new int[] { -1, 0 },
            new int[] { +1, 0 },
        };

        private static readonly int[][] MoveTableGin = new int[][]
        {
            new int[] { 0, -1 },
            new int[] { -1, -1 },
            new int[] { +1, -1 },
            new int[] { -1, +1 },
            new int[] { +1, +1 },
        };

        private static readonly int[][] MoveTableKei = new int[][]
        {
            new int[] { -1, -2 },
            new int[] { +1, -2 },
        };

        private static readonly int[][] MoveTableHu = new int[][]
        {
            new int[] { 0, -1 },
        };

        /// <summary>
        /// テーブルを使用し、指定の相対位置に動けるか調べます。(先手専用)
        /// </summary>
        private static bool CanMoveWithTable(Colour colour,
                                             int relFile, int relRank,
                                             int[][] table)
        {
            // 後手側なら上下反転します。
            var sign = (colour == Colour.White ? -1 : +1);

            relRank *= sign;
            return table.Any(_ => relFile == _[0] && relRank == _[1]);
        }

        /// <summary>
        /// テーブルを使用し、指定の相対位置に動けるか調べます。(先手専用)
        /// </summary>
        private static IEnumerable<Square> GetMoveRangeWithTable(Colour colour,
                                                                 Square square,
                                                                 int[][] table)
        {
            // 後手側なら上下反転します。
            var sign = (colour == Colour.White ? -1 : +1);

            foreach (var elem in table)
            {
                var file = square.GetFile() + elem[0];
                var rank = square.GetRank() + elem[1] * sign;
                if (SquareUtil.Validate(file, rank))
                {
                    yield return SquareUtil.Create(file, rank);
                }
            }
        }

        #region CanMovePiece
        /// <summary>
        /// 実際に駒が動けるか確認します。
        /// </summary>
        private bool CanMovePiece(Move move)
        {
            var relFile = move.DstSquare.GetFile() - move.SrcSquare.GetFile();
            var relRank = move.DstSquare.GetRank() - move.SrcSquare.GetRank();
            var piece = move.MovePiece;

            // 成り駒が指定の場所に動けるか調べます。
            switch (piece.GetPieceType())
            {
                case Piece.Pawn:
                    return CanMoveWithTable(move.Colour, relFile, relRank, MoveTableHu);
                case Piece.Knight:
                    return CanMoveWithTable(move.Colour, relFile, relRank, MoveTableKei);
                case Piece.Lance:
                    return CanMoveKyo(move.Colour, move.SrcSquare, relFile, relRank);
                case Piece.Silver:
                    return CanMoveWithTable(move.Colour, relFile, relRank, MoveTableGin);
                case Piece.Rook:
                    return CanMoveHisya(move.SrcSquare, relFile, relRank);
                case Piece.Bishop:
                    return CanMoveKaku(move.SrcSquare, relFile, relRank);
                case Piece.ProPawn:
                case Piece.ProLance:
                case Piece.ProKnight:
                case Piece.ProSilver:
                case Piece.Gold:
                    return CanMoveWithTable(move.Colour, relFile, relRank, MoveTableKin);
                case Piece.Horse:
                    if (CanMoveWithTable(move.Colour, relFile, relRank, MoveTableGyoku))
                    {
                        return true;
                    }
                    return CanMoveKaku(move.SrcSquare, relFile, relRank);
                case Piece.Dragon:
                    if (CanMoveWithTable(move.Colour, relFile, relRank, MoveTableGyoku))
                    {
                        return true;
                    }
                    return CanMoveHisya(move.SrcSquare, relFile, relRank);
                case Piece.King:
                    return CanMoveWithTable(move.Colour, relFile, relRank, MoveTableGyoku);
            }

            return false;
        }

        /// <summary>
        /// 香車が指定の場所に動けるか判断します。
        /// </summary>
        private bool CanMoveKyo(Colour colour, Square basePos, int relFile, int relRank)
        {
            // 香車は横には動けません。
            if (relFile != 0)
            {
                return false;
            }

            // 反対方向には動けません。
            if ((colour == Colour.Black && relRank >= 0) ||
                (colour == Colour.White && relRank <= 0))
            {
                return false;
            }

            var destRank = basePos.GetRank() + relRank;
            var addRank = (relRank >= 0 ? +1 : -1);

            // 基準点には自分がいるので、とりあえず一度は
            // 駒の位置をズラしておきます。
            var baseFile = basePos.GetFile();
            var baseRank = basePos.GetRank() + addRank;

            // 駒を動かしながら、目的地まで動かします。
            // 動かす途中に何か駒があれば、目的地へは動けません。
            while (baseRank != destRank)
            {
                // 自分の駒があっても相手の駒があってもダメです。
                if (!this[baseFile, baseRank].IsNone())
                {
                    return false;
                }

                baseRank += addRank;
            }

            return true;
        }

        /// <summary>
        /// 飛車が指定の場所に動けるか判断します。
        /// </summary>
        private bool CanMoveHisya(Square basePos, int relFile, int relRank)
        {
            var baseFile = basePos.GetFile();
            var baseRank = basePos.GetRank();
            var newFile = baseFile + relFile;
            var newRank = baseRank + relRank;
            var addFile = 0;
            var addRank = 0;

            if (relFile != 0 && relRank != 0)
            {
                return false;
            }

            if (relFile == 0)
            {
                addRank = (relRank >= 0 ? +1 : -1);
            }
            else
            {
                addFile = (relFile >= 0 ? +1 : -1);
            }

            // 基準点には自分がいるので、とりあえず一度は
            // 駒の位置をズラしておきます。
            baseFile += addFile;
            baseRank += addRank;

            // 駒を動かしながら、目的地まで動かします。
            // 動かす途中に何か駒があれば、目的地へは動けません。
            while (baseFile != newFile || baseRank != newRank)
            {
                // 自分の駒があっても相手の駒があってもダメです。
                if (!this[baseFile, baseRank].IsNone())
                {
                    return false;
                }

                baseFile += addFile;
                baseRank += addRank;
            }

            return true;
        }

        /// <summary>
        /// 角が指定の場所に動けるかどうか判断します。
        /// </summary>
        private bool CanMoveKaku(Square basePos, int relFile, int relRank)
        {
            var baseFile = basePos.GetFile();
            var baseRank = basePos.GetRank();
            var newFile = baseFile + relFile;
            var newRank = baseRank + relRank;

            if (Math.Abs(relFile) != Math.Abs(relRank))
            {
                return false;
            }

            var addFile = (relFile >= 0 ? +1 : -1);
            var addRank = (relRank >= 0 ? +1 : -1);

            // 基準点には自分がいるので、とりあえず一度は
            // 駒の位置をズラしておきます。
            baseFile += addFile;
            baseRank += addRank;

            // 駒を動かしながら、目的地まで動かします。
            // 動かす途中に何か駒があれば、目的地へは動けません。
            while (baseFile != newFile || baseRank != newRank)
            {
                // 自分の駒があっても相手の駒があってもダメです。
                if (!this[baseFile, baseRank].IsNone())
                {
                    return false;
                }

                baseFile += addFile;
                baseRank += addRank;
            }

            return true;
        }
        #endregion

        #region CanMoveRange
        /// <summary>
        /// <paramref name="square"/>にある駒が移動できる
        /// 可能性のある位置をすべて列挙します。
        /// </summary>
        private IEnumerable<Square> GetCanMoveRange(Square square, Colour colour)
        {
            if (!square.Validate())
            {
                return new Square[0];
            }

            var piece = this[square];
            if (piece.IsNone() || piece.GetColour() != colour)
            {
                return new Square[0];
            }

            return GetCanMoveRange(piece, square);
        }

        /// <summary>
        /// <paramref name="square"/>にある駒が移動できる
        /// 可能性のある位置をすべて列挙します。
        /// </summary>
        private static IEnumerable<Square> GetCanMoveRange(Piece piece, Square square)
        {
            if (!square.Validate())
            {
                yield break;
            }

            if (piece.IsNone())
            {
                yield break;
            }

            IEnumerable<Square> list;
            var colour = piece.GetColour();
            var file = square.GetFile();
            var rank = square.GetRank();

            switch (piece.GetPieceType())
            {
                case Piece.Pawn:
                    list = GetMoveRangeWithTable(colour, square, MoveTableHu);
                    foreach (var p in list)
                    {
                        yield return p;
                    }
                    break;
                case Piece.Lance:
                    for (var r = 1; r <= BoardSize; ++r)
                    {
                        yield return SquareUtil.Create(file, r);
                    }
                    break;
                case Piece.Knight:
                    list = GetMoveRangeWithTable(colour, square, MoveTableKei);
                    foreach (var p in list)
                    {
                        yield return p;
                    }
                    break;
                case Piece.Silver:
                    list = GetMoveRangeWithTable(colour, square, MoveTableGin);
                    foreach (var p in list)
                    {
                        yield return p;
                    }
                    break;
                case Piece.ProPawn:
                case Piece.ProLance:
                case Piece.ProKnight:
                case Piece.ProSilver:
                case Piece.Gold:
                    list = GetMoveRangeWithTable(colour, square, MoveTableKin);
                    foreach (var p in list)
                    {
                        yield return p;
                    }
                    break;
                case Piece.King:
                case Piece.Horse:
                case Piece.Dragon:
                    list = GetMoveRangeWithTable(colour, square, MoveTableGyoku);
                    foreach (var p in list)
                    {
                        yield return p;
                    }
                    break;
            }

            // 竜・馬の場合、近傍地点はすでに調査済みなので
            // 成りか不成りかで調査範囲を変更します。
            var range = (piece.IsPromoted() ? 2 : 1);

            // 飛車角は成り／不成りに関わらず調べる箇所があります。
            switch (piece.GetRawType())
            {
                case Piece.Bishop:
                    for (var index = -BoardSize; index <= BoardSize; ++index)
                    {
                        if (piece.IsPromoted() && Math.Abs(index) < range)
                        {
                            continue;
                        }

                        if (!SquareUtil.Validate(file + index, rank + index))
                        {
                            continue;
                        }

                        yield return SquareUtil.Create(file + index, rank + index);
                    }
                    for (var index = -BoardSize; index <= BoardSize; ++index)
                    {
                        if (piece.IsPromoted() && Math.Abs(index) < range)
                        {
                            continue;
                        }

                        if (!SquareUtil.Validate(file + index, rank - index))
                        {
                            continue;
                        }

                        yield return SquareUtil.Create(file + index, rank - index);
                    }
                    break;

                case Piece.Rook:
                    for (var f = 1; f <= BoardSize; ++f)
                    {
                        if (piece.IsPromoted() && Math.Abs(file - f) < range)
                        {
                            continue;
                        }

                        if (!SquareUtil.Validate(f, rank))
                        {
                            continue;
                        }

                        yield return SquareUtil.Create(f, rank);
                    }
                    for (var r = 1; r <= BoardSize; ++r)
                    {
                        if (piece.IsPromoted() && Math.Abs(rank - r) < range)
                        {
                            continue;
                        }

                        if (!SquareUtil.Validate(file, r))
                        {
                            continue;
                        }

                        yield return SquareUtil.Create(file, r);
                    }
                    break;
            }
        }
        #endregion
    }
}
