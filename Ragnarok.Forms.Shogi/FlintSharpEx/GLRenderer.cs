using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

using FlintSharp;
using FlintSharp.Particles;
using FlintSharp.Renderers;
using OpenTK;

using Ragnarok.Extra.Effect;
using Ragnarok.Utility;

namespace Ragnarok.Forms.Shogi.FlintSharpEx
{
    using Effect;

    /// <summary>
    /// EffectObjectのパーティクルを描画するためのレンダラです。
    /// </summary>
    [CLSCompliant(false)]
    public sealed class GLRenderer : Renderer
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GLRenderer(ShogiObject root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            ShogiObject = root;
        }

        /// <summary>
        /// 対象となるエフェクトオブジェクトを取得します。
        /// </summary>
        public ShogiObject ShogiObject
        {
            get;
            private set;
        }

        /// <summary>
        /// OpenGLへ描画するためのオブジェクトを取得または設定します。
        /// </summary>
        /// <remarks>
        /// ちょっと変ですが、毎フレームごとにRenderParticlesが呼ばれる前に設定します。
        /// </remarks>
        public GLUtil.RenderBuffer RenderBuffer
        {
            get;
            set;
        }

        /// <summary>
        /// uint型の色形式をColorに変換します。
        /// </summary>
        [CLSCompliant(false)]
        public static Color ToColor(uint value)
        {
            return Color.FromArgb(
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)((value >> 0) & 0xFF));
        }

        /// <summary>
        /// MaterialTypeをBlendTypeに変換します。
        /// </summary>
        public static BlendType GetBlend(MaterialType materialType)
        {
            switch (materialType)
            {
                case MaterialType.Diffuse:
                    return BlendType.Diffuse;
                case MaterialType.Emissive:
                    return BlendType.Emissive;
            }

            throw new NotSupportedException(
                "対応していないMaterialTypeです。");
        }

        /// <summary>
        /// パーティクルの描画設定をまとめて行います。
        /// </summary>
        public override void UpdateParticles(IEnumerable<Particle> particles)
        {
            base.UpdateParticles(particles);

            if (RenderBuffer == null)
            {
                throw new InvalidOperationException(
                    "RenderBufferが設定されていません。");
            }

            var inheritedOpacity = ShogiObject.InheritedOpacity;

            foreach (var particle in particles)
            {
                var data = particle.ImageData as GLImageData;
                if (data == null)
                {
                    continue;
                }

                // とりあえず必要なデータの作成を行います。
                data.Initialize();

                for (var i = 0; i < (data.IsDoubleParticle ? 2 : 1); ++i)
                {
                    var color = Color.White;

                    // 二重パーティクルの場合は、画像を２回描画します。
                    if (data.IsDoubleParticle && i == 0)
                    {
                        // 二重パーティクル前段の白いエフェクト
                        // 頂点カラーは不透明な白
                        color = Color.FromArgb(
                            (byte)(particle.Color.A * inheritedOpacity * 0.7),
                            Color.White);
                    }
                    else
                    {
                        // 通常のテクスチャ描画
                        color = Color.FromArgb(
                            (byte)(particle.Color.A * inheritedOpacity),
                            particle.Color.R,
                            particle.Color.G,
                            particle.Color.B);
                    }

                    var m = ShogiObject.Transform;
                    m.Translate(particle.X, particle.Y, 0.0);
                    m.Rotate(particle.Rotation, 0.0, 0.0, 1.0);
                    m.Scale(particle.Scale, particle.Scale, 1.0);

                    RenderBuffer.AddRender(
                        data.Texture, GetBlend(data.MaterialType), color,
                        m, ShogiZOrder.EffectZ);
                }
            }
        }
    }
}
