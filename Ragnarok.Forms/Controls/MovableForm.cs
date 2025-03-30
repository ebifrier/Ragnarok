using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Ragnarok.Forms.Controls
{
    using WindowOperation;

    public partial class MovableForm : Form
    {
        private IWindowOperationStarter[] starters;
        private WindowOperationBase operation;
        private bool isWindowFixed = false;
        private bool saveAspect = false;

        public MovableForm()
        {
            InitializeComponent();

            this.maximumToolStripMenuItem.Click += (_, __) => SetWindowState(FormWindowState.Maximized);
            this.normalToolStripMenuItem.Click += (_, __) => SetWindowState(FormWindowState.Normal);
            this.toggleFixToolStripMenuItem.Click += (_, __) => IsWindowFixed = !IsWindowFixed;
            this.closeToolStripMenuItem.Click += (_, __) => Close();

            SetWindowState(WindowState);
            SetWindowFixMenuTitle(IsWindowFixed);
            SaveAspectUpdated();
        }

        /// <summary>
        /// WindowStateを設定し、メニューの状態を更新します。
        /// </summary>
        private void SetWindowState(FormWindowState state)
        {
            this.maximumToolStripMenuItem.Enabled = (state != FormWindowState.Maximized);
            this.normalToolStripMenuItem.Enabled = (state != FormWindowState.Normal);
            WindowState = state;
        }

        /// <summary>
        /// startersのリストを更新します。
        /// </summary>
        private void SaveAspectUpdated()
        {
            if (this.operation != null)
            {
                this.operation.End();
                this.operation = null;
            }

            this.starters = (SaveAspect ?
                new IWindowOperationStarter[]
                {
                    new WindowScalerStarter(this, saveAspect:true, withShift:false),
                    new WindowScalerStarter(this, saveAspect:false, withShift:true),
                    new WindowMoverStarter(this),
                } :
                new IWindowOperationStarter[]
                {
                    new WindowScalerStarter(this, saveAspect:false),
                    new WindowMoverStarter(this),
                });
        }

        /// <summary>
        /// 操作可能な和訓幅を取得または設定します。
        /// </summary>
        public int EdgeLength
        {
            get;
            set;
        } = 24;

        /// <summary>
        /// ウィンドウの可動/不動状態を取得または設定します。
        /// </summary>
        public bool IsWindowFixed
        {
            get { return this.isWindowFixed; }
            set
            {
                if (this.isWindowFixed != value)
                {
                    this.isWindowFixed = value;
                    this.operation?.End();
                    this.operation = null;

                    SetWindowFixMenuTitle(this.isWindowFixed);
                }
            }
        }

        private void SetWindowFixMenuTitle(bool isFixed)
        {
            this.toggleFixToolStripMenuItem.Text = isFixed
                ? "ウィンドウを動かせるようにする"
                : "ウィンドウを固定する";
        }

        /// <summary>
        /// リサイズ時にアスペクト比を保存するかどうかを取得または設定します。
        /// </summary>
        public bool SaveAspect
        {
            get { return this.saveAspect; }
            set
            {
                if (this.saveAspect != value)
                {
                    this.saveAspect = value;
                    SaveAspectUpdated();
                }
            }
        }

        /// <summary>
        /// マウスのボタン押下時に呼ばれます。
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (this.operation != null)
            {
                this.operation.End();
                this.operation = null;
                return;
            }

            // 開始可能なオペレーションがあるなら、それを開始します。
            if (e.Button == MouseButtons.Left &&
                WindowState == FormWindowState.Normal &&
                !IsWindowFixed)
            {
                var wp = e.Location;
                foreach (var starter in this.starters)
                {
                    var op = starter.BeginOperation(wp);

                    if (op != null)
                    {
                        this.operation = op;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// マウスの移動時に呼ばれます。
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var wp = e.Location;
            if (this.operation != null)
            {
                this.operation.Operate(wp);
            }
            else if (WindowState == FormWindowState.Normal &&
                     !IsWindowFixed)
            {
                this.ShowCursor();

                // オペレーションがない場合は、
                // 必要に合わせてマウスカーソルを変更します。
                foreach (var starter in this.starters)
                {
                    var cursor = starter.GetCursor(wp);

                    if (cursor != null)
                    {
                        Cursor = cursor;
                        break;
                    }
                }
            }
            else
            {
                this.HideCursor();
            }
        }

        /// <summary>
        /// マウスの左ボタン押下後に呼ばれます。
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (this.operation != null)
            {
                this.operation.End();
                this.operation = null;
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (e.Button == MouseButtons.Left && !IsWindowFixed)
            {
                SetWindowState(WindowState == FormWindowState.Normal
                    ? FormWindowState.Maximized
                    : FormWindowState.Normal);
            }
        }
    }
}
