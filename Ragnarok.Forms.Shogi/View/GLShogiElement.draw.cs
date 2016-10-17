﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL;

using Ragnarok.Extra.Effect;
using Ragnarok.Shogi;
using Ragnarok.Utility;
using Ragnarok.ObjectModel;
using Ragnarok.OpenGL;

namespace Ragnarok.Forms.Shogi.View
{
    /// <summary>
    /// 盤面の描画を行います。
    /// </summary>
    public partial class GLShogiElement
    {
        private readonly RectangleF[] pieceBoxBounds = new RectangleF[3];
        private Dictionary<BoardPiece, Mesh> pieceMeshDic;
        private Texture boardTexture;
        private Texture pieceTexture;
        private Texture pieceBoxTexture;

        /// <summary>
        /// 対局者名の描画用のフォントです。
        /// </summary>
        private readonly TextTextureFont nameFont = new TextTextureFont
        {
            Color = Color.Black,
            EdgeColor = Color.White,
            EdgeLength = 0.0,
        };

        /// <summary>
        /// 思考時間などの時間描画用フォントです。
        /// </summary>
        private readonly TextTextureFont timeFont = new TextTextureFont
        {
            Color = Color.White,
            EdgeColor = Color.White,
            EdgeLength = 0.0,
            IsStretchSize = true,
        };

        /// <summary>
        /// 盤の符号を描画するためのフォントです。
        /// </summary>
        private readonly TextTextureFont boardSignFont = new TextTextureFont
        {
            Color = Color.Black,
            EdgeColor = Color.White,
            EdgeLength = 0.0,
            IsStretchSize = true,
        };

        /// <summary>
        /// 持ち駒の数を描画するためのフォントです。
        /// </summary>
        private readonly TextTextureFont pieceCountFont = new TextTextureFont
        {
            Font = new Font(TextTextureFont.DefaultFont, FontStyle.Bold),
            Color = Color.Black,
            EdgeColor = Color.FromArgb(255, Color.White),
            EdgeLength = 8.0,
            IsStretchSize = true,
        };

        #region 初期化
        /// <summary>
        /// 描画関係の初期化を行います。
        /// </summary>
        private void InitializeDraw()
        {
            InitializeBounds();
            InitializePieceMesh();

            // ハンドラの設定を行います。
            AddPropertyChangedHandler("BoardBitmap", BoardBitmapUpdated);
            AddPropertyChangedHandler("PieceBitmap", PieceBitmapUpdated);
            AddPropertyChangedHandler("PieceBoxBitmap", PieceBoxBitmapUpdated);

            // デフォルトのプロパティ値も設定します。
            BoardBitmap = DefaultBoardBitmap;
            PieceBoxBitmap = DefaultPieceBoxBitmap;
            PieceBitmap = DefaultPieceBitmap;
            IsTimeVisible = true;
            TebanPlayerNameBackgroundColor = Color.FromArgb(128, Color.White);
            UnTebanPlayerNameBackgroundColor = Color.FromArgb(32, Color.White);
            TimeBackgroundColor = Color.FromArgb(128, Color.Black);
            IsArrowVisible = true;
            MoveArrowList = new List<MoveArrowInfo>();
            AutoPlayColor = Color.FromArgb(96, 0, 24, 86);
            AutoPlayOpacity = 0.0;
            BoardOpacity = 1.0;

            LocalTransform.Scale(1.0 / 640.0, 1.0 / 360.0, 1.0);
        }

        /// <summary>
        /// 描画関係の終了処理を行います。
        /// </summary>
        private void TerminateDraw()
        {
            if (this.boardTexture != null)
            {
                this.boardTexture.Destroy();
                this.boardTexture = null;
            }

            if (this.pieceTexture != null)
            {
                this.pieceTexture.Destroy();
                this.pieceTexture = null;
            }

            if (this.pieceBoxTexture != null)
            {
                this.pieceBoxTexture.Destroy();
                this.pieceBoxTexture = null;
            }
        }

        /// <summary>
        /// OpenGL初期化後の描画関係の初期化を行います。
        /// </summary>
        public override void OnOpenGLInitialized(EventArgs e)
        {
            base.OnOpenGLInitialized(e);

            this.boardTexture = new Texture();
            this.pieceTexture = new Texture();
            this.pieceBoxTexture = new Texture();

            // イベントハンドラの設定後にテクスチャの実体化を行います。
            BoardBitmapUpdated(this, null);
            PieceBitmapUpdated(this, null);
            PieceBoxBitmapUpdated(this, null);
        }

        /// <summary>
        /// 盤などのサイズを設定します。
        /// </summary>
        private void InitializeBounds()
        {
            var w2 = 340 / 2;
            var h2 = 350 / 2;
            BoardBounds = new RectangleF(
                320 - w2, 180 - h2, w2 * 2, h2 * 2);

            // 駒の表示サイズを設定
            SquareSize = new SizeF(
                (float)(BoardBounds.Width / (Board.BoardSize + BoardBorderRate * 2)),
                (float)(BoardBounds.Height / (Board.BoardSize + BoardBorderRate * 2)));

            // 盤サイズの設定
            BoardSquareBounds = new RectangleF(
                (float)(BoardBounds.X + SquareSize.Width * BoardBorderRate),
                (float)(BoardBounds.Y + SquareSize.Height * BoardBorderRate),
                SquareSize.Width * Board.BoardSize,
                SquareSize.Height * Board.BoardSize);

            // 駒台のサイズ / 2
            w2 = 130 / 2;
            h2 = 170 / 2;

            // index=0が駒箱の駒となります。
            this.pieceBoxBounds[0] = new RectangleF(
                320 - 243 - w2, 360 - 5 - h2 * 2, w2 * 2, h2 * 2);
            this.pieceBoxBounds[1] = new RectangleF(
                320 + 243- w2, 360 - 5 - h2 * 2, w2 * 2, h2 * 2);
            this.pieceBoxBounds[2] = new RectangleF(
                320 - 243 - w2, 5, w2 * 2, h2 * 2);
        }

        /// <summary>
        /// 各駒の描画に使うメッシュを初期化します。
        /// </summary>
        private void InitializePieceMesh()
        {
            var pieceList =
                from bwType in EnumEx.GetValues<BWType>()
                from pieceType in EnumEx.GetValues<PieceType>()
                from promoted in new[] { false, true }
                let piece = new BoardPiece(pieceType, promoted, bwType)
                where piece.Validate()
                select piece;

            this.pieceMeshDic = pieceList
                .ToDictionary(_ => _, _ => CreatePieceMesh(_));
        }

        /// <summary>
        /// 駒の描画に使うメッシュを作成します。
        /// </summary>
        private Mesh CreatePieceMesh(BoardPiece piece)
        {
            var uv = GetPieceTextureUV(piece);

            return new Mesh(
                // 頂点配列
                new Point3d[]
                {
                    new Point3d(-0.5, -0.5, 0.0),
                    new Point3d(+0.5, -0.5, 0.0),
                    new Point3d(-0.5, +0.5, 0.0),
                    new Point3d(+0.5, +0.5, 0.0),
                },
                // テクスチャUV配列
                new Pointd[]
                {
                    new Pointd(uv.Left, uv.Top),
                    new Pointd(uv.Right, uv.Top),
                    new Pointd(uv.Left, uv.Bottom),
                    new Pointd(uv.Right, uv.Bottom),
                },
                // インデックス配列
                new int[]
                {
                    0, 2, 1,
                    1, 2, 3,
                });
        }

        /// <summary>
        /// 駒画像のUVを取得します。
        /// </summary>
        private RectangleF GetPieceTextureUV(BoardPiece piece)
        {
            // 先手番の駒か駒箱の駒なら、Yインデックスは0となります。
            var yIndex = (
                piece.BWType == BWType.None || piece.BWType == BWType.Black ?
                0 : 2);
            var tyi = yIndex + (!piece.IsPromoted ? 0 : 1);
            var txi = (int)piece.PieceType - 1;

            return new RectangleF(txi / 8.0f, tyi / 4.0f, 1.0f / 8.0f, 1.0f / 4.0f);
        }
        #endregion

        #region 見た目のプロパティ
        /// <summary>
        /// 時間の表示を行うかどうかを取得または設定します。
        /// </summary>
        public bool IsTimeVisible
        {
            get { return GetValue<bool>("IsTimeVisible"); }
            set { SetValue("IsTimeVisible", value); }
        }

        /// <summary>
        /// テクスチャを読み込みます。
        /// </summary>
        private void LoadTexture(Texture texture, Bitmap bitmap)
        {
            if (bitmap != null)
            {
                if (!texture.Create(bitmap))
                {
                    throw new RagnarokException(
                        "テクスチャの作成に失敗しました。");
                }
            }
            else
            {
                texture.Destroy();
            }
        }

        /// <summary>
        /// 駒画像を取得または設定します。
        /// </summary>
        public Bitmap PieceBitmap
        {
            get { return GetValue<Bitmap>("PieceBitmap"); }
            set { SetValue("PieceBitmap", value); }
        }

        /// <summary>
        /// PieceBitmapの更新時に呼ばれ、テクスチャの更新を行います。
        /// </summary>
        private void PieceBitmapUpdated(object sender, PropertyChangedEventArgs e)
        {
            if (this.pieceTexture != null)
            {
                LoadTexture(this.pieceTexture, PieceBitmap);
            }
        }

        /// <summary>
        /// 盤画像を取得または設定します。
        /// </summary>
        public Bitmap BoardBitmap
        {
            get { return GetValue<Bitmap>("BoardBitmap"); }
            set { SetValue("BoardBitmap", value); }
        }

        /// <summary>
        /// 盤画像の更新時に呼ばれ、テクスチャの更新を行います。
        /// </summary>
        private void BoardBitmapUpdated(object sender, PropertyChangedEventArgs e)
        {
            if (this.boardTexture != null)
            {
                LoadTexture(this.boardTexture, BoardBitmap);
            }
        }

        /// <summary>
        /// 駒台画像を取得または設定します。
        /// </summary>
        public Bitmap PieceBoxBitmap
        {
            get { return GetValue<Bitmap>("PieceBoxBitmap"); }
            set { SetValue("PieceBoxBitmap", value); }
        }

        /// <summary>
        /// 駒台画像の更新時に呼ばれ、テクスチャの更新を行います。
        /// </summary>
        private void PieceBoxBitmapUpdated(object sender, PropertyChangedEventArgs e)
        {
            if (this.pieceBoxTexture != null)
            {
                LoadTexture(this.pieceBoxTexture, PieceBoxBitmap);
            }
        }

        /// <summary>
        /// 駒箱を表示するかどうかを取得します。
        /// </summary>
        [DependOnProperty("EditMode")]
        public bool IsKomaBoxVisible
        {
            get { return (EditMode == EditMode.Editing); }
        }

        /// <summary>
        /// 盤と駒台画像の不透明度を取得または設定します。
        /// </summary>
        public double BoardOpacity
        {
            get { return GetValue<double>("BoardOpacity"); }
            set { SetValue("BoardOpacity", value); }
        }

        /// <summary>
        /// 手番側の対局者の名前の背景色を取得または設定します。
        /// </summary>
        public Color TebanPlayerNameBackgroundColor
        {
            get { return GetValue<Color>("TebanPlayerNameBackgroundColor"); }
            set { SetValue("TebanPlayerNameBackgroundColor", value); }
        }

        /// <summary>
        /// 手番ではない方の対局者の名前の背景色を取得または設定します。
        /// </summary>
        public Color UnTebanPlayerNameBackgroundColor
        {
            get { return GetValue<Color>("UnTebanPlayerNameBackgroundColor"); }
            set { SetValue("UnTebanPlayerNameBackgroundColor", value); }
        }

        /// <summary>
        /// 時間部分の背景色を取得または設定します。
        /// </summary>
        public Color TimeBackgroundColor
        {
            get { return GetValue<Color>("TimeBackgroundColor"); }
            set { SetValue("TimeBackgroundColor", value); }
        }

        /// <summary>
        /// 候補手を示す矢印の表示を行うかどうかを取得または設定します。
        /// </summary>
        public bool IsArrowVisible
        {
            get { return GetValue<bool>("IsArrowVisible"); }
            set { SetValue("IsArrowVisible", value); }
        }

        /// <summary>
        /// 画面上に矢印を表示するための候補手リストを取得または設定します。
        /// </summary>
        public IReadOnlyList<MoveArrowInfo> MoveArrowList
        {
            get { return GetValue<IReadOnlyList<MoveArrowInfo>>("MoveArrowList"); }
            set { SetValue("MoveArrowList", value); }
        }

        /// <summary>
        /// 自動再生時の色を取得または設定します。
        /// </summary>
        public Color AutoPlayColor
        {
            get { return GetValue<Color>("AutoPlayColor"); }
            set { SetValue("AutoPlayColor", value); }
        }

        /// <summary>
        /// 自動再生時のエフェクトの不透明度を取得または設定します。
        /// </summary>
        public double AutoPlayOpacity
        {
            get { return GetValue<double>("AutoPlayOpacity"); }
            set { SetValue("AutoPlayOpacity", value); }
        }

        /// <summary>
        /// 盤全体が描画される領域を取得します。
        /// </summary>
        public RectangleF BoardBounds
        {
            get;
            private set;
        }
        
        /// <summary>
        /// マス全体の領域を取得します。
        /// </summary>
        public RectangleF BoardSquareBounds
        {
            get;
            private set;
        }

        /// <summary>
        /// １マスのサイズを取得します。
        /// </summary>
        public SizeF SquareSize
        {
            get;
            private set;
        }
        #endregion

        #region エフェクトなどの追加・削除
        /// <summary>
        /// エフェクトを追加します。
        /// </summary>
        public void AddEffect(EffectObject effect)
        {
            if (effect == null)
            {
                return;
            }

            Children.Add(effect);
        }

        /// <summary>
        /// エフェクトを削除します。
        /// </summary>
        public void RemoveEffect(EffectObject effect)
        {
            if (effect == null)
            {
                return;
            }

            Children.Remove(effect);
        }

        /// <summary>
        /// 矢印表示用の指し手をクリアします。
        /// </summary>
        private void InitMoveArrowList()
        {
            MoveArrowList = new List<MoveArrowInfo>();
        }
        #endregion

        /// <summary>
        /// 各フレームごとに呼ばれます。
        /// </summary>
        protected override void OnEnterFrame(EnterFrameEventArgs e)
        {
            base.OnEnterFrame(e);
            var renderBuffer = (RenderBuffer)e.StateObject;
            
            if (IsVisible)
            {
                // 盤
                AddRenderBoard(renderBuffer);

                // 先手と後手の駒台と駒箱
                for (var index = 0; index < 3; ++index)
                {
                    AddRenderPieceBox(renderBuffer, index);
                }

                // 盤上の駒をすべて描画登録します。
                AddRenderPieceAll(renderBuffer);

                if (Math.Abs(AutoPlayOpacity) <= 1.0e-6)
                {
                    // 候補手となる指し手のリストを表示します。
                    AddRenderArrowMoveList(renderBuffer);
                }
                else
                {
                    // 自動再生時のエフェクトを描画します。
                    AddRenderAutoPlayEffect(renderBuffer);
                }
            }
        }

        /// <summary>
        /// 盤の描画を行います。
        /// </summary>
        private void AddRenderBoard(RenderBuffer renderBuffer)
        {
            // 盤
            renderBuffer.AddRender(
                this.boardTexture, BlendType.Diffuse,
                BoardBounds, Transform,
                ShogiZOrder.BoardZ, BoardOpacity);

            // 上部に描画する盤の符号の領域
            var totalBounds = RectangleF.FromLTRB(
                BoardSquareBounds.Left,
                BoardBounds.Top,
                BoardSquareBounds.Right,
                BoardSquareBounds.Top); // Topはミスではありません。
            var w = totalBounds.Width / Board.BoardSize;
            for (int n = 1; n <= Board.BoardSize; ++n)
            {
                // 符号を描画する領域
                var bounds = new RectangleF(
                    totalBounds.Left + w * (n - 1),
                    totalBounds.Top,
                    w,
                    totalBounds.Height);
                bounds.Inflate(0, -2.5f);
                var str = IntConverter.Convert(
                    NumberType.Big,
                    ViewSide == BWType.Black ? 10 - n : n);
                AddRenderText(
                    renderBuffer, str, this.boardSignFont,
                    bounds, ShogiZOrder.BoardZ);
            }

            // 右側に描画する盤の符号の領域
            totalBounds = RectangleF.FromLTRB(
                BoardSquareBounds.Right, // Rightはミスではありません。
                BoardSquareBounds.Top,
                BoardBounds.Right,
                BoardSquareBounds.Bottom);
            var h = totalBounds.Height / Board.BoardSize;
            for (int n = 1; n <= Board.BoardSize; ++n)
            {
                // 符号を描画する領域
                var bounds = new RectangleF(
                    totalBounds.Left,
                    totalBounds.Top + h * (n - 1),
                    totalBounds.Width,
                    w);
                bounds.Inflate(-1.5f, 0);
                var str = IntConverter.Convert(
                    NumberType.Kanji,
                    ViewSide == BWType.Black ? n : 10 - n);
                AddRenderText(
                    renderBuffer, str, this.boardSignFont,
                    bounds, ShogiZOrder.BoardZ);
            }
        }

        /// <summary>
        /// indexに対応した駒台を描画します。
        /// </summary>
        /// <param name="index">0なら駒箱、1なら先手用、2なら後手用の駒台となります。</param>
        private void AddRenderPieceBox(RenderBuffer renderBuffer, int index)
        {
            // テクスチャがないときは帰ります。
            if (this.pieceTexture == null || this.pieceTexture.TextureName == 0)
            {
                return;
            }

            // 駒箱の場合はキャンセルするかもしれません
            if (index == 0 && !IsKomaBoxVisible)
            {
                return;
            }

            // 盤面が反転している場合は、見た目の先後を入れ替えます。
            var viewIndex = (
                ViewSide != BWType.Black ? 
                (index == 0 ? 0 : index == 1 ? 2 : 1) :
                index);
            var pieceBoxBounds = this.pieceBoxBounds[viewIndex];

            // 駒箱テクスチャ
            renderBuffer.AddRender(
                this.pieceBoxTexture, BlendType.Diffuse,
                pieceBoxBounds, Transform,
                ShogiZOrder.BoardZ,  BoardOpacity);

            // 駒台の上に対局者名を描画します。
            {
                var y = (viewIndex == 2 ?
                    pieceBoxBounds.Bottom - 5 - 15 :
                    pieceBoxBounds.Top + 5);
                var bounds = new RectangleF(
                    pieceBoxBounds.Left + 5, y,
                    pieceBoxBounds.Width - 10, 15);

                // 名前の背景に色をつけます。
                var color = (
                    Board.Turn == (BWType)index ?
                    TebanPlayerNameBackgroundColor :
                    UnTebanPlayerNameBackgroundColor);
                renderBuffer.AddRender(
                    BlendType.Diffuse, bounds, Transform,
                    color, ShogiZOrder.PostBoardZ);

                // 対局者名を描画
                var name = (
                    index == 1 ? BlackPlayerName :
                    index == 2 ? WhitePlayerName :
                    "駒箱");
                if (name.HankakuLength() > 17)
                {
                    name = name.HankakuSubstring(14) + "...";
                }
                bounds.Inflate(-1, -1);
                AddRenderText(
                    renderBuffer, name, this.nameFont,
                    bounds, ShogiZOrder.PostBoardZ);
            }

            // 合計時間や消費時間の描画を行います。
            // 局面編集中など駒箱が表示されているときは残り時間を表示しません。
            if (IsTimeVisible && !IsKomaBoxVisible)
            {
                var y = (viewIndex == 2 ?
                    pieceBoxBounds.Bottom :
                    pieceBoxBounds.Top - 15);
                var bounds = new RectangleF(
                    pieceBoxBounds.Left, y,
                    pieceBoxBounds.Width, 15);
                renderBuffer.AddRender(
                    BlendType.Diffuse, bounds, Transform,
                    TimeBackgroundColor, ShogiZOrder.PostBoardZ);

                // 消費時間などを描画
                // 時間のフォーマットは '消費時間 / 合計時間' となっています。
                var totalTime = (index == 1 ? BlackTotalTime : WhiteTotalTime);
                var time = (index == 1 ? BlackTime : WhiteTime);
                var str = string.Format(
                    "{0:000}:{1:00} / {2:000}:{3:00}",
                    (int)time.TotalMinutes, time.Seconds,
                    (int)totalTime.TotalMinutes, totalTime.Seconds);
                bounds.Inflate(-4, -1);
                AddRenderText(
                    renderBuffer, str, this.timeFont,
                    bounds, ShogiZOrder.PostBoardZ);
            }
        }

        /// <summary>
        /// 盤上の全ての駒を描画します。
        /// </summary>
        private void AddRenderPieceAll(RenderBuffer renderBuffer)
        {
            if (this.pieceTexture == null || !this.pieceTexture.IsAvailable)
            {
                return;
            }

            var mp = this.movingPiece;
            var mpSquare = (mp != null ? mp.Square : null);
            var mpPiece = (mp != null ? mp.BoardPiece : null);

            // 盤上の駒
            foreach (var sq in Board.AllSquares())
            {
                var piece = Board[sq];
                if (piece != null && sq != mpSquare)
                {
                    var cpos = SquareToPoint(sq);
                    AddRenderPiece(
                        renderBuffer, piece, 1, cpos,
                        ShogiZOrder.PieceZ);
                }
            }

            // 先手・後手の駒台上の駒を更新します。
            var handList =
                from bw in EnumEx.GetValues<BWType>()
                from pc in EnumEx.GetValues<PieceType>()
                where bw != BWType.None || IsKomaBoxVisible
                where pc != PieceType.None
                let piece = new BoardPiece(pc, bw)
                let count = GetHandCount(pc, bw)
                let subCount = (mpSquare == null && mpPiece == piece ? 1 : 0)
                //where count - subCount > 0
                select new
                {
                    Piece = piece,
                    Count = count - subCount,
                };
            foreach (var hand in handList)
            {
                var cpos = HandPieceToPoint(hand.Piece);
                AddRenderPiece(
                    renderBuffer, hand.Piece, hand.Count, cpos,
                    ShogiZOrder.PieceZ);
            }

            // 移動中の駒を描画します。
            if (mp != null)
            {
                AddRenderPiece(
                    renderBuffer, mp.BoardPiece, 1, mp.Center,
                    ShogiZOrder.MovingPieceZ);
            }
        }

        /// <summary>
        /// 駒の描画を行います。
        /// </summary>
        private void AddRenderPiece(RenderBuffer renderBuffer, BoardPiece piece,
                                    int count, PointF cpos, double zorder)
        {
            if (this.pieceTexture == null || !this.pieceTexture.IsAvailable)
            {
                return;
            }

            if (count <= 0)
            {
                return;
            }

            var s = SquareSize;
            var bounds = new RectangleF(
                cpos.X - s.Width / 2, cpos.Y - s.Height / 2,
                s.Width, s.Height);

            // 駒自体の描画を行います。
            renderBuffer.AddRender(
                this.pieceTexture, BlendType.Diffuse,
                bounds, Transform, GetPieceMesh(piece), zorder);

            // 必要なら持ち駒の数も描画します。
            if (count >= 2)
            {
                var text = IntConverter.Convert(NumberType.Big, count);
                bounds = new RectangleF(
                    cpos.X - s.Width * 0.1f, cpos.Y - s.Height * 0.6f,
                    s.Width * 0.8f, s.Height * 0.5f);
                AddRenderText(
                    renderBuffer, text, this.pieceCountFont,
                    bounds, zorder + 0.05);
            }
        }

        /// <summary>
        /// 駒を描画するためのメッシュを取得します。
        /// </summary>
        private Mesh GetPieceMesh(BoardPiece piece)
        {
            if (piece == null || !piece.Validate())
            {
                throw new ArgumentException("piece");
            }

            // 見た目が後手番中心の場合は、手番を反転します。
            if (ViewSide == BWType.White)
            {
                piece = new BoardPiece(piece.Piece, piece.BWType.Flip());
            }

            Mesh mesh;
            if (!this.pieceMeshDic.TryGetValue(piece, out mesh))
            {
                return null;
            }

            return mesh;
        }

        /// <summary>
        /// 候補手を矢印による描画を行います。
        /// </summary>
        private void AddRenderArrowMoveList(RenderBuffer renderBuffer)
        {
            // オブジェクトが変更される可能性があるので、念のためキャッシュしておきます。
            var moveArrowList = MoveArrowList;
            if (moveArrowList == null || !IsArrowVisible)
            {
                return;
            }

            // 矢印が被ることがあるため、優先順位が低いものから描画する。
            var list = moveArrowList
                .SelectWithIndex((info, i) => new { info, priority = i + 1 })
                .Where(_ => _.info != null && _.info.Move != null)
                .GroupBy(_ => _.info.Move)
                .Select(_ => new
                {
                    info = _.First().info,
                    lowestPriority = _.Min(__ => __.priority),
                    //priorities = _.Select(__ => __.priority).ToArray(),
                })
                .OrderBy(_ => _.lowestPriority)
                .ToArray();

            var needLabel = list.Count() > 1;
            list.ForEach(_ =>
            {
                //var label = string.Join(",", _.priorities).ToString();
                var label = needLabel ? _.lowestPriority.ToString() : null;
                AddRenderMoveArrow(renderBuffer, _.info, _.lowestPriority, label);
            });
        }

        /// <summary>
        /// 指し手を表示する矢印メッシュを追加します。
        /// </summary>
        private void AddRenderMoveArrow(RenderBuffer renderBuffer,
                                        MoveArrowInfo info,
                                        int priority, string label)
        {
            var move = info.Move;
            if (move == null || !move.Validate())
            {
                return;
            }

            // 手番が違う場合は、前に設定された指し手が残っている可能性がある。
            if (move.BWType != Board.Turn || move.IsSpecialMove)
            {
                return;
            }

            // 駒の移動を開始した地点と終了した地点の座標を求めます。
            var fromPoint = (
                move.ActionType == ActionType.Drop ?
                HandPieceToPoint(move.DropPieceType, move.BWType) :
                SquareToPoint(move.SrcSquare)).ToPointd();
            var toPoint = SquareToPoint(move.DstSquare).ToPointd();
            var diff = toPoint - fromPoint;

            // 優先順位の高い矢印ほど、小さくなる値
            var priorityRate = MathEx.Between(0.0, 1.0, 1.0 / 3.0 * (priority - 1));

            // 矢印を決められた位置に描画します。
            AddRenderArrow(renderBuffer, fromPoint, toPoint, priorityRate, info.Color);

            // ラベルを描画
            if (!string.IsNullOrEmpty(label))
            {
                var font = new TextTextureFont
                {
                    Font = new Font(TextTextureFont.DefaultFont, FontStyle.Bold),
                    Color = Color.White,
                    EdgeColor = Color.Black,
                    EdgeLength = 4,
                    IsStretchSize = true,
                };
                var rect = new RectangleF(
                    (float)(fromPoint.X + diff.X * 1),
                    (float)(fromPoint.Y + diff.Y * 1),
                    SquareSize.Width / 3,
                    SquareSize.Height / 3);
                rect.Offset(-rect.Width / 2, -rect.Height / 2);

                AddRenderText(
                    renderBuffer, label, font,
                    rect, ShogiZOrder.PreEffectZ2 - priorityRate);
            }
        }

        private void AddRenderArrow(RenderBuffer renderBuffer,
                                    Pointd fromPoint, Pointd toPoint,
                                    double priorityRate, Color color)
        {
            var diff = toPoint - fromPoint;
            var length = diff.Distance;
            var lengthMax = SquareSize.Width * 5.0;

            // 距離が近いものほど小さくなる
            var lengthRate = Math.Min(length, lengthMax) / lengthMax;
            
            var rad = Math.Atan2(diff.Y, diff.X);
            var transform = new Matrix44d();
            transform.Translate(fromPoint.X, fromPoint.Y, 1.0);
            transform.Rotate(rad - Math.PI / 2, 0, 0, 1);
            transform.Scale(
                SquareSize.Width *
                    MathEx.InterpLiner(0.8, 0.8, lengthRate) *
                    MathEx.InterpLiner(0.8, 0.2, priorityRate),
                length, 1.0);

            // 矢印の不透明度を更新
            var newColor = Color.FromArgb(ArrowAlpha(priorityRate), color);
            
            // 矢印の中身を描画
            renderBuffer.AddRender(
                BlendType.Diffuse, newColor,
                CreateArrowMesh(length, priorityRate, false),
                transform, ShogiZOrder.PreEffectZ - priorityRate);

            // 矢印のアウトラインを描画
            renderBuffer.AddRenderAction(
                () =>
                {
                    GL.Color4(newColor.R, newColor.G, newColor.B, (byte)(newColor.A + 50));
                    GL.LineWidth(0.5f);
                    GL.LoadMatrix(transform.AsColumnMajorArray);

                    var mesh2 = CreateArrowMesh(length, priorityRate, true);
                    GL.Begin(BeginMode.LineLoop);
                        mesh2.VertexArray.ForEach(_ => GL.Vertex3(_.X, _.Y, _.Z));
                    GL.End();
                },
                ShogiZOrder.PreEffectZ - priorityRate);
        }

        private Mesh CreateArrowMesh(double length, double priorityRate, bool outline)
        {
            return MeshUtil.CreateArrow(
                SquareSize.Width / length *
                    MathEx.InterpLiner(0.6, 0.2, priorityRate),
                0.7, 0.05, 0.05, outline);
        }

        private int ArrowAlpha(double priorityRate)
        {
            return (int)MathEx.InterpLiner(110, 20, priorityRate);
        }

        /// <summary>
        /// 自動再生用のエフェクト描画を行います。
        /// </summary>
        private void AddRenderAutoPlayEffect(RenderBuffer renderBuffer)
        {
            if (AutoPlayOpacity <= 0.0)
            {
                return;
            }

            var bounds = new RectangleF(0.0f, 0.0f, 640.0f, 480.0f);
            var alpha = (byte)(AutoPlayColor.A * AutoPlayOpacity);

            renderBuffer.AddRender(
                BlendType.Diffuse, bounds, Transform,
                Color.FromArgb(alpha, AutoPlayColor), ShogiZOrder.PostEffectZ2);
        }

        /// <summary>
        /// 文字列の描画を行います。
        /// </summary>
        private void AddRenderText(RenderBuffer renderBuffer, string text,
                                   TextTextureFont font, RectangleF bounds,
                                   double zorder, double opacity = 1.0)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var textTexture = TextureCache.GetTextTexture(text, font);
            if (textTexture == null || textTexture.Texture == null)
            {
                // エラーだけどどうしようか。
                return;
            }

            var texture = textTexture.Texture;
            var r = (float)texture.OriginalWidth / texture.OriginalHeight;
            var br = (float)bounds.Width / bounds.Height;
            var w = (r >= br ? bounds.Width : bounds.Height * r);
            var h = (r >= br ? bounds.Width / r : bounds.Height);

            var result = new RectangleF(
                (bounds.Left + bounds.Right) / 2 - w / 2,
                (bounds.Top + bounds.Bottom) / 2 - h / 2,
                w, h);
            renderBuffer.AddRender(
                texture, BlendType.Diffuse,
                result, Transform, zorder, 1.0);
        }
    }
}
