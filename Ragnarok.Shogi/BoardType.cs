using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ragnarok.Utility;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 駒落ちなどの局面を示します。
    /// </summary>
    public enum BoardType
    {
        /// <summary>
        /// 該当する局面なし
        /// </summary>
        None,
        /// <summary>
        /// 平手
        /// </summary>
        [Label(Label = "平手")]
        NoHandicap,
        /// <summary>
        /// 香落ち
        /// </summary>
        [Label(Label = "香落ち")]
        HandicapKyo,
        /// <summary>
        /// 右香落ち
        /// </summary>
        [Label(Label = "右香落ち")]
        HandicapRightKyo,
        /// <summary>
        /// 角落ち
        /// </summary>
        [Label(Label = "角落ち")]
        HandicapKaku,
        /// <summary>
        /// 飛車落ち
        /// </summary>
        [Label(Label = "飛車落ち")]
        HandicapHisya,
        /// <summary>
        /// 飛香落ち
        /// </summary>
        [Label(Label = "飛香落ち")]
        HandicapHisyaKyo,
        /// <summary>
        /// 二枚落ち
        /// </summary>
        [Label(Label = "二枚落ち")]
        Handicap2,
        /// <summary>
        /// 三枚落ち
        /// </summary>
        [Label(Label = "三枚落ち")]
        Handicap3,
        /// <summary>
        /// 四枚落ち
        /// </summary>
        [Label(Label = "四枚落ち")]
        Handicap4,
        /// <summary>
        /// 五枚落ち
        /// </summary>
        [Label(Label = "五枚落ち")]
        Handicap5,
        /// <summary>
        /// 左五枚落ち
        /// </summary>
        [Label(Label = "左五枚落ち")]
        HandicapLeft5,
        /// <summary>
        /// 六枚落ち
        /// </summary>
        [Label(Label = "六枚落ち")]
        Handicap6,
        /// <summary>
        /// 八枚落ち
        /// </summary>
        [Label(Label = "八枚落ち")]
        Handicap8,
        /// <summary>
        /// 十枚落ち
        /// </summary>
        [Label(Label = "十枚落ち")]
        Handicap10,
    }

    /// <summary>
    /// BoardType列挙子の拡張メソッドを定義します。
    /// </summary>
    public static class BoardTypeUtil
    {
        /// <summary>
        /// 局面名から駒落ち初期局面を作成します。
        /// </summary>
        public static BoardType GetBoardTypeFromName(string handicap)
        {
            var list = EnumUtil.GetValues<BoardType>()
                .Where(_ => _ != BoardType.None)
                .Where(_ => EnumUtil.GetLabel(_) == handicap)
                .ToArray();

            if (!list.Any())
            {
                return BoardType.None;
            }

            return list.FirstOrDefault();
        }

        /// <summary>
        /// 局面名から駒落ち初期局面を作成します。
        /// </summary>
        public static Board CreateBoardFromName(string name)
        {
            var type = GetBoardTypeFromName(name);
            if (type == BoardType.None)
            {
                return null;
            }

            return type.ToBoard();
        }

        /// <summary>
        /// 局面から駒落ちなどの局面名を取得します。
        /// </summary>
        public static BoardType GetBoardTypeFromBoard(Board board)
        {
            var list = EnumUtil.GetValues<BoardType>()
                .Where(_ => _ != BoardType.None)
                .Where(_ => Board.BoardEquals(board, _.ToBoard()))
                .ToArray();

            if (!list.Any())
            {
                // 該当なし
                return BoardType.None;
            }

            return list.FirstOrDefault();
        }

        /// <summary>
        /// 局面識別子を実際の局面に直します。
        /// </summary>
        public static Board ToBoard(this BoardType type)
        {
            var board = new Board();

            switch (type)
            {
                case BoardType.NoHandicap:
                    return board;

                case BoardType.HandicapKyo:
                    board[1, 1] = Piece.None;
                    break;
                case BoardType.HandicapRightKyo:
                    board[9, 1] = Piece.None;
                    break;
                case BoardType.HandicapKaku:
                    board[2, 2] = Piece.None;
                    break;
                case BoardType.HandicapHisya:
                    board[8, 2] = Piece.None;
                    break;
                case BoardType.HandicapHisyaKyo:
                    board[1, 1] = Piece.None;
                    board[8, 2] = Piece.None;
                    break;
                case BoardType.Handicap2:
                    board[2, 2] = Piece.None;
                    board[8, 2] = Piece.None;
                    break;
                case BoardType.Handicap3:
                    board[9, 1] = Piece.None;
                    goto case BoardType.Handicap2;
                case BoardType.Handicap4:
                    board[1, 1] = Piece.None;
                    board[9, 1] = Piece.None;
                    goto case BoardType.Handicap2;
                case BoardType.Handicap5:
                    board[8, 1] = Piece.None;
                    goto case BoardType.Handicap4;
                case BoardType.HandicapLeft5:
                    board[2, 1] = Piece.None;
                    goto case BoardType.Handicap4;
                case BoardType.Handicap6:
                    board[2, 1] = Piece.None;
                    board[8, 1] = Piece.None;
                    goto case BoardType.Handicap4;
                case BoardType.Handicap8:
                    board[3, 1] = Piece.None;
                    board[7, 1] = Piece.None;
                    goto case BoardType.Handicap6;
                case BoardType.Handicap10:
                    board[4, 1] = Piece.None;
                    board[6, 1] = Piece.None;
                    goto case BoardType.Handicap8;
                default:
                    throw new ShogiException(
                        type + ": BoardTypeの値が間違っています。");
            }

            // 駒落ちの場合、最初に指すのは上手=後手となります。
            board.Turn = BWType.White;
            return board;
        }
    }
}
