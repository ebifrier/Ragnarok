﻿using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Windowing.Common;

namespace Ragnarok.OpenGL
{
    /// <summary>
    /// 画像オブジェクトをキャッシュします。
    /// </summary>
    /// <remarks>
    /// エフェクトはアニメーションするため、画像が縦や横に連なって保存されています。
    /// スクリーンより大きい画像はビデオカードによっては表示できないことがあるため、
    /// このクラスでは各画像をクロッピングして管理します。
    /// </remarks>
    public static class TextureCache
    {
        public readonly static long AnimationTextureCacheCapacity = 20L * 1024 * 1024; // 20MB
        public readonly static long TextTextureCacheCapacity = 20L * 1024 * 1024; // 20MB

        private readonly static object AnimationTextureSyncRoot = new object();
        private readonly static Dictionary<IGraphicsContext, AnimationTextureCache>
            AnimationTextureCacheDic =
            new Dictionary<IGraphicsContext, AnimationTextureCache>();

        private readonly static object TextTextureSyncRoot = new object();
        private readonly static Dictionary<IGraphicsContext, TextTextureCache>
            TextTextureCacheDic =
            new Dictionary<IGraphicsContext, TextTextureCache>();

        /// <summary>
        /// OpenGLオブジェクトに対応するテクスチャのキャッシュを取得します。
        /// </summary>
        private static AnimationTextureCache GetAnimationTextureCache(IGraphicsContext context)
        {
            lock (AnimationTextureSyncRoot)
            {
                AnimationTextureCache cache;
                if (AnimationTextureCacheDic.TryGetValue(context, out cache))
                {
                    return cache;
                }

                cache = new AnimationTextureCache(context, AnimationTextureCacheCapacity);
                AnimationTextureCacheDic.Add(context, cache);
                return cache;
            }
        }

        /// <summary>
        /// OpenGLオブジェクトに対応する文字列テクスチャのキャッシュを取得します。
        /// </summary>
        private static TextTextureCache GetTextTextureCache(IGraphicsContext context)
        {
            lock (TextTextureSyncRoot)
            {
                TextTextureCache cache;
                if (TextTextureCacheDic.TryGetValue(context, out cache))
                {
                    return cache;
                }

                cache = new TextTextureCache(context, TextTextureCacheCapacity);
                TextTextureCacheDic.Add(context, cache);
                return cache;
            }
        }

        /// <summary>
        /// テクスチャをキャッシュから探し、もしなければ読み込みます。
        /// </summary>
        public static Texture GetTexture(IGraphicsContext context, string imagePath)
        {
            var animTexture = GetAnimationTexture(context, imagePath, 1);
            if (animTexture == null || animTexture.TextureList.Count != 1)
            {
                return null;
            }

            return animTexture.TextureList[0];
        }

        /// <summary>
        /// アニメーション用テクスチャをキャッシュから探し、もしなければ読み込みます。
        /// </summary>
        public static AnimationTexture GetAnimationTexture(IGraphicsContext context,
                                                           string imagePath,
                                                           int count)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrEmpty(imagePath))
            {
                throw new ArgumentNullException(nameof(imagePath));
            }

            if (count <= 0)
            {
                throw new ArgumentException("countの値が小さすぎます。", nameof(count));
            }

            var cache = GetAnimationTextureCache(context);
            return cache.GetAnimationTexture(imagePath, count);
        }

        /// <summary>
        /// 指定のフォントで指定の文字列を描画するためのテクスチャを取得します。
        /// </summary>
        public static TextTexture GetTextTexture(IGraphicsContext context,
                                                 string text,
                                                 TextTextureFont font)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (font == null)
            {
                throw new ArgumentNullException(nameof(font));
            }

            var cache = GetTextTextureCache(context);
            return cache.GetTextTexture(text, font);
        }
    }
}
