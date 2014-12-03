using System;
using System.Collections.Generic;
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
        /// コントロールの基準となるZOrder値を取得または設定します。
        /// </summary>
        public double BaseZOrder
        {
            get { return GetValue<double>("BaseZOrder"); }
            set { SetValue("BaseZOrder", value); }
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
        }

        /// <summary>
        /// マウス移動時に呼ばれます。
        /// </summary>
        public virtual void OnMouseMove(MouseEventArgs e)
        {
        }

        /// <summary>
        /// マウスボタンの押下時に呼ばれます。
        /// </summary>
        public virtual void OnMouseUp(MouseEventArgs e)
        {
        }
    }
}
