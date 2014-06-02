﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

using Ragnarok.Shogi;
using Ragnarok.ObjectModel;
using Ragnarok.Presentation.Control;
using Ragnarok.Presentation.VisualObject;

namespace Ragnarok.Presentation.Shogi.Effects
{
    using View;

    /// <summary>
    /// 各駒を表示します。
    /// </summary>
    public sealed class PieceObject : EntityObject
    {
        public static readonly DependencyProperty IsAlwaysVisibleProperty =
            DependencyProperty.Register(
                "IsAlwaysVisible", typeof(bool), typeof(PieceObject),
                new UIPropertyMetadata(true, OnElementChanged));

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(
                "Position", typeof(Position), typeof(PieceObject),
                new UIPropertyMetadata(default(Position), OnElementChanged));

        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register(
                "Count", typeof(int), typeof(PieceObject),
                new UIPropertyMetadata(0, OnElementChanged));

        private readonly ShogiUIElement3D shogi;
        private DecoratedText numberText;

        /// <summary>
        /// 内部のモデルオブジェクトを取得します。
        /// </summary>
        public BoardPiece Piece
        {
            get;
            private set;
        }

        /// <summary>
        /// 常に駒を表示するかどうかを取得または設定します。
        /// </summary>
        /// <remarks>
        /// 駒台の駒は必要なときにしか表示しません。
        /// </remarks>
        public bool IsAlwaysVisible
        {
            get { return (bool)GetValue(IsAlwaysVisibleProperty); }
            set { SetValue(IsAlwaysVisibleProperty, value); }
        }

        /// <summary>
        /// 駒のある位置を取得または設定します。
        /// </summary>
        public Position Position
        {
            get { return (Position)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        /// <summary>
        /// 駒台の駒の数を取得または設定します。
        /// </summary>
        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        static void OnElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as PieceObject;

            if (self != null)
            {
                self.UpdateElement();
            }
        }

        /// <summary>
        /// 駒描画用のイメージブラシを作成します。
        /// </summary>
        public static ImageBrush CreateBrush(ShogiUIElement3D shogi, BoardPiece piece)
        {
            if (piece == null || piece.PieceType == PieceType.None)
            {
                return null;
            }

            var y = (piece.BWType == shogi.ViewSide ? 0 : 2) + (!piece.IsPromoted ? 0 : 1);
            var x = (int)piece.PieceType - 1;

            // 画像にはすべての駒の絵が入っています。
            return new ImageBrush(shogi.PieceImage)
            {
                ViewboxUnits = BrushMappingMode.RelativeToBoundingBox,
                Viewbox = new Rect(x / 8.0, y / 4.0, 1.0 / 8.0, 1.0 / 4.0),
                Stretch = Stretch.Fill,
            }.Apply(_ => _.Freeze());
        }

        /// <summary>
        /// 駒の表示用モデルを作成します。
        /// </summary>
        private GeometryModel3D CreatePieceModel()
        {
            var brush = CreateBrush(this.shogi, Piece);
            if (brush == null)
            {
                return null;
            }

            var image = brush.ImageSource;
            if (image == null)
            {
                return null;
            }

            return new GeometryModel3D
            {
                Geometry = WPFUtil.CreateDefaultMesh(
                    1.0, 1.0, image.Width, image.Height),
                Material = new DiffuseMaterial(brush),
            }.Apply(_ => _.Freeze());
        }

        /// <summary>
        /// モデルの中身を更新します。
        /// </summary>
        private void UpdateModel()
        {
            // 表示用の駒オブジェクトを追加します。
            var model = CreatePieceModel();
            if (model != null)
            {
                ModelGroup.Children.Add(model);
            }

            // 駒台の駒の場合
            if (Position == null)
            {
                // 駒数の表示用オブジェクトを追加します。
                var numberModel = new GeometryModel3D()
                {
                    Geometry = WPFUtil.CreateDefaultMesh(0.7, 0.4, 0, 0),
                    Material = new DiffuseMaterial(new VisualBrush(this.numberText)),
                    Transform = new TranslateTransform3D(0.4, -0.4, 0.0),
                };
                ModelGroup.Children.Add(numberModel);
            }
        }

        /// <summary>
        /// 駒オブジェクトなどを初期化します。
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();

            BaseScale = new Vector3D(
                this.shogi.CellSize.Width,
                this.shogi.CellSize.Height,
                1.0);

            // 駒台の駒の場合
            if (Position == null)
            {
                this.numberText = new DecoratedText()
                {
                    TextFormat = "×{0}",
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.ExtraBlack,
                    Stroke = Brushes.White,
                    StrokeThickness = 0.8,
                };
            }

            UpdateModel();
            UpdateElement();
        }

        /// <summary>
        /// 駒台上の駒の数や表示・非表示などを設定します。
        /// </summary>
        private void UpdateElement()
        {
            var isVisible = (IsAlwaysVisible || Count > 0);
            if (!isVisible)
            {
                ModelGroup.Children.Clear();
            }
            else
            {
                // 必要ならモデルを追加します。
                if (!ModelGroup.Children.Any())
                {
                    UpdateModel();
                }

                if (this.numberText != null)
                {
                    // 非表示時にCollapsedではなくHiddenを指定しないと表示時でも数字が表示
                    // されないことがあります。原因は不明。
                    this.numberText.Visibility =
                        (Count > 1 ? Visibility.Visible : Visibility.Hidden);
                    this.numberText.Text = Count.ToString();
                }
            }
        }

        /// <summary>
        /// 駒を文字列化します。
        /// </summary>
        public override string ToString()
        {
            if (Piece == null)
            {
                return string.Empty;
            }

            return Piece.ToString();
        }

        protected override Freezable CreateInstanceCore()
        {
            return new PieceObject(this.shogi, Piece, Position);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PieceObject(ShogiUIElement3D shogi, BoardPiece piece, Position position)
        {
            this.shogi = shogi;

            IsFastInitialize = true;
            Piece = piece;
            Position = position;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PieceObject(ShogiUIElement3D shogi, BoardPiece piece)
            : this(shogi, piece, null)
        {
        }
    }
}
