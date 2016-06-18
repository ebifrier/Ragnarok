using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;

using Ragnarok.Extra.Effect;
using Ragnarok.Utility;

namespace Ragnarok.Forms.Shogi.GLUtil
{
    /// <summary>
    /// 描画用のデータをまとめて持ちます。
    /// </summary>
    [CLSCompliant(false)]
    public sealed class RenderData
    {
        /// <summary>
        /// 描画用のメソッドを取得または設定します。
        /// </summary>
        public Action RenderAction
        {
            get;
            set;
        }

        /// <summary>
        /// 描画用のテクスチャを取得します。
        /// </summary>
        public Texture Texture
        {
            get;
            set;
        }

        /// <summary>
        /// テクスチャのブレンド方法を取得します。
        /// </summary>
        public BlendType Blend
        {
            get;
            set;
        }

        /// <summary>
        /// ブレンドカラーを取得します。
        /// </summary>
        public Color Color
        {
            get;
            set;
        }

        /// <summary>
        /// メッシュを取得します。
        /// </summary>
        public Mesh Mesh
        {
            get;
            set;
        }

        /// <summary>
        /// 変換行列を取得します。
        /// </summary>
        public Matrix44d Transform
        {
            get;
            set;
        }

        /// <summary>
        /// ZOrderを取得します。
        /// </summary>
        /// <remarks>
        /// ZOrderは数値が大きいほど画面に近くなります。
        /// </remarks>
        public double ZOrder
        {
            get;
            set;
        }

        /// <summary>
        /// オブジェクトの描画を行います。
        /// </summary>
        public void Render()
        {
            if (RenderAction != null)
            {
                GL.PushMatrix();
                RenderAction();
                GL.PopMatrix();
            }
            else
            {
                if (Mesh == null)
                {
                    return;
                }

                GL.Color4(Color.R, Color.G, Color.B, Color.A);
                SetBlend();

                BindTexture();

                // 座標系の設定
                GL.PushMatrix();
                SetMatrix();
                //gl.Translate(0, 0, ZOrder);

                SetMesh();
                GL.PopMatrix();

                //UnbindTexture(gl);
            }
        }

        /// <summary>
        /// 変換行列の設定を行います。
        /// </summary>
        private void SetMatrix()
        {
            if (Transform == null)
            {
                GL.LoadIdentity();
            }
            else
            {
                GL.LoadMatrix(Transform.AsColumnMajorArray);
            }
        }

        /// <summary>
        /// テクスチャのバインドを行います。
        /// </summary>
        private void BindTexture()
        {
            if (Texture != null && Texture.TextureName != 0)
            {
                Texture.Bind();
            }
            else
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
        }

        /// <summary>
        /// テクスチャのアンバインドを行います。
        /// </summary>
        private void UnbindTexture()
        {
            if (Texture != null)
            {
                Texture.Unbind();
            }
        }

        /// <summary>
        /// 描画のブレンド方法を指定します。
        /// </summary>
        private void SetBlend()
        {
            switch (Blend)
            {
                case BlendType.Diffuse:
                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                    break;
                case BlendType.Emissive:
                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
                    break;
            }
        }

        /// <summary>
        /// メッシュの設定を行います。
        /// </summary>
        private void SetMesh()
        {
#if false
            GL.Begin(PrimitiveType.Triangles);
#else
            GL.Begin(BeginMode.Triangles);
#endif

            for (int i = 0; i < Mesh.IndexArray.Count(); ++i)
            {
                var index = Mesh.IndexArray[i];

                // UVの設定
                var uv = Mesh.TextureUVArray[index];
                GL.TexCoord2(uv.X, uv.Y);

                // 頂点座標の設定
                var pos = Mesh.VertexArray[index];
                GL.Vertex3(pos.X, pos.Y, pos.Z);
            }

            GL.End();
        }
    }
}
