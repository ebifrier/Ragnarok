using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL;

using Ragnarok.Extra.Effect;
using Ragnarok.MathEx;

namespace Ragnarok.OpenGL
{
    public delegate void RenderAction(RenderData data);

    /// <summary>
    /// 描画用のデータをまとめて持ちます。
    /// </summary>
    public sealed class RenderData
    {
        /// <summary>
        /// 描画用のメソッドを取得または設定します。
        /// </summary>
        public RenderAction RenderAction
        {
            get;
            set;
        }

        /// <summary>
        /// 描画用のテクスチャを取得または設定します。
        /// </summary>
        public Texture Texture
        {
            get;
            set;
        }

        /// <summary>
        /// テクスチャのブレンド方法を取得または設定します。
        /// </summary>
        public BlendType Blend
        {
            get;
            set;
        } = BlendType.Diffuse;

        /// <summary>
        /// ブレンドカラーを取得または設定します。
        /// </summary>
        public Color Color
        {
            get;
            set;
        } = Color.White;

        /// <summary>
        /// メッシュを取得または設定します。
        /// </summary>
        public Mesh Mesh
        {
            get;
            set;
        } = Mesh.Default;

        /// <summary>
        /// 描画時の基本型を取得または設定します。
        /// </summary>
        public PrimitiveType PrimitiveType
        {
            get;
            set;
        } = PrimitiveType.Triangles;

        /// <summary>
        /// 変換行列を取得または設定します。
        /// </summary>
        public Matrix44d Transform
        {
            get;
            set;
        } = Matrix44d.Identity;

        /// <summary>
        /// ZOrderを取得または設定します。
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
            GL.Color4(Color);
            SetBlend(Blend);

            if (Texture != null && Texture.TextureName != 0)
            {
                Texture.Bind();
            }

            if (RenderAction != null)
            {
                RenderAction(this);
            }
            else if (Mesh != null)
            {
                RenderElements(Mesh, Transform, PrimitiveType);
            }

            Texture.Unbind();
        }

        /// <summary>
        /// 頂点要素からなるオブジェクトを描画します。
        /// </summary>
        public void RenderElements(Mesh mesh,
                                   Matrix44d transform,
                                   PrimitiveType primitiveType)
        {
            if (mesh == null)
            {
                return;
            }

            // 座標系の設定
            GL.PushMatrix();
            SetMatrix(transform);
            RenderMesh(mesh, primitiveType);
            GL.PopMatrix();
        }

        /// <summary>
        /// 変換行列の設定を行います。
        /// </summary>
        private static void SetMatrix(Matrix44d matrix)
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
        /// メッシュの設定を行います。
        /// </summary>
        private void RenderMesh(Mesh mesh, PrimitiveType primitiveType)
        {
            var tx = 1.0f;
            var ty = 1.0f;

            if (Texture != null)
            {
                tx = (float)Texture.OriginalWidth / Texture.Width;
                ty = (float)Texture.OriginalHeight / Texture.Height;
            }

            GL.Begin(primitiveType);

            foreach (var index in mesh.IndexArray)
            {
                // UVの設定
                if (mesh.TextureUVArray != null)
                {
                    var uv = mesh.TextureUVArray[index];
                    GL.TexCoord2((float)uv.X * tx, (float)uv.Y * ty);
                }

                // 頂点座標の設定
                var pos = mesh.VertexArray[index];
                GL.Vertex3((float)pos.X, (float)pos.Y, (float)pos.Z);
            }

            GL.End();
        }

        /// <summary>
        /// 描画のブレンド方法を指定します。
        /// </summary>
        public static void SetBlend(BlendType blend)
        {
            switch (blend)
            {
                case BlendType.Diffuse:
                    GLw.C(() => GL.BlendFunc(
                        BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha));
                    GLw.C(() => GL.Disable(EnableCap.AlphaTest));
                    break;
                case BlendType.Emissive:
                    GLw.C(() => GL.BlendFunc(
                        BlendingFactor.SrcAlpha, BlendingFactor.One));
                    GLw.C(() => GL.Disable(EnableCap.AlphaTest));
                    break;
                case BlendType.Copy:
                    GLw.C(() => GL.BlendFunc(
                        BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha));
                    GLw.C(() => GL.Enable(EnableCap.AlphaTest));
                    GLw.C(() => GL.AlphaFunc(AlphaFunction.Greater, 0.9f));
                    break;
            }
        }
    }
}
