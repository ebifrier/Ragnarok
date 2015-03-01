using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using OpenTK;
using OpenTK.Graphics;

using Ragnarok.Forms.Draw;
using Ragnarok.Utility;

namespace Ragnarok.Forms.Shogi.GLUtil
{
    /// <summary>
    /// アニメーションテクスチャのキーとして使うオブジェクトです。
    /// </summary>
    internal sealed class AnimationTextureKey : IEquatable<AnimationTextureKey>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AnimationTextureKey(Uri imageUri, int count)
        {
            ImageUri = imageUri;
            Count = count;
        }

        /// <summary>
        /// 画像URIを取得します。
        /// </summary>
        public Uri ImageUri
        {
            get;
            private set;
        }

        /// <summary>
        /// アニメーションさせる数を取得します。
        /// </summary>
        public int Count
        {
            get;
            private set;
        }

        /// <summary>
        /// オブジェクトが等しいか調べます。
        /// </summary>
        public override bool Equals(object obj)
        {
            var result = this.PreEquals(obj);
            if (result != null)
            {
                return result.Value;
            }

            return Equals((AnimationTextureKey)obj);
        }

        /// <summary>
        /// オブジェクトが等しいか調べます。
        /// </summary>
        public bool Equals(AnimationTextureKey other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return (
                ImageUri == other.ImageUri &&
                Count == other.Count);
        }

        /// <summary>
        /// ハッシュ値を計算します。
        /// </summary>
        public override int GetHashCode()
        {
            return (ImageUri.GetHashCode() ^ Count.GetHashCode());
        }
    }

    /// <summary>
    /// アニメーション用の画像テクスチャをキャッシュします。
    /// </summary>
    /// <remarks>
    /// エフェクトはアニメーションするため、画像が横に連なって保存されています。
    /// </remarks>
    public sealed class AnimationTextureCache
    {
        private readonly CacheManager<AnimationTextureKey, AnimationTexture> cache;
        private readonly IGraphicsContext context;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AnimationTextureCache(IGraphicsContext context, long capacity)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.cache = new CacheManager<AnimationTextureKey, AnimationTexture>(
                CreateAnimationTexture, capacity);
            this.context = context;
        }

        /// <summary>
        /// 分割されたアニメーション用テクスチャを読み込みます。
        /// </summary>
        private AnimationTexture CreateAnimationTexture(AnimationTextureKey key)
        {
            var imagePath = key.ImageUri.LocalPath;
            var animTexture = new AnimationTexture();

            animTexture.Load(imagePath, key.Count);
            return animTexture;
        }

        /// <summary>
        /// 画像をキャッシュから探し、もしなければ読み込みます。
        /// </summary>
        public AnimationTexture GetAnimationTexture(Uri imageUri, int count)
        {
            if (this.context != GraphicsContext.CurrentContext)
            {
                throw new GLException(
                    "OpenGLコンテキストが正しく設定れていません＞＜");
            }

            if (imageUri == null)
            {
                throw new ArgumentNullException("imageUri");
            }

            if (count <= 0)
            {
                throw new ArgumentException("count");
            }

            try
            {
                var key = new AnimationTextureKey(imageUri, count);

                return this.cache.GetOrCreate(key);
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);
                Log.ErrorException(ex,
                    "テクスチャの読み込みに失敗しました。");

                return null;
            }
        }

        /// <summary>
        /// キャッシュされているすべてのテクスチャをクリアします。
        /// </summary>
        public void Clear()
        {
            this.cache.Clear();
        }
    }
}
