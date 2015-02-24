using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using SharpGL;

using Ragnarok;
using Ragnarok.Extra.Effect;
using Ragnarok.ObjectModel;
using Ragnarok.Utility;

namespace Ragnarok.Forms.Shogi.View
{
    using GL = Ragnarok.Forms.Shogi.GL;

    /// <summary>
    /// 評価値画像の領域にオブジェクトを描画するための基本クラスです。
    /// </summary>
    public abstract class GLEvaluationElementInternal : NotifyObject
    {
        /// <summary>
        /// 設定されたImageSetを取得します。
        /// </summary>
        public ImageSetInfo ImageSet
        {
            get { return GetValue<ImageSetInfo>("ImageSet"); }
            protected set { SetValue("ImageSet", value); }
        }

        /// <summary>
        /// 評価値を取得します。
        /// </summary>
        public int CurrentValue
        {
            get { return GetValue<int>("CurrentValue"); }
            internal set { SetValue("CurrentValue", value); }
        }

        /// <summary>
        /// オブジェクトの初期化を行います。
        /// </summary>
        public virtual void Initialize(ImageSetInfo imageSet)
        {
            ImageSet = imageSet;
        }
        
        /// <summary>
        /// 評価値画像を描画リストに加えます。
        /// </summary>
        public abstract void AddRender(OpenGL gl, Matrix44d transform,
                                       GL.RenderBuffer renderBuffer);
    }

    public sealed class GLEvaluationElement_Image : GLEvaluationElementInternal
    {
        /// <summary>
        /// 評価値画像を描画リストに加えます。
        /// </summary>
        public override void AddRender(OpenGL gl, Matrix44d transform,
                                       GL.RenderBuffer renderBuffer)
        {
            var imageSet = ImageSet;
            if (imageSet == null)
            {
                // 評価値画像のセットがない場合は、何も表示しません。
                return;
            }

            var imagePath = imageSet.GetSelectedImagePath(CurrentValue);
            if (string.IsNullOrEmpty(imagePath))
            {
                // 評価値画像がない場合も、何もしません。
                return;
            }

            // 描画領域はこのクラスの外側で指定します。
            var bounds = new RectangleF(-0.5f, -0.5f, 1.0f, 1.0f);

            // 描画領域を設定します。
            var texture = GL.TextureCache.GetTexture(gl, new Uri(imagePath));
            if (texture != null && texture.IsAvailable)
            {
                //transform.Multiply(Transform);
                renderBuffer.AddRender(
                    texture, BlendType.Diffuse, bounds, transform, 0.0);
            }
        }
    }
}
