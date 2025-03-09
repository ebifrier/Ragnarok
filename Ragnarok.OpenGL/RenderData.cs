using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

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
        /// シェーダーを取得または設定します。
        /// </summary>
        public ShaderProgram Shader
        {
            get;
            set;
        }

        /// <summary>
        /// 頂点バッファを取得または設定します。
        /// </summary>
        public VertexBuffer VertexBuffer
        {
            get;
            set;
        }

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
        public void Render(Matrix4 projectionMatrix)
        {
            SetBlend(Blend);
            Shader.Use();
            SetUniform(Shader, projectionMatrix);

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
                RenderElements(Mesh, PrimitiveType);
            }

            Texture.Unbind();
            ShaderProgram.Unuse();
        }

        /// <summary>
        /// 頂点要素からなるオブジェクトを描画します。
        /// </summary>
        public void RenderElements(Mesh mesh,
                                   PrimitiveType primitiveType)
        {
            VertexBuffer.BeginMesh(mesh);
            GL.DrawElements(
                primitiveType,
                mesh.IndexArray.Length,
                DrawElementsType.UnsignedInt,
                0);
            VertexBuffer.EndMesh();
        }

        /// <summary>
        /// シェーダーにuniform変数を設定します。
        /// </summary>
        private void SetUniform(ShaderProgram shader,
                                Matrix4 projectionMatrix)
        {
            var loc = shader.GetUniformLocation("projectionMatrix");
            if (loc >= 0)
            {
                GL.UniformMatrix4(loc, false, ref projectionMatrix);
            }

            loc = shader.GetUniformLocation("modelViewMatrix");
            if (loc >= 0)
            {
                var values = new float[16];
                for (var i = 0; i < 16; ++i)
                {
                    values[i] = (float)Transform[i % 4, i / 4];
                }
                GL.UniformMatrix4(loc, 1, false, values);
            }

            loc = shader.GetUniformLocation("color");
            if (loc >= 0)
            {
                GL.Uniform4(loc, Color);
            }
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
    }
}
