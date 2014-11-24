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

                // 与えられた文字列の指し手を順次パースします。
                var move = ShogiParser.ParseMoveEx(
                    text, false, true, ref parsedText);
                if (move == null)
                {
                    break;
                }

                // パースが終わった部分から次のパースをはじめます。
                text = text.Substring(parsedText.Length);
                yield return move;
            }
        }

        private static readonly Regex SepRegex = new Regex(
            @"([^同])\s+",
            RegexOptions.IgnoreCase);

        /// <summary>
        /// 文字列から指し手オブジェクトを作成します。
        /// </summary>
        /// <example>
        /// 54歩同歩　コメント
        /// </example>
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
                var list = MakeMoveListInternal(thisText).ToArray();
                if (!list.Any())
                {
                    break;
                }

                result.AddRange(list);
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
        /// Moveは同金右などの移動方向を持ったオブジェクトです。
        /// 
        /// 盤面から検索された複数の着手可能な手から
        /// <paramref name="referenceMove"/>が指示するような手を
        /// 一つだけ検索します。
        /// 
        /// 該当する手が複数見つかった場合(<paramref name="referenceMove"/>に
        /// 右や左の情報が無い場合など)は、最初に見つかった手を返します。
        /// </remarks>
        private static BoardMove FilterBoardMove(List<BoardMove> boardMoveList,
                                                 Move referenceMove,
                                                 bool multipleIsNull = false)
        {
            IEnumerable<BoardMove> boardMoveListTmp = boardMoveList;

            // 移動前の座標情報があれば、それを使います。
            if (referenceMove.SrcSquare != null)
            {
                boardMoveListTmp = boardMoveListTmp.Where(
                    bm => referenceMove.SrcSquare == bm.SrcSquare);
            }

            // 上、引、寄るの判定をします。
            if (referenceMove.RankMoveType != RankMoveType.None)
            {
                boardMoveListTmp = boardMoveListTmp.Where(
                    bm => CheckRankMoveType(bm, referenceMove));
            }

            // 左、右、直の判定をします。
            if (referenceMove.RelFileType != RelFileType.None)
            {
                boardMoveListTmp = boardMoveListTmp.Where(
                    bm => CheckRelPosType(bm, referenceMove, boardMoveList));
            }

            // 駒打ちなどの判定を行います。
            // 
            // 駒打ちの場合
            //  1) 他に移動できる駒がない場合、「打」がなくても「打」と解釈
            //  2) 他に移動できる駒がある場合、「打」がなければ「打」と解釈されない
            // という風に、解釈の仕方が少し複雑になっています。
            // ここでは、canMoveというフラグを使って上記の場合分けを行っています。
            var canMove = boardMoveListTmp.Any(
                _ => _.ActionType != ActionType.Drop);

            boardMoveListTmp = boardMoveListTmp.Where(
                bm => CheckActionType(bm, referenceMove, canMove));

            // 適切な差し手が無い場合は、最初に見つかったものを返します。
            var list = boardMoveListTmp.ToList();
            return (
                multipleIsNull ?
                (list.Count() == 1 ? list.FirstOrDefault() : null) :
                list.FirstOrDefault());
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

            var fileMove = bm.DstSquare.File - bm.SrcSquare.File;
            var rankMove = bm.DstSquare.Rank - bm.SrcSquare.Rank;

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
                                            Move referenceMove,
                                            List<BoardMove> boardMoveList)
        {
            if (bm.ActionType == ActionType.Drop)
            {
                return false;
            }

            if (bm.MovePiece == Piece.Ryu || bm.MovePiece == Piece.Uma)
            {
                // 竜、馬の場合、「直」は使わずに「右左」のみを使用します。
                if (boardMoveList.Count() == 1)
                {
                    return (referenceMove.RelFileType == RelFileType.None);
                }
                else
                {
                    // 駒は二つしかないはずなので、相方に比べて自分が
                    // 左にあれば「左」、右にあれば「右」となっているか
                    // 判定します。
                    var other = (
                        ReferenceEquals(bm, boardMoveList[0])
                        ? boardMoveList[1]
                        : boardMoveList[0]);
                    var fileDif = bm.SrcSquare.File - other.SrcSquare.File;

                    switch (referenceMove.RelFileType)
                    {
                        case RelFileType.Left:
                            return (bm.BWType == BWType.Black
                                        ? (fileDif > 0)
                                        : (fileDif < 0));
                        case RelFileType.Right:
                            return (bm.BWType == BWType.Black
                                        ? (fileDif < 0)
                                        : (fileDif > 0));
                        case RelFileType.None:
                            return (fileDif == 0);
                    }
                }
            }
            else
            {
                var fileMove = bm.DstSquare.File - bm.SrcSquare.File;
                var rankMove = bm.DstSquare.Rank - bm.SrcSquare.Rank;

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
            }

            return false;
        }

        /// <summary>
        /// 打つ、成る、成らず、などを判定します。
        /// </summary>
        /// <remarks>
        /// 駒が指定の場所に移動できない場合は自動的に打つが選ばれます。
        /// (盤上に飛車がないのに、"32飛車"のときなど)
        /// </remarks>
        private static bool CheckActionType(BoardMove bm,
                                            Move referenceMove, bool canMove)
        {
            if (referenceMove.ActionType == ActionType.None)
            {
                // 指し手一覧の中に移動できる駒があれば、
                // 「打」と指定しなければ打つと判定されません。
                if (canMove)
                {
                    // 指定無しと成らずでおｋとします。
                    return (
                        bm.ActionType == ActionType.None ||
                        bm.ActionType == ActionType.Unpromote);
                }
                else
                {
                    return (
                        bm.ActionType == ActionType.None ||
                        bm.ActionType == ActionType.Unpromote ||
                        bm.ActionType == ActionType.Drop);
                }
            }
            else if (referenceMove.ActionType == ActionType.Unpromote)
            {
                // 「不成」の場合は無と成らずでおｋとします。
                return (
                    bm.ActionType == ActionType.None ||
                    bm.ActionType == ActionType.Unpromote);
            }
            
            return (bm.ActionType == referenceMove.ActionType);
        }
        #endregion

        /// <summary>
        /// 文字列から得られた指し手から、移動前の情報も含むような
        /// 指し手情報を取得します。
        /// </summary>
        public static BoardMove ConvertMove(this Board board, Move move,
                                            bool multipleIsNull = false)
        {
            return board.ConvertMove(move, board.Turn, multipleIsNull);
        }

        /// <summary>
        /// 文字列から得られた指し手から、移動前の情報も含むような
        /// 指し手情報を取得します。
        /// </summary>
        public static BoardMove ConvertMove(this Board board, Move move,
                                            BWType bwType,
                                            bool multipleIsNull = false)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }

            if (move == null)
            {
                return null;
            }

            if (move.SameAsOld && board.PrevMovedSquare == null)
            {
                return null;
            }

            if (move.IsSpecialMove)
            {
                return BoardMove.CreateSpecialMove(
                    bwType, move.SpecialMoveType);
            }

            // 移動後の位置を取得します。
            // 同○○なら前回の位置を使います。
            var dstSquare = move.DstSquare;
            if (move.SameAsOld)
            {
                move = move.Clone();
                move.DstSquare = board.PrevMovedSquare;

                dstSquare = board.PrevMovedSquare;
            }

            var boardMoveList = board.ListupMoves(
                move.Piece, bwType, dstSquare)
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
        /// 指し手情報を取得します。
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
        /// 指し手情報を取得します。
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
                var boardMove = ConvertMove(boardTmp, move, true);
                if (boardMove == null)
                {
                    ConvertMove(boardTmp, move);
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
        /// 複数ある指し手の中から、適切なひとつの指し手を選択します。
        /// </summary>
        /// <remarks>
        /// <paramref name="referenceMove"/>にはXからYに移動したという情報
        /// しかないため、これを52金右などの指し手に変換します。
        /// </remarks>
        private static Move FilterMove(this Board board,
                                       List<BoardMove> boardMoveList,
                                       BoardMove referenceMove,
                                       Piece fromPiece,
                                       bool useSrcSquare)
        {
            if (!boardMoveList.Any())
            {
                return null;
            }

            var nextPos = referenceMove.DstSquare;
            var move = new Move
            {
                BWType = referenceMove.BWType,
                Piece = fromPiece,
                File = nextPos.File,
                Rank = nextPos.Rank,
                ActionType = ( // '打', '不成'は消える可能性があります。
                    referenceMove.ActionType == ActionType.Promote ?
                    referenceMove.ActionType :
                    ActionType.None),
                SameAsOld = (board.PrevMovedSquare == nextPos),
            };

            // 移動元情報を使う場合は、これで終わりです。
            // （.kifファイルからの読み込み時は、駒の移動の場合は移動前情報がつき、
            //　 駒打ちの場合は"打"が必ずつきます）
            if (useSrcSquare)
            {
                move.SrcSquare = referenceMove.SrcSquare;
                move.ActionType = referenceMove.ActionType;
                return move;
            }

            // 駒打ち、成り、不成りなどでフィルターします。
            var tmpMoveList = FilterAction(move, referenceMove, boardMoveList);
            if (!tmpMoveList.Any())
            {
                return null;
            }

            // 段の位置でフィルターします。
            tmpMoveList = FilterRank(move, referenceMove, tmpMoveList);
            if (!tmpMoveList.Any())
            {
                return null;
            }

            // 列の位置でフィルターします。
            tmpMoveList = FilterFile(move, referenceMove, tmpMoveList);
            if (!tmpMoveList.Any())
            {
                return null;
            }

            return move;
        }

        /// <summary>
        /// 指し手の種類で手をフィルターし、Moveに適切なRankMoveTypeを設定します。
        /// </summary>
        private static List<BoardMove> FilterAction(Move move,
                                                    BoardMove referenceMove,
                                                    List<BoardMove> boardMoveList)
        {
            var tmpMoveList = boardMoveList.Where(
                mv => mv.ActionType == referenceMove.ActionType)
                .ToList();
            if (tmpMoveList.Count() != boardMoveList.Count())
            {
                move.ActionType = referenceMove.ActionType;
            }

            return tmpMoveList;
        }

        /// <summary>
        /// 段で指し手をフィルターし、Moveに適切なRankMoveTypeを設定します。
        /// </summary>
        private static List<BoardMove> FilterRank(Move move,
                                                  BoardMove referenceMove,
                                                  List<BoardMove> boardMoveList)
        {
            // 駒の移動前情報が必要です。
            var nextPos = referenceMove.DstSquare;
            var prevPos = referenceMove.SrcSquare;
            if (prevPos == null)
            {
                // 何もフィルターしません。
                return boardMoveList;
            }

            // 黒から見ると正の場合は引く or 左へ移動(右)で
            // 負の場合は上がる or 右へ移動(左)です。
            var relRank = nextPos.Rank - prevPos.Rank;

            // 段の位置でフィルターします。
            var tmpMoveList = boardMoveList.Where(mv =>
            {
                var rel = nextPos.Rank - mv.SrcSquare.Rank;
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
            var nextPos = referenceMove.DstSquare;
            var prevPos = referenceMove.SrcSquare;
            if (prevPos == null)
            {
                // 何もフィルターしません。
                return boardMoveList;
            }

            if ((move.Piece == Piece.Ryu || move.Piece == Piece.Uma) &&
                boardMoveList.Count() == 2)
            {
                // 馬と竜の場合は'直'ではなく、右と左しか使いません。
                var other = (
                    referenceMove == boardMoveList[0] ?
                    boardMoveList[1] : boardMoveList[0]);

                // 動かす前の駒が相方に比べて右にあるか左にあるか調べます。
                // 先手の場合は筋が小さい方が右です。
                var relFile = prevPos.File - other.SrcSquare.File;
                relFile *= (referenceMove.BWType == BWType.White ? -1 : +1);

                if (relFile == 0)
                {
                    return boardMoveList;
                }
                else if (relFile < 0)
                {
                    move.RelFileType = RelFileType.Right;
                }
                else
                {
                    move.RelFileType = RelFileType.Left;
                }

                return new List<BoardMove> { referenceMove };
            }
            else
            {
                // 黒から見ると正の場合は引く or 左へ移動(右)で
                // 負の場合は上がる or 右へ移動(左)です。
                var relFile = nextPos.File - prevPos.File;

                // 列の位置でフィルターします。
                var tmpMoveList = boardMoveList.Where(mv =>
                {
                    var rel = nextPos.File - mv.SrcSquare.File;
                    return (
                        relFile < 0 ? (rel < 0) :
                        relFile > 0 ? (rel > 0) :
                        (rel == 0));
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
        }
        #endregion

        /// <summary>
        /// 指し手をXX->YYの形式から、ZZ銀上などの形に変換します。
        /// </summary>
        /// <remarks>
        /// <paramref name="useSrcSquare"/>を真にすると、差し手の後に
        /// 古い位置の情報が付加されるようになります。(例: 32金(22))
        /// </remarks>
        public static Move ConvertMove(this Board board, BoardMove move,
                                       bool useSrcSquare)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }

            if (move == null || !move.Validate())
            {
                throw new ArgumentNullException("move");
            }

            if (move.IsSpecialMove)
            {
                return new Move
                {
                    BWType = move.BWType,
                    SpecialMoveType = move.SpecialMoveType,
                };
            }

            var fromPiece = (
                move.ActionType == ActionType.Drop ?
                new Piece(move.DropPieceType) :
                move.MovePiece);
            if (fromPiece == null)
            {
                return null;
            }

            // 駒の種類と最終位置から、あり得る指し手をすべて検索します。
            var boardMoveList = board.ListupMoves(
                fromPiece, move.BWType, move.DstSquare)
                .ToList();

            return FilterMove(
                board, boardMoveList, move, fromPiece, useSrcSquare);
        }

        /// <summary>
        /// 指し手をXX->YYの形式から、ZZ銀上などの形に変換します。
        /// </summary>
        /// <remarks>
        /// <paramref name="useSrcSquare"/>を真にすると、差し手の後に
        /// 古い位置の情報が付加されるようになります。(例: 32金(22))
        /// </remarks>
        public static List<Move> ConvertMove(this Board board,
                                             IEnumerable<BoardMove> bmoveList,
                                             bool useSrcSquare)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }

            var moveList = new List<Move>();
            var cloned = board.Clone();

            foreach (var boardMove in bmoveList)
            {
                var move = ConvertMove(cloned, boardMove, useSrcSquare);
                if (move == null)
                {
                    break;
                }

                if (!cloned.DoMove(boardMove))
                {
                    break;
                }

                moveList.Add(move);
            }

            return moveList;
        }

        /// <summary>
        /// 与えられた指し手が着手可能か調べ、もし着手可能な場合はそれを正式な表記に変換します。
        /// </summary>
        /// <remarks>
        /// 「正式な表記」にするとは、たとえば不要な「打」を削除したり、
        /// 右や直などの表記を正確なものに修正することです。
        /// </remarks>
        public static Move NormalizeMove(this Board board, Move move)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }

            if (!board.Validate())
            {
                throw new ArgumentException("board");
            }

            if (move == null)
            {
                throw new ArgumentNullException("move");
            }

            if (!move.Validate())
            {
                throw new ArgumentException("move");
            }

            // 投了などの特殊な指し手は常にさせることにします。
            /*if (move.IsSpecialMove)
            {
                return move;
            }*/

            // 一度、指し手の正規化を行います（打を消したり、左を追加するなど）
            // あり得る指し手が複数ある場合は失敗とします。
            var bmove = board.ConvertMove(move, true);
            if (bmove == null || !board.CanMove(bmove))
            {
                return null;
            }

            // 指し手を表記形式に再度変換します。
            // 移動元の情報は使いません。("65銀(55)"という表記にはしません)
            var newMove = board.ConvertMove(bmove, false);
            if (newMove == null)
            {
                return null;
            }

            // 最後に元の文字列を保存して返します。
            newMove.OriginalText = move.OriginalText;
            return newMove;
        }

        /// <summary>
        /// <paramref name="fromNumber"/>手からの指し手リストを作成します。
        /// </summary>
        /// <remarks>
        /// 主に指し手を文字列化するために使います。
        /// <paramref name="useSrcSquare"/>を真にすると、差し手の後に
        /// 古い位置の情報が付加されるようになります。(例: 32金(22))
        /// </remarks>
        public static List<Move> MakeMoveList(this Board board,
                                              int fromNumber,
                                              bool useSrcSquare)
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
                var move = ConvertMove(clonedBoard, boardMove, useSrcSquare);
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
    }
}
