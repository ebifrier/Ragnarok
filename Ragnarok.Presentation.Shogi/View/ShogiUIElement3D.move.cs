using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;

using Ragnarok;
using Ragnarok.Shogi;
using Ragnarok.Utility;
using Ragnarok.ObjectModel;
using Ragnarok.Presentation.Utility;
using Ragnarok.Presentation.Extra.Entity;

namespace Ragnarok.Presentation.Shogi.View
{
    using ViewModel;

    /// <summary>
    /// 将棋の盤面を扱う3D用のエレメントです。
    /// </summary>
    public partial class ShogiUIElement3D : UIElement3D
    {
        private Model3DGroup pieceContainer;
        private Model3DGroup capturedPieceContainer;        
        private readonly List<PieceObject> pieceObjectList =
            new List<PieceObject>();
        private readonly List<PieceObject>[] capturedPieceObjectList =
            new List<PieceObject>[3];

        private readonly Rect[] capturedPieceBoxBounds = new Rect[3];
        private readonly int[] komaboxCount = new int[(int)PieceType.Hu + 1];
        private PieceObject movingPiece;
        private Window promoteDialog;

        /// <summary>
        /// 盤などのサイズを設定します。
        /// </summary>
        private void InitializeBounds(Rect3D banBounds, Rect3D komaboxBounds,
                                      Rect3D komadai0Bounds, Rect3D komadai1Bounds)
        {
            // 駒の表示サイズを設定
            CellSize = new Size(
                banBounds.SizeX / (Board.BoardSize + BanBorderRate * 2),
                banBounds.SizeY / (Board.BoardSize + BanBorderRate * 2));

            // 盤サイズの設定
            BanBounds = new Rect(
                banBounds.X + CellSize.Width * BanBorderRate,
                banBounds.Y + CellSize.Height * BanBorderRate,
                CellSize.Width * Board.BoardSize,
                CellSize.Height * Board.BoardSize);

            // index=0が駒箱の駒となります。
            this.capturedPieceBoxBounds[0] = WPFUtil.MakeRectXY(komaboxBounds);
            this.capturedPieceBoxBounds[1] = WPFUtil.MakeRectXY(komadai0Bounds);
            this.capturedPieceBoxBounds[2] = WPFUtil.MakeRectXY(komadai1Bounds);
        }

        /// <summary>
        /// 表示用行列の逆変換を掛けます。
        /// </summary>
        private Point InvarseTranslate(Point pos)
        {
            var inv = Transform.Inverse;
            if (inv == null)
            {
                return pos;
            }

            var pos3d = new Point3D(pos.X, pos.Y, 0.0);
            pos3d = inv.Transform(pos3d);
            return new Point(pos3d.X, pos3d.Y);
        }

        /// <summary>
        /// マウスの左ボタン押下時に呼ばれます。
        /// </summary>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // マウスがどのセルにいるかです。
            var pos = InvarseTranslate(e.GetPosition(this));

            if (this.movingPiece == null)
            {
                // 自動再生中であれば、それを停止します。
                if (this.autoPlay != null && !this.autoPlay.IsImportant)
                {
                    StopAutoPlay();
                    return;
                }

                // 駒検索
                var square = PointToSquare(pos);
                if (square != null)
                {
                    var pieceObject = GetPieceObject(square);
                    if (pieceObject != null)
                    {
                        BeginMovePiece(pieceObject);
                        MovePiece(e);
                        return;
                    }
                }

                // 駒台の駒を検索
                var piece = PointToCapturedPiece(pos);
                if (piece != null)
                {
                    var pieceObject = GetCapturedPieceObject(
                        piece.PieceType, piece.BWType);
                    if (pieceObject != null && pieceObject.Count > 0)
                    {
                        BeginDropPiece(pieceObject);
                        MovePiece(e);
                        return;
                    }
                }
            }
            else
            {
                // 駒の移動を完了します。
                var square = PointToSquare(pos);
                if (square != null)
                {
                    DoMove(square);
                    return;
                }

                if (EditMode == EditMode.Editing)
                {
                    // 編集モードの場合は、駒を駒台に移動できます。
                    var boxColor = PointToCapturedBox(pos);
                    if (boxColor != null)
                    {
                        DoMoveEditing(null, boxColor);
                        return;
                    }
                }

                EndMove();
            }
        }

        /// <summary>
        /// マウスの右ボタンが上がったときに呼ばれます。
        /// </summary>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
        }

        #region 駒の移動開始
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
        private void BeginMovePiece(PieceObject pieceObject)
        {
            if (!CanBeginMove(pieceObject.Piece.BWType))
            {
                return;
            }

            this.movingPiece = pieceObject;

            // 描画順を変えます。
            this.pieceContainer.Children.Remove(pieceObject.ModelGroup);
            this.pieceContainer.Children.Add(pieceObject.ModelGroup);

            if (EditMode != EditMode.Editing && EffectManager != null)
            {
                EffectManager.BeginMove(pieceObject.Square, pieceObject.Piece);
            }

            InManipulating = true;
        }

        /// <summary>
        /// 駒打ちの処理を開始します。
        /// </summary>
        private void BeginDropPiece(PieceObject pieceObject)
        {
            var piece = pieceObject.Piece;

            if (!CanBeginMove(piece.BWType))
            {
                return;
            }

            if (GetCapturedPieceCount(piece.PieceType, piece.BWType) <= 0)
            {
                return;
            }

            // 表示用の駒を追加します。
            this.movingPiece = new PieceObject(this, piece);
            AddPieceObject(this.movingPiece);

            if (EditMode != EditMode.Editing && EffectManager != null)
            {
                EffectManager.BeginMove(null, piece);
            }

            InManipulating = true;
        }

        /// <summary>
        /// 駒の移動を終了します。
        /// </summary>
        private void EndMove()
        {
            if (this.movingPiece == null)
            {
                return;
            }

            var square = this.movingPiece.Square;
            if (square != null)
            {
                // 移動中の駒の位置を元に戻します。
                this.movingPiece.Coord = SquareToPoint(square);
            }
            else
            {
                // 駒打ちの場合は、表示用オブジェクトを新規に作成しています。
                RemovePieceObject(this.movingPiece);
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
        /// マウス移動時に呼ばれます。
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            MovePiece(e);
        }

        /// <summary>
        /// 移動中の駒を動かします。
        /// </summary>
        private void MovePiece(MouseEventArgs e)
        {
            if (this.movingPiece == null)
            {
                return;
            }

            // 駒とマウスの位置の差を求めておきます。
            var mousePos = InvarseTranslate(e.GetPosition(this));

            this.movingPiece.Coord = WPFUtil.MakeVector3D(mousePos, MovingPieceZ);
        }
        #endregion

        #region 駒の移動終了
        /// <summary>
        /// 指し手が実際に着手可能か確認します。
        /// </summary>
        private bool CanMove(BoardMove move, MoveFlags flags = MoveFlags.DoMoveDefault)
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
            var piece = this.movingPiece.Piece;
            BoardMove move = null;

            if (srcSquare != null)
            {
                // 駒の移動の場合
                move = new BoardMove()
                {
                    SrcSquare = srcSquare,
                    DstSquare = dstSquare,
                    MovePiece = piece.Piece,
                    BWType = piece.BWType,
                };

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
                move = new BoardMove()
                {
                    DstSquare = dstSquare,
                    DropPieceType = piece.PieceType,
                    BWType = piece.BWType,
                };

                if (!CanMove(move))
                {
                    EndMove();
                    return;
                }
            }

            EndMove();
            DoMove(move);
        }

        /// <summary>
        /// 実際に指し手を進めます。
        /// </summary>
        private void DoMove(BoardMove move)
        {
            if (move == null || !move.Validate())
            {
                return;
            }

            this.RaiseEvent(new BoardPieceRoutedEventArgs(
                BoardPieceChangingEvent, Board.Clone(), move));
            Board.DoMove(move);
            this.RaiseEvent(new BoardPieceRoutedEventArgs(
                BoardPieceChangedEvent, Board.Clone(), move));
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
            var piece = this.movingPiece.Piece;
            var srcSquare = this.movingPiece.Square;

            if (dstSquare == null && boxColor == null)
            {
                throw new ArgumentException("DoMoveEditing");
            }

            // 駒箱から持ってきた場合は先手番の駒として置きます。
            var bwType = (
                piece.BWType == BWType.None ?
                ViewSide : piece.BWType);

            // 先に盤面の状態を元に戻しておきます。
            EndMove();

            // 盤上に駒を動かす場合は、２歩を禁止する必要があります。
            if (dstSquare != null && piece.PieceType == PieceType.Hu)
            {
                // 同じ筋に動かす場合は２歩の判定を行いません。
                if ((srcSquare == null || dstSquare.File != srcSquare.File) &&
                    (Board.IsDoublePawn(bwType, dstSquare)))
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
                RemovePieceObject(srcSquare);
            }
            else
            {
                // 駒箱などの駒の数を減らします。
                DecCapturedPieceCount(piece.PieceType, piece.BWType);
            }
            
            if (boxColor != null)
            {
                // 駒箱へ移動する場合
                IncCapturedPieceCount(piece.PieceType, boxColor.Value);
            }
            else if (dstSquare != null)
            {
                // 移動先が盤上の場合
                var target = Board[dstSquare];
                if (target != null)
                {
                    IncCapturedPieceCount(target.PieceType, piece.BWType);

                    Board[dstSquare] = null;
                    RemovePieceObject(dstSquare);
                }

                // 移動先の駒を増やします。
                Board[dstSquare] = new BoardPiece(piece.PieceType, piece.IsPromoted, bwType);
                AddPieceObject(new PieceObject(this, Board[dstSquare], dstSquare));
            }
        }
        #endregion

        #region 局面編集専用
        /// <summary>
        /// マウスの右クリック時に呼ばれます。
        /// </summary>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            if (EditMode != EditMode.Editing)
            {
                return;
            }

            var mousePos = InvarseTranslate(e.GetPosition(this));
            var square = PointToSquare(mousePos);
            if (square == null)
            {
                return;
            }

            var piece = Board[square];
            if (piece == null)
            {
                return;
            }

            // 古い駒を削除
            RemovePieceObject(square);

            // 新しい駒を設定します。
            Board[square] = SuccPiece(piece, square);
            AddPieceObject(new PieceObject(this, Board[square], square));
        }

        /// <summary>
        /// 駒の手番や成・不成りを変更します。(局面編集モードで使います)
        /// </summary>
        private BoardPiece SuccPiece(BoardPiece piece, Square square)
        {
            var clone = piece.Clone();
            var type = clone.PieceType;

            // 金玉コンビは成れません。
            if (type == PieceType.Gyoku ||
                type == PieceType.Kin)
            {
                clone.BWType = piece.BWType.Flip();
            }
            else
            {
                if (!clone.IsPromoted)
                {
                    clone.IsPromoted = true;
                }
                else
                {
                    clone.BWType = piece.BWType.Flip();
                    clone.IsPromoted = false;
                }
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
        private Square PointToSquare(Point pos)
        {
            // とりあえず設定します。
            var file = (int)((pos.X - BanBounds.Left) / CellSize.Width);
            var rank = (int)((pos.Y - BanBounds.Top) / CellSize.Height);

            // 正しい位置にありましぇん。
            var square = new Square(Board.BoardSize - file, rank + 1);
            if (!square.Validate())
            {
                return null;
            }

            /*// 各セルの幅と高さを取得します。
            var gridX = pos.X % this.model.CellWidth;
            var gridY = pos.Y % this.model.CellHeight;

            // セルの端ぎりぎりならそのセルにいると判定しません。
            if (gridX < CellWidth * 0.1 || CellWidth * 0.9 < gridX)
            {
                return null;
            }

            if (gridY < CellHeight * 0.1 || CellHeight * 0.9 < gridY)
            {
                return null;
            }*/

            return (ViewSide == BWType.White ? square.Flip() : square);
        }

        /// <summary>
        /// 与えられたマスが表示される画面上の中心位置を取得します。
        /// </summary>
        public Vector3D SquareToPoint(Square square)
        {
            if ((object)square == null)
            {
                return new Vector3D();
            }

            var relative =
                (ViewSide == BWType.Black
                ? new Point(
                    (Board.BoardSize - square.File) * CellSize.Width,
                    (square.Rank - 1) * CellSize.Height)
                : new Point(
                    (square.File - 1) * CellSize.Width,
                    (Board.BoardSize - square.Rank) * CellSize.Height));

            var leftTop = BanBounds.TopLeft;
            return new Vector3D(
                leftTop.X + relative.X + (CellSize.Width / 2.0),
                leftTop.Y + relative.Y + (CellSize.Height / 2.0),
                PieceZ);
        }

        /// <summary>
        /// 指定の位置にある駒を取得します。
        /// </summary>
        private PieceObject GetPieceObject(Square square)
        {
            if (square == null || !square.Validate())
            {
                return null;
            }

            return this.pieceObjectList.FirstOrDefault(
                _ => _.Square == square);
        }

        /// <summary>
        /// 駒の表示用オブジェクトを取得します。
        /// </summary>
        private void AddPieceObject(PieceObject value)
        {
            if (value == null)
            {
                return;
            }

            // 駒をデフォルト位置まで移動させます。
            value.Coord =
                (value.Square != null
                ? SquareToPoint(value.Square)
                : CapturedPieceToPoint(value.Piece));

            this.pieceContainer.Children.Add(value.ModelGroup);
            this.pieceObjectList.Add(value);
        }

        /// <summary>
        /// 指定の位置にある駒を削除します。
        /// </summary>
        private void RemovePieceObject(PieceObject piece)
        {
            if (piece == null)
            {
                return;
            }

            this.pieceContainer.Children.Remove(piece.ModelGroup);
            this.pieceObjectList.Remove(piece);
        }

        /// <summary>
        /// 指定の位置にある駒を削除します。
        /// </summary>
        private void RemovePieceObject(Square square)
        {
            if (square == null || !square.Validate())
            {
                return;
            }

            // 指定のマスにある駒を探します。
            var index = this.pieceObjectList.FindIndex(
                _ => _.Square == square);
            if (index < 0)
            {
                return;
            }

            RemovePieceObject(this.pieceObjectList[index]);
        }

        /// <summary>
        /// 表示されている駒をすべて削除します。
        /// </summary>
        private void ClearPieceObjects()
        {
            this.pieceObjectList.Clear();
            this.pieceContainer.Children.Clear();
        }

        /// <summary>
        /// PieceObjectの比較用オブジェクトです。
        /// </summary>
        private sealed class PieceObjectComparer : IEqualityComparer<PieceObject>
        {
            public bool Equals(PieceObject x, PieceObject y)
            {
                if (x.Square != y.Square)
                {
                    return false;
                }

                if (x.Piece != y.Piece)
                {
                    return false;
                }

                return true;
            }

            public int GetHashCode(PieceObject obj)
            {
                return (
                    obj.Piece.GetHashCode() ^
                    obj.Square.GetHashCode());
            }
        }

        /// <summary>
        /// 今の局面と画面の表示を合わせます。
        /// </summary>
        private void SyncBoardPiece(bool forceUpdate)
        {
            if (Board == null)
            {
                return;
            }

            var newList = Board.AllSquares()
                .Where(_ => Board[_] != null)
                .Select(_ => new PieceObject(this, Board[_], _))
                .ToArray();

            if (forceUpdate)
            {
                // 全部更新
                ClearPieceObjects();
                newList.ForEach(_ => AddPieceObject(_));
            }
            else
            {
                // 差分のみを更新することで、画面のちらつきを抑えます。
                var union = this.pieceObjectList
                    .Intersect(newList, new PieceObjectComparer())
                    .ToArray();
                
                this.pieceObjectList.ToArray()
                    .Except(union, new PieceObjectComparer())
                    .ForEach(_ => RemovePieceObject(_));
                newList
                    .Except(union, new PieceObjectComparer())
                    .ForEach(_ => AddPieceObject(_));
            }
        }
        #endregion

        #region 持ち駒
        /// <summary>
        /// 手番を内部インデックスに変換します。(局面の反転を考慮します)
        /// </summary>
        private static int GetCapturedPieceIndex(BWType bwType, BWType viewSide)
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
        private BWType? PointToCapturedBox(Point pos)
        {
            foreach (var bwType in EnumEx.GetValues<BWType>())
            {
                // 盤の反転を考慮して駒台の領域を調べます。
                var viewIndex = GetCapturedPieceIndex(bwType, ViewSide);
                if (this.capturedPieceBoxBounds[viewIndex].Contains(pos))
                {
                    return bwType;
                }
            }

            return null;
        }

        /// <summary>
        /// 指定の座標値に駒台上の駒があればそれを取得します。
        /// </summary>
        private BoardPiece PointToCapturedPiece(Point pos)
        {
            var bwTypeN = PointToCapturedBox(pos);
            if (bwTypeN == null)
            {
                return null;
            }

            var bwType = bwTypeN.Value;
            foreach (var pieceType in EnumEx.GetValues<PieceType>())
            {
                var center = CapturedPieceToPoint(pieceType, bwType);
                var rect = new Rect(
                    center.X - CellSize.Width / 2,
                    center.Y - CellSize.Height / 2,
                    CellSize.Width,
                    CellSize.Height);

                if (rect.Contains(pos))
                {
                    return new BoardPiece(pieceType, false, bwType);
                }
            }

            return null;
        }

        /// <summary>
        /// 駒台上の駒のデフォルト位置を取得します。
        /// </summary>
        public Vector3D CapturedPieceToPoint(BoardPiece piece)
        {
            return CapturedPieceToPoint(piece.PieceType, piece.BWType);
        }

        /// <summary>
        /// 駒台上の駒のデフォルト位置を取得します。
        /// </summary>
        public Vector3D CapturedPieceToPoint(PieceType pieceType, BWType bwType)
        {
            var index = GetCapturedPieceIndex(bwType, ViewSide);
            var bounds = this.capturedPieceBoxBounds[index];

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
            var offsetY = 0.0;

            // 駒箱の玉も表示します。
            if (pieceType == PieceType.Gyoku)
            {
                x = 1;
                y = 3;
            }

            if (bwType == BWType.None || bwType == ViewSide)
            {
                offsetY = 1.7;
            }
            else
            {
                x = 1 - x;
                y = 3 - y;
                offsetY = 0.3;
            }

            // 駒の中心位置
            // 駒の数を右肩に表示するため、少し左にずらしています。
            // また、対局者名などを表示するため上下にずらしています。
            return new Vector3D(
                bounds.Left + hw * (x * 3 + 2 - 0.2),
                bounds.Top + hh * (y * 3 + 2 + offsetY),
                PieceZ);
        }

        /// <summary>
        /// 駒台上の表示用の駒を取得します。
        /// </summary>
        private PieceObject GetCapturedPieceObject(PieceType pieceType, BWType bwType)
        {
            var index = GetCapturedPieceIndex(bwType, BWType.Black);
            var capturedPieceList = this.capturedPieceObjectList[index];

            return (
                (int)pieceType < capturedPieceList.Count ?
                capturedPieceList[(int)pieceType] :
                null);
        }

        /// <summary>
        /// 局面と表示で駒台の駒の数を合わせます。
        /// </summary>
        private void SyncCapturedPieceCount(PieceType pieceType, BWType bwType)
        {
            var piece = GetCapturedPieceObject(pieceType, bwType);

            if (piece != null)
            {
                piece.Count = GetCapturedPieceCount(pieceType, bwType);
            }
        }

        /// <summary>
        /// 現在の局面から、駒箱に入るべき駒数を調べます。
        /// </summary>
        private void InitKomaboxPieceCount()
        {
            foreach (var pieceType in EnumEx.GetValues<PieceType>())
            {
                var count = Board.GetLeavePieceCount(pieceType);

                this.komaboxCount[(int)pieceType] = count;
            }
        }

        /// <summary>
        /// 表示用の持ち駒をすべて削除します。
        /// </summary>
        private void ClearCapturedPieceObjects()
        {
            foreach (var list in this.capturedPieceObjectList)
            {
                if (list != null)
                {
                    list.ToArray()
                        .ForEach(_ => _.Terminate());
                    list.Clear();
                }
            }

            this.capturedPieceContainer.Children.Clear();
        }

        /// <summary>
        /// 持ち駒や駒箱の駒の数を取得します。
        /// </summary>
        private int GetCapturedPieceCount(PieceType pieceType, BWType bwType)
        {
            if (bwType == BWType.None)
            {
                // 駒箱の駒の数
                return this.komaboxCount[(int)pieceType];
            }
            else
            {
                // 持ち駒の駒の数
                if (Board == null)
                {
                    return 0;
                }

                return Board.GetCapturedPieceCount(pieceType, bwType);
            }
        }

        /// <summary>
        /// 持ち駒や駒箱の駒の数を設定します。
        /// </summary>
        private void SetCapturedPieceCount(PieceType pieceType, BWType bwType,
                                           int count)
        {
            if (count < 0)
            {
                throw new ArgumentException("count");
            }

            if (bwType == 0)
            {
                // 駒箱の駒の数
                this.komaboxCount[(int)pieceType] = count;
            }
            else
            {
                // 持ち駒の駒の数
                Board.SetCapturedPieceCount(pieceType, bwType, count);
            }

            SyncCapturedPieceCount(pieceType, bwType);
        }

        /// <summary>
        /// 持ち駒や駒箱の駒の数を増やします。
        /// </summary>
        private void IncCapturedPieceCount(PieceType pieceType, BWType bwType)
        {
            SetCapturedPieceCount(
                pieceType, bwType,
                GetCapturedPieceCount(pieceType, bwType) + 1);
        }

        /// <summary>
        /// 持ち駒や駒箱の駒の数を減らします。
        /// </summary>
        private void DecCapturedPieceCount(PieceType pieceType, BWType bwType)
        {
            SetCapturedPieceCount(
                pieceType, bwType,
                GetCapturedPieceCount(pieceType, bwType) - 1);
        }

        /// <summary>
        /// 駒台上の駒の表示用オブジェクトを取得します。
        /// </summary>
        private PieceObject CreateCapturedPieceObject(PieceType pieceType, BWType bwType)
        {
            var piece = new BoardPiece(pieceType, false, bwType);
            var value = new PieceObject(this, piece)
            {
                Count = GetCapturedPieceCount(pieceType, bwType),
                Coord = CapturedPieceToPoint(pieceType, bwType),
                IsAlwaysVisible = false,
            };

            return value;
        }

        /// <summary>
        /// 駒台上に表示する描画用の駒を設定します。
        /// </summary>
        /// <remarks>
        /// 盤上の駒と違って、駒台上の駒はプロパティで表示・非表示などを
        /// 切り替えるため、駒の移動ごとに追加・削除をする必要はありません。
        /// </remarks>
        private void SyncCapturedPieceObject(bool forceUpdate)
        {
            if (Board == null)
            {
                return;
            }

            // 駒箱に入る駒を初期化します。
            InitKomaboxPieceCount();

            // 先に駒を削除します。
            ClearCapturedPieceObjects();

            // 駒箱・先手・後手用の駒台にある駒を用意します。
            EnumEx.GetValues<BWType>().ForEachWithIndex((bwType, index) =>
            {
                if (EditMode != EditMode.Editing && bwType == BWType.None)
                {
                    // 局面編集モード以外では、駒箱の駒は表示しません。
                    this.capturedPieceObjectList[index] = new List<PieceObject>();
                }
                else
                {
                    this.capturedPieceObjectList[index] =
                        EnumEx.GetValues<PieceType>()
                            .Select(_ => CreateCapturedPieceObject(_, bwType))
                            .ToList();

                    // 駒台に表示します。
                    this.capturedPieceObjectList[index]
                        .Where(_ => _.Piece.PieceType != PieceType.None)
                        .ForEach(_ => this.capturedPieceContainer.Children.Add(_.ModelGroup));
                }
            });
        }
        #endregion

        #region 成・不成りダイアログ
        /// <summary>
        /// 成るか不成りかダイアログによる選択を行います。
        /// </summary>
        private bool CheckToPromote(PieceType pieceType, BWType bwType)
        {
            var dialog = DialogUtil.CreateDialog(
                null,
                "成りますか？",
                "成り／不成り",
                MessageBoxButton.YesNo,
                MessageBoxResult.Yes);
            dialog.Topmost = true;

            dialog.Loaded += (sender, e) =>
            {
                var p = WPFUtil.GetMousePosition(dialog);
                var screenPos = dialog.PointToScreen(p);

                dialog.WindowStartupLocation = WindowStartupLocation.Manual;
                dialog.Left = screenPos.X - (dialog.ActualWidth / 2);
                dialog.Top = screenPos.Y + CellSize.Height / 2;
                dialog.AdjustInDisplay();
            };

            try
            {
                ClosePromoteDialog();

                // 成り・不成り選択中に外から局面が設定されることがあります。
                // その場合に備えてダイアログ自体を持っておきます。
                this.promoteDialog = dialog;

                var result = dialog.ShowDialog();
                ClosePromoteDialog();

                return (result != null ? result.Value : false);
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
