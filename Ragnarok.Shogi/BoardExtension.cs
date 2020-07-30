using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Ragnarok;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 指し手から盤上の駒を動かすためのクラスです。
    /// </summary>
    public static class BoardExtension
    {
        #region MakeMoveListFromText
        /// <summary>
        /// 文字列から差し手オブジェクトを作成します。
        /// </summary>
        public static List<LiteralMove> MakeMoveListFromText(string text)
        {
            string tmp;

            return MakeMoveListFromText(text, out tmp);
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
        public static List<LiteralMove> MakeMoveListFromText(string text, out string comment)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            // 指し手の解析前には文字列の正規化が必要ですが、
            // 最初に全部正規化してしまうと、最後のコメントも
            // 変わってしまうことがあります。
            // このため、文字列を空白で区切りながら
            // 正規化とパースを繰り返していきます。
            var result = new List<LiteralMove>();
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
                var list = MakeMoveListFromTextInternal(thisText).ToArray();
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

        /// <summary>
        /// 文字列に差し手が含まれていれば、順次切り出していきます。
        /// </summary>
        private static IEnumerable<LiteralMove> MakeMoveListFromTextInternal(string text)
        {
            while (true)
            {
                var parsedText = string.Empty;

                // 与えられた文字列の指し手を順次パースします。
                var move = ShogiParser.ParseMoveEx(
                    text, false, ref parsedText);
                if (move == null)
                {
                    break;
                }

                // パースが終わった部分から次のパースをはじめます。
                text = text.Substring(parsedText.Length);
                yield return move;
            }
        }
        #endregion

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
        private static Move FilterMoveFromLiteral(List<Move> boardMoveList,
                                                  LiteralMove referenceMove,
                                                  bool multipleIsNull = false)
        {
            IEnumerable<Move> boardMoveListTmp = boardMoveList;

            // 移動前の座標情報があれば、それを使います。
            if (!referenceMove.SrcSquare.IsEmpty())
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
                (list.Count == 1 ? list.FirstOrDefault() : null) :
                list.FirstOrDefault());
        }

        /// <summary>
        /// 上、引、寄るの判定をします。
        /// </summary>
        private static bool CheckRankMoveType(Move bm,
                                              LiteralMove referenceMove)
        {
            if (bm.ActionType == ActionType.Drop)
            {
                return false;
            }

            var fileMove = bm.DstSquare.GetFile() - bm.SrcSquare.GetFile();
            var rankMove = bm.DstSquare.GetRank() - bm.SrcSquare.GetRank();

            switch (referenceMove.RankMoveType)
            {
                case RankMoveType.Back:
                    return (bm.Colour == Colour.Black
                                ? (rankMove > 0)
                                : (rankMove < 0));
                case RankMoveType.Up:
                    return (bm.Colour == Colour.Black
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
        private static bool CheckRelPosType(Move bm,
                                            LiteralMove referenceMove,
                                            List<Move> boardMoveList)
        {
            if (bm.ActionType == ActionType.Drop)
            {
                return false;
            }

            if (bm.MovePiece == Piece.Dragon || bm.MovePiece == Piece.Horse)
            {
                // 竜、馬の場合、「直」は使わずに「右左」のみを使用します。
                if (boardMoveList.Count == 1)
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
                    var fileDif = bm.SrcSquare.GetFile() - other.SrcSquare.GetFile();

                    switch (referenceMove.RelFileType)
                    {
                        case RelFileType.Left:
                            return (bm.Colour == Colour.Black
                                        ? (fileDif > 0)
                                        : (fileDif < 0));
                        case RelFileType.Right:
                            return (bm.Colour == Colour.Black
                                        ? (fileDif < 0)
                                        : (fileDif > 0));
                        case RelFileType.None:
                            return (fileDif == 0);
                    }
                }
            }
            else
            {
                var fileMove = bm.DstSquare.GetFile() - bm.SrcSquare.GetFile();
                var rankMove = bm.DstSquare.GetRank() - bm.SrcSquare.GetRank();

                switch (referenceMove.RelFileType)
                {
                    case RelFileType.Left:
                        return (bm.Colour == Colour.Black
                                    ? (fileMove < 0)
                                    : (fileMove > 0));
                    case RelFileType.Right:
                        return (bm.Colour == Colour.Black
                                    ? (fileMove > 0)
                                    : (fileMove < 0));
                    case RelFileType.Straight:
                        return (bm.Colour == Colour.Black
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
        private static bool CheckActionType(Move bm,
                                            LiteralMove referenceMove, bool canMove)
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
        public static Move ConvertMoveFromLiteral(this Board board, LiteralMove move,
                                                  bool multipleIsNull = false)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            return board.ConvertMoveFromLiteral(move, board.Turn, multipleIsNull);
        }

        /// <summary>
        /// 文字列から得られた指し手から、移動前の情報も含むような
        /// 指し手情報を取得します。
        /// </summary>
        public static Move ConvertMoveFromLiteral(this Board board, LiteralMove move,
                                                  Colour colour,
                                                  bool multipleIsNull = false)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (move == null)
            {
                return null;
            }

            if (move.SameAsPrev && board.PrevMovedSquare.IsEmpty())
            {
                return null;
            }

            if (move.IsSpecialMove)
            {
                return Move.CreateSpecial(move.SpecialMoveType, colour);
            }

            // 移動後の位置を取得します。
            // 同○○なら前回の位置を使います。
            var dstSquare = move.DstSquare;
            if (move.SameAsPrev)
            {
                move = move.Clone();
                move.DstSquare = board.PrevMovedSquare;

                dstSquare = board.PrevMovedSquare;
            }

            var boardMoveList = board.ListupMoves(
                move.Piece.With(colour), dstSquare)
                .ToList();

            // 複数の指し手の中から適切な一つを選びます。
            var boardMove = FilterMoveFromLiteral(boardMoveList, move, multipleIsNull);
            if (boardMove == null)
            {
                return null;
            }

            return boardMove;
        }

        /// <summary>
        /// 指し手がすべて着手可能かどうかを確認します。
        /// </summary>
        public static bool CanMoveList(this Board board,
                                       IEnumerable<Move> moveList)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (moveList == null)
            {
                throw new ArgumentNullException(nameof(moveList));
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

        #region FilterMove
        /// <summary>
        /// 複数ある指し手の中から、適切なひとつの指し手を選択します。
        /// </summary>
        /// <remarks>
        /// <paramref name="referenceMove"/>にはXからYに移動したという情報
        /// しかないため、これを52金右などの指し手に変換します。
        /// </remarks>
        private static LiteralMove FilterLiteralFromMove(this Board board,
                                                         List<Move> moveList,
                                                         Move referenceMove,
                                                         Piece fromPiece,
                                                         bool useSrcSquare)
        {
            if (!moveList.Any())
            {
                return null;
            }

            var nextPos = referenceMove.DstSquare;

            // ベースとなるLiteralMoveを作成します。
            // 必要であればこのオブジェクトに引・寄などの属性を付けていきます。
            var baseMove = new LiteralMove
            {
                Colour = referenceMove.Colour,
                Piece = fromPiece,
                File = nextPos.GetFile(),
                Rank = nextPos.GetRank(),
                ActionType = ( // '打'は消える可能性があります。
                    referenceMove.ActionType == ActionType.Promote
                    ? ActionType.Promote
                    : referenceMove.ActionType != ActionType.Drop && Board.CanPromote(referenceMove)
                    ? ActionType.Unpromote
                    : ActionType.None),
                SameAsPrev = (board.PrevMovedSquare == nextPos),
            };

            // 移動元情報を使う場合は、これで終わりです。
            // （.kifファイルからの読み込み時は、駒の移動の場合は移動前情報がつき、
            //　 駒打ちの場合は"打"が必ずつきます）
            if (useSrcSquare)
            {
                baseMove.SrcSquare = referenceMove.SrcSquare;
                baseMove.ActionType = referenceMove.ActionType;
                return baseMove;
            }

            // 駒打ち、成り、不成りなどでフィルターします。
            var tmpMoveList = FilterAction(baseMove, referenceMove, moveList);
            if (!tmpMoveList.Any())
            {
                return null;
            }

            // 段の位置でフィルターします。
            tmpMoveList = FilterRank(baseMove, referenceMove, tmpMoveList);
            if (!tmpMoveList.Any())
            {
                return null;
            }

            // 列の位置でフィルターします。
            tmpMoveList = FilterFile(baseMove, referenceMove, tmpMoveList);
            if (!tmpMoveList.Any())
            {
                return null;
            }

            return baseMove;
        }

        /// <summary>
        /// 指し手の種類で手をフィルターし、Moveに適切なRankMoveTypeを設定します。
        /// </summary>
        private static List<Move> FilterAction(LiteralMove move,
                                               Move referenceMove,
                                               List<Move> boardMoveList)
        {
            var actionType = referenceMove.ActionType;
            var tmpMoveList = boardMoveList.Where(mv => mv.ActionType == actionType)
                .ToList();
            if (tmpMoveList.Count != boardMoveList.Count)
            {
                move.ActionType =
                    actionType == ActionType.Promote || actionType == ActionType.Drop
                    ? actionType
                    : Board.CanPromote(referenceMove)
                    ? ActionType.Unpromote
                    : ActionType.None;
            }

            return tmpMoveList;
        }

        /// <summary>
        /// 段で指し手をフィルターし、Moveに適切なRankMoveTypeを設定します。
        /// </summary>
        private static List<Move> FilterRank(LiteralMove move,
                                             Move referenceMove,
                                             List<Move> boardMoveList)
        {
            // 駒の移動前情報が必要です。
            var nextPos = referenceMove.DstSquare;
            var prevPos = referenceMove.SrcSquare;
            if (prevPos.IsEmpty())
            {
                // 何もフィルターしません。
                return boardMoveList;
            }

            // 黒から見ると正の場合は引く or 左へ移動(右)で
            // 負の場合は上がる or 右へ移動(左)です。
            var relRank = nextPos.GetRank() - prevPos.GetRank();

            // 段の位置でフィルターします。
            var tmpMoveList = boardMoveList.Where(mv =>
            {
                var rel = nextPos.GetRank() - mv.SrcSquare.GetRank();
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
            if (tmpMoveList.Count != boardMoveList.Count)
            {
                var relRank2 = relRank * referenceMove.Colour.Sign();

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
        private static List<Move> FilterFile(LiteralMove move,
                                             Move referenceMove,
                                             List<Move> boardMoveList)
        {
            // 駒の移動前情報が必要です。
            var nextPos = referenceMove.DstSquare;
            var prevPos = referenceMove.SrcSquare;
            if (prevPos.IsEmpty())
            {
                // 何もフィルターしません。
                return boardMoveList;
            }

            if ((move.Piece == Piece.Dragon || move.Piece == Piece.Horse) &&
                boardMoveList.Count == 2)
            {
                // 馬と竜の場合は'直'ではなく、右と左しか使いません。
                var other = (
                    referenceMove == boardMoveList[0] ?
                    boardMoveList[1] : boardMoveList[0]);

                // 動かす前の駒が相方に比べて右にあるか左にあるか調べます。
                // 先手の場合は筋が小さい方が右です。
                var relFile = prevPos.GetFile() - other.SrcSquare.GetFile();
                relFile *= (referenceMove.Colour == Colour.White ? -1 : +1);

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

                return new List<Move> { referenceMove };
            }
            else
            {
                // 黒から見ると正の場合は引く or 左へ移動(右)で
                // 負の場合は上がる or 右へ移動(左)です。
                var relFile = nextPos.GetFile() - prevPos.GetFile();

                // 列の位置でフィルターします。
                var tmpMoveList = boardMoveList.Where(mv =>
                {
                    var rel = nextPos.GetFile() - mv.SrcSquare.GetFile();
                    return (
                        relFile < 0 ? (rel < 0) :
                        relFile > 0 ? (rel > 0) :
                        (rel == 0));
                }).ToList();

                // 列情報でフィルターされた場合は、列の移動情報を付加します。
                if (tmpMoveList.Count != boardMoveList.Count)
                {
                    var relFile2 = relFile * referenceMove.Colour.Sign();

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
        public static LiteralMove ConvertLiteralFromMove(this Board board, Move move,
                                                         bool useSrcSquare)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (move == null || !move.Validate())
            {
                throw new ArgumentNullException(nameof(move));
            }

            if (move.IsSpecialMove)
            {
                return new LiteralMove
                {
                    Colour = move.Colour,
                    SpecialMoveType = move.SpecialMoveType,
                };
            }

            var fromPiece = move.MovePiece;
            if (fromPiece.IsNone())
            {
                return null;
            }

            // 駒の種類と最終位置から、あり得る指し手をすべて検索します。
            var boardMoveList = board.ListupMoves(fromPiece, move.DstSquare)
                .ToList();

            return FilterLiteralFromMove(
                board, boardMoveList, move, fromPiece, useSrcSquare);
        }

        /// <summary>
        /// 指し手をXX->YYの形式から、ZZ銀上などの形に変換します。
        /// </summary>
        /// <remarks>
        /// <paramref name="useSrcSquare"/>を真にすると、差し手の後に
        /// 古い位置の情報が付加されるようになります。(例: 32金(22))
        /// </remarks>
        public static List<LiteralMove> ConvertLiteralListFromMove(this Board board,
                                                                   IEnumerable<Move> moveList,
                                                                   bool useSrcSquare)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (moveList == null)
            {
                throw new ArgumentNullException(nameof(moveList));
            }

            var lmoveList = new List<LiteralMove>();
            var cloned = board.Clone();

            foreach (var move in moveList)
            {
                var lmove = ConvertLiteralFromMove(cloned, move, useSrcSquare);
                if (lmove == null)
                {
                    break;
                }

                if (!cloned.DoMove(move))
                {
                    break;
                }

                lmoveList.Add(lmove);
            }

            return lmoveList;
        }

        /// <summary>
        /// 与えられた指し手が着手可能か調べ、もし着手可能な場合はそれを正式な表記に変換します。
        /// </summary>
        /// <remarks>
        /// 「正式な表記」にするとは、たとえば不要な「打」を削除したり、
        /// 右や直などの表記を正確なものに修正することです。
        /// </remarks>
        public static LiteralMove NormalizeMove(this Board board, LiteralMove lmove)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (!board.Validate())
            {
                throw new ArgumentException(
                    "局面の状態が正しくありません。", nameof(board));
            }

            if (lmove == null)
            {
                throw new ArgumentNullException(nameof(lmove));
            }

            if (!lmove.Validate())
            {
                throw new ArgumentException(
                    "指し手の状態が正しくありません。", nameof(lmove));
            }

            // 投了などの特殊な指し手は常にさせることにします。
            /*if (move.IsSpecialMove)
            {
                return move;
            }*/

            // 一度、指し手の正規化を行います（打を消したり、左を追加するなど）
            // あり得る指し手が複数ある場合は失敗とします。
            var move = board.ConvertMoveFromLiteral(lmove, true);
            if (move == null || !board.CanMove(move))
            {
                return null;
            }

            // 指し手を表記形式に再度変換します。
            // 移動元の情報は使いません。("65銀(55)"という表記にはしません)
            var newLMove = board.ConvertLiteralFromMove(move, false);
            if (newLMove == null)
            {
                return null;
            }

            // 最後に元の文字列を保存して返します。
            newLMove.OriginalText = lmove.OriginalText;
            return newLMove;
        }
    }
}
