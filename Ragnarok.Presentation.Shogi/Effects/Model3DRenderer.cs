using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using FlintSharp.Particles;
using FlintSharp.Renderers;

namespace Ragnarok.Presentation.Shogi.Effects
{
    using View;

    /// <summary>
    /// The ParticleCreator is used by the Emitter public class to manage the creation and reuse of particles.
    /// To speed up the particle system, the ParticleCreator public class maintains a pool of dead particles 
    /// and reuses them when a new particle is needed, rather than creating a whole new particle.
    /// </summary>
    [CLSCompliant(false)]
    public class Model3DRenderer : Renderer
    {
        private Model3DGroup m_root;

        /// <summary>
        /// The constructor creates a ParticleCreator object.
        /// </summary>
        public Model3DRenderer(Model3DGroup root)
        {
            m_root = root;
        }

        [CLSCompliant(false)]
        public override void AddParticle(Particle particle)
        {
            base.AddParticle(particle);

            m_root.Children.Add(particle.Model);
        }

        [CLSCompliant(false)]
        public override void RemoveParticle(Particle particle)
        {
            base.RemoveParticle(particle);

            m_root.Children.Remove(particle.Model);
        }

        /// <summary>
        /// Converts ARGB to the Color instance.
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

        public override void RenderParticles(IEnumerable<Particle> particles)
        {
            base.RenderParticles(particles);

            foreach (var particle in particles)
            {
                if (particle.Brush != null)
                    particle.Brush.Opacity = (double)((particle.Color >> 24) & 0xFF) / 255.0;

                if (particle.Material != null)
                {
                    var em = particle.Material as EmissiveMaterial;
                    if (em != null)
                    {
                        em.Color = ToColor(particle.Color);
                    }
                    else
                    {
                        var dm = particle.Material as DiffuseMaterial;
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

                m.Translate(new Vector3D(particle.X, particle.Y, ShogiControl.PostEffectZ));

                particle.Model.Transform = new MatrixTransform3D(m);
            }
        }
    }
}
