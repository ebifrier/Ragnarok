using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            'K', // Gyoku = 1
            'R', // Hisya = 2
            'B', // Kaku = 3
            'G', // Kin = 4
            'S', // Gin = 5
            'N', // Kei = 6
            'L', // Kyo = 7
            'P', // Hu = 8
        };

        /// <summary>
        /// SFEN形式の対応する駒文字(大文字)を取得します。
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
        public static string PieceTypeToSfen(PieceType pieceType)
        {
            return new string(SfenPieceList[(int)pieceType], 1);
        }

        /// <summary>
        /// SFEN形式の対応する駒文字(手番/成り不成りの区別あり）を取得します。
        /// </summary>
        public static string PieceToSfen(BoardPiece piece)
        {
            var c = SfenPieceList[(int)piece.PieceType];

            if (piece.BWType == BWType.White)
            {
                c = char.ToLower(c);
            }

            return ((piece.IsPromoted ? "+" : "") + c);
        }

        /// <summary>
        /// 文字をSFEN形式の駒として解釈します。
        /// </summary>
        /// <remarks>
        /// 大文字小文字は無視します。
        /// </remarks>
        public static PieceType SfenToPieceType(char piece)
        {
            for (var i = 0; i < SfenPieceList.Count(); ++i)
            {
                if (char.ToUpper(piece) == SfenPieceList[i])
                {
                    return (PieceType)i;
                }
            }

            return PieceType.None;
        }

        /// <summary>
        /// 文字をSFEN形式の駒として解釈します。
        /// </summary>
        /// <remarks>
        /// 大文字の場合は先手、小文字の場合は後手となります。
        /// </remarks>
        public static BoardPiece SfenToPiece(char piece)
        {
            for (var i = 0; i < SfenPieceList.Count(); ++i)
            {
                if (piece == SfenPieceList[i])
                {
                    return new BoardPiece((PieceType)i, false, BWType.Black);
                }

                if (piece == char.ToLower(SfenPieceList[i]))
                {
                    return new BoardPiece((PieceType)i, false, BWType.White);
                }
            }

            return null;
        }
    }
}
