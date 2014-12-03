using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using SharpGL;

using Ragnarok.Forms.Draw;
using Ragnarok.Utility;

namespace Ragnarok.Forms.Shogi.GL
{
    /// <summary>
    /// 画像オブジェクトをキャッシュします。
    /// </summary>
    /// <remarks>
    /// エフェクトはアニメーションするため、画像が縦や横に連なって保存されています。
    /// スクリーンより大きい画像はビデオカードによっては表示できないことがあるため、
    /// このクラスでは各画像をクロッピングして管理します。
    /// </remarks>
    [CLSCompliant(false)]
    public class TextureCache
    {
        public readonly static long AnimationCacheCapacity = 20L * 1024 * 1024; // 20MB
        public readonly static long TextCacheCapacity = 20L * 1024 * 1024; // 20MB

        private readonly static object SyncRoot = new object();
        private readonly static Dictionary<OpenGL, AnimationTextureCache> CacheDic =
            new Dictionary<OpenGL, AnimationTextureCache>();

        /// <summary>
        /// OpenGLオブジェクトに対応するキャッシュを取得します。
        /// </summary>
        private static AnimationTextureCache GetAnimationCache(OpenGL gl)
        {
            lock (SyncRoot)
            {
                AnimationTextureCache cache;
                if (CacheDic.TryGetValue(gl, out cache))
                {
                    return cache;
                }

                cache = new AnimationTextureCache(gl, AnimationCacheCapacity);
                CacheDic.Add(gl, cache);
                return cache;
            }
        }

        /// <summary>
        /// テクスチャをキャッシュから探し、もしなければ読み込みます。
        /// </summary>
        public static Texture GetTexture(OpenGL gl, Uri imageUri)
        {
            var animTexture = GetAnimationTexture(gl, imageUri, 1);
            if (animTexture == null || animTexture.TextureList.Count() != 1)
            {
                return null;
            }

            return animTexture.TextureList[0];
        }

        /// <summary>
        /// アニメーション用テクスチャをキャッシュから探し、もしなければ読み込みます。
        /// </summary>
        public static AnimationTexture GetAnimationTexture(OpenGL gl, Uri imageUri,
                                                           int count)
        {
            if (gl == null)
            {
                throw new ArgumentNullException("gl");
            }

            if (imageUri == null)
            {
                throw new ArgumentNullException("imageUri");
            }

            if (count <= 0)
            {
                throw new ArgumentException("count");
            }

            var cache = GetAnimationCache(gl);
            return cache.GetAnimationTexture(imageUri, count);
        }
    }
}
