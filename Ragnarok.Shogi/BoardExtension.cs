using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Ragnarok;
using Ragnarok.Utility;

namespace Ragnarok.Shogi
{
    using Csa;

    /// <summary>
    /// 指し手から盤上の駒を動かすためのクラスです。
    /// </summary>
    public static class BoardExtension
    {
        /// <summary>
        /// 文字列から差し手オブジェクトを作成します。
        /// </summary>
        public static List<Move> MakeMoveList(string text)
        {
            string tmp;

            return MakeMoveList(text, out tmp);
        }

        /// <summary>
        /// 文字列に差し手が含まれていれば、順次切り出していきます。
        /// </summary>
        private static IEnumerable<Move> MakeMoveListInternal(string text)
        {
            while (true)
            {
                var parsedText = string.Empty;

                // 与えられた文字列の差し手を解析します。
                var move = ShogiParser.ParseMoveEx(
                    text, false, true, ref parsedText);
                if (move == null)
                {
                    break;
                }

                // 次の解析は解析が終わった部分からはじめます。
                text = text.Substring(parsedText.Length);
                yield return move;
            }
        }

        private static readonly Regex SepRegex = new Regex(
            @"([^同])\s+",
            RegexOptions.IgnoreCase);

        /// <summary>
        /// 文字列から差し手オブジェクトを作成します。
        /// </summary>
        public static List<Move> MakeMoveList(string text, out string comment)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }

            // 指し手の解析前には文字列の正規化が必要ですが、
            // 最初に全部正規化してしまうと、最後のコメントも
            // 変わってしまうことがあります。
            // このため、文字列を空白で区切りながら
            // 正規化とパースを繰り返していきます。
            var result = new List<Move>();
            while (text.Length > 0)
            {
                var m = SepRegex.Match(text);
                //var index = Util.IndexOfWhitespace(text);
                var thisText = string.Empty;
                var nextText = string.Empty;

                // 空白までの文字列を解析します。
                if (m.Success)
                {
                    thisText = text.Substring(0, m.Index + m.Groups[1].Length);
                    nextText = text.Substring(m.Index + m.Length);
                }
                else
                {
                    thisText = text;
                    nextText = string.Empty;
                }

                // 指し手のパースが上手くいかなくなったら、そこで解析を終了します。
                var oldCount = result.Count();
                result.AddRange(MakeMoveListInternal(thisText));
                if (result.Count() == oldCount)
                {
                    break;
                }

                text = nextText;
            }

            comment = text.Trim();
            return result;
        }

        #region FilterBoardMove
        /// <summary>
        /// 複数ある差し手の中から、適切なひとつの差し手を選択します。
        /// </summary>
        /// <remarks>
        /// BoardMoveは移動前と移動後の情報を持ったオブジェクトで
        /// Moveは同金右などの移動情報を持ったオブジェクトです。
        /// 
        /// 盤面から検索された複数の着手可能な手から
        /// <paramref name="referenceMove"/>が指示するような手を
        /// 一つだけ検索します。
        /// 
        /// 適切な手が複数見つかった場合(<paramref name="referenceMove"/>に
        /// 右や左の情報が無い場合など)は、最初に見つかった手を返します。
        /// </remarks>
        private static BoardMove FilterBoardMove(List<BoardMove> boardMoveList,
                                                 Move referenceMove,
                                                 bool multipleIsNull = false)
        {
            if (boardMoveList.Count() == 1)
            {
                var result = boardMoveList.First();

                // 差し手が"飛車打ち"などの場合、打ではない差し手を選んでしまうと、
                // 意味が変わってしまいます。
                // (指定が42飛車なら、駒を打つ可能性がありますが、
                //  打ちの指定がある場合、駒を打たないという選択肢はありません)
                if (referenceMove.ActionType == ActionType.Drop &&
                    referenceMove.ActionType != result.ActionType)
                {
                    return null;
                }

                return result;
            }

            // 作業用
            var boardMoveListTmp = boardMoveList;

            // 移動前の座標情報があれば、それを使います。
            if (referenceMove.OldPosition != null)
            {
                boardMoveListTmp = boardMoveListTmp.Where(
                    bm => referenceMove.OldPosition == bm.OldPosition)
                    .ToList();
                if (boardMoveListTmp.Count() == 1)
                {
                    return boardMoveListTmp.First();
                }
            }

            // 上、引、寄るの判定をします。
            if (referenceMove.RankMoveType != RankMoveType.None)
            {
                boardMoveListTmp = boardMoveListTmp.Where(
                    bm => CheckRankMoveType(bm, referenceMove))
                    .ToList();
                if (boardMoveListTmp.Count() == 1)
                {
                    return boardMoveListTmp.First();
                }
            }

            // 左、右、直の判定をします。
            if (referenceMove.RelFileType != RelFileType.None)
            {
                boardMoveListTmp = boardMoveListTmp.Where(
                    bm => CheckRelPosType(bm, referenceMove))
                    .ToList();
                if (boardMoveListTmp.Count() == 1)
                {
                    return boardMoveListTmp.First();
                }
            }

            // 駒打ちなどの判定を行います。
            boardMoveListTmp = boardMoveListTmp.Where(
                bm => CheckActionType(bm, referenceMove))
                .ToList();
            if (boardMoveListTmp.Count() == 1)
            {
                return boardMoveListTmp.First();
            }

            // 適切な差し手が無い場合は、最初に見つかったものを返します。
            return (multipleIsNull ? null : boardMoveListTmp.FirstOrDefault());
        }

        /// <summary>
        /// 上、引、寄るの判定をします。
        /// </summary>
        private static bool CheckRankMoveType(BoardMove bm,
                                              Move referenceMove)
        {
            if (bm.ActionType == ActionType.Drop)
            {
                return false;
            }

            var fileMove = bm.NewPosition.File - bm.OldPosition.File;
            var rankMove = bm.NewPosition.Rank - bm.OldPosition.Rank;

            switch (referenceMove.RankMoveType)
            {
                case RankMoveType.Back:
                    return (bm.BWType == BWType.Black
                                ? (rankMove > 0)
                                : (rankMove < 0));
                case RankMoveType.Up:
                    return (bm.BWType == BWType.Black
                                ? (rankMove < 0)
                                : (rankMove > 0));
                case RankMoveType.Sideways:
                    return (fileMove != 0 && rankMove == 0);
            }

            return false;
        }

        /// <summary>
        /// 左、右、直の判定をします。
        /// </summary>
        private static bool CheckRelPosType(BoardMove bm,
                                            Move referenceMove)
        {
            if (bm.ActionType == ActionType.Drop)
            {
                return false;
            }

            var fileMove = bm.NewPosition.File - bm.OldPosition.File;
            var rankMove = bm.NewPosition.Rank - bm.OldPosition.Rank;

            switch (referenceMove.RelFileType)
            {
                case RelFileType.Left:
                    return (bm.BWType == BWType.Black
                                ? (fileMove < 0)
                                : (fileMove > 0));
                case RelFileType.Right:
                    return (bm.BWType == BWType.Black
                                ? (fileMove > 0)
                                : (fileMove < 0));
                case RelFileType.Straight:
                    return (bm.BWType == BWType.Black
                                ? (rankMove < 0)
                                : (rankMove > 0)) &&
                           (fileMove == 0);
            }

            return false;
        }

        /// <summary>
        /// 打つ、成る、成らず、などを判定します。
        /// </summary>
        /// <remarks>
        /// 駒が指定の場所に移動できない場合は自動的に打つが選ばれます。
        /// (盤上に飛車がないのに、"32飛車"のときなど)
        /// 選択できる変化が複数ある場合は、移動を選びます。
        /// 
        /// このメソッドでは指定無しの場合は、移動となります。
        /// </remarks>
        private static bool CheckActionType(BoardMove bm,
                                            Move referenceMove)
        {
            if (referenceMove.ActionType == ActionType.None)
            {
                // 指定無しと成らずでおｋとします。
                return (
                    bm.ActionType == ActionType.None ||
                    bm.ActionType == ActionType.Unpromote);
            }
            
            return (bm.ActionType == referenceMove.ActionType);
        }
        #endregion

        /// <summary>
        /// 文字列から得られた差し手から、移動前の情報も含むような
        /// 差し手情報を取得します。
        /// </summary>
        public static BoardMove ConvertMove(this Board board, Move move,
                                            bool multipleIsNull = false)
        {
            return board.ConvertMove(move, board.Turn, multipleIsNull);
        }

        /// <summary>
        /// 文字列から得られた差し手から、移動前の情報も含むような
        /// 差し手情報を取得します。
        /// </summary>
        public static BoardMove ConvertMove(this Board board, Move move,
                                            BWType bwType,
                                            bool multipleIsNull = false)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }

            if (move == null || move.IsResigned)
            {
                return null;
            }

            if (move.SameAsOld && board.PrevMovedPosition == null)
            {
                return null;
            }

            // 移動後の位置を取得します。
            // 同○○なら前回の位置を使います。
            var newPosition = (
                move.SameAsOld ?
                board.PrevMovedPosition :
                new Position(move.File, move.Rank));

            var boardMoveList = board.SearchMoveList(
                new BoardPiece(
                    bwType,
                    move.Piece.PieceType,
                    move.Piece.IsPromoted),
                newPosition)
                .ToList();

            // 複数の指し手の中から適切な一つを選びます。
            var boardMove = FilterBoardMove(boardMoveList, move, multipleIsNull);
            if (boardMove == null)
            {
                return null;
            }

            return boardMove;
        }

        /// <summary>
        /// 文字列から得られた差し手から、移動前の情報も含むような
        /// 差し手情報を取得します。
        /// </summary>
        public static bool CanMoveList(this Board board,
                                       IEnumerable<BoardMove> moveList)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }

            // 駒を動かしながら差し手を検証するため、
            // 盤の一時オブジェクトが必要になります。
            var boardTmp = board.Clone();
            foreach (var boardMove in moveList)
            {
                // 次の手を検証する必要があるため、実際に駒を動かします。
                if (!boardTmp.DoMove(boardMove))
                {
                    return true;
                }
            }

            return true;
        }

        /// <summary>
        /// 文字列から得られた差し手から、移動前の情報も含むような
        /// 差し手情報を取得します。
        /// </summary>
        public static List<BoardMove> ConvertMove(this Board board,
                                                  IEnumerable<Move> moveList)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }

            // 駒を動かしながら差し手を検証するため、
            // 盤の一時オブジェクトが必要になります。
            var boardTmp = board.Clone();

            var result = new List<BoardMove>();
            foreach (var move in moveList)
            {
                var boardMove = ConvertMove(boardTmp, move);
                if (boardMove == null)
                {
                    break;
                }

                // 次の手を検証する必要があるため、実際に駒を動かします。
                if (!boardTmp.DoMove(boardMove))
                {
                    break;
                }

                result.Add(boardMove);
            }

            return result;
        }

        #region FilterMove
        /// <summary>
        /// 複数ある差し手の中から、適切なひとつの差し手を選択します。
        /// </summary>
        /// <remarks>
        /// <paramref name="referenceMove"/>にはXからYに移動したという情報
        /// しかないため、これを52金右などの指し手に変換します。
        /// </remarks>
        private static Move FilterMove(this Board board,
                                       List<BoardMove> boardMoveList,
                                       BoardMove referenceMove,
                                       BoardPiece fromPiece,
                                       bool useOldPosition)
        {
            if (!boardMoveList.Any())
            {
                return null;
            }

            var nextPos = referenceMove.NewPosition;
            var move = new Move()
            {
                BWType = referenceMove.BWType,
                Piece = new Piece(
                    fromPiece.PieceType,
                    fromPiece.IsPromoted),
                File = nextPos.File,
                Rank = nextPos.Rank,
                SameAsOld = (board.PrevMovedPosition == nextPos),
            };

            // 移動元情報を使う場合は、これで終わりです。
            // （.kifファイルからの読み込み時は、駒の移動の場合は移動前情報がつき、
            //　 駒打ちの場合は"打"が必ずつきます）
            if (useOldPosition)
            {
                move.OldPosition = referenceMove.OldPosition;
                move.ActionType = referenceMove.ActionType;
                return move;
            }

            if (boardMoveList.Count() == 1)
            {
                return move;
            }            

            // 駒打ち、成り、不成りなどでフィルターします。
            var tmpMoveList = boardMoveList.Where(
                mv => mv.ActionType == referenceMove.ActionType)
                .ToList();
            if (tmpMoveList.Count() != boardMoveList.Count())
            {
                move.ActionType = referenceMove.ActionType;
            }
            if (tmpMoveList.Count() == 1)
            {
                return move;
            }

            // 段の位置でフィルターします。
            tmpMoveList = FilterRank(move, referenceMove, tmpMoveList);
            if (tmpMoveList.Count() == 1)
            {
                return move;
            }

            // 列の位置でフィルターします。
            tmpMoveList = FilterFile(move, referenceMove, tmpMoveList);
            if (tmpMoveList.Count() == 1)
            {
                return move;
            }

            // 不明。
            return null;
        }

        /// <summary>
        /// 段で指し手をフィルターし、Moveに適切なRankMoveTypeを設定します。
        /// </summary>
        private static List<BoardMove> FilterRank(Move move,
                                                  BoardMove referenceMove,
                                                  List<BoardMove> boardMoveList)
        {
            // 駒の移動前情報が必要です。
            var nextPos = referenceMove.NewPosition;
            var prevPos = referenceMove.OldPosition;
            if (prevPos == null)
            {
                return null;
            }

            // 黒から見ると正の場合は引く or 左へ移動(右)で
            // 負の場合は上がる or 右へ移動(左)です。
            var relRank = nextPos.Rank - prevPos.Rank;

            // 段の位置でフィルターします。
            var tmpMoveList = boardMoveList.Where(mv =>
            {
                var rel = nextPos.Rank - mv.OldPosition.Rank;
                if (relRank < 0)
                {
                    return (rel < 0);
                }
                else if (relRank > 0)
                {
                    return (rel > 0);
                }
                else
                {
                    return (rel == 0);
                }
            }).ToList();

            // 段情報でフィルターされた場合は、段の移動情報を付加します。
            if (tmpMoveList.Count() != boardMoveList.Count())
            {
                var relRank2 = relRank * referenceMove.BWType.Sign();

                if (relRank2 < 0)
                {
                    move.RankMoveType = RankMoveType.Up;
                }
                else if (relRank2 > 0)
                {
                    move.RankMoveType = RankMoveType.Back;
                }
                else
                {
                    move.RankMoveType = RankMoveType.Sideways;
                }
            }

            return tmpMoveList;
        }

        /// <summary>
        /// 列で指し手をフィルターし、Moveに適切なRelPosTypeを設定します。
        /// </summary>
        private static List<BoardMove> FilterFile(Move move,
                                                  BoardMove referenceMove,
                                                  List<BoardMove> boardMoveList)
        {
            // 駒の移動前情報が必要です。
            var nextPos = referenceMove.NewPosition;
            var prevPos = referenceMove.OldPosition;
            if (prevPos == null)
            {
                return null;
            }

            // 黒から見ると正の場合は引く or 左へ移動(右)で
            // 負の場合は上がる or 右へ移動(左)です。
            var relFile = nextPos.File - prevPos.File;

            // 列の位置でフィルターします。
            var tmpMoveList = boardMoveList.Where(mv =>
            {
                var rel = nextPos.File - mv.OldPosition.File;
                if (relFile < 0)
                {
                    return (rel < 0);
                }
                else if (relFile > 0)
                {
                    return (rel > 0);
                }
                else
                {
                    return (rel == 0);
                }
            }).ToList();

            // 列情報でフィルターされた場合は、列の移動情報を付加します。
            if (tmpMoveList.Count() != boardMoveList.Count())
            {
                var relFile2 = relFile * referenceMove.BWType.Sign();

                if (relFile2 < 0)
                {
                    move.RelFileType = RelFileType.Left;
                }
                else if (relFile2 > 0)
                {
                    move.RelFileType = RelFileType.Right;
                }
                else
                {
                    // 直の場合は、左右の動きはありません。
                    move.RankMoveType = RankMoveType.None;
                    move.RelFileType = RelFileType.Straight;
                }
            }

            return tmpMoveList;
        }
        #endregion

        /// <summary>
        /// 指し手をXX->YYの形式から、ZZ銀上などの形に変換します。
        /// </summary>
        /// <remarks>
        /// <paramref name="useOldPosition"/>を真にすると、差し手の後に
        /// 古い位置の情報が付加されるようになります。(例: 32金(22))
        /// </remarks>
        public static Move ConvertMove(this Board board, BoardMove move,
                                       bool useOldPosition)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }

            if (move == null || !move.Validate())
            {
                throw new ArgumentNullException("move");
            }

            var fromPiece = (
                move.ActionType != ActionType.Drop ?
                board[move.OldPosition] :
                new BoardPiece(move.BWType, move.DropPieceType, false));
            if (fromPiece == null)
            {
                return null;
            }

            // 駒の種類と最終位置から、あり得る指し手をすべて検索します。
            var boardMoveList = board.SearchMoveList(
                fromPiece,
                move.NewPosition)
                .ToList();

            return FilterMove(
                board, boardMoveList, move, fromPiece, useOldPosition);
        }

        /// <summary>
        /// <paramref name="fromNumber"/>手からの指し手リストを作成します。
        /// </summary>
        /// <remarks>
        /// 主に指し手を文字列化するために使います。
        /// <paramref name="useOldPosition"/>を真にすると、差し手の後に
        /// 古い位置の情報が付加されるようになります。(例: 32金(22))
        /// </remarks>
        public static List<Move> MakeMoveList(this Board board,
                                              int fromNumber,
                                              bool useOldPosition)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }

            if (fromNumber < 0)
            {
                throw new ArgumentException("fromNumber");
            }

            // 指し手を文字列化するためには、指し手とそのときの局面
            // が必要なので、一度局面の手を戻し局面を設定した後、
            // もう一度手を進めていきます。
            var clonedBoard = board.Clone();
            var boardMoveList = new LinkedList<BoardMove>();

            // 局面を戻しながら指し手を取得します。
            while (clonedBoard.MoveCount > fromNumber)
            {
                var boardMove = clonedBoard.Undo();
                if (boardMove == null)
                {
                    return null;
                }

                boardMoveList.AddFirst(boardMove);
            }

            // 文字列化するための指し手リストを取得します。
            var moveList = new List<Move>();
            foreach (var boardMove in boardMoveList)
            {
                var move = ConvertMove(clonedBoard, boardMove, useOldPosition);
                if (move == null)
                {
                    return null;
                }

                if (!clonedBoard.DoMove(boardMove))
                {
                    return null;
                }

                moveList.Add(move);
            }

            return moveList;
        }

        /// <summary>
        /// <see cref="CsaMove"/>を<see cref="BoardMove"/>に変換します。
        /// </summary>
        public static BoardMove ConvertCsaMove(this Board board, CsaMove csaMove)
        {
            if (csaMove == null)
            {
                return null;
            }

            var newPiece = board[csaMove.NewPosition];
            if (csaMove.IsDrop)
            {
                if (newPiece != null)
                {
                    return null;
                }

                return new BoardMove
                {
                    BWType = board.Turn,
                    NewPosition = csaMove.NewPosition,
                    ActionType = ActionType.Drop,
                    DropPieceType = csaMove.Piece.PieceType,
                };
            }
            else
            {
                var oldPiece = board[csaMove.OldPosition];
                if (oldPiece == null)
                {
                    return null;
                }

                return new BoardMove
                {
                    BWType = board.Turn,
                    NewPosition = csaMove.NewPosition,
                    OldPosition = csaMove.OldPosition,
                    TookPiece = newPiece,
                    ActionType = (
                        !oldPiece.IsPromoted && csaMove.Piece.IsPromoted ?
                        ActionType.Promote :
                        ActionType.None),
                };
            }
        }

        /// <summary>
        /// <see cref="BoardMove"/>を<see cref="CsaMove"/>に変換します。
        /// </summary>
        public static CsaMove ConvertBoardMove(this Board board, BoardMove bmove)
        {
            if (bmove == null || !bmove.Validate())
            {
                return null;
            }

            var newPiece = board[bmove.NewPosition];
            if (bmove.ActionType == ActionType.Drop)
            {
                if (newPiece != null)
                {
                    return null;
                }

                return new CsaMove
                {
                    Side = board.Turn,
                    NewPosition = bmove.NewPosition,
                    Piece = new Piece(bmove.DropPieceType, false),
                };
            }
            else
            {
                var oldPiece = board[bmove.OldPosition];
                if (oldPiece == null)
                {
                    return null;
                }

                return new CsaMove
                {
                    Side = board.Turn,
                    NewPosition = bmove.NewPosition,
                    OldPosition = bmove.OldPosition,
                    Piece = new Piece(
                        oldPiece.PieceType,
                        oldPiece.IsPromoted ||
                        bmove.ActionType == ActionType.Promote),
                };
            }
        }
    }
}
