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
    [CLSCompliant(false)]
    public sealed class AnimationTextureCache
    {
        private readonly CacheManager<AnimationTextureKey, AnimationTexture> cache;
        private readonly OpenGL gl;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AnimationTextureCache(OpenGL gl, long capacity)
        {
            this.cache = new CacheManager<AnimationTextureKey, AnimationTexture>(
                CreateAnimationTexture, capacity);
            this.gl = gl;
        }

        /// <summary>
        /// 分割されたアニメーション用テクスチャを読み込みます。
        /// </summary>
        private AnimationTexture CreateAnimationTexture(AnimationTextureKey key)
        {
            var imagePath = key.ImageUri.LocalPath;
            var animTexture = new AnimationTexture(this.gl);

            animTexture.Load(imagePath, key.Count);
            return animTexture;
        }

        /// <summary>
        /// 画像をキャッシュから探し、もしなければ読み込みます。
        /// </summary>
        public AnimationTexture GetAnimationTexture(Uri imageUri, int count)
        {
            if (imageUri == null)
            {
                throw new ArgumentNullException("imageUri");
            }

            if (count <= 0)
            {
                throw new ArgumentException("count");
            }

            var key = new AnimationTextureKey(imageUri, count);
            return this.cache.GetOrCreate(key);
        }
    }
}
