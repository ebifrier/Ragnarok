using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Ragnarok.Extra.Effect;
using Ragnarok.Utility;

namespace Ragnarok.Forms.Shogi.GLUtil
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
        /// 登録されたデータをまとめて描画します。
        /// </summary>
        public void Render()
        {
            lock(this.syncObject)
            {
                var list =
                    from data in this.dataList
                    orderby data.ZOrder
                    select data;

                list.ForEach(_ => _.Render());
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
        /// RectangleF型を同じ変換を行う行列に直します。
        /// </summary>
        /// <param name="transform">
        /// 事前に積算する基準となる行列です。
        /// </param>
        private static Matrix44d ToMatrix(RectangleF bounds, Matrix44d transform)
        {
            var m = (transform != null ? transform.Clone() : new Matrix44d());
            m.Translate(
                bounds.Left + bounds.Width / 2,
                bounds.Top + bounds.Height / 2,
                0.0);
            m.Scale(bounds.Width, bounds.Height, 1.0);
            return m;
        }

        /// <summary>
        /// 描画用リストを追加します。
        /// </summary>
        public void AddRenderAction(Action renderAction, double zorder)
        {
            if (renderAction == null)
            {
                throw new ArgumentNullException("renderAction");
            }

            lock (this.syncObject)
            {
                // ZOrder値に下駄をはかせます。
                var data = new RenderData
                {
                    RenderAction = renderAction,
                    ZOrder = BaseZOrder + zorder,
                };

                this.dataList.Add(data);
            }
        }

        /// <summary>
        /// 描画オブジェクトを追加します。
        /// </summary>
        private void AddRenderInternal(RenderData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            lock (this.syncObject)
            {
                // ZOrder値に下駄をはかせます。
                data.ZOrder += BaseZOrder;

                this.dataList.Add(data);
            }
        }

        /// <summary>
        /// 描画オブジェクトを追加します。
        /// </summary>
        public void AddRender(Texture texture, BlendType blend, Color color,
                              Mesh mesh, Matrix44d transform, double zorder)
        {
            if (mesh == null)
            {
                throw new ArgumentNullException("mesh");
            }

            AddRenderInternal(new RenderData
            {
                Texture = texture,
                Blend = blend,
                Color = color,
                Mesh = mesh,
                Transform = transform,
                ZOrder = zorder,
            });
        }

        /// <summary>
        /// 描画オブジェクトを追加します。
        /// </summary>
        public void AddRender(Texture texture, BlendType blend, Color color,
                              Matrix44d transform, double zorder)
        {
            AddRenderInternal(new RenderData
            {
                Mesh = DefaultMesh,
                Texture = texture,
                Blend = blend,
                Color = color,
                Transform = transform,
                ZOrder = zorder,
            });
        }

        /// <summary>
        /// 描画オブジェクトを追加します。
        /// </summary>
        public void AddRender(BlendType blend, Color color, Mesh mesh,
                              Matrix44d transform, double zorder)
        {
            if (mesh == null)
            {
                throw new ArgumentNullException("mesh");
            }

            AddRenderInternal(new RenderData
            {
                Blend = blend,
                Color = color,
                Mesh = mesh,
                Transform = transform,
                ZOrder = zorder,
            });
        }

        /// <summary>
        /// 描画オブジェクトを追加します。
        /// </summary>
        public void AddRender(BlendType blend, Color color,
                              Matrix44d transform, double zorder)
        {
            AddRenderInternal(new RenderData
            {
                Mesh = DefaultMesh,
                Blend = blend,
                Color = color,
                Transform = transform,
                ZOrder = zorder,
            });
        }

        /// <summary>
        /// 描画オブジェクトを追加します。
        /// </summary>
        public void AddRender(BlendType blend, Color color,
                              RectangleF bounds, Matrix44d transform,
                              double zorder, double opacity = 1.0)
        {
            var alphaByte = (byte)Math.Min(color.A * opacity, 255);
            var color2 = Color.FromArgb(alphaByte, color);
            var transform2 = ToMatrix(bounds, transform);

            AddRender(blend, color2, transform2, zorder);
        }

        /// <summary>
        /// 描画オブジェクトを追加します。
        /// </summary>
        public void AddRender(GLUtil.Texture texture, BlendType blend,
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

            AddRender(texture, blend, color, transform2, zorder);
        }

        /// <summary>
        /// 描画オブジェクトを追加します。
        /// </summary>
        public void AddRender(GLUtil.Texture texture, BlendType blend,
                              RectangleF bounds, Matrix44d transform,
                              Mesh mesh, double zorder,
                              double opacity = 1.0)
        {
            if (texture == null || texture.TextureName == 0)
            {
                return;
            }

            var alphaByte = (byte)Math.Min(256 * opacity, 255);
            var color = Color.FromArgb(alphaByte, Color.White);
            var transform2 = ToMatrix(bounds, transform);

            AddRender(texture, blend, color, mesh, transform2, zorder);
        }

        /// <summary>
        /// 描画オブジェクトを追加します。
        /// </summary>
        public void AddRender(BlendType blend, RectangleF bounds,
                              Matrix44d transform, Color color, double zorder)
        {
            var transform2 = ToMatrix(bounds, transform);

            AddRender(blend, color, transform2, zorder);
        }
    }
}
