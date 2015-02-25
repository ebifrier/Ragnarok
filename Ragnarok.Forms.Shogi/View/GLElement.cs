using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpGL;

using Ragnarok.ObjectModel;
using Ragnarok.Extra.Effect;

namespace Ragnarok.Forms.Shogi.View
{
    using Effect;

    /// <summary>
    /// GLContainer配下となるコントロールの基本インターフェースです。
    /// </summary>
    public class GLElement : EffectObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GLElement()
        {
            // 表示期間はすごく長く設定します。
            Duration = TimeSpan.MaxValue;

            IsVisible = true;
            BaseZOrder = 0.0;
        }

        /// <summary>
        /// OpenGLオブジェクトの初期化を行います。
        /// </summary>
        internal void InitializeOpenGL(GLContainer container)
        {
            GLContainer = container;

            if (container != null)
            {
                OnOpenGLInitialized(EventArgs.Empty);

                ForeachChildren(_ => _.InitializeOpenGL(GLContainer));
            }
        }

        /// <summary>
        /// OpenGL用のコンテナオブジェクトを取得します。
        /// </summary>
        [CLSCompliant(false)]
        public GLContainer GLContainer
        {
            get { return GetValue<GLContainer>("GLContainer"); }
            private set { SetValue("GLContainer", value); }
        }

        /// <summary>
        /// OpenGLオブジェクトを取得または設定します。
        /// </summary>
        [CLSCompliant(false)]
        [DependOnProperty("GLContainer")]
        public OpenGL OpenGL
        {
            get
            {
                if (GLContainer == null)
                {
                    return null;
                }

                return GLContainer.OpenGL;
            }
        }

        /// <summary>
        /// コントロールの表示を行うかどうかを取得または設定します。
        /// </summary>
        public bool IsVisible
        {
            get
            {
                var parent = Parent as GLElement;
                if (parent != null)
                {
                    return parent.IsVisible;
                }

                return GetValue<bool>("IsVisible");
            }
            set
            {
                SetValue("IsVisible", value);
            }
        }

        /// <summary>
        /// コントロールの基準となるZOrder値を取得または設定します。
        /// </summary>
        public double BaseZOrder
        {
            get { return GetValue<double>("BaseZOrder"); }
            set { SetValue("BaseZOrder", value); }
        }

        /// <summary>
        /// クライアント座標を(1,1)のローカル座標系に変換します。
        /// </summary>
        public PointF ClientToLocal(PointF p)
        {
            if (GLContainer == null)
            {
                throw new InvalidOperationException(
                    "親コンテナに追加されていません。");
            }

            var m = Transform.Invert();
            var s = GLContainer.ClientSize;
            var np = new PointF(p.X * 640 / s.Width, p.Y * 360 / s.Height);

            return new PointF(
                (float)(np.X * m[0, 0] + np.Y * m[0, 1] + m[0, 3]),
                (float)(np.X * m[1, 0] + np.Y * m[1, 1] + m[1, 3]));
        }

        protected override void OnParentAdded(EffectObject parent)
        {
            base.OnParentAdded(parent);

            var glParent = parent as GLEvaluationElement;
            if (glParent != null && glParent.GLContainer != null)
            {
                InitializeOpenGL(glParent.GLContainer);
            }
        }

        protected override void OnParentRemoved(EffectObject parent)
        {
            base.OnParentRemoved(parent);

            GLContainer = null;
        }

        /// <summary>
        /// GLElement型の子要素にのみアクションを適用します。
        /// </summary>
        private void ForeachChildren(Action<GLElement> action)
        {
            Children.OfType<GLElement>()
                .Where(_ => _ != null)
                .ForEach(_ => action(_));
        }

        /// <summary>
        /// OpenGLの初期化後に呼ばれます。
        /// </summary>
        public virtual void OnOpenGLInitialized(EventArgs e)
        {
        }

        /// <summary>
        /// マウスボタンの押下時に呼ばれます。
        /// </summary>
        public virtual void OnMouseDown(MouseEventArgs e)
        {
            ForeachChildren(_ => _.OnMouseDown(e));
        }

        /// <summary>
        /// マウス移動時に呼ばれます。
        /// </summary>
        public virtual void OnMouseMove(MouseEventArgs e)
        {
            ForeachChildren(_ => _.OnMouseMove(e));
        }

        /// <summary>
        /// マウスボタンの押下時に呼ばれます。
        /// </summary>
        public virtual void OnMouseUp(MouseEventArgs e)
        {
            ForeachChildren(_ => _.OnMouseUp(e));
        }
    }
}
