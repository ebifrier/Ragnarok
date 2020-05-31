using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Shogi.Sfen
{
    /// <summary>
    /// SFEN形式を扱うときの便利クラス
    /// </summary>
    public static class SfenUtil
    {
        private static readonly char[] SfenPieceList =
        {
            '?', // None = 0
            'P', // Pawn = 1
            'L', // Lance = 2
            'N', // Knight = 3
            'S', // Silver = 4
            'B', // Bishop = 5
            'R', // Rook = 6
            'G', // Gold = 7
            'K', // King = 8
        };

        /// <summary>
        /// SFEN形式の対応する駒文字(手番/成り不成りの区別あり）を取得します。
        /// </summary>
        /// <remarks>
        /// 先手の玉：K、後手の玉：k （Kingの頭文字）
        /// 先手の飛車：R、後手の飛車：r （Rookの頭文字）
        /// 先手の角：B、後手の角：b （Bishopの頭文字）
        /// 先手の金：G、後手の金：g （Goldの頭文字）
        /// 先手の銀：S、後手の銀：s （Silverの頭文字）
        /// 先手の桂馬：N、後手の桂馬：n （kNightより）
        /// 先手の香車：L、後手の香車：l （Lanceの頭文字）
        /// 先手の歩：P、後手の歩：p （Pawnの頭文字）
        /// </remarks>
        public static string PieceToSfen(Piece piece)
        {
            var c = SfenPieceList[(int)piece.GetRawType()];

            if (piece.GetColor() == BWType.White)
            {
                c = char.ToLowerInvariant(c);
            }

            return ((piece.IsPromoted() ? "+" : "") + c);
        }

        /// <summary>
        /// 文字をSFEN形式の駒として解釈します。
        /// </summary>
        /// <remarks>
        /// 大文字の場合は先手、小文字の場合は後手となります。
        /// </remarks>
        public static Piece SfenToPiece(char pieceCh)
        {
            for (var i = 0; i < SfenPieceList.Length; ++i)
            {
                if (pieceCh == SfenPieceList[i])
                {
                    return ((Piece)i).Modify(BWType.Black);
                }

                if (pieceCh == char.ToLowerInvariant(SfenPieceList[i]))
                {
                    return ((Piece)i).Modify(BWType.White);
                }
            }

            return Piece.None;
        }
    }
}
