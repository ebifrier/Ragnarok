using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;

using Ragnarok.Utility;

namespace Ragnarok.OpenGL
{
    /// <summary>
    /// 文字列用テクスチャのキャッシュキーとして使うオブジェクトです。
    /// </summary>
    internal sealed class TextTextureKey : IEquatable<TextTextureKey>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TextTextureKey(string text, TextTextureFont textureFont)
        {
            Text = text;
            TextureFont = textureFont;
        }

        /// <summary>
        /// 描画する文字列を取得します。
        /// </summary>
        public string Text
        {
            get;
            private set;
        }

        /// <summary>
        /// 描画する文字列のフォントデータを取得します。
        /// </summary>
        public TextTextureFont TextureFont
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

            return Equals((TextTextureFont)obj);
        }

        /// <summary>
        /// オブジェクトが等しいか調べます。
        /// </summary>
        public bool Equals(TextTextureKey other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return (Text == other.Text && TextureFont == other.TextureFont);
        }

        /// <summary>
        /// ハッシュ値を計算します。
        /// </summary>
        public override int GetHashCode()
        {
            return (Text.GetHashCode() ^ TextureFont.GetHashCode());
        }
    }

    /// <summary>
    /// 文字列用のテクスチャをキャッシュします。
    /// </summary>
    [CLSCompliant(false)]
    public sealed class TextTextureCache
    {
        private readonly CacheManager<TextTextureKey, TextTexture> cache;
        private readonly IGraphicsContext context;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TextTextureCache(IGraphicsContext context, long capacity)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.cache = new CacheManager<TextTextureKey, TextTexture>(
                CreateTextTexture, capacity);
            this.context = context;
        }

        /// <summary>
        /// 文字列用テクスチャを作成します。
        /// </summary>
        private TextTexture CreateTextTexture(TextTextureKey key)
        {
            var textTexture = new TextTexture()
            {
                Text = key.Text,
                TextureFont = key.TextureFont,
            };

            // テクスチャを作成してからオブジェクトを返します。
            textTexture.UpdateTexture();
            return textTexture;
        }

        /// <summary>
        /// 文字列用テクスチャをキャッシュから探し、もしなければ読み込みます。
        /// </summary>
        public TextTexture GetTextTexture(string text,
                                          TextTextureFont textureFont)
        {
            if (this.context != GraphicsContext.CurrentContext)
            {
                throw new GLException(
                    "OpenGLコンテキストが正しく設定れていません＞＜");
            }

            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (textureFont == null)
            {
                throw new ArgumentNullException("textureFont");
            }

            try
            {
                var key = new TextTextureKey(text, textureFont);

                return this.cache.GetOrCreate(key);
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);
                Log.ErrorException(ex,
                    "文字列テクスチャの作成に失敗しました。");

                return null;
            }
        }

        /// <summary>
        /// キャッシュをすべてからにします。
        /// </summary>
        public void Clear()
        {
            this.cache.Clear();
        }
    }
}
