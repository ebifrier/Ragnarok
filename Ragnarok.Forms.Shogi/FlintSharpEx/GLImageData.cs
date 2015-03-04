using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using FlintSharp;

namespace Ragnarok.Forms.Shogi.FlintSharpEx
{
    /// <summary>
    /// OpenGL用のイメージデータを保持します。
    /// </summary>
    internal sealed class GLImageData : IImageData
    {
        private static readonly Dictionary<IGraphicsContext, GLUtil.Texture> particleDic =
            new Dictionary<IGraphicsContext, GLUtil.Texture>();

        /// <summary>
        /// パーティクル用のテクスチャを取得します。
        /// </summary>
        private static GLUtil.Texture GetParticleTexture()
        {
            var context = GraphicsContext.CurrentContext;
            if (context == null)
            {
                throw new GLUtil.GLException(
                    "OpenGLコンテキストが設定されていません。");
            }

            GLUtil.Texture texture;
            if (particleDic.TryGetValue(context, out texture))
            {
                return texture;
            }

            var bitmap = Properties.Resources.particle;
            if (bitmap == null)
            {
                throw new RagnarokException(
                    "リソース画像'particle'の読み込みに失敗しました。");
            }

            texture = new GLUtil.Texture();
            if (!texture.Create(bitmap))
            {
                texture.Destroy();
                return null;
            }

            particleDic.Add(context, texture);
            return texture;
        }

        /// <summary>
        /// テクスチャの初期化を行います。
        /// </summary>
        public void Initialize()
        {
            // 初期化済みなら再初期化を行いません。
            if (Texture != null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(ImagePath))
            {
                // 通常画像
                var texture = GLUtil.TextureCache.GetTexture(ImagePath);
                if (texture == null)
                {
                    // うーん、どうしよう。
                    return;
                }

                Texture = texture;
            }
            else if (IsParticle)
            {
                // パーティクル
                Texture = GetParticleTexture();
            }
            else
            {
                throw new NotSupportedException(
                    "テクスチャの読み込みに失敗しました。");
            }
        }

        /// <summary>
        /// オブジェクトの破棄時に呼ばれます。
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// パーティクル用のテクスチャかどうかを取得または設定します。
        /// </summary>
        public bool IsParticle
        {
            get;
            set;
        }

        /// <summary>
        /// 画像データのURIを取得または設定します。
        /// </summary>
        public string ImagePath
        {
            get;
            set;
        }

        /// <summary>
        /// 描画方法を取得または設定します。
        /// </summary>
        public MaterialType MaterialType
        {
            get;
            set;
        }

        /// <summary>
        /// パーティクルを二重に描画するかどうかを取得または設定します。
        /// </summary>
        public bool IsDoubleParticle
        {
            get;
            set;
        }

        /// <summary>
        /// 対象となるOpenGLオブジェクトを取得します。
        /// </summary>
        public GL OpenGL
        {
            get;
            private set;
        }

        /// <summary>
        /// テクスチャを取得します。
        /// </summary>
        public GLUtil.Texture Texture
        {
            get;
            private set;
        }
    }
}
