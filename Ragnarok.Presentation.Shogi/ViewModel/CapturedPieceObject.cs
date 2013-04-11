using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Ragnarok;
using Ragnarok.Shogi;
using Ragnarok.Presentation.Control;

namespace VoteSystem.PluginShogi.ViewModel
{
    using View;

    /// <summary>
    /// 駒台にある駒を表示します。
    /// </summary>
    public class CapturedPieceObject : EntityObject
    {
        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register(
                "Count", typeof(int), typeof(CapturedPieceObject),
                new UIPropertyMetadata(0, OnCountChanged));
        
        private ShogiControl shogi;
        private DecoratedText numberText;

        /// <summary>
        /// 表示用のビジュアル要素を取得します。
        /// </summary>
        public ModelUIElement3D Element
        {
            get;
            private set;
        }

        /// <summary>
        /// 駒の種別を取得します。
        /// </summary>
        public PieceType PieceType
        {
            get;
            private set;
        }

        /// <summary>
        /// 先手／後手を取得します。
        /// </summary>
        public BWType BWType
        {
            get;
            private set;
        }

        /// <summary>
        /// 駒の数を取得または設定します。
        /// </summary>
        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        static void OnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as CapturedPieceObject;

            if (self != null)
            {
                self.UpdateElement();
            }
        }        

        /// <summary>
        /// 駒オブジェクトなどを初期化します。
        /// </summary>
        protected override void InitializeImpl()
        {
            base.InitializeImpl();

            // 駒はマウスクリックを識別したいので、
            // ModelUIElement3Dクラスを使います。
            Element = new ModelUIElement3D()
            {
                Model = ModelGroup,
            };

            // 表示用の駒オブジェクトを追加します。
            var pieceObject = new PieceObject(
                this.shogi, new BoardPiece(BWType, PieceType));
            AddChild(pieceObject);

            this.numberText = new DecoratedText()
            {
                TextFormat = "×{0}",
                Foreground = Brushes.Black,
                FontWeight = FontWeights.ExtraBlack,
                Stroke = Brushes.White,
                StrokeThickness = 0.8,
            };

            // 駒数の表示用オブジェクトを追加します。
            var w = this.shogi.CellSize.Width;
            var h = this.shogi.CellSize.Height;

            var numberModel = new GeometryModel3D()
            {
                Geometry = CreateDefaultMesh(0.7, 0.4),
                Material = new DiffuseMaterial(new VisualBrush(this.numberText)),
                Transform = new Transform3DGroup()
                    .Apply(_ => _.Children.Add(new TranslateTransform3D(0.4, -0.4, 0.0)),
                           _ => _.Children.Add(new ScaleTransform3D(w, h, 1.0))),
            };
            ModelGroup.Children.Add(numberModel);

            UpdateElement();
        }

        /// <summary>
        /// 駒台上の駒の数や表示・非表示などを設定します。
        /// </summary>
        private void UpdateElement()
        {
            if (Element == null || this.numberText == null)
            {
                return;
            }

            var count = Count;
            Element.Visibility =
                (Count > 0 ? Visibility.Visible : Visibility.Hidden);
            Element.IsHitTestVisible = (Count > 0);

            // 非表示時にCollapsedではなくHiddenを指定しないと表示時でも数字が表示
            // されないことがある。原因は不明。
            this.numberText.Visibility =
                (count > 1 ? Visibility.Visible : Visibility.Hidden);
            this.numberText.Text = count.ToString();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CapturedPieceObject(ShogiControl shogi, BWType bwType, PieceType pieceType)
        {
            this.shogi = shogi;

            PieceType = pieceType;
            BWType = bwType;
            Count = 0;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new CapturedPieceObject(this.shogi, BWType, PieceType)
            {
                Count = Count,
            };
        }
    }
}
