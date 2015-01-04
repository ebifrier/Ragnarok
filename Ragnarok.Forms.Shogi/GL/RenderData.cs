using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using SharpGL;

using Ragnarok.Extra.Effect;
using Ragnarok.Utility;

namespace Ragnarok.Forms.Shogi.GL
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
        public Action<OpenGL> RenderAction
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
        public void Render(OpenGL gl)
        {
            if (RenderAction != null)
            {
                gl.PushMatrix();
                RenderAction(gl);
                gl.PopMatrix();
            }
            else
            {
                if (Mesh == null)
                {
                    return;
                }

                gl.Color(Color.R, Color.G, Color.B, Color.A);
                SetBlend(gl);

                BindTexture(gl);

                // 座標系の設定
                gl.PushMatrix();
                SetMatrix(gl);
                //gl.Translate(0, 0, ZOrder);

                SetMesh(gl);
                gl.PopMatrix();

                //UnbindTexture(gl);
            }
        }

        /// <summary>
        /// 変換行列の設定を行います。
        /// </summary>
        private void SetMatrix(OpenGL gl)
        {
            if (Transform == null)
            {
                gl.LoadIdentity();
            }
            else
            {
                gl.LoadMatrix(Transform.AsColumnMajorArray);
            }
        }

        /// <summary>
        /// テクスチャのバインドを行います。
        /// </summary>
        private void BindTexture(OpenGL gl)
        {
            if (Texture != null && Texture.TextureName != 0)
            {
                Texture.Bind();
            }
            else
            {
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            }
        }

        /// <summary>
        /// テクスチャのアンバインドを行います。
        /// </summary>
        private void UnbindTexture(OpenGL gl)
        {
            if (Texture != null)
            {
                Texture.Unbind();
            }
        }

        /// <summary>
        /// 描画のブレンド方法を指定します。
        /// </summary>
        private void SetBlend(OpenGL gl)
        {
            switch (Blend)
            {
                case BlendType.Diffuse:
                    gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
                    break;
                case BlendType.Emissive:
                    gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE);
                    break;
            }
        }

        /// <summary>
        /// メッシュの設定を行います。
        /// </summary>
        private void SetMesh(OpenGL gl)
        {
            gl.Begin(OpenGL.GL_TRIANGLES);

            for (int i = 0; i < Mesh.IndexArray.Count(); ++i)
            {
                var index = Mesh.IndexArray[i];

                // UVの設定
                var uv = Mesh.TextureUVArray[index];
                gl.TexCoord(uv.X, uv.Y);

                // 頂点座標の設定
                var pos = Mesh.VertexArray[index];
                gl.Vertex(pos.X, pos.Y, pos.Z);
            }

            gl.End();
        }
    }
}
