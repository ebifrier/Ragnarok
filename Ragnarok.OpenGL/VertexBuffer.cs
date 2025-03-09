using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL;

using Ragnarok.Extra.Effect;

namespace Ragnarok.OpenGL
{
    /// <summary>
    /// VertexBufferやIndexBufferをまとめて管理します。
    /// </summary>
    /// <remarks>
    /// 正確にはOpenGLのVertexBufferをまとめて管理する
    /// VertexArrayObject(VAO)に当たるクラスになります。
    /// ただ、VertexArrayは通常のVertex配列と区別しにくいため
    /// このような名前にしています。
    /// </remarks>
    public sealed class VertexBuffer : GLObject
    {
        private int glVertexArrayName;
        private int glVertexBufferName;
        private int glIndexBufferName;

        /// <summary>
        /// 頂点データを不用意に変更しないようにするため、
        /// すでに設定したMeshを記憶するようにしています。
        /// </summary>
        private Mesh currentMesh;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public VertexBuffer(IGraphicsContext context)
            : base(context)
        {
        }

        /// <summary>
        /// オブジェクトをすべて破棄します。
        /// </summary>
        public override void Destroy()
        {
            var vertexBufferName = this.glVertexBufferName;
            var indexBufferName = this.glIndexBufferName;
            var vertexArrayName = this.glVertexArrayName;

            GLDisposer.AddTarget(Context,
                () =>
                {
                    if (vertexBufferName != 0)
                    {
                        GL.DeleteBuffer(vertexBufferName);
                    }

                    if (indexBufferName != 0)
                    {
                        GL.DeleteBuffer(indexBufferName);
                    }

                    if (vertexArrayName != 0)
                    {
                        GL.DeleteVertexArray(vertexArrayName);
                    }
                });

            this.currentMesh = null;
            this.glVertexBufferName = 0;
            this.glIndexBufferName = 0;
            this.glVertexArrayName = 0;
        }

        /// <summary>
        /// OpenGLのVertexArrayオブジェクトを作成します。
        /// </summary>
        public void Create()
        {
            ValidateContext();

            int arrayName = 0;
            int vertexName = 0;
            int indexName = 0;
            try
            {
                arrayName = GL.GenVertexArray();
                vertexName = GL.GenBuffer();
                indexName = GL.GenBuffer();

                GL.BindVertexArray(arrayName);

                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexName);
                GL.VertexAttribPointer(0, 3,
                    VertexAttribPointerType.Float,
                    false,
                    5 * sizeof(float),
                    0);
                GL.EnableVertexAttribArray(0);

                GL.VertexAttribPointer(1, 2,
                    VertexAttribPointerType.Float,
                    false,
                    5 * sizeof(float),
                    3 * sizeof(float));
                GL.EnableVertexAttribArray(1);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindVertexArray(0);

                Destroy();
                this.glVertexArrayName = arrayName;
                this.glVertexBufferName = vertexName;
                this.glIndexBufferName = indexName;
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "VertexBufferの作成に失敗しました。");

                if (indexName != 0)
                {
                    GL.DeleteBuffer(indexName);
                }

                if (vertexName != 0)
                {
                    GL.DeleteBuffer(vertexName);
                }

                if (arrayName != 0)
                {
                    GL.DeleteVertexArray(arrayName);
                }
            }
        }

        public void BeginMesh(Mesh mesh)
        {
            if (this.glVertexArrayName == 0 ||
                this.glVertexBufferName == 0 ||
                this.glIndexBufferName == 0)
            {
                throw new GLException(
                    "VertexBufferの初期化が完了していません。");
            }

            ValidateContext();

            GL.BindVertexArray(this.glVertexArrayName);

            GL.BindBuffer(BufferTarget.ArrayBuffer,
                this.glVertexBufferName);
            if (mesh != null && !Equals(mesh, this.currentMesh))
            {
                GL.BufferData(BufferTarget.ArrayBuffer,
                    mesh.VertexArray.Length * sizeof(float),
                    mesh.VertexArray,
                    BufferUsageHint.DynamicDraw);
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer,
                this.glIndexBufferName);
            if (mesh != null && !Equals(mesh, this.currentMesh))
            {
                GL.BufferData(BufferTarget.ElementArrayBuffer,
                    mesh.IndexArray.Length * sizeof(int),
                    mesh.IndexArray,
                    BufferUsageHint.DynamicDraw);
            }

            this.currentMesh = mesh;
        }

        public void EndMesh()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }
    }
}
