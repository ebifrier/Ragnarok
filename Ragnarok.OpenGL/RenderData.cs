using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL;

using Ragnarok.Extra.Effect;
using Ragnarok.MathEx;

namespace Ragnarok.OpenGL
{
    /// <summary>
    /// 描画用のデータをまとめて持ちます。
    /// </summary>
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
                SetBlend(Blend);

                BindTexture(Texture);

                // 座標系の設定
                GL.PushMatrix();
                SetMatrix(Transform);
                SetMesh();
                GL.PopMatrix();

                UnbindTexture();
            }
        }

        /// <summary>
        /// 変換行列の設定を行います。
        /// </summary>
        public static void SetMatrix(Matrix44d matrix)
        {
            if (matrix == null)
            {
                GL.LoadIdentity();
            }
            else
            {
                GL.LoadMatrix(matrix.AsColumnMajorArray);
            }
        }

        /// <summary>
        /// テクスチャのバインドを行います。
        /// </summary>
        public static void BindTexture(Texture texture)
        {
            if (texture != null && texture.TextureName != 0)
            {
                texture.Bind();
            }
            else
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
        }

        /// <summary>
        /// テクスチャのアンバインドを行います。
        /// </summary>
        public static void UnbindTexture()
        {
            Texture.Unbind();
        }

        /// <summary>
        /// 描画のブレンド方法を指定します。
        /// </summary>
        public static void SetBlend(BlendType blend)
        {
            switch (blend)
            {
                case BlendType.Diffuse:
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                    GL.Disable(EnableCap.AlphaTest);
                    break;
                case BlendType.Emissive:
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
                    GL.Disable(EnableCap.AlphaTest);
                    break;
                case BlendType.Copy:
                    // ある程度高い不透明度がある場合しか、データのコピーを行わないようにします。
                    // エッジが不透明になるのを避けたい場合に使います。
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                    GL.Enable(EnableCap.AlphaTest);
                    GL.AlphaFunc(AlphaFunction.Greater, 0.9f);
                    break;
            }
        }

        /// <summary>
        /// メッシュの設定を行います。
        /// </summary>
        private void SetMesh()
        {
            var tx = 1.0;
            var ty = 1.0;

            if (Texture != null)
            {
                tx = (double)Texture.OriginalWidth / Texture.Width;
                ty = (double)Texture.OriginalHeight / Texture.Height;
            }

            // 困ったことにOpenTKのバージョンによって使える方が異なる
#if true
            GL.Begin(PrimitiveType.Triangles);
#else
            GL.Begin(BeginMode.Triangles);
#endif

            for (int i = 0; i < Mesh.IndexArray.Count; ++i)
            {
                var index = Mesh.IndexArray[i];

                // UVの設定
                var uv = Mesh.TextureUVArray[index];
                GL.TexCoord2(uv.X * tx, uv.Y * ty);

                // 頂点座標の設定
                var pos = Mesh.VertexArray[index];
                GL.Vertex3(pos.X, pos.Y, pos.Z);
            }

            GL.End();
        }
    }
}
