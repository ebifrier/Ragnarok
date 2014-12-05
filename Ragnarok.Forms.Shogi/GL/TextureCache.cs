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
        public readonly static long AnimationTextureCacheCapacity = 20L * 1024 * 1024; // 20MB
        public readonly static long TextTextureCacheCapacity = 20L * 1024 * 1024; // 20MB

        private readonly static object AnimationTextureSyncRoot = new object();
        private readonly static Dictionary<OpenGL, AnimationTextureCache>
            AnimationTextureCacheDic =
            new Dictionary<OpenGL, AnimationTextureCache>();

        private readonly static object TextTextureSyncRoot = new object();
        private readonly static Dictionary<OpenGL, TextTextureCache>
            TextTextureCacheDic =
            new Dictionary<OpenGL, TextTextureCache>();

        /// <summary>
        /// OpenGLオブジェクトに対応するテクスチャのキャッシュを取得します。
        /// </summary>
        private static AnimationTextureCache GetAnimationTextureCache(OpenGL gl)
        {
            lock (AnimationTextureSyncRoot)
            {
                AnimationTextureCache cache;
                if (AnimationTextureCacheDic.TryGetValue(gl, out cache))
                {
                    return cache;
                }

                cache = new AnimationTextureCache(gl, AnimationTextureCacheCapacity);
                AnimationTextureCacheDic.Add(gl, cache);
                return cache;
            }
        }

        /// <summary>
        /// OpenGLオブジェクトに対応する文字列テクスチャのキャッシュを取得します。
        /// </summary>
        private static TextTextureCache GetTextTextureCache(OpenGL gl)
        {
            lock(TextTextureSyncRoot)
            {
                TextTextureCache cache;
                if (TextTextureCacheDic.TryGetValue(gl, out cache))
                {
                    return cache;
                }

                cache = new TextTextureCache(gl, TextTextureCacheCapacity);
                TextTextureCacheDic.Add(gl, cache);
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

            var cache = GetAnimationTextureCache(gl);
            return cache.GetAnimationTexture(imageUri, count);
        }

        /// <summary>
        /// 指定のフォントで指定の文字列を描画するためのテクスチャを取得します。
        /// </summary>
        public static TextTexture GetTextTexture(OpenGL gl, string text,
                                                 TextTextureFont font)
        {
            if (gl == null)
            {
                throw new ArgumentNullException("gl");
            }

            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            var cache = GetTextTextureCache(gl);
            return cache.GetTextTexture(text, font);
        }
    }
}
