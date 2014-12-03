using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FlintSharp;
using SharpGL;

namespace Ragnarok.Forms.Shogi.FlintSharpEx
{
    /// <summary>
    /// OpenGL用のイメージデータを保持します。
    /// </summary>
    internal sealed class GLImageData : IImageData
    {
        private static readonly Dictionary<OpenGL, GL.Texture> particleDic =
            new Dictionary<OpenGL, GL.Texture>();

        /// <summary>
        /// パーティクル用のテクスチャを取得します。
        /// </summary>
        private static GL.Texture GetParticleTexture(OpenGL gl)
        {
            GL.Texture texture;
            if (particleDic.TryGetValue(gl, out texture))
            {
                return texture;
            }

            var bitmap = Properties.Resources.particle;
            if (bitmap == null)
            {
                throw new RagnarokException(
                    "リソース画像'particle'の読み込みに失敗しました。");
            }

            texture = new GL.Texture(gl);
            if (!texture.Create(bitmap))
            {
                texture.Destroy();
                return null;
            }

            particleDic.Add(gl, texture);
            return texture;
        }

        /// <summary>
        /// テクスチャの初期化を行います。
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            // 初期化済みなら再初期化を行いません。
            if (Texture != null && OpenGL == gl)
            {
                return;
            }

            if (ImageUri != null)
            {
                // 通常画像
                var texture = GL.TextureCache.GetTexture(gl, ImageUri);
                if (texture == null)
                {
                    // うーん、どうしよう。
                    return;
                }

                Texture = texture;
                OpenGL = gl;
            }
            else if (IsParticle)
            {
                // パーティクル
                Texture = GetParticleTexture(gl);
                OpenGL = gl;
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
        public Uri ImageUri
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
        public OpenGL OpenGL
        {
            get;
            private set;
        }

        /// <summary>
        /// テクスチャを取得します。
        /// </summary>
        public GL.Texture Texture
        {
            get;
            private set;
        }
    }
}
