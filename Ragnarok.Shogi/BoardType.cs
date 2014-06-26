using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 駒落ちなどの局面を示します。
    /// </summary>
    public enum BoardType
    {
        /// <summary>
        /// 平手
        /// </summary>
        NoHandicap,
        /// <summary>
        /// 香落ち
        /// </summary>
        HandicapKyo,
        /// <summary>
        /// 角落ち
        /// </summary>
        HandicapKaku,
        /// <summary>
        /// 飛車落ち
        /// </summary>
        HandicapHisya,
        /// <summary>
        /// 飛香落ち
        /// </summary>
        HandicapHisyaKyo,
        /// <summary>
        /// ２枚落ち
        /// </summary>
        Handicap2,
        /// <summary>
        /// ４枚落ち
        /// </summary>
        Handicap4,
        /// <summary>
        /// ６枚落ち
        /// </summary>
        Handicap6,
        /// <summary>
        /// ８枚落ち
        /// </summary>
        Handicap8,
        /// <summary>
        /// １０枚落ち
        /// </summary>
        Handicap10,
        /// <summary>
        /// 歩３兵
        /// </summary>
        HandicapHu3,
    }

    /// <summary>
    /// BoardType列挙子の拡張メソッドを定義します。
    /// </summary>
    public static class BoardTypeExtension
    {
        /// <summary>
        /// 局面識別子を実際の局面に直します。
        /// </summary>
        public static Board ToBoard(this BoardType type)
        {
            var board = new Board();
            var i = 0;

            switch (type)
            {
                case BoardType.NoHandicap:
                    break;
                case BoardType.HandicapKyo:
                    board[1, 1] = null;
                    break;
                case BoardType.HandicapKaku:
                    board[2, 2] = null;
                    break;
                case BoardType.HandicapHisya:
                    board[8, 2] = null;
                    break;
                case BoardType.HandicapHisyaKyo:
                    board[1, 1] = null;
                    board[8, 2] = null;
                    break;
                case BoardType.Handicap2:
                    board[2, 2] = null;
                    board[8, 2] = null;
                    break;
                case BoardType.Handicap4:
                    board[1, 1] = null;
                    board[9, 1] = null;
                    goto case BoardType.Handicap2;
                case BoardType.Handicap6:
                    board[2, 1] = null;
                    board[8, 1] = null;
                    goto case BoardType.Handicap4;
                case BoardType.Handicap8:
                    board[3, 1] = null;
                    board[7, 1] = null;
                    goto case BoardType.Handicap6;
                case BoardType.Handicap10:
                    board[4, 1] = null;
                    board[6, 1] = null;
                    goto case BoardType.Handicap8;
                case BoardType.HandicapHu3:
                    for (i = 1; i <= Board.BoardSize; ++i)
                    {
                        board[i, 3] = null;
                    }
                    board.SetCapturedPieceCount(PieceType.Hu, BWType.White, 3);
                    goto case BoardType.Handicap10;
                default:
                    throw new ShogiException(
                        type + ": BoardTypeの値が間違っています。");
            }

            return board;
        }
    }
}
