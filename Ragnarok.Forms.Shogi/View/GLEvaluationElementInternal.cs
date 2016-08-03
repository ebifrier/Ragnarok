using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Ragnarok.Extra.Effect;
using Ragnarok.Shogi;
using Ragnarok.Utility;

namespace Ragnarok.Forms.Shogi.View
{
    using GL = Ragnarok.OpenGL;

    /// <summary>
    /// 評価値画像の領域にオブジェクトを描画するための基本クラスです。
    /// </summary>
    public abstract class GLEvaluationElementInternal : GLElement
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
        public Score CurrentScore
        {
            get { return GetValue<Score>("CurrentScore"); }
            internal set { SetValue("CurrentScore", value); }
        }

        /// <summary>
        /// オブジェクトの初期化を行います。
        /// </summary>
        public virtual void Initialize(ImageSetInfo imageSet)
        {
            ImageSet = imageSet;
        }
    }

    /// <summary>
    /// 評価値画像の描画を行います。
    /// </summary>
    public sealed class GLEvaluationElementImage : GLEvaluationElementInternal
    {
        /// <summary>
        /// 評価値画像を描画リストに加えます。
        /// </summary>
        protected override void OnEnterFrame(EnterFrameEventArgs e)
        {
            base.OnEnterFrame(e);
            var renderBuffer = (GL.RenderBuffer)e.StateObject;

            if (IsVisible)
            {
                var imageSet = ImageSet;
                if (imageSet == null)
                {
                    // 評価値画像のセットがない場合は、何も表示しません。
                    return;
                }

                var value = CurrentScore.Value;
                var imagePath = imageSet.GetSelectedImagePath(value);
                if (string.IsNullOrEmpty(imagePath))
                {
                    // 評価値画像がない場合も、何もしません。
                    return;
                }

                // 描画領域を設定します。
                var texture = GL.TextureCache.GetTexture(imagePath);
                if (texture != null && texture.IsAvailable)
                {
                    // 描画領域はこのクラスの外側で指定します。
                    var rate = (float)texture.OriginalHeight / texture.OriginalWidth;
                    var bounds = new RectangleF(-0.5f, 0.5f - rate, 1.0f, rate);

                    renderBuffer.AddRender(
                        texture, BlendType.Diffuse, bounds, Transform, 0.0);
                }
            }
        }
    }
}
