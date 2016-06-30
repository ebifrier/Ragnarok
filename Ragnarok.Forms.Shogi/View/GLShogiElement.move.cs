using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Ragnarok.Shogi;
using Ragnarok.Utility;

namespace Ragnarok.Forms.Shogi.View
{
    /// <summary>
    /// 将棋の盤面を扱うコントロールです。
    /// </summary>
    public partial class GLShogiElement
    {
        private MovingPiece movingPiece;
        private PromoteDialog promoteDialog;

        /// <summary>
        /// クライアント座標を(640,380)のローカル座標系に変換します。
        /// </summary>
        private Point ClientToLocal(Point p)
        {
            if (GLContainer == null)
            {
                throw new InvalidOperationException(
                    "親コンテナに追加されていません。");
            }

            var m = Transform.Invert();
            var s = GLContainer.ClientSize;

            var np = new Pointd(
                p.X * 640.0 / s.Width,
                p.Y * 360.0 / s.Height);

            return new Point(
                (int)(np.X * m[0,0] + np.Y * m[0,1]),
                (int)(np.X * m[1,0] + np.Y * m[1,1]));
        }

        /// <summary>
        /// マウスの左ボタン押下時に呼ばれます。
        /// </summary>
        public override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            var pos = ClientToLocal(e.Location);
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (this.movingPiece == null)
                    {
                        BeginMove(pos);
                    }
                    else
                    {
                        EndMovePiece(pos);
                    }
                    break;
                case MouseButtons.Right:
                    ProcessEdit(pos);
                    break;
            }
        }

        /// <summary>
        /// マウス移動時に呼ばれます。
        /// </summary>
        public override void OnMouseMove(MouseEventArgs e)
        {
            var pos = ClientToLocal(e.Location);

            MovePiece(pos);
        }

        /// <summary>
        /// マウスの右ボタンが上がったときに呼ばれます。
        /// </summary>
        public override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
        }

        #region 駒の移動開始
        /// <summary>
        /// 駒の移動を開始します。
        /// </summary>
        private void BeginMove(Point pos)
        {
            // 自動再生中であれば、それを停止します。
            if (this.autoPlay != null && !this.autoPlay.IsImportant)
            {
                StopAutoPlay();
                return;
            }

            // 駒検索
            var square = PointToSquare(pos);
            if (square != null && Board != null)
            {
                var mpiece = Board[square];
                if (mpiece != null)
                {
                    BeginMovePiece(mpiece, square);
                    MovePiece(pos);
                    return;
                }
            }

            // 駒台の駒を検索
            var piece = PointToHandPiece(pos);
            if (piece != null &&
                GetHandCount(piece.PieceType, piece.BWType) > 0)
            {
                BeginDropPiece(piece);
                MovePiece(pos);
                return;
            }
        }

        /// <summary>
        /// 駒の移動などを開始できるかどうか調べます。
        /// </summary>
        private bool CanBeginMove(BWType pieceSide)
        {
            if (this.movingPiece != null)
            {
                return false;
            }

            if (this.autoPlay != null)
            {
                return false;
            }

            var turn = (Board != null ? Board.Turn : BWType.None);
            if ((EditMode == EditMode.NoEdit) ||
                (EditMode == EditMode.Normal && turn != pieceSide))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 駒の移動を開始します。
        /// </summary>
        private void BeginMovePiece(BoardPiece piece, Square square)
        {
            if (!CanBeginMove(piece.BWType))
            {
                return;
            }

            this.movingPiece = new MovingPiece(piece, square);

            if (EditMode != EditMode.Editing && EffectManager != null)
            {
                EffectManager.BeginMove(square, piece);
            }

            InManipulating = true;
        }

        /// <summary>
        /// 駒打ちの処理を開始します。
        /// </summary>
        private void BeginDropPiece(BoardPiece piece)
        {
            if (!CanBeginMove(piece.BWType))
            {
                return;
            }

            if (GetHandCount(piece.PieceType, piece.BWType) <= 0)
            {
                return;
            }

            // 表示用の駒を追加します。
            this.movingPiece = new MovingPiece(piece, null);

            if (EditMode != EditMode.Editing && EffectManager != null)
            {
                EffectManager.BeginMove(null, piece);
            }

            InManipulating = true;
        }

        /// <summary>
        /// 駒の移動を完了します。
        /// </summary>
        private void EndMovePiece(Point pos)
        {
            var square = PointToSquare(pos);
            if (square != null)
            {
                DoMove(square);
                return;
            }

            if (EditMode == EditMode.Editing)
            {
                // 編集モードの場合は、駒を駒台に移動できます。
                var boxColor = PointToPieceBoxType(pos);
                if (boxColor != null)
                {
                    DoMoveEditing(null, boxColor);
                    return;
                }
            }

            EndMove();
        }

        /// <summary>
        /// 駒の移動を終了します。
        /// </summary>
        public void EndMove()
        {
            if (this.movingPiece == null)
            {
                return;
            }

            this.movingPiece = null;
            ClosePromoteDialog();

            if (EffectManager != null)
            {
                EffectManager.EndMove();
            }

            InManipulating = false;
        }
        #endregion

        #region 駒の移動
        /// <summary>
        /// 移動中の駒を動かします。
        /// </summary>
        private void MovePiece(Point pos)
        {
            if (this.movingPiece == null)
            {
                return;
            }

            // 駒の位置を移動させます。
            this.movingPiece.Center = pos;
        }
        #endregion

        #region 駒の移動終了
        /// <summary>
        /// 指し手が実際に着手可能か確認します。
        /// </summary>
        private bool CanMove(Move move, MoveFlags flags = MoveFlags.DoMoveDefault)
        {
            var tmp = Board.Clone();

            // 成り・不成りの選択ダイアログを出す前に
            // 駒の移動ができるか調べておきます。
            // 失敗したら移動中だった駒は元の位置に戻されます。
            if (!tmp.DoMove(move, flags))
            {
                return false;
            }

            // 通常モードの場合、今指した側の玉が王手されていたら
            // その手は採用しません。（王手放置禁止）
            if (EditMode == EditMode.Normal &&
                tmp.IsChecked(tmp.Turn.Flip()))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 移動中の駒を新しい位置に移動します。
        /// </summary>
        private void DoMove(Square dstSquare)
        {
            if (this.movingPiece == null)
            {
                return;
            }

            if (EditMode == EditMode.Normal)
            {
                DoMoveNormal(dstSquare);
            }
            else if (EditMode == EditMode.Editing)
            {
                DoMoveEditing(dstSquare, null);
            }
            else
            {
                // 局面編集不可のとき
                EndMove();
            }
        }

        /// <summary>
        /// 移動中の駒を新しい位置に移動します。
        /// </summary>
        /// <remarks>
        /// 指せない指し手の場合は、駒の移動を終了します。
        /// </remarks>
        private void DoMoveNormal(Square dstSquare)
        {
            var srcSquare = this.movingPiece.Square;
            var piece = this.movingPiece.BoardPiece;
            Move move = null;

            if (srcSquare != null)
            {
                // 駒の移動の場合
                move = Move.CreateMove(
                    piece.BWType, srcSquare, dstSquare, piece.Piece, false);

                // 成／不成りのダイアログを出す前に着手可能か確認します。
                if (!CanMove(move))
                {
                    move.IsPromote = true;
                    if (!CanMove(move))
                    {
                        EndMove();
                        return;
                    }
                }

                // 成れる場合は選択用のダイアログを出します。
                if (!move.IsPromote && Board.CanPromote(move))
                {
                    var promote = CheckToPromote(piece.PieceType, move.BWType);

                    move.IsPromote = promote;
                }
            }
            else
            {
                // 駒打ちの場合
                move = Move.CreateDrop(
                    piece.BWType, dstSquare, piece.PieceType);

                if (!CanMove(move))
                {
                    EndMove();
                    return;
                }
            }

            EndMove();
            MakeMove(move);
        }

        /// <summary>
        /// 実際に指し手を進めます。
        /// </summary>
        private void MakeMove(Move move)
        {
            if (move == null || !move.Validate())
            {
                return;
            }

            Board.DoMove(move);
            MovedByGui.SafeRaiseEvent(
                this, new BoardPieceEventArgs(Board, move));
        }

        /// <summary>
        /// 編集モードでの駒の移動を行います。
        /// </summary>
        /// <remarks>
        /// 駒の移動元・移動先は
        /// ・盤面上のマス
        /// ・駒台 or 駒箱
        /// の２種類があります。
        /// </remarks>
        private void DoMoveEditing(Square dstSquare, BWType? boxColor)
        {
            var piece = this.movingPiece.BoardPiece;
            var srcSquare = this.movingPiece.Square;

            if (dstSquare == null && boxColor == null)
            {
                throw new ArgumentException("DoMoveEditing");
            }

            // 駒箱から持ってきた場合は先手側の駒として置きます。
            var bwType = (
                piece.BWType == BWType.None ?
                ViewSide : piece.BWType);

            // 先に盤面の状態を元に戻しておきます。
            EndMove();

            // 盤上に駒を動かす場合は、２歩を禁止する必要があります。
            if (dstSquare != null && piece.Piece == Piece.Hu)
            {
                // 同じ筋に動かす場合は２歩の判定を行いません。
                if ((srcSquare == null || dstSquare.File != srcSquare.File) &&
                    (Board.GetPawnCount(bwType, dstSquare.File) > 0))
                {
                    return;
                }
            }

            if (dstSquare != null &&
                Board.IsPromoteForce(piece.Piece, piece.BWType, dstSquare))
            {
                piece.IsPromoted = true;
            }

            // 移動元の駒の消去
            if (srcSquare != null)
            {
                // 盤上の移動前にあった駒を削除します。
                Board[srcSquare] = null;
            }
            else
            {
                // 駒箱などの駒の数を減らします。
                DecHandCount(piece.PieceType, piece.BWType);
            }
            
            if (boxColor != null)
            {
                // 駒箱へ移動する場合
                IncHandCount(piece.PieceType, boxColor.Value);
            }
            else if (dstSquare != null)
            {
                // 移動先が盤上の場合
                var target = Board[dstSquare];
                if (target != null)
                {
                    IncHandCount(target.PieceType, piece.BWType);

                    Board[dstSquare] = null;
                }

                // 移動先の駒を増やします。
                Board[dstSquare] =
                    new BoardPiece(piece.PieceType, piece.IsPromoted, bwType);
            }

            BoardEdited.SafeRaiseEvent(
                this, new BoardPieceEventArgs(Board, null));
        }
        #endregion

        #region 局面編集専用
        /// <summary>
        /// マウスの右クリック時に呼ばれます。
        /// </summary>
        private void ProcessEdit(System.Drawing.Point pos)
        {
            if (EditMode != EditMode.Editing)
            {
                return;
            }

            var square = PointToSquare(pos);
            if (square == null)
            {
                return;
            }

            var piece = Board[square];
            if (piece == null)
            {
                return;
            }

            // 新しい駒を設定します。
            Board[square] = SuccPiece(piece, square);
        }

        /// <summary>
        /// 駒の手番や成・不成りを変更します。(局面編集モードで使います)
        /// </summary>
        private BoardPiece SuccPiece(BoardPiece piece, Square square)
        {
            var clone = piece.Clone();
            var type = clone.PieceType;

            if (clone.PieceType == PieceType.Gyoku)
            {
                // 玉は同じ側に２つあることができません。
                if (Board.GetGyoku(clone.BWType.Flip()) == null)
                {
                    clone.BWType = clone.BWType.Flip();
                }
            }
            else if (clone.PieceType == PieceType.Kin)
            {
                // 金は成れません。
                clone.BWType = clone.BWType.Flip();
            }
            else
            {
                if (!clone.IsPromoted)
                {
                    clone.IsPromoted = true;
                }
                else
                {
                    clone.BWType = clone.BWType.Flip();
                    clone.IsPromoted = false;
                }
            }

            // 歩の場合は二歩を警戒する必要があります。
            if (clone.Piece == Piece.Hu &&
                Board.GetPawnCount(clone.BWType, square.File) > 0)
            {
                clone.IsPromoted = true;
            }

            // 成りを強制する場合もあります。
            if (Board.IsPromoteForce(clone.Piece, clone.BWType, square))
            {
                clone.IsPromoted = true;
            }

            return clone;
        }
        #endregion

        #region 盤上の駒
        /// <summary>
        /// 与えられた座標にあるマスを取得します。
        /// </summary>
        private Square PointToSquare(System.Drawing.Point pos)
        {
            if (!BoardSquareBounds.Contains(pos))
            {
                return null;
            }

            // とりあえず設定します。
            var file = (int)((pos.X - BoardSquareBounds.Left) / SquareSize.Width);
            var rank = (int)((pos.Y - BoardSquareBounds.Top) / SquareSize.Height);

            // 正しい位置にありましぇん。
            var square = new Square(Board.BoardSize - file, rank + 1);
            if (!square.Validate())
            {
                return null;
            }

            return (ViewSide == BWType.White ? square.Flip() : square);
        }

        /// <summary>
        /// 与えられたマスが表示される画面上の中心位置を取得します。
        /// </summary>
        public PointF SquareToPoint(Square square)
        {
            if ((object)square == null)
            {
                return PointF.Empty;
            }

            var relative =
                (ViewSide == BWType.Black
                ? new PointF(
                    (Board.BoardSize - square.File) * SquareSize.Width,
                    (square.Rank - 1) * SquareSize.Height)
                : new PointF(
                    (square.File - 1) * SquareSize.Width,
                    (Board.BoardSize - square.Rank) * SquareSize.Height));

            var x = BoardSquareBounds.Left;
            var y = BoardSquareBounds.Top;
            return new PointF(
                x + relative.X + (SquareSize.Width / 2.0f),
                y + relative.Y + (SquareSize.Height / 2.0f));
        }
        #endregion

        #region 持ち駒
        /// <summary>
        /// 手番を内部インデックスに変換します。(局面の反転を考慮します)
        /// </summary>
        private static int GetViewIndex(BWType bwType, BWType viewSide)
        {
            if (bwType == BWType.None)
            {
                return 0;
            }
            else if (bwType == viewSide)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        /// <summary>
        /// 指定の座標値に駒台があればそれを取得します。
        /// </summary>
        private BWType? PointToPieceBoxType(System.Drawing.Point pos)
        {
            foreach (var boxType in EnumEx.GetValues<BWType>())
            {
                // 盤の反転を考慮して駒台の領域を調べます。
                var viewIndex = GetViewIndex(boxType, ViewSide);
                if (this.pieceBoxBounds[viewIndex].Contains(pos))
                {
                    return boxType;
                }
            }

            return null;
        }

        /// <summary>
        /// 指定の座標値に駒台上の駒があればそれを取得します。
        /// </summary>
        private BoardPiece PointToHandPiece(Point pos)
        {
            var boxTypeN = PointToPieceBoxType(pos);
            if (boxTypeN == null)
            {
                return null;
            }

            var boxType = boxTypeN.Value;
            foreach (var pieceType in EnumEx.GetValues<PieceType>())
            {
                var center = HandPieceToPoint(pieceType, boxType);
                var rect = new RectangleF(
                    (float)(center.X - SquareSize.Width / 2),
                    (float)(center.Y - SquareSize.Height / 2),
                    SquareSize.Width,
                    SquareSize.Height);

                if (rect.Contains(pos))
                {
                    return new BoardPiece(pieceType, false, boxType);
                }
            }

            return null;
        }

        /// <summary>
        /// 駒台上の駒のデフォルト中心位置を取得します。
        /// </summary>
        public PointF HandPieceToPoint(BoardPiece piece)
        {
            return HandPieceToPoint(piece.PieceType, piece.BWType);
        }

        /// <summary>
        /// 駒台上の駒のデフォルト中心位置を取得します。
        /// </summary>
        public PointF HandPieceToPoint(PieceType pieceType, BWType bwType)
        {
            var viewIndex = GetViewIndex(bwType, ViewSide);
            var bounds = this.pieceBoxBounds[viewIndex];

            // ○ 駒位置の計算方法
            // 駒台には駒を横に２つ並べます。また、両端と駒と駒の間には
            // 駒の幅/2をスペースとして挿入します。
            // このため、駒の幅/2 を基本区間とし、
            //   2(両端) + 1(駒間) + 4(駒数*2) = 7
            // を区間数として、駒の位置を計算します。
            //
            // また、縦の計算では先手・後手などの文字列を表示するため、
            // 手前側は上部、向かい側は下部に余計な空間を作ります。
            //   4(上下端+α) + 3(駒間) + 8(駒数*4) = 15
            var hw = bounds.Width / 7;
            var hh = bounds.Height / 15;
            var x = ((int)pieceType - 2) % 2;
            var y = ((int)pieceType - 2) / 2;
            var offsetY = 0.0f;

            // 駒箱の玉も表示します。
            if (pieceType == PieceType.Gyoku)
            {
                x = 1;
                y = 3;
            }

            if (bwType == BWType.None || bwType == ViewSide)
            {
                offsetY = 1.9f;
            }
            else
            {
                x = 1 - x;
                y = 3 - y;
                offsetY = 0.3f;
            }

            // 駒の中心位置
            // 駒の数を右肩に表示するため、少し左にずらしています。
            // また、対局者名などを表示するため上下にずらしています。
            return new PointF(
                bounds.Left + hw * (x * 3 + 2 - 0.2f),
                bounds.Top + hh * (y * 3 + 2 + offsetY));
        }
        #endregion

        #region 成・不成りダイアログ
        /// <summary>
        /// 成るか不成りかダイアログによる選択を行います。
        /// </summary>
        private bool CheckToPromote(PieceType pieceType, BWType bwType)
        {
            var dialog = new PromoteDialog();
            var screenPos = Cursor.Position;

            dialog.StartPosition = FormStartPosition.Manual;
            dialog.Left = screenPos.X - (dialog.Width / 2);
            dialog.Top = screenPos.Y - dialog.Height - (int)SquareSize.Height / 2;
            dialog.AdjustInDisplay();

            try
            {
                ClosePromoteDialog();

                // 成り・不成り選択中に外から局面が設定されることがあります。
                // その場合に備えてダイアログ自体を持っておきます。
                this.promoteDialog = dialog;

                var result = dialog.ShowDialog();
                ClosePromoteDialog();

                return (result == DialogResult.OK);
            }
            finally
            {
                ClosePromoteDialog();
            }
        }

        /// <summary>
        /// 成り・不成り選択中に外から局面が設定されることがあります。
        /// その場合には選択ダイアログを強制的にクローズします。
        /// </summary>
        private void ClosePromoteDialog()
        {
            if (this.promoteDialog != null)
            {
                this.promoteDialog.Close();
                this.promoteDialog = null;
            }
        }
        #endregion
    }
}
