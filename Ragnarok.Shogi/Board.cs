using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

using Ragnarok;
using Ragnarok.ObjectModel;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 局面が変わったときに使われます。
    /// </summary>
    public class BoardChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 指された指し手を取得または設定します。
        /// </summary>
        public BoardMove Move
        {
            get;
            set;
        }

        /// <summary>
        /// アンドゥしたかどうかを取得または設定します。
        /// </summary>
        public bool IsUndo
        {
            get;
            set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BoardChangedEventArgs(BoardMove move, bool isUndo)
        {
            Move = move;
            IsUndo = isUndo;
        }
    }

    /// <summary>
    /// 盤面を示すクラスです。
    /// </summary>
    [DataContract()]
    public partial class Board : NotifyObject
    {
        /// <summary>
        /// 将棋盤のサイズです。
        /// </summary>
        public const int BoardSize = 9;

        /// <summary>
        /// [0,0]が盤面の11地点を示します。
        /// </summary>
        private BoardPiece[] board = new BoardPiece[81];
        [DataMember(Order = 1, IsRequired = true)]
        private CapturedPieceBox blackCapturedPieceBox = new CapturedPieceBox(BWType.Black);
        [DataMember(Order = 2, IsRequired = true)]
        private CapturedPieceBox whiteCapturedPieceBox = new CapturedPieceBox(BWType.White);
        [DataMember(Order = 3, IsRequired = true)]
        private BWType viewSide = BWType.Black;
        [DataMember(Order = 4, IsRequired = true)]
        private BWType turn = BWType.Black;
        [DataMember(Order = 5, IsRequired = true)]
        private Square prevMovedSquare = null;
        //[DataMember(Order = 6, IsRequired = true)]
        private List<BoardMove> moveList = new List<BoardMove>();
        //[DataMember(Order = 7, IsRequired = true)]
        private List<BoardMove> redoList = new List<BoardMove>();

        /// <summary>
        /// プロパティ値の変更を通知します。
        /// </summary>
        public override event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 局面に変更があったときに呼ばれます。
        /// </summary>
        public virtual event EventHandler<BoardChangedEventArgs> BoardChanging;

        /// <summary>
        /// 局面に変更があったときに呼ばれます。
        /// </summary>
        public virtual event EventHandler<BoardChangedEventArgs> BoardChanged;

        /// <summary>
        /// プロパティ値の変更を通知します。
        /// </summary>
        public override void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            Util.CallPropertyChanged(PropertyChanged, this, e);
        }

        /// <summary>
        /// 局面の変更直前の通知を出します。
        /// </summary>
        private void NotifyBoardChanging(BoardMove move, bool isUndo)
        {
            var handler = BoardChanging;

            if (handler != null)
            {
                Util.CallEvent(
                    () => handler(this, new BoardChangedEventArgs(move, isUndo)));
            }
        }

        /// <summary>
        /// 局面の変更を通知します。
        /// </summary>
        private void NotifyBoardChanged(BoardMove move, bool isUndo)
        {
            var handler = BoardChanged;

            if (handler != null)
            {
                Util.CallEvent(
                    () => handler(this, new BoardChangedEventArgs(move, isUndo)));
            }

            this.RaisePropertyChanged("MoveCount");
            this.RaisePropertyChanged("CanUndo");
            this.RaisePropertyChanged("CanRedo");
        }

        /// <summary>
        /// 盤を作成します。
        /// </summary>
        private BoardPiece[] CreatePieceMatrix()
        {
            return new BoardPiece[81];
        }

        /// <summary>
        /// オブジェクトのコピーを作成します。
        /// </summary>
        public Board Clone()
        {
            using (LazyLock())
            {
                var cloned = new Board(false)
                {
                    blackCapturedPieceBox = this.blackCapturedPieceBox.Clone(),
                    whiteCapturedPieceBox = this.whiteCapturedPieceBox.Clone(),
                    viewSide = this.viewSide,
                    turn = this.turn,
                    prevMovedSquare = (
                        this.prevMovedSquare != null ?
                        this.prevMovedSquare.Clone() :
                        null),
                    moveList = this.moveList.Select(move => move.Clone()).ToList(),
                    redoList = this.redoList.Select(move => move.Clone()).ToList(),
                };

                // 各位置に駒を設定します。
                for (var rank = 1; rank <= BoardSize; ++rank)
                {
                    for (var file = 1; file <= BoardSize; ++file)
                    {
                        var square = new Square(file, rank);
                        var piece = this[square];

                        cloned[square] = (
                            piece != null ?
                            piece.Clone() :
                            null);
                    }
                }

                return cloned;
            }
        }

        /// <summary>
        /// 今までの指し手をすべて取得します。
        /// </summary>
        public ReadOnlyCollection<BoardMove> MoveList
        {
            get
            {
                using (LazyLock())
                {
                    return new ReadOnlyCollection<BoardMove>(this.moveList);
                }
            }
        }

        /// <summary>
        /// 手を戻すことができるか取得します。
        /// </summary>
        public bool CanUndo
        {
            get
            {
                using (LazyLock())
                {
                    return this.moveList.Any();
                }
            }
        }

        /// <summary>
        /// 手を戻すことができる回数を取得します。
        /// </summary>
        public int CanUndoCount
        {
            get
            {
                using (LazyLock())
                {
                    return this.moveList.Count();
                }
            }
        }

        /// <summary>
        /// 手を進めることができるか取得します。
        /// </summary>
        public bool CanRedo
        {
            get
            {
                using (LazyLock())
                {
                    return this.redoList.Any();
                }
            }
        }

        /// <summary>
        /// 手を進めることができる回数を取得します。
        /// </summary>
        public int CanRedoCount
        {
            get
            {
                using (LazyLock())
                {
                    return this.redoList.Count();
                }
            }
        }

        /// <summary>
        /// 一番最後の指し手を取得します。
        /// </summary>
        public BoardMove LastMove
        {
            get
            {
                using (LazyLock())
                {
                    var move = this.moveList.LastOrDefault();

                    return (move != null ? move.Clone() : null);
                }
            }
        }

        /// <summary>
        /// <paramref name="dstSquare"/> にある駒を取得します。
        /// </summary>
        public BoardPiece this[Square square]
        {
            get
            {
                if (square == null || !square.Validate())
                {
                    return null;
                }

                var rank = square.Rank - 1;
                var file = BoardSize - square.File;

                using (LazyLock())
                {
                    return this.board[rank * 9 + file];
                }
            }
            set
            {
                if (square == null || !square.Validate())
                {
                    throw new ArgumentException("dstSquare");
                }

                var rank = square.Rank - 1;
                var file = BoardSize - square.File;

                using (LazyLock())
                {
                    this.board[rank * 9 + file] = value;
                }
            }
        }

        /// <summary>
        /// <paramref name="file"/> <paramref name="rank"/> にある駒を取得します。
        /// </summary>
        public BoardPiece this[int file, int rank]
        {
            get { return this[new Square(file, rank)]; }
            set { this[new Square(file, rank)] = value; }
        }

        /// <summary>
        /// 手番を取得または設定します。
        /// </summary>
        public BWType Turn
        {
            get { return this.turn; }
            set { SetValue("Turn", value, ref this.turn); }
        }

        /// <summary>
        /// 前回動かした駒の位置を取得します。
        /// </summary>
        public Square PrevMovedSquare
        {
            get { return this.prevMovedSquare; }
            set { SetValue("PrevMovedSquare", value, ref this.prevMovedSquare); }
        }

        /// <summary>
        /// 指し手の手数を取得します。
        /// </summary>
        public int MoveCount
        {
            get
            {
                using (LazyLock())
                {
                    return this.moveList.Count();
                }
            }
        }

        /// <summary>
        /// 持ち駒の数を取得します。
        /// </summary>
        public CapturedPieceBox GetCapturedPieceBox(BWType bwType)
        {
            if (bwType == BWType.Black)
            {
                return this.blackCapturedPieceBox;
            }
            else
            {
                return this.whiteCapturedPieceBox;
            }
        }

        /// <summary>
        /// 持ち駒の数を取得します。
        /// </summary>
        public int GetCapturedPieceCount(PieceType pieceType, BWType bwType)
        {
            using (LazyLock())
            {
                var capturedPiece = GetCapturedPieceBox(bwType);

                return capturedPiece.GetCount(pieceType);
            }
        }

        /// <summary>
        /// 持ち駒の数を設定します。
        /// </summary>
        public void SetCapturedPieceCount(PieceType pieceType, BWType bwType,
                                          int count)
        {
            using (LazyLock())
            {
                var capturedPiece = GetCapturedPieceBox(bwType);

                capturedPiece.SetCount(pieceType, count);
            }
        }

        /// <summary>
        /// 持ち駒の数を増やします。
        /// </summary>
        public void IncCapturedPieceCount(PieceType pieceType, BWType bwType)
        {
            using (LazyLock())
            {
                var capturedPiece = GetCapturedPieceBox(bwType);

                capturedPiece.Increment(pieceType);
            }
        }

        /// <summary>
        /// 持ち駒の数を減らします。
        /// </summary>
        public void DecCapturedPieceCount(PieceType pieceType, BWType bwType)
        {
            using (LazyLock())
            {
                var capturedPiece = GetCapturedPieceBox(bwType);

                capturedPiece.Decrement(pieceType);
            }
        }

        /// <summary>
        /// １手戻します。
        /// </summary>
        public BoardMove Undo()
        {
            using (LazyLock())
            {
                if (!this.moveList.Any())
                {
                    return null;
                }

                var move = this.moveList.Last();

                NotifyBoardChanging(move, true);

                // 盤の各状態を意って戻した状態に設定します。
                this.moveList.RemoveAt(this.moveList.Count - 1);

                // リドゥリストの最後に追加します。
                // 再現するときは最後の要素から使います。
                this.redoList.Add(move);

                // this.moveListからmoveから取り除かれた状態じゃないと
                // PrevMovedSquareの設定が上手くいかない。
                DoUndo(move);

                NotifyBoardChanged(move, true);
                return move;
            }
        }

        /// <summary>
        /// undo操作を実行します。
        /// </summary>
        private void DoUndo(BoardMove move)
        {
            if (move.ActionType == ActionType.Drop)
            {
                // 駒打ちの場合は、その駒を駒台に戻します。
                this[move.DstSquare] = null;

                IncCapturedPieceCount(move.DropPieceType, move.BWType);
            }
            else
            {
                var movedPiece = this[move.DstSquare];

                // 駒を成った場合はそれを元に戻します。
                if (move.ActionType == ActionType.Promote)
                {
                    movedPiece.IsPromoted = false;
                }

                // 駒を取った場合は、その駒を元に戻します。
                if (move.TookPiece != null)
                {
                    this[move.DstSquare] = new BoardPiece(
                        move.TookPiece.Clone(),
                        move.BWType.Toggle());

                    // 駒を取ったはずなので、その分を駒台から減らします。
                    DecCapturedPieceCount(
                        move.TookPiece.PieceType,
                        move.BWType);
                }
                else
                {
                    this[move.DstSquare] = null;
                }

                this[move.SrcSquare] = movedPiece;
            }

            Turn = Turn.Toggle();
            PrevMovedSquare = (
                this.moveList.Any() ?
                this.moveList.Last().DstSquare :
                null);
        }

        /// <summary>
        /// １手戻したのを再び復活します。
        /// </summary>
        public BoardMove Redo()
        {
            using (LazyLock())
            {
                var move = this.redoList.LastOrDefault();
                if (move == null)
                {
                    return null;
                }
                
                if (!CheckAndDoMove(move, false))
                {
                    // リドゥに失敗したら、どうすればいいんだ・・・
                    this.redoList.Clear();
                    return null;
                }

                return move;
            }
        }

        /// <summary>
        /// 可能な限り局面をUndoします。
        /// </summary>
        public void UndoAll()
        {
            using (LazyLock())
            {
                while (Undo() != null)
                {
                }
            }
        }

        /// <summary>
        /// 可能な限り局面をRedoします。
        /// </summary>
        public void RedoAll()
        {
            using (LazyLock())
            {
                while (Redo() != null)
                {
                }
            }
        }

        /// <summary>
        /// リドゥ用のリストをすべてクリアします。
        /// </summary>
        public void ClearRedoList()
        {
            using (LazyLock())
            {
                this.redoList.Clear();

                this.RaisePropertyChanged("CanRedo");
            }
        }

        /// <summary>
        /// その差し手が実際に実現できるか調べます。
        /// </summary>
        public bool CanMove(BoardMove move)
        {
            return CheckAndDoMove(move, true);
        }

        /// <summary>
        /// その差し手を実際に実行します。
        /// </summary>
        public bool DoMove(BoardMove move)
        {
            return CheckAndDoMove(move, false);
        }

        /// <summary>
        /// 駒を動かすか、または駒が動かせるか調べます。
        /// </summary>
        private bool CheckAndDoMove(BoardMove move, bool checkOnly)
        {
            if (move == null || !move.Validate())
            {
                throw new ArgumentNullException("move");
            }

            using (LazyLock())
            {
                // 手番があわなければ失敗とします。
                if (this.turn == BWType.None ||
                    this.turn != move.BWType)
                {
                    return false;
                }

                if (move.ActionType == ActionType.Drop)
                {
                    return CheckAndDoDrop(move, checkOnly);
                }
                else
                {
                    return CheckAndDoMoveOnly(move, checkOnly);
                }
            }
        }

        /// <summary>
        /// １手指したときに呼ばれます。
        /// </summary>
        private void MoveDone(BoardMove move)
        {
            NotifyBoardChanging(move, false);

            Turn = Turn.Toggle();
            PrevMovedSquare = move.DstSquare;

            this.moveList.Add(move);

            // リドゥスタックの更新を行います。
            var redoMove = this.redoList.LastOrDefault();
            if (redoMove != null && redoMove.Equals(move))
            {
                // 指し手がリドゥと同じなら手を一つ削るだけです。
                this.redoList.RemoveAt(this.redoList.Count - 1);
            }
            else
            {
                this.redoList.Clear();
            }

            // 局面の変化を通知します。
            NotifyBoardChanged(move, false);
        }

        /// <summary>
        /// 駒打ちの動作が行えるか調べ、必要なら実行します。
        /// </summary>
        private bool CheckAndDoDrop(BoardMove move, bool checkOnly)
        {
            if (GetCapturedPieceCount(move.DropPieceType, move.BWType) <= 0)
            {
                return false;
            }

            // 駒を打つ場所に駒があれば、当然失敗です。
            var piece = this[move.DstSquare];
            if (piece != null)
            {
                return false;
            }

            // 先手はそのまま後手なら上下を反転してから、駒が置けるか確かめます。
            var rank =
                ( move.BWType == BWType.Black
                ? move.DstSquare.Rank
                : (BoardSize + 1) - move.DstSquare.Rank);
            switch (move.DropPieceType)
            {
                case PieceType.None: // これは打てない
                    return false;
                case PieceType.Hu:
                    // 2歩のチェックを行います。
                    if (IsDoublePawn(move.BWType, move.DstSquare))
                    {
                        return false;
                    }
                    goto case PieceType.Kyo;
                case PieceType.Kyo:
                    if (rank == 1)
                    {
                        return false;
                    }
                    break;
                case PieceType.Kei:
                    if (rank < 3) // 1,2はダメ
                    {
                        return false;
                    }
                    break;
            }

            if (!checkOnly)
            {
                // 駒を盤面に置き、持ち駒から駒を減らします。
                this[move.DstSquare] = new BoardPiece(
                    move.DropPieceType, false, move.BWType);

                DecCapturedPieceCount(move.DropPieceType, move.BWType);

                MoveDone(move);
            }

            return true;
        }

        /// <summary>
        /// ２歩のチェックを行います。
        /// </summary>
        private bool IsDoublePawn(BWType bwType, Square square)
        {
            for (var i = 0; i < BoardSize; ++i)
            {
                var piece = this[square.File, i];

                if (piece != null &&
                    piece.BWType == bwType &&
                    piece.PieceType == PieceType.Hu &&
                    !piece.IsPromoted)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 駒の移動のみの動作を調べるか実際にそれを行います。
        /// </summary>
        private bool CheckAndDoMoveOnly(BoardMove move, bool checkOnly)
        {
            // 駒の移動元に自分の駒がなければダメ
            var srcPiece = this[move.SrcSquare];
            if (srcPiece == null || srcPiece.BWType != move.BWType)
            {
                return false;
            }

            // 駒の移動先に自分の駒があったらダメ
            var dstPiece = this[move.DstSquare];
            if (dstPiece != null && dstPiece.BWType == move.BWType)
            {
                return false;
            }

            // これはエラーだけど。。。ｗ
            if (srcPiece.Piece != move.MovePiece)
            {
                return false;
            }

            // 各駒が動ける位置に移動するかどうか確認します。
            if (!CanMovePiece(move))
            {
                return false;
            }

            if (move.ActionType == ActionType.Promote)
            {
                // 成れない場合は帰ります。
                if (!CanPromote(move)) return false;
            }
            else
            {
                // 成らないといけない場合は帰ります。
                if (IsPromoteForce(move)) return false;
            }

            if (!checkOnly)
            {
                var pieceType = srcPiece.PieceType;

                // 移動先に駒があれば、それを自分のものにします。
                if (dstPiece != null)
                {
                    IncCapturedPieceCount(dstPiece.PieceType, move.BWType);

                    // 取った駒を記憶しておきます。
                    move.TookPiece = dstPiece.Piece;
                }

                // 移動後の駒の成り/不成りを決定します。
                var promoted = (
                    srcPiece.IsPromoted ||
                    move.ActionType == ActionType.Promote);

                this[move.DstSquare] = new BoardPiece(
                    pieceType, promoted, move.BWType);

                // 移動前の位置からは駒をなくします。
                this[move.SrcSquare] = null;

                MoveDone(move);
            }

            return true;
        }

        /// <summary>
        /// 駒が成れるか調べます。
        /// </summary>
        public static bool CanPromote(BoardMove move)
        {
            if (move == null)
            {
                throw new ArgumentNullException("move");
            }

            if (move.DstSquare == null || !move.DstSquare.Validate())
            {
                return false;
            }

            // 駒の移動でない場合は成れません。
            if (move.SrcSquare == null || !move.SrcSquare.Validate())
            {
                return false;
            }

            // 既に成っている駒を再度成ることはできません。
            var piece = move.MovePiece;
            if (piece == null || piece.IsPromoted)
            {
                return false;
            }

            var srcRank = move.SrcSquare.Rank;
            var dstRank = move.DstSquare.Rank;

            if (move.BWType == BWType.White)
            {
                srcRank = (BoardSize + 1) - srcRank;
                dstRank = (BoardSize + 1) - dstRank;
            }

            // 1,2,3の段の時だけ、成ることができます。
            if (srcRank > 3 && dstRank > 3)
            {
                return false;
            }

            // 金玉は成れません。
            if (piece.PieceType == PieceType.None ||
                piece.PieceType == PieceType.Gyoku ||
                piece.PieceType == PieceType.Kin)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 駒を強制的に成る必要があるか調べます。
        /// </summary>
        public static bool IsPromoteForce(BoardMove move)
        {
            if (move.DstSquare == null || !move.DstSquare.Validate())
            {
                return false;
            }

            // 駒の移動元に自分の駒がなければダメ
            var piece = move.MovePiece;
            if (piece == null || piece.IsPromoted)
            {
                return false;
            }

            var normalizedRank = (
                move.BWType == BWType.White ?
                (BoardSize + 1) - move.DstSquare.Rank :
                move.DstSquare.Rank);

            if (piece.PieceType == PieceType.Kei)
            {
                return (normalizedRank <= 2);
            }
            else if (piece.PieceType == PieceType.Kyo ||
                     piece.PieceType == PieceType.Hu)
            {
                return (normalizedRank == 1);
            }

            return false;
        }

        /// <summary>
        /// オブジェクトの妥当性を検証します。
        /// </summary>
        public bool Validate()
        {
            using (LazyLock())
            {
                if (this.blackCapturedPieceBox == null ||
                    !this.blackCapturedPieceBox.Validate())
                {
                    return false;
                }

                if (this.whiteCapturedPieceBox == null ||
                    !this.whiteCapturedPieceBox.Validate())
                {
                    return false;
                }

                if (!Enum.IsDefined(typeof(BWType), this.viewSide) ||
                    this.viewSide == BWType.None)
                {
                    return false;
                }

                if (!Enum.IsDefined(typeof(BWType), this.turn) ||
                    this.turn == BWType.None)
                {
                    return false;
                }

                if (this.prevMovedSquare != null &&
                    !this.prevMovedSquare.Validate())
                {
                    return false;
                }

                if (this.moveList == null ||
                    this.moveList.Any(move => !move.Validate()))
                {
                    return false;
                }

                // 盤上の各駒が正しいか調べます。
                for (var rank = 1; rank <= BoardSize; ++rank)
                {
                    for (var file = 1; file <= BoardSize; ++file)
                    {
                        var piece = this[file, rank];

                        if (piece != null && !piece.Validate())
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// オブジェクトの等値性を判定します。
        /// </summary>
        public static bool BoardEquals(Board x, Board y)
        {
            var result = Util.PreEquals(x, y);
            if (result != null)
            {
                return result.Value;
            }

            return x.BoardEquals(y);
        }

        /// <summary>
        /// オブジェクトの等値性を判定します。
        /// </summary>
        public bool BoardEquals(Board other)
        {
            if ((object)other == null)
            {
                return false;
            }

            using (LazyLock())
            using (other.LazyLock())
            {
                for (var rank = 1; rank < BoardSize; ++rank)
                {
                    for (var file = 1; file < BoardSize; ++file)
                    {
                        var square = new Square(file, rank);

                        if (this[square] != other[square])
                        {
                            return false;
                        }
                    }
                }

                if (!this.blackCapturedPieceBox.Equals(other.blackCapturedPieceBox) ||
                    !this.whiteCapturedPieceBox.Equals(other.whiteCapturedPieceBox))
                {
                    return false;
                }

                if (this.turn != other.turn)
                {
                    return false;
                }

                /*if (this.prevMovedSquare != other.prevMovedSquare)
                {
                    return false;
                }*/

                //private List<BoardMove> moveList = new List<BoardMove>();
                return true;
            }
        }

        /// <summary>
        /// ハッシュ値を返します。
        /// </summary>
        public int GetBoardHash()
        {
            using (LazyLock())
            {
                var hash = 0;

                for (var rank = 1; rank < BoardSize; ++rank)
                {
                    for (var file = 1; file < BoardSize; ++file)
                    {
                        var piece = this[file, rank];

                        if (piece != null)
                        {
                            hash ^= piece.GetHashCode();
                        }
                    }
                }

                //private List<BoardMove> moveList = new List<BoardMove>();
                return (hash ^
                    this.blackCapturedPieceBox.GetHashCode() ^
                    this.whiteCapturedPieceBox.GetHashCode() ^
                    this.turn.GetHashCode() ^
                    (this.prevMovedSquare != null ?
                        this.prevMovedSquare.GetHashCode() : 0));
            }
        }

        #region Serialize
        /// <summary>
        /// 局面をシリアライズするために一時的に使います。
        /// </summary>
        /// <remarks>
        /// protobuf-netが二次元配列に対応していないため、
        /// 一次元配列を使っています。
        /// </remarks>
        [DataMember(Order = 10, IsRequired = true)]
        private byte[] serializeBoard = null;
        [DataMember(Order = 11, IsRequired = true)]
        private byte[] moveListBytes = null;
        [DataMember(Order = 12, IsRequired = true)]
        private byte[] redoListBytes = null;

        /// <summary>
        /// 局面をバイト列に変換します。
        /// </summary>
        private byte[] SerializePieces()
        {
            var result = new byte[BoardSize * BoardSize];

            // 局面の正規化(nullオブジェクトを作らない)を行います。
            for (var rank = 1; rank <= BoardSize; ++rank)
            {
                for (var file = 1; file <= BoardSize; ++file)
                {
                    var index = (rank - 1) * BoardSize + (file - 1);
                    var piece = this[file, rank];
                    
                    // PieceType.Noneが0のため、
                    // 正しいピースのシリアライズデータは０以外の
                    // 数字となります。
                    result[index] = (piece == null ?
                        (byte)0 :
                        piece.Serialize());
                }
            }

            return result;
        }

        /// <summary>
        /// 局面をバイト列から取得します。
        /// </summary>
        private void DeserializePieces(byte[] bytes)
        {
            // 局面の正規化(nullオブジェクトを作らない)を行います。
            for (var rank = 1; rank <= BoardSize; ++rank)
            {
                for (var file = 1; file <= BoardSize; ++file)
                {
                    var index = (rank - 1) * BoardSize + (file - 1);

                    // PieceType.Noneが0のため、
                    // 正しいピースのシリアライズデータは０以外の
                    // 数字となります。
                    BoardPiece piece = null;
                    if (bytes[index] != 0)
                    {
                        piece = new BoardPiece();
                        piece.Deserialize(bytes[index]);
                    }

                    this[file, rank] = piece;
                }
            }
        }

        /// <summary>
        /// 指し手リストをバイト列に直します。
        /// </summary>
        private byte[] SerializeMoveList(List<BoardMove> moveList)
        {
            var result = new byte[4 * moveList.Count()];

            for (var i = 0; i < moveList.Count(); ++i)
            {
                var bits = (int)moveList[i].Serialize();

                result[i * 4 + 0] = (byte)((bits >>  0) & 0xff);
                result[i * 4 + 1] = (byte)((bits >>  8) & 0xff);
                result[i * 4 + 2] = (byte)((bits >> 16) & 0xff);
                result[i * 4 + 3] = (byte)((bits >> 24) & 0xff);
            }

            return result;
        }

        /// <summary>
        /// バイト列から指し手リストを取得します。
        /// </summary>
        private List<BoardMove> DeserializeMoveList(byte[] moveListBytes)
        {
            // 指し手が無い場合、配列はnullになります。
            if (moveListBytes == null)
            {
                return new List<BoardMove>();
            }

            var result = new List<BoardMove>(moveListBytes.Count() / 4);
            for (var i = 0; i < moveListBytes.Count(); i += 4)
            {
                var bits = (
                    (moveListBytes[i + 0] <<  0) |
                    (moveListBytes[i + 1] <<  8) |
                    (moveListBytes[i + 2] << 16) |
                    (moveListBytes[i + 3] << 24));

                var boardMove = new BoardMove();
                boardMove.Deserialize((uint)bits);

                result.Add(boardMove);
            }

            return result;
        }

        /// <summary>
        /// シリアライズ前に呼び出されます。
        /// </summary>
        [OnSerializing()]
        private void OnBeforeSerialize(StreamingContext context)
        {
            using (LazyLock())
            {
                this.serializeBoard = SerializePieces();

                // 指し手のシリアライズを行います。
                this.moveListBytes = SerializeMoveList(this.moveList);
                this.redoListBytes = SerializeMoveList(this.redoList);
            }
        }

        /// <summary>
        /// シリアライズ後に呼び出されます。
        /// </summary>
        [OnDeserialized()]
        private void OnAfterDeserialize(StreamingContext context)
        {
            if (this.serializeBoard == null)
            {
                throw new InvalidOperationException(
                    "serializeBoardがnullです。");
            }

            // 盤面を設定します。
            this.board = CreatePieceMatrix();
            DeserializePieces(this.serializeBoard);

            // 指し手のデシリアライズを行います。
            this.moveList = DeserializeMoveList(this.moveListBytes);
            this.redoList = DeserializeMoveList(this.redoListBytes);
        }
        #endregion

        /// <summary>
        /// 局面をシリアライズします。
        /// </summary>
        public byte[] Serialize()
        {
            // 局面をデシリアライズします。
            try
            {
                return Ragnarok.Net.ProtoBuf.PbUtil.Serialize(this);
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "局面のシリアライズに失敗しました。");
                return null;
            }
        }

        /// <summary>
        /// 局面をデシリアライズします。
        /// </summary>
        public static Board Deserialize(byte[] binData)
        {
            // 局面をデシリアライズします。
            try
            {
                return Ragnarok.Net.ProtoBuf.PbUtil.Deserialize<Board>(binData);
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "局面のデシリアライズに失敗しました。");
                return null;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Board(bool isInitPiece)
        {
            this.board = CreatePieceMatrix();

            if (isInitPiece)
            {
                this[1, 9] = new BoardPiece(PieceType.Kyo, false, BWType.Black);
                this[2, 9] = new BoardPiece(PieceType.Kei, false, BWType.Black);
                this[3, 9] = new BoardPiece(PieceType.Gin, false, BWType.Black);
                this[4, 9] = new BoardPiece(PieceType.Kin, false, BWType.Black);
                this[5, 9] = new BoardPiece(PieceType.Gyoku, false, BWType.Black);
                this[6, 9] = new BoardPiece(PieceType.Kin, false, BWType.Black);
                this[7, 9] = new BoardPiece(PieceType.Gin, false, BWType.Black);
                this[8, 9] = new BoardPiece(PieceType.Kei, false, BWType.Black);
                this[9, 9] = new BoardPiece(PieceType.Kyo, false, BWType.Black);
                this[2, 8] = new BoardPiece(PieceType.Hisya, false, BWType.Black);
                this[8, 8] = new BoardPiece(PieceType.Kaku, false, BWType.Black);
                for (var file = 1; file <= BoardSize; ++file)
                {
                    this[file, 7] = new BoardPiece(PieceType.Hu, false, BWType.Black);
                }

                this[1, 1] = new BoardPiece(PieceType.Kyo, false, BWType.White);
                this[2, 1] = new BoardPiece(PieceType.Kei, false, BWType.White);
                this[3, 1] = new BoardPiece(PieceType.Gin, false, BWType.White);
                this[4, 1] = new BoardPiece(PieceType.Kin, false, BWType.White);
                this[5, 1] = new BoardPiece(PieceType.Gyoku, false, BWType.White);
                this[6, 1] = new BoardPiece(PieceType.Kin, false, BWType.White);
                this[7, 1] = new BoardPiece(PieceType.Gin, false, BWType.White);
                this[8, 1] = new BoardPiece(PieceType.Kei, false, BWType.White);
                this[9, 1] = new BoardPiece(PieceType.Kyo, false, BWType.White);
                this[2, 2] = new BoardPiece(PieceType.Kaku, false, BWType.White);
                this[8, 2] = new BoardPiece(PieceType.Hisya, false, BWType.White);
                for (var file = 1; file <= BoardSize; ++file)
                {
                    this[file, 3] = new BoardPiece(PieceType.Hu, false, BWType.White);
                }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Board()
            : this(true)
        {
        }
    }
}
