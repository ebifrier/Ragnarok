using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ragnarok.Utility;

namespace Live2DSharp.Framework
{
    /// <summary>
    /// Live2Dのモデルに適用すると便利な行列クラスです。
    /// </summary>
    public sealed class L2DModelMatrix
    {
        private Matrix44d matrix = new Matrix44d();
        private double modelWidth;
        private double modelHeight;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public L2DModelMatrix(double modelWidth, double modelHeight)
        {
            // 縦横比を固定するために使います。
            this.modelWidth = modelWidth;
            this.modelHeight = modelHeight;

            // 原点(0,0)を中心にして、画面に収まるような大きさで初期化。
            if (modelWidth > modelHeight)
            {
                SetHeight(modelHeight / modelWidth);
            }
            else
            {
                SetWidth(2.0);
            }

            SetCenterX(0.0);
            SetCenterY(0.0);
        }

        /// <summary>
        /// 作成したモデル用の変換行列を取得します。
        /// </summary>
        public Matrix44d Transform
        {
            get { return this.matrix.Clone(); }
        }

        /// <summary>
        /// モデルの中心を与えられた座標に設定します。
        /// </summary>
        public void SetCenterPosition(double x, double y)
        {
            SetCenterX(x);
            SetCenterY(y);
        }

        /// <summary>
        /// モデルの左端位置を設定します。
        /// </summary>
        public void SetLeft(double left)
        {
            this.matrix[0, 3] = left;
        }

        /// <summary>
        /// モデルの上端位置を設定します。
        /// </summary>
        public void SetTop(double top)
        {
            this.matrix[1, 3] = top;
        }

        /// <summary>
        /// モデルの右端位置を設定します。
        /// </summary>
        public void SetRight(double right)
        {
            var w = this.modelWidth * this.matrix[0, 0];

            this.matrix[0, 3] = right - w;
        }

        /// <summary>
        /// モデルの下端位置を設定します。
        /// </summary>
        public void SetBottom(double bottom)
        {
            var h = this.modelHeight * this.matrix[1, 1];

            this.matrix[1, 3] = bottom - h;
        }

        /// <summary>
        /// モデルの中心を指定のＸ座標に設定します。
        /// </summary>
        public void SetCenterX(double x)
        {
            var w = this.modelWidth * this.matrix[0, 0];

            this.matrix[0, 3] = x - w / 2;
        }

        /// <summary>
        /// モデルの中心を指定のＹ座標に設定します。
        /// </summary>
        public void SetCenterY(double y)
        {
            var h = this.modelHeight * this.matrix[1, 1];

            this.matrix[1, 3] = y - h / 2;
        }

        /// <summary>
        /// 縦横比をそのままにしてモデルの新たな幅を設定します。
        /// </summary>
        public void SetWidth(double width)
        {
            var scaleX = width / this.modelWidth;
            var scaleY = -scaleX;

            this.matrix[0, 0] = scaleX;
            this.matrix[1, 1] = scaleY;
        }

        /// <summary>
        /// 縦横比をそのままにしてモデルの新たな高さを設定します。
        /// </summary>
        public void SetHeight(double height)
        {
            var scaleY = height / this.modelHeight;
            var scaleX = -scaleY;

            this.matrix[0, 0] = scaleX;
            this.matrix[1, 1] = scaleY;
        }

        /// <summary>
        /// レイアウトを設定します。
        /// </summary>
        public void SetupLayout(L2DLayoutData layout)
        {
            if (layout.Width != null)
            {
                SetWidth(layout.Width.Value);
            }

            if (layout.Height != null)
            {
                SetHeight(layout.Height.Value);
            }

            if (layout.CenterX != null)
            {
                SetCenterX(layout.CenterX.Value);
            }

            if (layout.CenterY != null)
            {
                SetCenterY(layout.CenterY.Value);
            }

            if (layout.X != null)
            {
                SetLeft(layout.X.Value);
            }

            if (layout.Y != null)
            {
                SetTop(layout.Y.Value);
            }

            if (layout.Left != null)
            {
                SetLeft(layout.Left.Value);
            }

            if (layout.Top != null)
            {
                SetTop(layout.Top.Value);
            }

            if (layout.Right != null)
            {
                SetRight(layout.Right.Value);
            }

            if (layout.Bottom != null)
            {
                SetBottom(layout.Bottom.Value);
            }
        }
    }
}
