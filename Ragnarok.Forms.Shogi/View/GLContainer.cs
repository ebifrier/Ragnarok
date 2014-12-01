using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SharpGL;
using SharpGL.WinForms;

using Ragnarok.Extra.Effect;

namespace Ragnarok.Forms.Shogi.View
{
    [CLSCompliant(false)]
    public partial class GLContainer : OpenGLControl
    {
        private readonly List<GLElement> glElements = new List<GLElement>();
        private readonly GL.RenderBuffer renderBuffer = new GL.RenderBuffer();
        private int screenWidth = 640;
        private int screenHeight = 360;
        private bool glInitialized;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GLContainer()
        {
            InitializeComponent();

            /*ControlAdded += GLContainer_ControlAdded;
            ControlRemoved += GLContainer_ControlRemoved;*/

            OpenGLInitialized += GLContainer_OpenGLInitialized;
            Resized += GLContainer_Resized;
            OpenGLDraw += GLContainer_OpenGLDraw;
        }

        /// <summary>
        /// OpenGLの初期設定を行います。
        /// </summary>
        void GLContainer_OpenGLInitialized(object sender, EventArgs e)
        {
            var gl = OpenGL;

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Enable(OpenGL.GL_BLEND);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.DepthFunc(OpenGL.GL_LEQUAL);
            //gl.ClearDepth(1.0);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.CullFace(OpenGL.GL_BACK);
            gl.Hint(OpenGL.GL_PERSPECTIVE_CORRECTION_HINT, OpenGL.GL_NICEST);

            // 背景は透明色
            gl.ClearColor(0, 0, 0, 0);

            Elements
                .Where(_ => _ != null)
                .ForEach(_ => _.Initialize(this));
            this.glInitialized = true;
        }

        /*void GLContainer_ControlAdded(object sender, ControlEventArgs e)
        {
            var glelem = e.Control as OpenGLElement;

            if (this.glInitialized && glelem != null)
            {
                glelem.InitializeOpenGL(OpenGL);
            }
        }

        void GLContainer_ControlRemoved(object sender, ControlEventArgs e)
        {
            var glelem = e.Control as OpenGLElement;

            if (glelem != null)
            {
                glelem.InitializeOpenGL(null);
            }
        }*/

        /// <summary>
        /// OpenGL用の画面要素のリストを取得します。
        /// </summary>
        public List<GLElement> Elements
        {
            get { return this.glElements; }
        }

        /// <summary>
        /// 描画用のオブジェクトを取得します。
        /// </summary>
        public GL.RenderBuffer RenderBuffer
        {
            get { return this.renderBuffer; }
        }

        /// <summary>
        /// 想定画面サイズを取得または設定します。
        /// </summary>
        public int ScreenWidth
        {
            get { return this.screenWidth; }
            set
            {
                if (this.screenWidth != value)
                {
                    this.screenWidth = value;
                    UpdateProjectionMatrix();
                }
            }
        }

        /// <summary>
        /// 想定画面サイズを取得または設定します。
        /// </summary>
        public int ScreenHeight
        {
            get { return this.screenHeight; }
            set
            {
                if (this.screenHeight != value)
                {
                    this.screenHeight = value;
                    UpdateProjectionMatrix();
                }
            }
        }

        /// <summary>
        /// 投影行列の更新を行います。
        /// </summary>
        private void UpdateProjectionMatrix()
        {
            var gl = OpenGL;

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Ortho(0, ScreenWidth, ScreenHeight, 0, -1000, +1000);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
        }

        void GLContainer_Resized(object sender, EventArgs e)
        {
            UpdateProjectionMatrix();
        }

        /// <summary>
        /// 描画処理を行います。
        /// </summary>
        void GLContainer_OpenGLDraw(object sender, RenderEventArgs e)
        {
            var gl = OpenGL;
            
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();

            this.renderBuffer.Render(gl);

            Elements
                .Where(_ => _ != null)
                .ForEach(_ => _.OnOpenGLDraw(e));

            gl.Flush();
        }

        /// <summary>
        /// 毎フレームごとの更新処理を行います。
        /// </summary>
        public void DoEnterFrame(TimeSpan elapsedTime)
        {
            this.renderBuffer.Clear();

            Elements
                .Where(_ => _ != null)
                .ForEach(_ => _.OnEnterFrame(elapsedTime));
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            Elements
                .Where(_ => _ != null)
                .ForEach(_ => _.OnClosed(e));

            base.OnHandleDestroyed(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Elements
                .Where(_ => _ != null)
                .ForEach(_ => _.OnMouseDown(e));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Elements
                .Where(_ => _ != null)
                .ForEach(_ => _.OnMouseMove(e));
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            Elements
                .Where(_ => _ != null)
                .ForEach(_ => _.OnMouseUp(e));
        }
    }
}
