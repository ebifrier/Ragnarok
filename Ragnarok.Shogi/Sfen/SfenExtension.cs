using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi.Sfen
{
    /// <summary>
    /// SFEN形式の指し手を解釈します。
    /// </summary>
    public static class SfenExtension
    {
        /// <summary>
        /// 指し手をSFEN形式の文字列に変換します。
        /// </summary>
        /// <remarks>
        /// 筋に関しては１から９までの数字で表記され、段に関してはaからiまでの
        /// アルファベット（１段目がa、２段目がb、・・・、９段目がi）
        /// というように表記されます。位置の表記はこの２つを組み合わせ、
        /// ５一なら5a、１九なら1iとなります。
        /// 
        /// 指し手に関しては、駒の移動元の位置と移動先の位置を並べて書きます。
        /// ７七の駒が７六に移動したのであれば、7g7fと表記します。
        /// （駒の種類を表記する必要はありません。）
        /// 駒が成るときは最後に+を追加します。８八の駒が２二に移動して
        /// 成るなら8h2b+です。
        /// 持ち駒を打つときは "[駒の種類(大文字)]*[打った場所]" となります。
        /// 金を５二に打つ場合はG*5bとなります
        /// </remarks>
        public static string ToSfen(this Move move)
        {
            if (move == null)
            {
                throw new ArgumentNullException("move");
            }

            if (!move.Validate())
            {
                throw new ArgumentException("move");
            }

            if (move.IsSpecialMove)
            {
                // 投了などの特殊な指し手(sfenに変換することはできない)
                return "";
            }

            var dstFile = move.DstSquare.File;
            var dstRank = (char)((int)'a' + (move.DstSquare.Rank - 1));

            if (move.ActionType == ActionType.Drop)
            {
                // 駒打ちの場合
                var piece = SfenUtil.PieceTypeToSfen(move.DropPieceType);

                return string.Format("{0}*{1}{2}",
                    piece, dstFile, dstRank);
            }
            else
            {
                // 駒の移動の場合
                var srcFile = move.SrcSquare.File;
                var srcRank = (char)((int)'a' + (move.SrcSquare.Rank - 1));
                var isPromote = (move.ActionType == ActionType.Promote);

                return string.Format("{0}{1}{2}{3}{4}",
                    srcFile, srcRank, dstFile, dstRank,
                    (isPromote ? "+" : ""));
            }
        }

        /// <summary>
        /// SFEN形式の指し手を、指し手に変換します。
        /// </summary>
        public static Move SfenToMove(this Board board, string sfen)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }

            if (string.IsNullOrEmpty(sfen))
            {
                throw new ArgumentNullException("sfen");
            }

            var type = SfenToSpecialMoveType(sfen);
            if (type != SpecialMoveType.None)
            {
                return Move.CreateSpecialMove(board.Turn, type);
            }

            if (sfen.Length < 4)
            {
                return null;
            }

            var dropPieceType = SfenUtil.SfenToPieceType(sfen[0]);
            if (dropPieceType != PieceType.None)
            {
                // 駒打ちの場合
                if ((sfen[1] != '*') ||
                    (sfen[2] < '1' || '9' < sfen[2]) ||
                    (sfen[3] < 'a' || 'i' < sfen[3]))
                {
                    return null;
                }

                var dstFile = (sfen[2] - '1') + 1;
                var dstRank = (sfen[3] - 'a') + 1;

                return Move.CreateDrop(
                    board.Turn, new Square(dstFile, dstRank), dropPieceType);
            }
            else
            {
                // 駒の移動の場合
                if ((sfen[0] < '1' || '9' < sfen[0]) ||
                    (sfen[2] < '1' || '9' < sfen[2]) ||
                    (sfen[1] < 'a' || 'i' < sfen[1]) ||
                    (sfen[3] < 'a' || 'i' < sfen[3]))
                {
                    return null;
                }

                var srcFile = (sfen[0] - '1') + 1;
                var srcRank = (sfen[1] - 'a') + 1;
                var dstFile = (sfen[2] - '1') + 1;
                var dstRank = (sfen[3] - 'a') + 1;
                var piece = board[srcFile, srcRank];
                if (piece == null)
                {
                    return null;
                }

                var promote = (sfen.Length > 4 && sfen[4] == '+');
                return Move.CreateMove(
                    board.Turn,
                    new Square(srcFile, srcRank),
                    new Square(dstFile, dstRank),
                    piece.Piece,
                    promote,
                    BoardPiece.GetPiece(board[dstFile, dstRank]));
            }
        }

        private static SpecialMoveType SfenToSpecialMoveType(string sfen)
        {
            switch (sfen)
            {
                case "resign":
                    return SpecialMoveType.Resign;
                case "win":
                    return SpecialMoveType.Jishogi;
                case "lose":
                    return SpecialMoveType.Jishogi;
                case "rep_draw":
                    return SpecialMoveType.Sennichite;
                case "rep_inf":
                    break;
                case "rep_sup":
                    break;
            }

            return SpecialMoveType.None;
        }

        /// <summary>
        /// 連続したSFEN形式の指し手を、連続した指し手に変換します。
        /// </summary>
        public static IEnumerable<Move> SfenToMoveList(this Board board,
                                                            IEnumerable<string> sfenList)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }

            if (sfenList == null)
            {
                throw new ArgumentNullException("sfenList");
            }

            var tmpBoard = board.Clone();
            foreach (var sfen in sfenList)
            {
                var move = tmpBoard.SfenToMove(sfen);
                if (move == null || !move.Validate())
                {
                    yield break;
                }

                if (!tmpBoard.DoMove(move))
                {
                    yield break;
                }

                yield return move;
            }
        }
    }
}
