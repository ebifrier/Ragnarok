using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using FlintSharp.Particles;
using FlintSharp.Renderers;

namespace Ragnarok.Presentation.Extra.FlintSharpEx
{
    using Effect;

    /// <summary>
    /// EffectObjectのパーティクルを描画するためのレンダラです。
    /// </summary>
    [CLSCompliant(false)]
    public class WPFRenderer : Renderer
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WPFRenderer(EffectObject root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            EffectObject = root;
        }

        /// <summary>
        /// 対象となるエフェクトオブジェクトを取得します。
        /// </summary>
        public EffectObject EffectObject
        {
            get;
            private set;
        }

        /// <summary>
        /// モデルのルートオブジェクトを取得します。
        /// </summary>
        private Model3DGroup RootModelGroup
        {
            get { return EffectObject.ModelGroup; }
        }

        /// <summary>
        /// パーティクルを追加します。
        /// </summary>
        [CLSCompliant(false)]
        public override void AddParticle(Particle particle)
        {
            base.AddParticle(particle);

            var data = particle.ImageData as WPFImageData;
            if (data != null)
            {
                RootModelGroup.Children.Add(data.Model);
            }
        }

        /// <summary>
        /// パーティクルを削除します。
        /// </summary>
        [CLSCompliant(false)]
        public override void RemoveParticle(Particle particle)
        {
            base.RemoveParticle(particle);

            var data = particle.ImageData as WPFImageData;
            if (data != null)
            {
                RootModelGroup.Children.Remove(data.Model);
            }
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
        /// パーティクルの描画設定をまとめて行います。
        /// </summary>
        public override void RenderParticles(IEnumerable<Particle> particles)
        {
            base.RenderParticles(particles);

            foreach (var particle in particles)
            {
                var data = particle.ImageData as WPFImageData;
                if (data == null)
                {
                    continue;
                }

                if (data.Brush != null)
                {
                    var opacity = (double)((particle.Color >> 24) & 0xFF) / 255.0;

                    data.Brush.Opacity = EffectObject.InheritedOpacity * opacity;
                }

                if (data.Material != null)
                {
                    var em = data.Material as EmissiveMaterial;
                    if (em != null)
                    {
                        em.Color = ToColor(particle.Color);
                    }
                    else
                    {
                        var dm = data.Material as DiffuseMaterial;
                        if (dm != null)
                        {
                            dm.Color = ToColor(particle.Color);
                        }
                    }
                }

                // 行列変換
                var m = new Matrix3D();

                if (particle.Scale != 1.0)
                {
                    m.Scale(new Vector3D(particle.Scale, particle.Scale, 1.0));
                }

                if (particle.Rotation != 0.0)
                {
                    double rot = MathEx.ToDeg(particle.Rotation);
                    m.Rotate(new Quaternion(new Vector3D(0, 0, 1), rot));
                }

                m.Translate(new Vector3D(particle.X, particle.Y, -15.0));

                data.Model.Transform = new MatrixTransform3D(m);
            }
        }
    }
}
