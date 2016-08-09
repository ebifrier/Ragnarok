using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

using Ragnarok.ObjectModel;
using Ragnarok.OpenGL;

namespace Ragnarok.Forms.Shogi.View
{
    /// <summary>
    /// GLElementを管理するためのクラスです。
    /// </summary>
    /// <remarks>
    /// デザインモード時にOpenGL系の関数を呼ぶと、VS自体が落ちます。
    /// </remarks>
    /// <see cref="GLElement"/>
    public partial class GLContainer : GLControl
    {
        private readonly NotifyCollection<GLElement> glElements =
            new NotifyCollection<GLElement>();
        private readonly RenderBuffer renderBuffer = new RenderBuffer();
        private Color clearColor = Color.Transparent;
        private int screenWidth = 640;
        private int screenHeight = 360;
        private bool glInitialized;
        private bool isDesignModeCache;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GLContainer()
        {
            InitializeComponent();

            this.glElements.CollectionChanged += glElements_CollectionChanged;
        }

        /// <summary>
        /// OpenGLの初期設定を行います。
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
 	        base.OnLoad(e);

            if (IsDesignMode)
            {
                return;
            }

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            // 背景色の更新
            BackColorChanged += OnBackColorChanged;

            GLElements
                .Where(_ => _ != null)
                .ForEach(_ => _.InitializeOpenGL(this));
            this.glInitialized = true;

            // Viewportを更新しておきます。
            UpdateViewport();
        }

        /// <summary>
        /// コレクション変更時に呼ばれます。
        /// </summary>
        void glElements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDesignMode || !this.glInitialized)
            {
                // 未初期化の時は初期化時に同じ処理が行われます。
                return;
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    var glelem = item as GLElement;
                    if (glelem != null)
                    {
                        glelem.InitializeOpenGL(this);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    var glelem = item as GLElement;
                    if (glelem != null)
                    {
                        glelem.InitializeOpenGL(null);
                    }
                }
            }
        }

        /// <summary>
        /// ハンドルが閉じられたときに呼ばれます。
        /// </summary>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (IsDesignMode)
            {
                base.OnHandleDestroyed(e);
                return;
            }

            GLElements
                .Where(_ => _ != null)
                .ForEach(_ => _.Terminate());
            GLElements.Clear();

            // このOpenGLに登録されているすべてのテクスチャを削除します。
            if (this.glInitialized)
            {
                Texture.DeleteAll(Context);
            }

            base.OnHandleDestroyed(e);
        }

        /// <summary>
        /// デザインモード時かどうかを取得します。
        /// </summary>
        private bool IsDesignMode
        {
            get
            {
                // Note: the DesignMode property may be incorrect when nesting controls.
                // We use LicenseManager.UsageMode as a workaround (this only works in
                // the constructor).
                this.isDesignModeCache =
                    this.isDesignModeCache || DesignMode ||
                    LicenseManager.UsageMode == LicenseUsageMode.Designtime;

                return this.isDesignModeCache;
            }
        }

        /// <summary>
        /// OpenGL用の画面要素のリストを取得します。
        /// </summary>
        public NotifyCollection<GLElement> GLElements
        {
            get { return this.glElements; }
        }

        /// <summary>
        /// 描画用のオブジェクトを取得します。
        /// </summary>
        public RenderBuffer RenderBuffer
        {
            get { return this.renderBuffer; }
        }

        /// <summary>
        /// OpenGLのClearColorを更新します。
        /// </summary>
        private void OnBackColorChanged(object sender, EventArgs e)
        {
            GL.ClearColor(
                MathEx.Between(0.0f, 1.0f, BackColor.R / 255.0f),
                MathEx.Between(0.0f, 1.0f, BackColor.G / 255.0f),
                MathEx.Between(0.0f, 1.0f, BackColor.B / 255.0f),
                MathEx.Between(0.0f, 1.0f, BackColor.A / 255.0f));
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
                    UpdateViewport();
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
                    UpdateViewport();
                }
            }
        }

        /// <summary>
        /// 投影行列の更新を行います。
        /// </summary>
        private void UpdateViewport()
        {
            if (IsDesignMode || Context == null)
            {
                return;
            }

            GL.Viewport(0, 0, Width, Height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, ScreenWidth, ScreenHeight, 0, -1000, +1000);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        /// <summary>
        /// コントロールのリサイズ時に呼ばれます。
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (IsDesignMode)
            {
                return;
            }

            MakeCurrent();
            UpdateViewport();
        }

        /// <summary>
        /// 毎フレームごとの更新処理を行います。
        /// </summary>
        public void DoEnterFrame(TimeSpan elapsedTime)
        {
            if (IsDesignMode)
            {
                return;
            }

            MakeCurrent();

            this.renderBuffer.Clear();

            GLElements
                .Where(_ => _ != null)
                .ForEach(_ =>
                {
                    // 一時的にZOrderの下駄をはかせます。
                    this.renderBuffer.BaseZOrder = _.BaseZOrder;
                    _.DoEnterFrame(elapsedTime, this.renderBuffer);
                });

            TextureDisposer.Update(Context);
        }

        /// <summary>
        /// 描画処理を行います。
        /// </summary>
        public void DoRender()
        {
            if (IsDesignMode)
            {
                return;
            }

            MakeCurrent();

            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.LoadIdentity();

            this.renderBuffer.Render();

            GLElements
                .Where(_ => _ != null)
                .ForEach(_ => _.DoRender());

            SwapBuffers();
        }

        #region イベント伝達
        /// <summary>
        /// 背景描画は無視します。
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (IsDesignMode)
            {
                base.OnPaintBackground(e);
                return;
            }
        }

        /// <summary>
        /// 再描画時はDoRenderを呼びます。
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (IsDesignMode)
            {
                base.OnPaint(e);
                return;
            }

            //DoRender();
        }

        /// <summary>
        /// マウスボタンの押下時に呼ばれます。
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            GLElements
                .Where(_ => _ != null)
                .ForEach(_ => _.OnMouseDown(e));
        }

        /// <summary>
        /// マウスの移動時に呼ばれます。
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            GLElements
                .Where(_ => _ != null)
                .ForEach(_ => _.OnMouseMove(e));
        }

        /// <summary>
        /// マウスボタンが離されたときに呼ばれます。
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            GLElements
                .Where(_ => _ != null)
                .ForEach(_ => _.OnMouseUp(e));
        }
        #endregion
    }
}
