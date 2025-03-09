using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;

using Ragnarok.Extra.Effect;
using Ragnarok.MathEx;

namespace Ragnarok.OpenGL
{
    /// <summary>
    /// オブジェクトの描画順序をzorder順にするためのクラスです。
    /// </summary>
    public sealed class RenderBuffer
    {
        /// <summary>
        /// デフォルトのメッシュデータです。
        /// </summary>
        private static readonly Mesh DefaultMesh = Mesh.CreateDefault(1, 1, 0, 0);

        private readonly object syncObject = new object();
        private readonly List<RenderData> dataList = new List<RenderData>();

        /// <summary>
        /// 基準となるZOrder値を取得または設定します。
        /// </summary>
        /// <remarks>
        /// この値は後のAddRenderで使用され、
        /// 以降はこの値を基準にZOrderの値を設定します。
        /// </remarks>
        public double BaseZOrder
        {
            get;
            set;
        }

        /// <summary>
        /// Projection行列を取得します。
        /// </summary>
        public Matrix4 ProjectionMatrix
        {
            get;
            private set;
        }

        /// <summary>
        /// スクリーンサイズを修正し、Projection行列を更新します。
        /// </summary>
        public void ResizeScreen(int screenWidth, int screenHeight)
        {
            lock (this.syncObject)
            {
                ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(
                    0, screenWidth, screenHeight, 0, -1000, +1000);
            }
        }

        /// <summary>
        /// バッファ内容をすべてクリアします。
        /// </summary>
        public void Clear()
        {
            lock (this.syncObject)
            {
                this.dataList.Clear();
            }
        }

        /// <summary>
        /// 登録されたデータをまとめて描画します。
        /// </summary>
        public void Render()
        {
            lock (this.syncObject)
            {
                var list =
                    from data in this.dataList
                    orderby data.ZOrder
                    select data;

                list.ForEach(_ => _.Render(ProjectionMatrix));
            }
        }

        /// <summary>
        /// RectangleF型を同じ変換を行う行列に直します。
        /// </summary>
        /// <param name="transform">
        /// 事前に積算する基準となる行列です。
        /// </param>
        private static Matrix44d ToMatrix(RectangleF bounds, Matrix44d transform)
        {
            var m = transform != null
                ? transform.Clone()
                : new Matrix44d();
            m.Translate(
                bounds.Left + bounds.Width / 2,
                bounds.Top + bounds.Height / 2,
                0.0);
            m.Scale(bounds.Width, bounds.Height, 1.0);
            return m;
        }

        /// <summary>
        /// 描画オブジェクトを追加します。
        /// </summary>
        public void AddRender(RenderData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            lock (this.syncObject)
            {
                // ZOrder値に下駄をはかせます。
                data.ZOrder += BaseZOrder;

                this.dataList.Add(data);
            }
        }

        /// <summary>
        /// 描画用リストを追加します。
        /// </summary>
        public void AddRenderAction(RenderAction renderAction, double zorder)
        {
            if (renderAction == null)
            {
                throw new ArgumentNullException(nameof(renderAction));
            }

            AddRender(new RenderData
            {
                RenderAction = renderAction,
                Transform = new Matrix44d(),
                Mesh = Mesh.Default,
                ZOrder = zorder,
                Shader = this.colorShaderProgram,
                VertexBuffer = this.vertexBuffer,
            });
        }

        /// <summary>
        /// 描画オブジェクトを追加します。
        /// </summary>
        public void AddRender(BlendType blend,
                              Matrix44d transform,
                              double zorder,
                              Color? color = null,
                              Texture texture = null,
                              Mesh mesh = null,
                              PrimitiveType primitiveType = PrimitiveType.Triangles,
                              double? opacity = null)
        {
            if (texture != null && texture.TextureName == 0)
            {
                throw new ArgumentException(
                    "invalid argument", nameof(texture));
            }

            var ncolor = color ?? Color.White;
            if (opacity != null)
            {
                var alpha = (byte)Math.Min(ncolor.A * opacity.Value, 255);
                ncolor = Color.FromArgb(alpha, ncolor);
            }

            AddRender(new RenderData
            {
                Texture = texture,
                Blend = blend,
                Color = ncolor,
                Mesh = mesh ?? Mesh.Default,
                PrimitiveType = primitiveType,
                Transform = transform,
                ZOrder = zorder,
                Shader = texture != null
                    ? this.texShaderProgram
                    : this.colorShaderProgram,
                VertexBuffer = this.vertexBuffer,
            });
        }

        /// <summary>
        /// 描画オブジェクトを追加します。
        /// </summary>
        public void AddRender(BlendType blend,
                              RectangleF bounds,
                              Matrix44d transform,
                              double zorder,
                              Color? color = null,
                              Texture texture = null,
                              Mesh mesh = null,
                              PrimitiveType primitiveType = PrimitiveType.Triangles,
                              double? opacity = null)
        {
            if (texture != null && texture.TextureName == 0)
            {
                throw new ArgumentException(
                    "invalid argument", nameof(texture));
            }

            var ncolor = color ?? Color.White;
            if (opacity != null)
            {
                var alpha = (byte)Math.Min(ncolor.A * opacity.Value, 255);
                ncolor = Color.FromArgb(alpha, ncolor);
            }

            AddRender(new RenderData
            {
                Texture = texture,
                Blend = blend,
                Color = ncolor,
                Mesh = mesh ?? Mesh.Default,
                PrimitiveType = primitiveType,
                Transform = ToMatrix(bounds, transform),
                ZOrder = zorder,
                Shader = texture != null
                    ? this.texShaderProgram
                    : this.colorShaderProgram,
                VertexBuffer = this.vertexBuffer,
            });
        }

        /// <summary>
        /// 描画オブジェクトを追加します。
        /// </summary>
        public void AddRender(Texture texture, BlendType blend,
                              RectangleF bounds, Matrix44d transform,
                              double zorder, double opacity = 1.0)
        {
            if (texture == null || texture.TextureName == 0)
            {
                return;
            }

            var alphaByte = (byte)Math.Min(256 * opacity, 255);
            var color = Color.FromArgb(alphaByte, Color.White);
            var transform2 = ToMatrix(bounds, transform);

            AddRender(blend, transform2, zorder,
                color: color,
                texture: texture);
        }
    }
}
