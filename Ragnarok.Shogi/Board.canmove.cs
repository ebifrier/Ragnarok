﻿using System;
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
        private bool CanMoveWithTable(BWType bwType, int relFile, int relRank,
                                      int[][] table)
        {
            // 後手側なら上下反転します。
            var sign = (bwType == BWType.White ? -1 : +1);

            relRank *= sign;
            return table.Any(_ => relFile == _[0] && relRank == _[1]);
        }

        /// <summary>
        /// テーブルを使用し、指定の相対位置に動けるか調べます。(先手専用)
        /// </summary>
        private IEnumerable<Square> GetMoveRangeWithTable(BWType bwType,
                                                          Square square,
                                                          int[][] table)
        {
            // 後手側なら上下反転します。
            var sign = (bwType == BWType.White ? -1 : +1);

            foreach (var elem in table)
            {
                var sq = new Square(
                    square.File + elem[0],
                    square.Rank + elem[1] * sign);

                if (sq.Validate())
                {
                    yield return sq;
                }
            }
        }

        #region CanMovePiece
        /// <summary>
        /// 実際に駒が動けるか確認します。
        /// </summary>
        private bool CanMovePiece(Move move)
        {
            var relFile = move.DstSquare.File - move.SrcSquare.File;
            var relRank = move.DstSquare.Rank - move.SrcSquare.Rank;
            var piece = move.MovePiece;

            if (piece.IsPromoted)
            {
                // 成り駒が指定の場所に動けるか調べます。
                switch (piece.PieceType)
                {
                    case PieceType.Gyoku:
                        return CanMoveWithTable(move.BWType, relFile, relRank, MoveTableGyoku);
                    case PieceType.Hisya:
                        if (CanMoveWithTable(move.BWType, relFile, relRank, MoveTableGyoku))
                        {
                            return true;
                        }
                        return CanMoveHisya(move.SrcSquare, relFile, relRank);
                    case PieceType.Kaku:
                        if (CanMoveWithTable(move.BWType, relFile, relRank, MoveTableGyoku))
                        {
                            return true;
                        }
                        return CanMoveKaku(move.SrcSquare, relFile, relRank);
                    case PieceType.Kin:
                    case PieceType.Gin:
                    case PieceType.Kei:
                    case PieceType.Kyo:
                    case PieceType.Hu:
                        return CanMoveWithTable(move.BWType, relFile, relRank, MoveTableKin);
                }
            }
            else
            {
                // 成り駒以外の駒が指定の場所に動けるか調べます。
                switch (piece.PieceType)
                {
                    case PieceType.Gyoku:
                        return CanMoveWithTable(move.BWType, relFile, relRank, MoveTableGyoku);
                    case PieceType.Hisya:
                        return CanMoveHisya(move.SrcSquare, relFile, relRank);
                    case PieceType.Kaku:
                        return CanMoveKaku(move.SrcSquare, relFile, relRank);
                    case PieceType.Kin:
                        return CanMoveWithTable(move.BWType, relFile, relRank, MoveTableKin);
                    case PieceType.Gin:
                        return CanMoveWithTable(move.BWType, relFile, relRank, MoveTableGin);
                    case PieceType.Kei:
                        return CanMoveWithTable(move.BWType, relFile, relRank, MoveTableKei);
                    case PieceType.Kyo:
                        return CanMoveKyo(move.BWType, move.SrcSquare, relFile, relRank);
                    case PieceType.Hu:
                        return CanMoveWithTable(move.BWType, relFile, relRank, MoveTableHu);
                }
            }

            return false;
        }

        /// <summary>
        /// 香車が指定の場所に動けるか判断します。
        /// </summary>
        private bool CanMoveKyo(BWType bwType, Square basePos, int relFile, int relRank)
        {
            // 香車は横には動けません。
            if (relFile != 0)
            {
                return false;
            }

            // 反対方向には動けません。
            if ((bwType == BWType.Black && relRank >= 0) ||
                (bwType == BWType.White && relRank <= 0))
            {
                return false;
            }

            var destRank = basePos.Rank + relRank;
            var addRank = (relRank >= 0 ? +1 : -1);

            // 基準点には自分がいるので、とりあえず一度は
            // 駒の位置をズラしておきます。
            var baseFile = basePos.File;
            var baseRank = basePos.Rank + addRank;

            // 駒を動かしながら、目的地まで動かします。
            // 動かす途中に何か駒があれば、目的地へは動けません。
            while (baseRank != destRank)
            {
                // 自分の駒があっても相手の駒があってもダメです。
                if (this[baseFile, baseRank] != null)
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
            var baseFile = basePos.File;
            var baseRank = basePos.Rank;
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
                if (this[baseFile, baseRank] != null)
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
            var baseFile = basePos.File;
            var baseRank = basePos.Rank;
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
                if (this[baseFile, baseRank] != null)
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
        private IEnumerable<Square> GetCanMoveRange(Square square, BWType bwType)
        {
            if (square == null || !square.Validate())
            {
                return new Square[0];
            }

            var piece = this[square];
            if (piece == null || piece.BWType != bwType)
            {
                return new Square[0];
            }

            return GetCanMoveRange(piece.Piece, square, bwType);
        }

        /// <summary>
        /// <paramref name="square"/>にある駒が移動できる
        /// 可能性のある位置をすべて列挙します。
        /// </summary>
        private IEnumerable<Square> GetCanMoveRange(Piece piece, Square square, BWType bwType)
        {
            if (square == null || !square.Validate())
            {
                yield break;
            }

            if (piece == null)
            {
                yield break;
            }

            IEnumerable<Square> list;
            var file = square.File;
            var rank = square.Rank;

            if (piece.IsPromoted)
            {
                // 成り駒の場合
                switch (piece.PieceType)
                {
                    case PieceType.Gyoku:
                    case PieceType.Hisya:
                    case PieceType.Kaku:
                        list = GetMoveRangeWithTable(bwType, square, MoveTableGyoku);
                        foreach (var p in list)
                        {
                            yield return p;
                        }
                        break;
                    case PieceType.Kin:
                    case PieceType.Gin:
                    case PieceType.Kei:
                    case PieceType.Kyo:
                    case PieceType.Hu:
                        list = GetMoveRangeWithTable(bwType, square, MoveTableKin);
                        foreach (var p in list)
                        {
                            yield return p;
                        }
                        break;
                }
            }
            else
            {
                // 成り駒で無い場合
                switch (piece.PieceType)
                {
                    case PieceType.Gyoku:
                        list = GetMoveRangeWithTable(bwType, square, MoveTableGyoku);
                        foreach (var p in list)
                        {
                            yield return p;
                        }
                        break;
                    case PieceType.Kin:
                        list = GetMoveRangeWithTable(bwType, square, MoveTableKin);
                        foreach (var p in list)
                        {
                            yield return p;
                        }
                        break;
                    case PieceType.Gin:
                        list = GetMoveRangeWithTable(bwType, square, MoveTableGin);
                        foreach (var p in list)
                        {
                            yield return p;
                        }
                        break;
                    case PieceType.Kei:
                        list = GetMoveRangeWithTable(bwType, square, MoveTableKei);
                        foreach (var p in list)
                        {
                            yield return p;
                        }
                        break;
                    case PieceType.Hu:
                        list = GetMoveRangeWithTable(bwType, square, MoveTableHu);
                        foreach (var p in list)
                        {
                            yield return p;
                        }
                        break;
                    case PieceType.Kyo:
                        for (var r = 1; r <= BoardSize; ++r)
                        {
                            yield return new Square(file, r);
                        }
                        break;
                }
            }

            // 竜・馬の場合、近傍地点はすでに調査済みなので
            // 成りか不成りかで調査範囲を変更します。
            var range = (piece.IsPromoted ? 2 : 1);

            // 飛車角は成り／不成りに関わらず調べる箇所があります。
            switch (piece.PieceType)
            {
                case PieceType.Hisya:
                    for (var f = 1; f <= BoardSize; ++f)
                    {
                        if (piece.IsPromoted && Math.Abs(file - f) < range)
                        {
                            continue;
                        }

                        var sq = new Square(f, rank);
                        if (sq.Validate())
                        {
                            yield return sq;
                        }
                    }
                    for (var r = 1; r <= BoardSize; ++r)
                    {
                        if (piece.IsPromoted && Math.Abs(rank - r) < range)
                        {
                            continue;
                        }

                        var sq = new Square(file, r);
                        if (sq.Validate())
                        {
                            yield return sq;
                        }
                    }
                    break;

                case PieceType.Kaku:
                    for (var index = -BoardSize; index <= BoardSize; ++index)
                    {
                        if (piece.IsPromoted && Math.Abs(index) < range)
                        {
                            continue;
                        }

                        var sq = new Square(file + index, rank + index);
                        if (sq.Validate())
                        {
                            yield return sq;
                        }
                    }
                    for (var index = -BoardSize; index <= BoardSize; ++index)
                    {
                        if (piece.IsPromoted && Math.Abs(index) < range)
                        {
                            continue;
                        }

                        var sq = new Square(file + index, rank - index);
                        if (sq.Validate())
                        {
                            yield return sq;
                        }
                    }
                    break;
            }
        }
        #endregion
    }
}
