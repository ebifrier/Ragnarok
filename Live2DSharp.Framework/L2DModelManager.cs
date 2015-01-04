using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;

using Live2DSharp;
using Live2DSharp.Framework;

using Ragnarok;
using Ragnarok.Extra.Effect;
using Ragnarok.ObjectModel;
using Ragnarok.Utility;

namespace Live2DSharp.Framework
{
    /// <summary>
    /// Live2D用のモデルクラスの管理を行います。
    /// </summary>
    public class L2DModelManager : NotifyObject
    {
        public static readonly string HIT_AREA_NAME_HEAD = "head";
        public static readonly string HIT_AREA_NAME_BODY = "body";

        private const float VIEW_LOGICAL_LEFT = -1;
        private const float VIEW_LOGICAL_RIGHT = +1;

        private readonly List<L2DModel> models = new List<L2DModel>();
        private Matrix44d viewMatrix = new Matrix44d();
        private Matrix44d deviceToLogical = new Matrix44d();

        /// <summary>
        /// カメラ行列の初期化を行います。
        /// </summary>
        public L2DModelManager(OpenGL gl)
        {
            OpenGL = gl;
        }

        /// <summary>
        /// OpenGL用のオブジェクトを取得します。
        /// </summary>
        public OpenGL OpenGL
        {
            get;
            private set;
        }

        /// <summary>
        /// 画面全体の幅を取得します。
        /// </summary>
        public double DeviceWidth
        {
            get;
            private set;
        }

        /// <summary>
        /// 画面全体の高さを取得します。
        /// </summary>
        public double DeviceHeight
        {
            get;
            private set;
        }

        /// <summary>
        /// SE再生用のオブジェクトを取得または設定します。
        /// </summary>
        public IEffectSoundManager SoundManager
        {
            get;
            set;
        }

        /// <summary>
        /// モデルの追加を行います。
        /// </summary>
        public void AddModel(L2DModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            using (LazyLock())
            {
                model.ModelManager = this;

                this.models.Add(model);
            }
        }

        /// <summary>
        /// モデルをすべて解放します。
        /// </summary>
        public void ClearModels()
        {
            using (LazyLock())
            {
                this.models.Clear();
            }
        }

        /// <summary>
        /// 画面全体のサイズを設定します。
        /// </summary>
        public void SetDeviceSize(double width, double height)
        {
            if (width == 0)
            {
                throw new ArgumentException("width");
            }

            if (height == 0)
            {
                throw new ArgumentException("width");
            }

            using (LazyLock())
            {
                // デバイスに対応する画面の範囲を、(-1,+1)-(+1,-1)からなる
                // Live2Dの論理座標に変換するための行列です。
                var screenW = Math.Abs(VIEW_LOGICAL_RIGHT - VIEW_LOGICAL_LEFT);
                this.deviceToLogical = new Matrix44d();
                this.deviceToLogical.Scale(screenW / width, -screenW / width, 1.0);
                this.deviceToLogical.Translate(-width / 2.0, -height / 2.0, 0.0);

                DeviceWidth = width;
                DeviceHeight = height;
            }
        }

        /// <summary>
        /// 画面全体の中で、モデルを表示させる位置を設定します。
        /// </summary>
        /// <remarks>
        /// Live2Dの論理座標と外の座標系を合わせるため、
        /// モデルの移動には専用のカメラ行列を使用します。
        /// </remarks>
        public void SetViewport(double left, double top, double width,
                                double height)
        {
            if (DeviceWidth == 0 || DeviceHeight == 0)
            {
                throw new InvalidOperationException(
                    "DeviceSizeを設定してください。");
            }

            using (LazyLock())
            {
                var deviceLogicalWidth = DeviceWidth / width;
                var deviceLogicalHeight = DeviceHeight / height;

                // モデルの表示位置を固定化させます。
                this.viewMatrix = new Matrix44d();
                this.viewMatrix.Scale(
                    width / DeviceWidth, height / DeviceHeight, 1.0);
                this.viewMatrix.Translate(
                    -deviceLogicalWidth + (left / width + 1.0),
                    deviceLogicalHeight - (top / height + 1.0),
                    0.0);

                UpdateModelMatrix();
            }
        }

        /// <summary>
        /// モデル行列の更新を行います。
        /// </summary>
        private void UpdateModelMatrix()
        {
            using (LazyLock())
            {
                // モデルの縦横比とカメラによる描画位置を設定します。
                var projection = new Matrix44d();
                //projection.Scale(1.0, DeviceWidth / DeviceHeight, 1.0);
                projection.Multiply(this.viewMatrix);

                this.models.ForEach(_ => _.UpdateMatrix(projection));
            }
        }

        /// <summary>
        /// モデル行列の更新を行います。
        /// </summary>
        public void UpdateModelMatrix(Matrix44d projection)
        {
            if (projection == null)
            {
                throw new ArgumentNullException("projection");
            }

            using (LazyLock())
            {
                this.models.ForEach(_ => _.UpdateMatrix(projection));
            }
        }

        /// <summary>
        /// 顔を向ける座標値を設定します。
        /// </summary>
        public void SetTarget(double x, double y)
        {
            using (LazyLock())
            {
                this.models.ForEach(_ => _.SetTarget(x, y));
            }
        }

        /// <summary>
        /// タップ処理を行います。
        /// </summary>
        public void OnTap(double x, double y)
        {
            using (LazyLock())
            {
                this.models.ForEach(_ =>
                {
                    if (_.HitTest(HIT_AREA_NAME_HEAD, x, y))
                    {
                        _.SetRandomExpression();
                    }
                    else if (_.HitTest(HIT_AREA_NAME_BODY, x, y))
                    {
                        _.StartRandomMotion("flick_head", Priority.Normal);
                    }
                });
            }
        }

        /// <summary>
        /// モデルの更新処理を行います。
        /// </summary>
        public void Update(TimeSpan elapsed)
        {
            using (LazyLock())
            {
                this.models.ForEach(_ => _.Update(elapsed));
            }
        }

        /// <summary>
        /// 描画処理を行います。
        /// </summary>
        public void Render()
        {
            var gl = OpenGL;

            using (LazyLock())
            {
                try
                {
                    DrawProfileCocos2D.preDraw();

                    gl.PushAttrib(OpenGL.GL_ALL_ATTRIB_BITS);

                    //gl.MatrixMode(OpenGL.GL_PROJECTION);
                    //gl.PushMatrix();
                    //gl.MatrixMode(OpenGL.GL_MODELVIEW);
                    //gl.PushMatrix();

                    this.models.ForEach(_ => _.Render());
                }
                finally
                {
                    //gl.PopMatrix();
                    //gl.MatrixMode(OpenGL.GL_PROJECTION);
                    //gl.PopMatrix();
                    //gl.MatrixMode(OpenGL.GL_MODELVIEW);

                    gl.PopAttrib();

                    DrawProfileCocos2D.postDraw();
                }
            }
        }

        /// <summary>
        /// デバイス座標をビュー上の座標に変換します。
        /// </summary>
        public Pointd DeviceToView(Pointd deviceP)
        {
            var logicalP = this.deviceToLogical.Transform(deviceP);

            // カメラ行列は各点をLive2Dの論理座標に移動させる行列のため、
            // 論理座標からカメラ視点を得るには、その逆変換を行います。
            var inv = this.viewMatrix.Clone().Invert();
            return inv.Transform(logicalP);
        }

        /// <summary>
        /// デバイス座標をLive2Dの論理座標に変換します。
        /// </summary>
        public Pointd DeviceToLogical(Pointd deviceP)
        {
            return this.deviceToLogical.Transform(deviceP);
        }
    }
}
