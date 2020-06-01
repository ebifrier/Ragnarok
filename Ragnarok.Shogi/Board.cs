using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 指し手を指すときのオプションです。
    /// </summary>
    [Flags()]
    public enum MoveFlags
    {
        /// <summary>
        /// 特になし。
        /// </summary>
        None = 0x0000,
        /// <summary>
        /// 手番をチェックします。
        /// </summary>
        CheckTurn = 0x0001,
        /// <summary>
        /// 自玉が王手されているかチェックします。
        /// </summary>
        CheckChecked = 0x0002,
        /// <summary>
        /// 実際に指さず、指せるかどうかの確認のみを行います。
        /// </summary>
        CheckOnly = 0x0004,
        /// <summary>
        /// 打ち歩詰めの確認を行います。
        /// </summary>
        CheckPawnDropCheckMate = 0x0008,
        /// <summary>
        /// 投了などの特殊な指し手を自動的に削除します。
        /// </summary>
        AutoRemoveSpecialMove = 0x0010,

        /// <summary>
        /// CanMoveメソッドのフラグデフォルト値です。
        /// </summary>
        CanMoveDefault = CheckTurn | CheckChecked |
                         CheckPawnDropCheckMate | CheckOnly,
        /// <summary>
        /// DoMoveメソッドのフラグデフォルト値です。
        /// </summary>
        DoMoveDefault = CheckTurn | CheckChecked |
                        CheckPawnDropCheckMate | AutoRemoveSpecialMove,
    }

    /// <summary>
    /// 盤面を示すクラスです。
    /// </summary>
    [DataContract()]
    public partial class Board
    {
        /// <summary>
        /// 各駒の最大駒数を保持します。
        /// </summary>
        public static readonly int[] MaxPieceCountList = new int[]
        {
            0, // None
            18, // Pawn
            4, // Lance
            4, // Knight
            4, // Silver
            2, // Bishop
            2, // Rook
            4, // Gold
            2, // Kind
        };

        /// <summary>
        /// 将棋盤の一方向のサイズです。
        /// </summary>
        public const int BoardSize = 9;
        /// <summary>
        /// 将棋盤のマスの数です。
        /// </summary>
        public const int SquareCount = 9 * 9;

        /// <summary>
        /// すべてのマスを巡回するためのオブジェクトを取得します。
        /// </summary>
        public static IEnumerable<Square> Squares()
        {
            for (var rank = 1; rank <= Board.BoardSize; ++rank)
            {
                for (var file = 1; file <= Board.BoardSize; ++file)
                {
                    yield return SquareUtil.Create(file, rank);
                }
            }
        }

        private static long[,,] zobrist = new long[SquareCount, 9 * 2, 2];
        private static long[,] zobristHand = new long[9 * 2, 2];
        private static long zobristTurn;

        /// <summary>
        /// [0,0]が盤面の11地点を示します。
        /// </summary>
        private Piece[] board = new Piece[81];
        [DataMember(Order = 1, IsRequired = true)]
        private Hand blackHandBox = new Hand(Colour.Black, 0);
        [DataMember(Order = 2, IsRequired = true)]
        private Hand whiteHandBox = new Hand(Colour.White, 0);
        [DataMember(Order = 4, IsRequired = true)]
        private Colour turn = Colour.Black;
        [DataMember(Order = 5, IsRequired = true)]
        private Square prevMovedSquare = Square.Empty;
        //[DataMember(Order = 6, IsRequired = true)]
        private List<Move> moveList = new List<Move>();
        //[DataMember(Order = 7, IsRequired = true)]
        private List<Move> redoList = new List<Move>();

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static Board()
        {
            InitZobrist();
        }

        /// <summary>
        /// ハッシュ用のzobristを初期化します。
        /// </summary>
        private static void InitZobrist()
        {
            var random = new Random();

            // 駒の成りと不成りも入れる
            for (var piece = 0; piece < 9 * 2; ++piece)
            {
                for (var sqi = 0; sqi < SquareCount; ++sqi)
                {
                    zobrist[sqi, piece, 0] = (long)(random.NextDouble() * 0xfffffffffffffe);
                    zobrist[sqi, piece, 1] = (long)(random.NextDouble() * 0xfffffffffffffe);
                }

                zobristHand[piece, 0] = (long)(random.NextDouble() * 0xfffffffffffffe);
                zobristHand[piece, 1] = (long)(random.NextDouble() * 0xfffffffffffffe);
            }

            zobristTurn = 1;
        }

        /// <summary>
        /// Zobrist Hashingの値を計算します。
        /// </summary>
        private long CalcHashValue()
        {
            var hashValue = 0L;

            foreach (var sq in Squares())
            {
                var pc = this[sq];
                if (pc.IsNone()) continue;

                hashValue += zobrist[(int)sq, (int)pc, pc.GetColour().GetIndex()];
            }

            foreach (var piece in PieceUtil.PieceTypes())
            {
                var count = GetHand(piece);
                while (count-- > 0)
                {
                    hashValue += zobristHand[(int)piece, piece.GetColour().GetIndex()];
                }
            }

            if (Turn == Colour.White)
            {
                hashValue ^= zobristTurn;
            }

            return hashValue;
        }

        /// <summary>
        /// オブジェクトのコピーを作成します。
        /// </summary>
        public Board Clone(bool cloneMoveList = true)
        {
            var cloned = new Board(false)
            {
                blackHandBox = this.blackHandBox.Clone(),
                whiteHandBox = this.whiteHandBox.Clone(),
                turn = this.turn,
                prevMovedSquare = this.prevMovedSquare
            };

            // 指し手リストもコピーする場合
            if (cloneMoveList)
            {
                cloned.moveList = this.moveList.Select(move => move.Clone()).ToList();
                cloned.redoList = this.redoList.Select(move => move.Clone()).ToList();
            }
            else
            {
                cloned.moveList = new List<Move>();
                cloned.redoList = new List<Move>();
            }

            // 各位置に駒を設定します。
            for (var rank = 1; rank <= BoardSize; ++rank)
            {
                for (var file = 1; file <= BoardSize; ++file)
                {
                    cloned[file, rank] = this[file, rank];
                }
            }

            return cloned;
        }

        /// <summary>
        /// 今までの指し手をすべて取得します。
        /// </summary>
        public ReadOnlyCollection<Move> MoveList
        {
            get
            {
                return new ReadOnlyCollection<Move>(this.moveList);
            }
        }

        /// <summary>
        /// Redoできる指し手のリストを取得します。
        /// </summary>
        public ReadOnlyCollection<Move> RedoList
        {
            get
            {
                return new ReadOnlyCollection<Move>(this.redoList);
            }
        }

        /// <summary>
        /// 手を戻すことができるか取得します。
        /// </summary>
        public bool CanUndo
        {
            get
            {
                return this.moveList.Any();
            }
        }

        /// <summary>
        /// 手を戻すことができる回数を取得します。
        /// </summary>
        public int CanUndoCount
        {
            get
            {
                return this.moveList.Count;
            }
        }

        /// <summary>
        /// 手を進めることができるか取得します。
        /// </summary>
        public bool CanRedo
        {
            get
            {
                return this.redoList.Any();
            }
        }

        /// <summary>
        /// 手を進めることができる回数を取得します。
        /// </summary>
        public int CanRedoCount
        {
            get
            {
                return this.redoList.Count;
            }
        }

        /// <summary>
        /// 一番最後の指し手を取得します。
        /// </summary>
        public Move LastMove
        {
            get
            {
                var move = this.moveList.LastOrDefault();

                return move?.Clone();
            }
        }

        /// <summary>
        /// 投了などの特殊な指し手があるかどうかを取得します。
        /// </summary>
        public bool HasSpecialMove
        {
            get
            {
                return (LastMove?.IsSpecialMove == true);
            }
        }


        /// <summary>
        /// <paramref name="dstSquare"/> にある駒を取得します。
        /// </summary>
        [SuppressMessage("Design", "CA1043:インデクサーには整数または文字列引数を使用します")]
        public Piece this[Square square]
        {
            get { return this[square.GetFile(), square.GetRank()]; }
            set { this[square.GetFile(), square.GetRank()] = value; }
        }

        /// <summary>
        /// <paramref name="file"/> <paramref name="rank"/> にある駒を取得します。
        /// </summary>
        public Piece this[int file, int rank]
        {
            get
            {
                if (!SquareUtil.Validate(file, rank))
                {
                    return Piece.None;
                }

                var file2 = BoardSize - file;
                var rank2 = rank - 1;
                return this.board[rank2 * 9 + file2];
            }
            set
            {
                if (!SquareUtil.Validate(file, rank))
                {
                    throw new ArgumentException("fileかrankが正しくありません。");
                }

                var file2 = BoardSize - file;
                var rank2 = rank - 1;
                this.board[rank2 * 9 + file2] = value;
            }
        }

        /// <summary>
        /// 手番を取得または設定します。
        /// </summary>
        public Colour Turn
        {
            get { return this.turn; }
            set { this.turn = value; }
        }

        /// <summary>
        /// 前回動かした駒の位置を取得します。
        /// </summary>
        public Square PrevMovedSquare
        {
            get { return this.prevMovedSquare; }
            set { this.prevMovedSquare = value; }
        }

        /// <summary>
        /// 局面のハッシュ値を取得します。
        /// </summary>
        public long HashValue
        {
            get { return /*this.hashValue*/CalcHashValue(); }
        }

        /// <summary>
        /// 指し手の手数を取得します。
        /// </summary>
        public int MoveCount
        {
            get
            {
                return this.moveList.Count;
            }
        }

        /// <summary>
        /// 文字列化した局面を取得します。
        /// </summary>
        /// <remarks>
        /// Visual Studioで局面を文字列で確認するためのプロパティです。
        /// ほぼデバッグ用
        /// </remarks>
        public string BodString
        {
            get
            {
                return ToBod();
            }
        }

        /// <summary>
        /// 持ち駒の数を取得します。
        /// </summary>
        public Hand GetHand(Colour colour)
        {
            if (colour == Colour.Black)
            {
                return this.blackHandBox;
            }
            else
            {
                return this.whiteHandBox;
            }
        }

        /// <summary>
        /// 持ち駒の数を取得します。
        /// </summary>
        public int GetHand(Piece piece)
        {
            var handBox = GetHand(piece.GetColour());

            return handBox.Get(piece.GetRawType());
        }

        /// <summary>
        /// 持ち駒の数を設定します。
        /// </summary>
        public void SetHand(Piece piece, int count)
        {
            var handBox = GetHand(piece.GetColour());

            handBox.Set(piece.GetRawType(), count);
        }

        /// <summary>
        /// 持ち駒の数を増やします。
        /// </summary>
        public void IncHand(Piece piece)
        {
            var handBox = GetHand(piece.GetColour());

            handBox.Increment(piece.GetRawType());
        }

        /// <summary>
        /// 持ち駒の数を減らします。
        /// </summary>
        public void DecHand(Piece piece)
        {
            var handBox = GetHand(piece.GetColour());

            handBox.Decrement(piece.GetRawType());
        }

        /// <summary>
        /// 駒の最大数を調べます。
        /// </summary>
        public static int GetMaxPieceCount(Piece pieceType)
        {
            return MaxPieceCountList[(int)pieceType.GetRawType()];
        }

        /// <summary>
        /// 簡略化
        /// </summary>
        private delegate int F(Piece Piece);

        /// <summary>
        /// 現在の局面で、先手と後手の両方が持つ駒の数の合計を調べます。
        /// </summary>
        public int GetTotalPieceCount(Piece pieceType)
        {
            var funcs = new F[]
            {
                new F(_ => GetHand(_.Modify(Colour.Black))),
                new F(_ => GetHand(_.Modify(Colour.White))),
                new F(_ => Squares()
                    .Select(sq => this[sq])
                    .Sum(p => p.GetRawType() == _ ? 1 : 0)),
            };

            return funcs.Sum(f => f(pieceType.GetRawType()));
        }

        /// <summary>
        /// 現在の局面で、駒箱にも局面にもない駒の数を調べます。
        /// </summary>
        public int GetLeavePieceCount(Piece pieceType)
        {
            var rawType = pieceType.GetRawType();
            var maximum = GetMaxPieceCount(rawType);
            var current = GetTotalPieceCount(rawType);
            return Math.Max(maximum - current, 0);
        }

        /// <summary>
        /// １手戻します。
        /// </summary>
        public Move Undo()
        {
            if (!this.moveList.Any())
            {
                return null;
            }

            var move = this.moveList.Last();

            // 盤の各状態を意って戻した状態に設定します。
            this.moveList.RemoveAt(this.moveList.Count - 1);

            // リドゥリストの最後に追加します。
            // 再現するときは最後の要素から使います。
            this.redoList.Add(move);

            // this.moveListからmoveから取り除かれた状態じゃないと
            // PrevMovedSquareの設定が上手くいかない。
            DoUndo(move);

            return move;
        }

        /// <summary>
        /// undo操作を実行します。
        /// </summary>
        private void DoUndo(Move move)
        {
            if (move.IsSpecialMove)
            {
                // 何もしません。
            }
            else if (move.ActionType == ActionType.Drop)
            {
                // 駒打ちの場合は、その駒を駒台に戻します。
                this[move.DstSquare] = Piece.None;

                IncHand(move.DropPieceType.Modify(move.Colour));
            }
            else
            {
                var movedPiece = this[move.DstSquare];

                // 駒を成った場合はそれを元に戻します。
                if (move.ActionType == ActionType.Promote)
                {
                    movedPiece = movedPiece.Unpromote();
                }

                // 駒を取った場合は、その駒を元に戻します。
                if (!move.TookPiece.IsNone())
                {
                    this[move.DstSquare] = move.TookPiece.Modify(
                        move.Colour.Flip());

                    // 駒を取ったはずなので、その分を駒台から減らします。
                    DecHand(move.TookPiece.GetRawType().Modify(move.Colour));
                }
                else
                {
                    this[move.DstSquare] = Piece.None;
                }

                this[move.SrcSquare] = movedPiece;
            }

            if (!move.IsSpecialMove)
            {
                Turn = Turn.Flip();
                PrevMovedSquare =
                    this.moveList.LastOrDefault()?.DstSquare ??
                    Square.Empty;
            }
        }

        /// <summary>
        /// １手戻したのを再び復活します。
        /// </summary>
        public Move Redo()
        {
            var move = this.redoList.LastOrDefault();
            if (move == null)
            {
                return null;
            }

            if (!CheckAndMakeMove(move, MoveFlags.CheckTurn))
            {
                // リドゥに失敗したら、どうすればいいんだ・・・
                Log.Error("{0}: リドゥ操作に失敗しました。", move);

                this.redoList.Clear();
                return null;
            }

            return move;
        }

        /// <summary>
        /// 可能な限り局面をUndoします。
        /// </summary>
        public void UndoAll()
        {
            while (Undo() != null)
            {
            }
        }

        /// <summary>
        /// 局面を元に戻すことができないようにします。
        /// </summary>
        public void ClearUndoList()
        {
            this.moveList.Clear();
            PrevMovedSquare = Square.Empty;
        }

        /// <summary>
        /// 可能な限り局面をRedoします。
        /// </summary>
        public void RedoAll()
        {
            while (Redo() != null)
            {
            }
        }

        /// <summary>
        /// リドゥ用のリストをすべてクリアします。
        /// </summary>
        public void ClearRedoList()
        {
            this.redoList.Clear();
        }

        /// <summary>
        /// 投了などの特殊な指し手があれば、それを削除します。
        /// </summary>
        public void RemoveSpecialMove()
        {
            if (HasSpecialMove)
            {
                Undo();
                ClearRedoList();
            }
        }

        /// <summary>
        /// その指し手が実際に実現できるか調べます。
        /// </summary>
        public bool CanMove(Move move, MoveFlags flags = MoveFlags.CanMoveDefault)
        {
            flags |= MoveFlags.CheckOnly;

            return CheckAndMakeMove(move, flags);
        }

        /// <summary>
        /// その指し手を実際に実行します。
        /// </summary>
        public bool DoMove(Move move, MoveFlags flags = MoveFlags.DoMoveDefault)
        {
            flags &= ~MoveFlags.CheckOnly;

            return CheckAndMakeMove(move, flags);
        }

        /// <summary>
        /// 駒を動かすか、または駒が動かせるか調べます。
        /// </summary>
        private bool CheckAndMakeMove(Move move, MoveFlags flags)
        {
            if (move == null || !move.Validate())
            {
                throw new ArgumentNullException(nameof(move));
            }

            // 投了などの特殊な指し手がある場合はゲームが既に終了しているので
            // 指し手を進めることはできません。
            if (HasSpecialMove)
            {
                if (flags.HasFlag(MoveFlags.CheckOnly))
                {
                    // CheckOnlyの場合は、ここで失敗させません。
                }
                else if (flags.HasFlag(MoveFlags.AutoRemoveSpecialMove))
                {
                    // 特殊な指し手を取り除きます。
                    RemoveSpecialMove();
                }
                else
                {
                    return false;
                }
            }

            {
                // 手番があわなければ失敗とします。
                if (EnumUtil.HasFlag(flags, MoveFlags.CheckTurn))
                {
                    if (this.turn == Colour.None || this.turn != move.Colour)
                    {
                        return false;
                    }
                }

                if (move.IsSpecialMove)
                {
                    return CheckAndMakeSpecialMove(move, flags);
                }
                else if (move.ActionType == ActionType.Drop)
                {
                    return CheckAndMakeDrop(move, flags);
                }
                else
                {
                    return CheckAndMakeMoveOnly(move, flags);
                }
            }
        }

        /// <summary>
        /// １手指したときに呼ばれます。
        /// </summary>
        private void MoveDone(Move move)
        {
            // 特殊な指し手の場合は手番などの入れ替えを行いません。
            // （投了後の局面から駒を移動するため）
            if (!move.IsSpecialMove)
            {
                Turn = Turn.Flip();
                PrevMovedSquare = move.DstSquare;
            }

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
        }

        /// <summary>
        /// 特殊な指し手の着手が行えるか調べ、必要なら実行します。
        /// </summary>
        private bool CheckAndMakeSpecialMove(Move move, MoveFlags flags)
        {
            if (!EnumUtil.HasFlag(flags, MoveFlags.CheckOnly))
            {
                MoveDone(move);
            }

            return true;
        }

        /// <summary>
        /// 駒打ちの動作が行えるか調べ、必要なら実行します。
        /// </summary>
        private bool CheckAndMakeDrop(Move move, MoveFlags flags)
        {
            var dropPiece = move.DropPieceType.Modify(move.Colour);

            if (GetHand(dropPiece) <= 0)
            {
                return false;
            }

            // 駒を打つ場所に駒があれば、当然失敗です。
            var piece = this[move.DstSquare];
            if (!piece.IsNone())
            {
                return false;
            }

            // 駒が置けるか確かめます。
            if (!CanDrop(move.Colour, move.DstSquare, move.DropPieceType))
            {
                return false;
            }

            // 打ち歩詰のチェックを行います。
            if (EnumUtil.HasFlag(flags, MoveFlags.CheckPawnDropCheckMate) &&
                move.DropPieceType == Piece.Pawn &&
                IsPawnDropCheckMate(move.Colour, move.DstSquare))
            {
                return false;
            }

            if (!EnumUtil.HasFlag(flags, MoveFlags.CheckOnly))
            {
                // 駒を盤面に置き、持ち駒から駒を減らします。
                this[move.DstSquare] = PieceUtil.Modify(
                    move.DropPieceType, move.Colour);

                DecHand(dropPiece);
                MoveDone(move);
            }

            return true;
        }

        /// <summary>
        /// 打ち歩詰のチェックを行います。
        /// </summary>
        private bool IsPawnDropCheckMate(Colour colour, Square square)
        {
#if OUTE_SHOGI
            // 王手将棋では打ち歩詰めを反則にしません。
            return false;
#else
            // 歩の前に敵の玉がいるか確認します。
            var rankDif = (colour == Colour.Black ? -1 : +1);
            if (!SquareUtil.Validate(square.GetFile(), square.GetRank() + rankDif))
            {
                return false;
            }

            var gyoku = this[square.GetFile(), square.GetRank() + rankDif];
            if (gyoku.GetPieceType() != Piece.King ||
                gyoku.GetColour() != colour.Flip())
            {
                return false;
            }

            var newPiece = Piece.Pawn.Modify(colour);
            var oldPiece = this[square];

            // 打ち歩詰の判定には、実際に歩を打ってみるのが簡単なため、
            // 必要なプロパティのみ更新し、詰まされているか調べます。
            this[square] = newPiece;
            DecHand(newPiece);
            Turn = Turn.Flip();

            // 打ち歩詰かどうか確認します。
            var mated = IsCheckMated();

            this[square] = oldPiece;
            IncHand(newPiece);
            Turn = Turn.Flip();

            return mated;
#endif
        }

        /// <summary>
        /// 指定の位置に駒が打てるか調べます。
        /// </summary>
        public bool CanDrop(Colour colour, Square square, Piece pieceType)
        {
            if (!this[square].IsNone())
            {
                return false;
            }

            var rank =
                ( colour == Colour.Black
                ? square.GetRank()
                : (BoardSize + 1) - square.GetRank());

            switch (pieceType)
            {
                case Piece.None: // これは打てない
                    return false;
                case Piece.Pawn:
                    // 2歩のチェックを行います。
                    if (GetPawnCount(colour, square.GetFile()) > 0)
                    {
                        return false;
                    }
                    goto case Piece.Lance;
                case Piece.Lance:
                    if (rank == 1)
                    {
                        return false;
                    }
                    break;
                case Piece.Knight:
                    if (rank < 3) // 1,2はダメ
                    {
                        return false;
                    }
                    break;
            }

            return true;
        }

        /// <summary>
        /// 駒の移動のみの動作を調べるか実際にそれを行います。
        /// </summary>
        private bool CheckAndMakeMoveOnly(Move move, MoveFlags flags)
        {
            // 駒の移動元に自分の駒がなければダメ
            var srcPiece = this[move.SrcSquare];
            if (srcPiece.IsNone() || srcPiece.GetColour() != move.Colour)
            {
                return false;
            }

            // 駒の移動先に自分の駒があったらダメ
            var dstPiece = this[move.DstSquare];
            if (!dstPiece.IsNone() && dstPiece.GetColour() == move.Colour)
            {
                return false;
            }

            // これはエラーだけど。。。ｗ
            if (srcPiece.GetPieceType() != move.MovePiece)
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

            if (!EnumUtil.HasFlag(flags, MoveFlags.CheckOnly))
            {
                // 移動先に駒があれば、それを自分のものにします。
                if (!dstPiece.IsNone())
                {
                    IncHand(dstPiece.GetRawType().Modify(move.Colour));

                    // 取った駒を記憶しておきます。
                    move.TookPiece = dstPiece.GetPieceType();
                }

                // 移動後の駒の成り/不成りを決定します。
                var promoted = (
                    srcPiece.IsPromoted() ||
                    move.ActionType == ActionType.Promote);

                this[move.DstSquare] = srcPiece.Modify(promoted).Modify(move.Colour);

                // 移動前の位置からは駒をなくします。
                this[move.SrcSquare] = Piece.None;

                // 前回の指し手と位置が同じか調べます。
                move.HasSameSquareAsPrev = (move.DstSquare == PrevMovedSquare);

                MoveDone(move);
            }

            return true;
        }

        /// <summary>
        /// 駒を強制的に成る必要があるか調べます。
        /// </summary>
        public static bool CanPromote(Move move)
        {
            if (move == null)
            {
                throw new ArgumentNullException(nameof(move));
            }

            return CanPromote(move.MovePiece, move.Colour,
                              move.DstSquare, move.SrcSquare);
        }

        /// <summary>
        /// 駒が成れるか調べます。
        /// </summary>
        public static bool CanPromote(Piece piece, Colour colour,
                                      Square dstSquare, Square srcSquare)
        {
            if (!dstSquare.Validate())
            {
                return false;
            }

            // 駒の移動でない場合は成れません。
            if (!srcSquare.Validate())
            {
                return false;
            }

            // 既に成っている駒を再度成ることはできません。
            if (piece.IsNone() || piece.IsPromoted())
            {
                return false;
            }

            var srcRank = srcSquare.GetRank();
            var dstRank = dstSquare.GetRank();

            if (colour == Colour.White)
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
            var pieceType = piece.GetPieceType();
            if (pieceType == Piece.None ||
                pieceType == Piece.King ||
                pieceType == Piece.Gold)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 駒を強制的に成る必要があるか調べます。
        /// </summary>
        public static bool IsPromoteForce(Move move)
        {
            if (move == null)
            {
                throw new ArgumentNullException(nameof(move));
            }

            return IsPromoteForce(
                PieceUtil.Modify(move.MovePiece, move.Colour),
                move.DstSquare);
        }

        /// <summary>
        /// 駒を強制的に成る必要があるか調べます。
        /// </summary>
        public static bool IsPromoteForce(Piece piece, Square dstSquare)
        {
            if (!dstSquare.Validate())
            {
                return false;
            }

            // 駒の移動元に自分の駒がなければダメ
            if (piece.IsNone() || piece.IsPromoted())
            {
                return false;
            }

            var colour = piece.GetColour();
            var pieceType = piece.GetPieceType();
            var normalizedRank = (
                colour == Colour.White ?
                (BoardSize + 1) - dstSquare.GetRank() :
                dstSquare.GetRank());

            if (pieceType == Piece.Knight)
            {
                return (normalizedRank <= 2);
            }
            else if (pieceType == Piece.Lance || pieceType == Piece.Pawn)
            {
                return (normalizedRank == 1);
            }

            return false;
        }

        /// <summary>
        /// 局面上にある駒の数を取得します。
        /// </summary>
        public int GetPieceCount(Piece piece)
        {
            var list =
                from sq in Board.Squares()
                where this[sq] == piece
                select sq;

            return list.Count();
        }

        /// <summary>
        /// 玉の数を取得します。
        /// </summary>
        private int GetGyokuCount(Colour colour)
        {
            return GetPieceCount(Piece.King.Modify(colour));
        }

        /// <summary>
        /// 指定の筋にある歩の数を確認します。
        /// </summary>
        public int GetPawnCount(Colour colour, int file)
        {
            var target = Piece.Pawn.Modify(colour);
            return Enumerable.Range(1, BoardSize)
                .Where(_ => this[file, _] == target)
                .Count();
        }

        /// <summary>
        /// 盤上の駒や持ち駒の先手・後手をすべて入れ替えます。
        /// </summary>
        public void FlipPieces()
        {
            // すべての駒の先後を入れ替えます。
            Board.Squares()
                .Where(_ => !this[_].IsNone())
                .ForEach(_ => this[_] = this[_].FlipColor());

            // 各駒の位置を入れ替えます。
            for (var rank = 1; rank <= Board.BoardSize / 2 + 1; ++rank)
            {
                // ５段目は途中の列まで交換します。
                var fileMax = Board.BoardSize / (rank == 5 ? 2 : 1);
                for (var file = 1; file <= fileMax; ++file)
                {
                    var sq1 = SquareUtil.Create(file, rank);
                    var sq2 = sq1.Flip();

                    var p = this[sq1];
                    this[sq1] = this[sq2];
                    this[sq2] = p;
                }
            }

            // 持ち駒の先後を入れ替えます。(数を設定し直します)
            var result =
                from colour in ColourUtil.BlackWhite()
                let countList = (
                    from rawType in PieceUtil.RawTypes()
                    let count = GetHand(rawType.Modify(colour))
                    select new { Piece = rawType, Count = count }
                    ).ToList()
                select new { Colour = colour, CountList = countList };

            result.ToList().ForEach(_ =>
            {
                var flipped = _.Colour.Flip();

                _.CountList.ForEach(data =>
                    SetHand(data.Piece.Modify(flipped), data.Count));
            });
        }

        /// <summary>
        /// オブジェクトの妥当性を検証します。
        /// </summary>
        public bool Validate()
        {
            if (this.blackHandBox == null ||
                !this.blackHandBox.Validate())
            {
                return false;
            }

            if (this.whiteHandBox == null ||
                !this.whiteHandBox.Validate())
            {
                return false;
            }

            if (!Enum.IsDefined(typeof(Colour), this.turn) ||
                this.turn == Colour.None)
            {
                return false;
            }

            if (this.moveList == null ||
                this.moveList.Any(move => !move.Validate()))
            {
                return false;
            }

            // 局面上の駒の数を確認します。
            foreach (var rawType in PieceUtil.RawTypes())
            {
                var count = GetTotalPieceCount(rawType);
                var maxCount = GetMaxPieceCount(rawType);
                if (count > maxCount)
                {
                    return false;
                }
            }

            foreach (var colour in ColourUtil.BlackWhite())
            {
                // ２歩の確認を行います。
                for (var file = 1; file <= BoardSize; ++file)
                {
                    if (GetPawnCount(colour, file) > 1)
                    {
                        return false;
                    }
                }
            }

            // 玉の数をカウントします。
            if (GetGyokuCount(Colour.Black) > 1 ||
                GetGyokuCount(Colour.White) > 1)
            {
                return false;
            }

            return true;
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

            if (!Squares().All(_ => x[_] == y[_]))
            {
                return false;
            }

            if (!x.blackHandBox.Equals(y.blackHandBox) ||
                !x.whiteHandBox.Equals(y.whiteHandBox))
            {
                return false;
            }

            if (x.turn != y.turn)
            {
                return false;
            }

            return true;
        }

#if false
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
                    
                    // Piece.Noneが0のため、
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

                    // Piece.Noneが0のため、
                    // 正しいピースのシリアライズデータは０以外の
                    // 数字となります。
                    Piece piece = null;
                    if (bytes[index] != 0)
                    {
                        piece = new Piece();
                        piece.Deserialize(bytes[index]);
                    }

                    this[file, rank] = piece;
                }
            }
        }

        /// <summary>
        /// 指し手リストをバイト列に直します。
        /// </summary>
        private byte[] SerializeMoveList(List<Move> moveList)
        {
            var result = new byte[4 * moveList.Count];

            for (var i = 0; i < moveList.Count; ++i)
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
        private List<Move> DeserializeMoveList(byte[] moveListBytes)
        {
            // 指し手が無い場合、配列はnullになります。
            if (moveListBytes == null)
            {
                return new List<Move>();
            }

            var result = new List<Move>(moveListBytes.Length / 4);
            for (var i = 0; i < moveListBytes.Length; i += 4)
            {
                var bits = (
                    (moveListBytes[i + 0] <<  0) |
                    (moveListBytes[i + 1] <<  8) |
                    (moveListBytes[i + 2] << 16) |
                    (moveListBytes[i + 3] << 24));

                var boardMove = new Move();
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
            this.serializeBoard = SerializePieces();

            // 指し手のシリアライズを行います。
            this.moveListBytes = SerializeMoveList(this.moveList);
            this.redoListBytes = SerializeMoveList(this.redoList);
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
            this.board = new Piece[81];
            DeserializePieces(this.serializeBoard);

            // 指し手のデシリアライズを行います。
            this.moveList = DeserializeMoveList(this.moveListBytes);
            this.redoList = DeserializeMoveList(this.redoListBytes);
        }
        #endregion
#endif

#if false
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
#endif

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Board(bool isInitPiece)
        {
            this.board = new Piece[81];

            if (isInitPiece)
            {
                this[1, 9] = Piece.BlackLance;
                this[2, 9] = Piece.BlackKnight;
                this[3, 9] = Piece.BlackSilver;
                this[4, 9] = Piece.BlackGold;
                this[5, 9] = Piece.BlackKing;
                this[6, 9] = Piece.BlackGold;
                this[7, 9] = Piece.BlackSilver;
                this[8, 9] = Piece.BlackKnight;
                this[9, 9] = Piece.BlackLance;
                this[2, 8] = Piece.BlackRook;
                this[8, 8] = Piece.BlackBishop;
                for (var file = 1; file <= BoardSize; ++file)
                {
                    this[file, 7] = Piece.BlackPawn;
                }

                this[1, 1] = Piece.WhiteLance;
                this[2, 1] = Piece.WhiteKnight;
                this[3, 1] = Piece.WhiteSilver;
                this[4, 1] = Piece.WhiteGold;
                this[5, 1] = Piece.WhiteKing;
                this[6, 1] = Piece.WhiteGold;
                this[7, 1] = Piece.WhiteSilver;
                this[8, 1] = Piece.WhiteKnight;
                this[9, 1] = Piece.WhiteLance;
                this[2, 2] = Piece.WhiteBishop;
                this[8, 2] = Piece.WhiteRook;
                for (var file = 1; file <= BoardSize; ++file)
                {
                    this[file, 3] = Piece.WhitePawn;
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
