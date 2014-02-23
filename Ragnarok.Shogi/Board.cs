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
    public class Board : NotifyObject
    {
        /// <summary>
        /// 将棋盤のサイズです。
        /// </summary>
        public const int BoardSize = 9;

        /// <summary>
        /// [0,0]が盤面の11地点を示します。
        /// </summary>
        private List<NotifyCollection<BoardPiece>> board =
            new List<NotifyCollection<BoardPiece>>();
        [DataMember(Order = 1, IsRequired = true)]
        private CapturedPieceBox blackCapturedPieceBox = new CapturedPieceBox(BWType.Black);
        [DataMember(Order = 2, IsRequired = true)]
        private CapturedPieceBox whiteCapturedPieceBox = new CapturedPieceBox(BWType.White);
        [DataMember(Order = 3, IsRequired = true)]
        private BWType viewSide = BWType.Black;
        [DataMember(Order = 4, IsRequired = true)]
        private BWType turn = BWType.Black;
        [DataMember(Order = 5, IsRequired = true)]
        private Position prevMovedPosition = null;
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
        private List<NotifyCollection<BoardPiece>> CreatePieceMatrix()
        {
            var result = new List<NotifyCollection<BoardPiece>>();

            for (var rank = 0; rank < Board.BoardSize; ++rank)
            {
                var list = new NotifyCollection<BoardPiece>();

                for (var file = 0; file < Board.BoardSize; ++file)
                {
                    list.Add(null);
                }

                result.Add(list);
            }

            return result;
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
                    prevMovedPosition = (
                        this.prevMovedPosition != null ?
                        this.prevMovedPosition.Clone() :
                        null),
                    moveList = this.moveList.Select(move => move.Clone()).ToList(),
                    redoList = this.redoList.Select(move => move.Clone()).ToList(),
                };

                // 各位置に駒を設定します。
                for (var rank = 1; rank <= BoardSize; ++rank)
                {
                    for (var file = 1; file <= BoardSize; ++file)
                    {
                        var position = new Position(file, rank);
                        var piece = this[position];

                        cloned[position] = (
                            piece != null ?
                            piece.Clone() :
                            null);
                    }
                }

                return cloned;
            }
        }

        /// <summary>
        /// 駒の二次元配列を取得します。
        /// </summary>
        public List<NotifyCollection<BoardPiece>> PieceMatrix
        {
            get
            {
                return this.board;
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
        /// <paramref name="position"/> にある駒を取得します。
        /// </summary>
        public BoardPiece this[Position position]
        {
            get
            {
                if (position == null || !position.Validate())
                {
                    return null;
                }

                var rank = position.Rank - 1;
                var file = BoardSize - position.File;

                using (LazyLock())
                {
                    return this.board[rank][file];
                }
            }
            set
            {
                if (position == null || !position.Validate())
                {
                    throw new ArgumentException("position");
                }

                var rank = position.Rank - 1;
                var file = BoardSize - position.File;

                using (LazyLock())
                {
                    this.board[rank][file] = value;
                }
            }
        }

        /// <summary>
        /// <paramref name="file"/> <paramref name="rank"/> にある駒を取得します。
        /// </summary>
        public BoardPiece this[int file, int rank]
        {
            get { return this[new Position(file, rank)]; }
            set { this[new Position(file, rank)] = value; }
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
        public Position PrevMovedPosition
        {
            get { return this.prevMovedPosition; }
            set { SetValue("PrevMovedPosition", value, ref this.prevMovedPosition); }
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
        public int GetCapturedPieceCount(BoardPiece boardPiece)
        {
            if (boardPiece == null)
            {
                return -1;
            }

            return GetCapturedPieceCount(boardPiece.BWType, boardPiece.PieceType);
        }

        /// <summary>
        /// 持ち駒の数を取得します。
        /// </summary>
        public int GetCapturedPieceCount(BWType bwType, PieceType pieceType)
        {
            using (LazyLock())
            {
                var capturedPiece = GetCapturedPieceBox(bwType);

                return capturedPiece.GetCount(pieceType);
            }
        }

        /// <summary>
        /// 持ち駒の数を増やします。
        /// </summary>
        public void IncCapturedPieceCount(BWType bwType, PieceType pieceType)
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
        public void DecCapturedPieceCount(BWType bwType, PieceType pieceType)
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
                // PrevMovedPositionの設定が上手くいかない。
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
                this[move.NewPosition] = null;

                IncCapturedPieceCount(move.BWType, move.DropPieceType);
            }
            else
            {
                var movedPiece = this[move.NewPosition];

                // 駒を成った場合はそれを元に戻します。
                if (move.ActionType == ActionType.Promote)
                {
                    movedPiece.IsPromoted = false;
                }

                // 駒を取った場合は、その駒を元に戻します。
                if (move.TookPiece != null)
                {
                    this[move.NewPosition] = move.TookPiece.Clone();

                    // 駒を取ったはずなので、その分を駒台から減らします。
                    DecCapturedPieceCount(
                        move.BWType,
                        move.TookPiece.PieceType);
                }
                else
                {
                    this[move.NewPosition] = null;
                }

                this[move.OldPosition] = movedPiece;
            }

            Turn = Turn.Toggle();
            PrevMovedPosition = (
                this.moveList.Any() ?
                this.moveList.Last().NewPosition :
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
        /// ２歩のチェックを行います。
        /// </summary>
        private bool IsDoublePawn(BWType bwType, Position position)
        {
            for (var i = 0; i < BoardSize; ++i)
            {
                var piece = this[position.File, i];

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
        /// １手指したときに呼ばれます。
        /// </summary>
        private void MoveDone(BoardMove move)
        {
            NotifyBoardChanging(move, false);

            Turn = Turn.Toggle();
            PrevMovedPosition = move.NewPosition;

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
            if (GetCapturedPieceCount(move.BWType, move.DropPieceType) <= 0)
            {
                return false;
            }

            // 駒を打つ場所に駒があれば、当然失敗です。
            var piece = this[move.NewPosition];
            if (piece != null)
            {
                return false;
            }

            // 先手はそのまま後手なら上下を反転してから、駒が置けるか確かめます。
            var rank =
                ( move.BWType == BWType.Black
                ? move.NewPosition.Rank
                : (BoardSize + 1) - move.NewPosition.Rank);
            switch (move.DropPieceType)
            {
                case PieceType.None: // これは打てない
                    return false;
                case PieceType.Hu:
                    // 2歩のチェックを行います。
                    if (IsDoublePawn(move.BWType, move.NewPosition))
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
                this[move.NewPosition] = new BoardPiece()
                {
                    PieceType = move.DropPieceType,
                    BWType = move.BWType,
                    IsPromoted = false,
                };

                DecCapturedPieceCount(move.BWType, move.DropPieceType);

                MoveDone(move);
            }

            return true;
        }

        /// <summary>
        /// 駒が成れるか調べます。
        /// </summary>
        public static bool CanPromote(BoardMove move, BoardPiece piece)
        {
            if (move.NewPosition == null || !move.NewPosition.Validate())
            {
                return false;
            }

            // 駒の移動元に自分の駒がなければダメ
            if (piece == null || piece.BWType != move.BWType)
            {
                return false;
            }

            if (piece.IsPromoted)
            {
                return false;
            }

            // 駒の移動でない場合は成れません。
            if (move.OldPosition == null || !move.OldPosition.Validate())
            {
                return false;
            }

            var moveFromRank = move.OldPosition.Rank;
            var moveToRank = move.NewPosition.Rank;

            if (move.BWType == BWType.White)
            {
                moveFromRank = (BoardSize + 1) - moveFromRank;
                moveToRank = (BoardSize + 1) - moveToRank;
            }

            // 1,2,3の段の時だけ、成ることができます。
            if (moveFromRank > 3 && moveToRank > 3)
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
        public static bool IsPromoteForce(BoardMove move, BoardPiece piece)
        {
            if (move.NewPosition == null || !move.NewPosition.Validate())
            {
                return false;
            }

            // 駒の移動元に自分の駒がなければダメ
            if (piece == null || piece.BWType != move.BWType)
            {
                return false;
            }

            if (piece.IsPromoted)
            {
                return false;
            }

            var normalizedRank = (
                move.BWType == BWType.White ?
                (BoardSize + 1) - move.NewPosition.Rank :
                move.NewPosition.Rank);

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
        /// 駒の移動のみの動作を調べるか実際にそれを行います。
        /// </summary>
        private bool CheckAndDoMoveOnly(BoardMove move, bool checkOnly)
        {
            // 駒の移動元に自分の駒がなければダメ
            var moveFromPiece = this[move.OldPosition];
            if (moveFromPiece == null || moveFromPiece.BWType != move.BWType)
            {
                return false;
            }

            // 駒の移動先に自分の駒があったらダメ
            var moveToPiece = this[move.NewPosition];
            if (moveToPiece != null && moveToPiece.BWType == move.BWType)
            {
                return false;
            }

            // 各駒が動ける位置に移動するかどうか確認します。
            if (!CanMovePiece(move, moveFromPiece))
            {
                return false;
            }

            // 成ることができなければ帰ります。
            if (move.ActionType == ActionType.Promote &&
                !CanPromote(move, moveFromPiece))
            {
                return false;
            }
#if false
            if (move.ActionType == ActionType.Promote)
            {
                if (!CanPromote(move, moveFromPiece)) return false;
            }
            else
            {
                if (IsPromoteForce(move, moveFromPiece)) return false;
            }
#endif

            if (!checkOnly)
            {
                var pieceType = moveFromPiece.PieceType;

                // 移動先に駒があれば、それを自分のものにします。
                if (moveToPiece != null)
                {
                    IncCapturedPieceCount(move.BWType, moveToPiece.PieceType);

                    // 取った駒を記憶しておきます。
                    move.TookPiece = moveToPiece.Clone();
                }

                // 移動後の駒の成り/不成りを決定します。
                var promoted = (
                    moveFromPiece.IsPromoted ||
                    move.ActionType == ActionType.Promote ||
                    IsPromoteForce(move, moveFromPiece));

                this[move.NewPosition] = new BoardPiece()
                {
                    PieceType = pieceType,
                    IsPromoted = promoted,
                    BWType = move.BWType,
                };

                // 移動前の位置からは駒をなくします。
                this[move.OldPosition] = null;

                MoveDone(move);
            }

            return true;
        }

        #region move table
        private static readonly int[] MoveTableGyoku = new int[]
        {
            0 + BoardSize * (-1),
            0 + BoardSize * (+1),
            -1 + BoardSize * (0),
            +1 + BoardSize * (0),
            -1 + BoardSize * (-1),
            +1 + BoardSize * (-1),
            -1 + BoardSize * (+1),
            +1 + BoardSize * (+1),
        };
        private static readonly int[] MoveTableKin = new int[]
        {
            0 + BoardSize * (-1),
            0 + BoardSize * (+1),
            -1 + BoardSize * (-1),
            +1 + BoardSize * (-1),
            -1 + BoardSize * (0),
            +1 + BoardSize * (0),
        };
        private static readonly int[] MoveTableGin = new int[]
        {
            0 + BoardSize * (-1),
            -1 + BoardSize * (-1),
            +1 + BoardSize * (-1),
            -1 + BoardSize * (+1),
            +1 + BoardSize * (+1),
        };
        private static readonly int[] MoveTableKei = new int[]
        {
            -1 + BoardSize * (-2),
            +1 + BoardSize * (-2),
        };
        private static readonly int[] MoveTableHu = new int[]
        {
            0 + BoardSize * (-1),
        };

        /// <summary>
        /// 指定の相対位置に動けるか調べます。(先手専用)
        /// </summary>
        private bool CanMovePiece(BWType bwType, int relFile, int relRank, int[] table)
        {
            // 後手側なら上下反転します。
            if (bwType == BWType.White)
            {
                relRank = -relRank;
            }

            // これをしないと右から左へジャンプする。
            if (Math.Abs(relFile) > 2 || Math.Abs(relRank) > 2)
            {
                return false;
            }

            var value = relFile + BoardSize * relRank;

            return (Array.IndexOf(table, value) >= 0);
        }
        #endregion

        /// <summary>
        /// 香車が指定の場所に動けるか判断します。
        /// </summary>
        private bool CanMoveKyo(BWType bwType, Position basePos, int relFile, int relRank)
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
        private bool CanMoveHisya(Position basePos, int relFile, int relRank)
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
        private bool CanMoveKaku(Position basePos, int relFile, int relRank)
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

        /// <summary>
        /// 実際に駒が動けるか確認します。
        /// </summary>
        private bool CanMovePiece(BoardMove move, BoardPiece piece)
        {
            var relFile = move.NewPosition.File - move.OldPosition.File;
            var relRank = move.NewPosition.Rank - move.OldPosition.Rank;

            if (piece.PieceType == PieceType.None)
            {
                return false;
            }

            if (piece.IsPromoted)
            {
                // 成り駒が指定の場所に動けるか調べます。
                switch (piece.PieceType)
                {
                    case PieceType.Gyoku:
                        return CanMovePiece(piece.BWType, relFile, relRank, MoveTableGyoku);
                    case PieceType.Hisya:
                        if (CanMovePiece(piece.BWType, relFile, relRank, MoveTableGyoku))
                        {
                            return true;
                        }
                        return CanMoveHisya(move.OldPosition, relFile, relRank);
                    case PieceType.Kaku:
                        if (CanMovePiece(piece.BWType, relFile, relRank, MoveTableGyoku))
                        {
                            return true;
                        }
                        return CanMoveKaku(move.OldPosition, relFile, relRank);
                    case PieceType.Kin:
                    case PieceType.Gin:
                    case PieceType.Kei:
                    case PieceType.Kyo:
                    case PieceType.Hu:
                        return CanMovePiece(piece.BWType, relFile, relRank, MoveTableKin);
                }
            }
            else
            {
                // 成り駒以外の駒が指定の場所に動けるか調べます。
                switch (piece.PieceType)
                {
                    case PieceType.Gyoku:
                        return CanMovePiece(piece.BWType, relFile, relRank, MoveTableGyoku);
                    case PieceType.Hisya:
                        return CanMoveHisya(move.OldPosition, relFile, relRank);
                    case PieceType.Kaku:
                        return CanMoveKaku(move.OldPosition, relFile, relRank);
                    case PieceType.Kin:
                        return CanMovePiece(piece.BWType, relFile, relRank, MoveTableKin);
                    case PieceType.Gin:
                        return CanMovePiece(piece.BWType, relFile, relRank, MoveTableGin);
                    case PieceType.Kei:
                        return CanMovePiece(piece.BWType, relFile, relRank, MoveTableKei);
                    case PieceType.Kyo:
                        return CanMoveKyo(piece.BWType, move.OldPosition, relFile, relRank);
                    case PieceType.Hu:
                        return CanMovePiece(piece.BWType, relFile, relRank, MoveTableHu);
                }
            }

            return false;
        }

        /// <summary>
        /// 駒がそこに動かせるか調べ、動かせる場合は可能な指し手を追加します。
        /// </summary>
        private IEnumerable<BoardMove> GetMovableMove(BoardPiece piece,
                                                      Position newPosition,
                                                      Position oldPosition)
        {
            if (!oldPosition.Validate())
            {
                yield break;
            }

            // 移動前の駒が指定の駒と同じかどうか
            // 判定する必要があります。
            var moveFromPiece = this[oldPosition];
            if (moveFromPiece == null ||
                moveFromPiece.PieceType != piece.PieceType ||
                moveFromPiece.IsPromoted != piece.IsPromoted)
            {
                yield break;
            }

            var canUnpromote = false;

            // 成り駒でなければ、成る可能性があります。
            if (!piece.IsPromoted)
            {
                var movePromote = new BoardMove()
                {
                    BWType = piece.BWType,
                    NewPosition = newPosition,
                    OldPosition = oldPosition,
                    ActionType = ActionType.Promote,
                };
                if (CanMove(movePromote))
                {
                    yield return movePromote;

                    // 成れるということは成らずの選択が必要になります。
                    canUnpromote = true;
                }
            }

            var moveUnpromote = new BoardMove()
            {
                BWType = piece.BWType,
                NewPosition = newPosition,
                OldPosition = oldPosition,
                ActionType = (canUnpromote ?
                    ActionType.Unpromote :
                    ActionType.None),
            };
            if (CanMove(moveUnpromote))
            {
                yield return moveUnpromote;
            }
        }

        /// <summary>
        /// 指定の領域で駒が動ける箇所を検索します。
        /// </summary>
        private IEnumerable<Position> GetRange(int fileRange,
                                               int rankRange,
                                               Position position)
        {
            var minFile = Math.Max(position.File - fileRange, 1);
            var maxFile = Math.Min(position.File + fileRange, BoardSize);
            var minRank = Math.Max(position.Rank - rankRange, 1);
            var maxRank = Math.Min(position.Rank + rankRange, BoardSize);

            for (var file = minFile; file <= maxFile; ++file)
            {
                for (var rank = minRank; rank <= maxRank; ++rank)
                {
                    yield return new Position(file, rank);
                }
            }
        }

        /// <summary>
        /// 駒を<paramref name="newPosition"/>に移動できる可能性のある
        /// 異動元の位置をすべて列挙します。
        /// </summary>
        private IEnumerable<Position> GetMovableFromRange_(BoardPiece piece,
                                                           Position newPosition)
        {
            if (newPosition == null || !newPosition.Validate())
            {
                yield break;
            }

            using (LazyLock())
            {
                if (piece.IsPromoted)
                {
                    // 成り駒の場合は、とりあえず段列±１の領域を調べます。
                    foreach (var p in GetRange(1, 1, newPosition))
                    {
                        yield return p;
                    }
                }
                else
                {
                    // 成り駒で無い場合
                    switch (piece.PieceType)
                    {
                        case PieceType.Gyoku:
                        case PieceType.Kin:
                        case PieceType.Gin:
                            foreach (var p in GetRange(1, 1, newPosition))
                            {
                                yield return p;
                            }
                            break;
                        case PieceType.Kei:
                            foreach (var p in GetRange(1, 2, newPosition))
                            {
                                yield return p;
                            }
                            break;
                        case PieceType.Kyo:
                            foreach (var p in GetRange(0, BoardSize, newPosition))
                            {
                                yield return p;
                            }
                            break;
                        case PieceType.Hu:
                            foreach (var p in GetRange(0, 1, newPosition))
                            {
                                yield return p;
                            }
                            break;
                    }
                }
                
                // 飛車角は成り／不成りに関わらず調べる箇所があります。
                switch (piece.PieceType)
                {
                    case PieceType.Hisya:
                        for (var file = 1; file <= BoardSize; ++file)
                        {
                            if (piece.IsPromoted &&
                                Math.Abs(newPosition.File - file) <= 1)
                            {
                                continue;
                            }

                            yield return new Position(file, newPosition.Rank);
                        }
                        for (var rank = 1; rank <= BoardSize; ++rank)
                        {
                            if (piece.IsPromoted &&
                                Math.Abs(newPosition.Rank - rank) <= 1)
                            {
                                continue;
                            }

                            yield return new Position(newPosition.File, rank);
                        }
                        break;

                    case PieceType.Kaku:
                        for (var index = -BoardSize; index <= BoardSize; ++index)
                        {
                            if (piece.IsPromoted && Math.Abs(index) <= 1)
                            {
                                continue;
                            }

                            yield return new Position(
                                newPosition.File + index,
                                newPosition.Rank + index);
                        }
                        for (var index = -BoardSize; index <= BoardSize; ++index)
                        {
                            if (piece.IsPromoted && Math.Abs(index) <= 1)
                            {
                                continue;
                            }

                            yield return new Position(
                                newPosition.File + index,
                                newPosition.Rank - index);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 駒の種類と新しい位置から、可能な差し手をすべて検索します。
        /// </summary>
        public IEnumerable<BoardMove> SearchMoveList(BoardPiece piece,
                                                     Position newPosition)
        {
            using (LazyLock())
            {
                // 駒打ちが可能か調べます。
                if (!piece.IsPromoted)
                {
                    var move = new BoardMove()
                    {
                        BWType = piece.BWType,
                        NewPosition = newPosition,
                        ActionType = ActionType.Drop,
                        DropPieceType = piece.PieceType,
                    };
                    if (CanMove(move))
                    {
                        yield return move;
                    }
                }

                var list = GetMovableFromRange_(piece, newPosition)
                    .SelectMany(_ => GetMovableMove(piece, newPosition, _));

                foreach (var m in list)
                {
                    yield return m;
                }
            }
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

                if (this.prevMovedPosition != null &&
                    !this.prevMovedPosition.Validate())
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
                        var position = new Position(file, rank);

                        if (this[position] != other[position])
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

                if (this.prevMovedPosition != other.prevMovedPosition)
                {
                    return false;
                }

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
                    (this.prevMovedPosition != null ?
                        this.prevMovedPosition.GetHashCode() : 0));
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
                this[5, 9] = new BoardPiece(BWType.Black, PieceType.Gyoku);
                this[4, 9] = new BoardPiece(BWType.Black, PieceType.Kin);
                this[6, 9] = new BoardPiece(BWType.Black, PieceType.Kin);
                this[3, 9] = new BoardPiece(BWType.Black, PieceType.Gin);
                this[7, 9] = new BoardPiece(BWType.Black, PieceType.Gin);
                this[2, 9] = new BoardPiece(BWType.Black, PieceType.Kei);
                this[8, 9] = new BoardPiece(BWType.Black, PieceType.Kei);
                this[1, 9] = new BoardPiece(BWType.Black, PieceType.Kyo);
                this[9, 9] = new BoardPiece(BWType.Black, PieceType.Kyo);
                this[8, 8] = new BoardPiece(BWType.Black, PieceType.Kaku);
                this[2, 8] = new BoardPiece(BWType.Black, PieceType.Hisya);
                for (var file = 1; file <= BoardSize; ++file)
                {
                    this[file, 7] = new BoardPiece(BWType.Black, PieceType.Hu);
                }

                this[5, 1] = new BoardPiece(BWType.White, PieceType.Gyoku);
                this[4, 1] = new BoardPiece(BWType.White, PieceType.Kin);
                this[6, 1] = new BoardPiece(BWType.White, PieceType.Kin);
                this[3, 1] = new BoardPiece(BWType.White, PieceType.Gin);
                this[7, 1] = new BoardPiece(BWType.White, PieceType.Gin);
                this[2, 1] = new BoardPiece(BWType.White, PieceType.Kei);
                this[8, 1] = new BoardPiece(BWType.White, PieceType.Kei);
                this[1, 1] = new BoardPiece(BWType.White, PieceType.Kyo);
                this[9, 1] = new BoardPiece(BWType.White, PieceType.Kyo);
                this[2, 2] = new BoardPiece(BWType.White, PieceType.Kaku);
                this[8, 2] = new BoardPiece(BWType.White, PieceType.Hisya);
                for (var file = 1; file <= BoardSize; ++file)
                {
                    this[file, 3] = new BoardPiece(BWType.White, PieceType.Hu);
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
