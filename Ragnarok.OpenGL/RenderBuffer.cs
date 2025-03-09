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
    public sealed class RenderBuffer : GLObject
    {
        public static readonly string VertexShaderSource = @"
#version 330 core
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoord;
uniform mat4 projectionMatrix;
uniform mat4 modelViewMatrix;
out vec2 TexCoord;
void main()
{
    gl_Position = projectionMatrix * modelViewMatrix * vec4(aPosition, 1.0);
    TexCoord = aTexCoord;
}";

        public static readonly string TexFragmentShaderSource = @"
#version 330 core
in vec2 TexCoord;
out vec4 FragColor;
uniform sampler2D texture1;
uniform vec4 color;
void main()
{
    FragColor = texture(texture1, TexCoord) * color;
}";

        public static readonly string CopyFragmentShaderSource = @"
#version 330 core
in vec2 TexCoord;
out vec4 FragColor;
uniform sampler2D texture1;
uniform vec4 color;
void main()
{
    vec4 outColor = texture(texture1, TexCoord) * color;
    if (outColor.a < 0.9)
    {
        discard;
    }
    FragColor = outColor;
}";

        public static readonly string ColorFragmentShaderSource = @"
#version 330 core
out vec4 FragColor;
uniform vec4 color;
void main()
{
    FragColor = color;
}";

        private readonly object syncObject = new();
        private readonly List<RenderData> dataList = new();
        private ShaderProgram texShaderProgram;
        private ShaderProgram colorShaderProgram;
        private ShaderProgram copyShaderProgram;
        private VertexBuffer vertexBuffer;

        public RenderBuffer(IGraphicsContext context)
            : base(context)
        {
            this.texShaderProgram = new ShaderProgram(context);
            this.colorShaderProgram = new ShaderProgram(context);
            this.copyShaderProgram = new ShaderProgram(context);
            this.vertexBuffer = new VertexBuffer(context);

            ResizeScreen(640, 360);
        }

        /// <summary>
        /// OpenGLオブジェクトを初期化します。
        /// </summary>
        public void Init()
        {
            if (this.texShaderProgram == null ||
                this.colorShaderProgram == null ||
                this.copyShaderProgram == null ||
                this.vertexBuffer == null)
            {
                throw new ObjectDisposedException(
                    "オブジェクトはすでに破棄されています。");
            }

            this.texShaderProgram.Create(
                VertexShaderSource, TexFragmentShaderSource);
            this.colorShaderProgram.Create(
                VertexShaderSource, ColorFragmentShaderSource);
            this.copyShaderProgram.Create(
                VertexShaderSource, CopyFragmentShaderSource);
            this.vertexBuffer.Create();
        }

        /// <summary>
        /// OpenGLオブジェクトを削除します。
        /// </summary>
        public override void Destroy()
        {
            if (this.texShaderProgram != null)
            {
                this.texShaderProgram.Dispose();
                this.texShaderProgram = null;
            }

            if (this.colorShaderProgram != null)
            {
                this.colorShaderProgram.Dispose();
                this.colorShaderProgram = null;
            }

            if (this.copyShaderProgram != null)
            {
                this.copyShaderProgram.Dispose();
                this.copyShaderProgram = null;
            }

            if (this.vertexBuffer != null)
            {
                this.vertexBuffer.Dispose();
                this.vertexBuffer = null;
            }
        }

        /// <summary>
        /// テクスチャを描画するためのシェーダーを取得します。
        /// </summary>
        public ShaderProgram TexShaderProgram => this.texShaderProgram;

        /// <summary>
        /// 色を描画するためのシェーダーを取得します。
        /// </summary>
        public ShaderProgram ColorShaderProgram => this.colorShaderProgram;

        /// <summary>
        /// コピー時に使うシェーダーを取得します。
        /// </summary>
        /// <remarks>
        /// コピー時はエッジを背景と混ぜないようにするため
        /// 不透明度が低い場合は色を出力しないようにします。
        /// </remarks>
        public ShaderProgram CopyShaderProgram => this.copyShaderProgram;

        /// <summary>
        /// 頂点シェーダーを取得します。
        /// </summary>
        public VertexBuffer VertexBuffer => this.vertexBuffer;

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
        public void AddRender(RenderAction renderAction,
                              BlendType blend,
                              Matrix44d transform,
                              double zorder,
                              Color? color = null,
                              Mesh mesh = null,
                              PrimitiveType primitiveType = PrimitiveType.Triangles,
                              double? opacity = null)
        {
            if (renderAction == null)
            {
                throw new ArgumentNullException(nameof(renderAction));
            }

            var ncolor = color ?? Color.White;
            if (opacity != null)
            {
                var alpha = (byte)Math.Min(ncolor.A * opacity.Value, 255);
                ncolor = Color.FromArgb(alpha, ncolor);
            }

            AddRender(new RenderData
            {
                RenderAction = renderAction,
                Blend = blend,
                Color = ncolor,
                Mesh = mesh ?? Mesh.Default,
                PrimitiveType = primitiveType,
                Transform = transform,
                ZOrder = zorder,
                Shader = this.colorShaderProgram,
                VertexBuffer = this.vertexBuffer,
            });
        }

        private ShaderProgram GetShader(BlendType blend, Texture texture)
        {
            if (texture == null)
            {
                return this.colorShaderProgram;
            }

            return blend == BlendType.Copy
                ? this.copyShaderProgram
                : this.texShaderProgram;
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
                Shader = GetShader(blend, texture),
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
                Transform = transform * Matrix44d.FromRectangle(bounds),
                ZOrder = zorder,
                Shader = GetShader(blend, texture),
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
            var transform2 = transform * Matrix44d.FromRectangle(bounds);

            AddRender(blend, transform2, zorder,
                color: color,
                texture: texture);
        }
    }
}
