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
    /// オブジェクトの描画順序をzorder順にするためのクラスです。
    /// </summary>
    public sealed class RenderBuffer
    {
        private readonly object syncObject = new();
        private readonly List<RenderData> dataList = new();

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

                list.ForEach(_ => _.Render());
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
            AddRender(blend,
                transform * Matrix44d.FromRectangle(bounds),
                zorder,
                color: Color.FromArgb(alphaByte, Color.White),
                texture: texture);
        }
    }
}
