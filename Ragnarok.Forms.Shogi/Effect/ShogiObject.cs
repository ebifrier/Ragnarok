using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;
using OpenTK;

using Ragnarok.Extra.Effect;

namespace Ragnarok.Forms.Shogi.Effect
{
    /// <summary>
    /// エフェクト用のオブジェクトです。
    /// </summary>
    /// <remarks>
    /// <list type="bullet" | "number">
    ///   <listheader>
    ///     <description><see cref="EffectObject"/>との違い</description>
    ///   </listheader>
    ///   <item>
    ///     <description>DiffuseやEmissiveなどの各種Materialや不透明度に対応</description>
    ///   </item>
    ///   <item>
    ///     <description>画像のアニメーション表示に対応</description>
    ///   </item>
    /// </list>
    /// </remarks>
    public class ShogiObject : VisualEffect
    {
        private readonly FlintSharpEx.GLRenderer renderer;
        private bool needToUpdateTexture;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ShogiObject()
        {
            this.renderer = new FlintSharpEx.GLRenderer(this);
            ParticleRenderer = this.renderer;
            Color = Color.White;
            this.needToUpdateTexture = false;

            AddPropertyChangedHandler("ImageUri",
                (_, __) => this.needToUpdateTexture = true);
            AddPropertyChangedHandler("AnimationImageIndex",
                (_, __) => this.needToUpdateTexture = true);
        }

        #region 基本プロパティ
        /// <summary>
        /// 頂点カラーを取得または設定します。
        /// </summary>
        public Color Color
        {
            get { return GetValue<Color>("Color"); }
            set { SetValue("Color", value); }
        }

        /// <summary>
        /// 描画するテクスチャを取得または設定します。
        /// </summary>
        public GLUtil.Texture Texture
        {
            get { return GetValue<GLUtil.Texture>("Texture"); }
            set { SetValue("Texture", value); }
        }
        #endregion

        /// <summary>
        /// エフェクトのアニメーション画像の位置などを修正します。
        /// </summary>
        private void UpdateTexture()
        {
            if (ImageUri == null)
            {
                Texture = null;
                return;
            }

            var animTexture = GLUtil.TextureCache.GetAnimationTexture(
                MakeContentPath(ImageUri),
                AnimationImageCount);
            if (animTexture == null)
            {
                Texture = null;
                return;
            }

            // メッシュがない場合はデフォルトのメッシュで初期化します。
            if (Mesh == null)
            {
                Mesh = Mesh.CreateDefault(1, 1, 0, 0);
            }

            var list = animTexture.TextureList;
            Texture = list[AnimationImageIndex % list.Count()];
        }

        /// <summary>
        /// フレーム毎の処理を行います。
        /// </summary>
        protected override void OnEnterFrame(EnterFrameEventArgs e)
        {
            // パーティクルの更新前にRenderBufferを設定します。
            var renderBuffer = (GLUtil.RenderBuffer)e.StateObject;
            this.renderer.RenderBuffer = renderBuffer;

            base.OnEnterFrame(e);

            // 必要ならテクスチャの更新を行います。
            if (this.needToUpdateTexture)
            {
                UpdateTexture();
                this.needToUpdateTexture = false;
            }

            if (Mesh != null)
            {
                // 頂点カラーを算出します。
                var a = (byte)(Color.A * InheritedOpacity);
                var color = Color.FromArgb(a, Color);

                // 描画用オブジェクトとして登録します。
                renderBuffer.AddRender(
                    Texture, Blend, color,
                    Mesh, Transform, InheritedZOrder);
            }
        }

        /// <summary>
        /// 描画処理を行います。
        /// </summary>
        protected override void OnRender(EventArgs e)
        {
        }
    }
}
