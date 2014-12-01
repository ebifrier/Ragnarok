using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SharpGL;

using Ragnarok.Extra.Effect;

namespace Ragnarok.Forms.Shogi.View
{
    /// <summary>
    /// GLContainer配下となるコントロールの基本インターフェースです。
    /// </summary>
    [CLSCompliant(false)]
    public class GLElement
    {
        private GLContainer container;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GLElement()
        {
        }

        /// <summary>
        /// OpenGLオブジェクトの初期化を行います。
        /// </summary>
        public void Initialize(GLContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            GLContainer = container;
            OnOpenGLInitialized(EventArgs.Empty);
        }

        /// <summary>
        /// OpenGL用のコンテナオブジェクトを取得します。
        /// </summary>
        public GLContainer GLContainer
        {
            get { return this.container; }
            private set { this.container = value; }
        }

        /// <summary>
        /// OpenGLオブジェクトを取得または設定します。
        /// </summary>
        public OpenGL OpenGL
        {
            get { return this.container.OpenGL; }
        }

        /// <summary>
        /// 描画用のオブジェクトを取得します。
        /// </summary>
        public GL.RenderBuffer RenderBuffer
        {
            get { return this.container.RenderBuffer; }
        }

        /// <summary>
        /// コントロールが閉じられたときに呼ばれます。
        /// </summary>
        public virtual void OnClosed(EventArgs e)
        {
        }

        /// <summary>
        /// OpenGLの初期化後に呼ばれます。
        /// </summary>
        public virtual void OnOpenGLInitialized(EventArgs e)
        {
        }
        
        /// <summary>
        /// 描画のタイミングで呼ばれます。
        /// </summary>
        public virtual void OnOpenGLDraw(RenderEventArgs e)
        {
        }
        
        /// <summary>
        /// 毎フレームごとの更新処理を行います。
        /// </summary>
        public virtual void OnEnterFrame(TimeSpan elapsedTime)
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
