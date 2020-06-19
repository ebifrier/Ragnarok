using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Ragnarok.Forms.Controls.WindowOperation
{
    /// <summary>
    /// マウスのあるウィンドウ上の位置を判別します。
    /// </summary>
    [Flags]
    enum WindowEdge
    {
        None = 0x00,
        Left = 0x01,
        Top = 0x02,
        Right = 0x04,
        Bottom = 0x08,
        LeftTop = (Left | Top),
        LeftBottom = (Left | Bottom),
        RightTop = (Right | Top),
        RightBottom = (Right | Bottom),
    }

    /// <summary>
    /// ウィンドウのリサイズを開始します。
    /// </summary>
    class WindowScalerStarter : IWindowOperationStarter
    {
        private readonly MovableForm form;
        private readonly bool saveAspect;
        private readonly bool? withShift;

        public WindowScalerStarter(MovableForm window, bool saveAspect, bool? withShift = null)
        {
            this.form = window;
            this.saveAspect = saveAspect;
            this.withShift = withShift;
        }

        /// <summary>
        /// ウィンドウ上のエッジ位置を取得します。
        /// </summary>
        private WindowEdge GetWindowEdge(Point wp)
        {
            var edge = WindowEdge.None;
            var width = this.form.Width;
            var height = this.form.Height;

            if (Math.Abs(wp.X) < this.form.EdgeLength)
            {
                edge |= WindowEdge.Left;
            }
            else if (Math.Abs(width - wp.X) < this.form.EdgeLength)
            {
                edge |= WindowEdge.Right;
            }

            if (Math.Abs(wp.Y) < this.form.EdgeLength)
            {
                edge |= WindowEdge.Top;
            }
            else if (Math.Abs(height - wp.Y) < this.form.EdgeLength)
            {
                edge |= WindowEdge.Bottom;
            }

            return edge;
        }

        public Cursor GetCursor(Point wp)
        {
            var edge = GetWindowEdge(wp);

            switch (edge)
            {
                case WindowEdge.Left:
                case WindowEdge.Right:
                    return Cursors.SizeWE;
                case WindowEdge.Top:
                case WindowEdge.Bottom:
                    return Cursors.SizeNS;
                case WindowEdge.LeftTop:
                case WindowEdge.RightBottom:
                    return Cursors.SizeNWSE;
                case WindowEdge.LeftBottom:
                case WindowEdge.RightTop:
                    return Cursors.SizeNESW;
            }

            return null;
        }

        public WindowOperationBase BeginOperation(Point wp)
        {
            // shiftキーが押されている必要がある場合
            if (this.withShift != null)
            {
                var isShiftDown = Form.ModifierKeys.HasFlag(Keys.Shift);

                if ((this.withShift == true && !isShiftDown) ||
                    (this.withShift == false && isShiftDown))
                {
                    return null;
                }
            }

            var edge = GetWindowEdge(wp);
            if (edge == WindowEdge.None)
            {
                return null;
            }

            var operation = new WindowScaler(this.form, this.saveAspect, edge);
            operation.Begin(wp);
            return operation;
        }
    }

    /// <summary>
    /// ウィンドウのリサイズを行います。
    /// </summary>
    class WindowScaler : WindowOperationBase
    {
        private readonly bool saveAspect;
        private readonly WindowEdge edge;
        private double aspectRatio;
        private Point relativePoint;

        public WindowScaler(MovableForm window, bool saveAspect, WindowEdge edge)
            : base(window)
        {
            this.saveAspect = saveAspect;
            this.edge = edge;
        }

        protected override void OnBegin(Point wp)
        {
            this.aspectRatio = (double)Form.Width / Form.Height;
            this.relativePoint = wp;

            if ((this.edge & WindowEdge.Right) != 0)
            {
                this.relativePoint.X = wp.X - Form.Width;
            }

            if ((this.edge & WindowEdge.Bottom) != 0)
            {
                this.relativePoint.Y = wp.Y - Form.Height;
            }
        }

        protected override void OnOperate(Point wp)
        {
            var screenPos = Form.PointToScreen(wp);

            if (this.saveAspect)
            {
                ResizeFixedAspect(screenPos);
            }
            else
            {
                ResizeFreeAspect(screenPos);
            }
        }

        /// <summary>
        /// ウィンドウの上下左右の位置を設定します。
        /// </summary>
        private void SetBounds(int l, int t, int r, int b)
        {
            Form.Left = l;
            Form.Top = t;
            Form.Width = r - l;
            Form.Height = b - t;
        }

        /// <summary>
        /// アスペクト比に関係なくウィンドウをリサイズします。
        /// </summary>
        private void ResizeFreeAspect(Point screenPos)
        {
            var l = Form.Left;
            var t = Form.Top;
            var r = l + Form.Width;
            var b = t + Form.Height;

            if ((this.edge & WindowEdge.Left) != 0)
            {
                l = screenPos.X - this.relativePoint.X;
                l = Math.Min(l, r - 32);
            }
            else if ((this.edge & WindowEdge.Right) != 0)
            {
                r = screenPos.X - this.relativePoint.X;
                r = Math.Max(r, l + 32);
            }

            if ((this.edge & WindowEdge.Top) != 0)
            {
                t = screenPos.Y - this.relativePoint.Y;
                t = Math.Min(t, b - 32);
            }
            else if ((this.edge & WindowEdge.Bottom) != 0)
            {
                b = screenPos.Y - this.relativePoint.Y;
                b = Math.Max(b, t + 32);
            }

            SetBounds(l, t, r, b);
        }

        /// <summary>
        /// アスペクト比固定でウィンドウをリサイズします。
        /// </summary>
        private void ResizeFixedAspect(Point screenPos)
        {
            var l = Form.Left;
            var t = Form.Top;
            var r = l + Form.Width;
            var b = t + Form.Height;
            var changedWidth = false;

            // 横方向をリサイズ
            if (EnumUtil.HasFlag(this.edge, WindowEdge.Left))
            {
                l = screenPos.X - this.relativePoint.X;
                l = Math.Min(l, r - 32);
                changedWidth = true;
            }
            else if (EnumUtil.HasFlag(this.edge, WindowEdge.Right))
            {
                r = screenPos.X - this.relativePoint.X;
                r = Math.Max(r, l + 32);
                changedWidth = true;
            }

            if (changedWidth)
            {
                // 横方向が変わった場合、高さをアスペクト比に合わせます。
                var height = (int)((r - l) / this.aspectRatio);

                if (EnumUtil.HasFlag(this.edge, WindowEdge.Top))
                {
                    t = b - height;
                }
                else if (EnumUtil.HasFlag(this.edge, WindowEdge.Bottom))
                {
                    b = t + height;
                }
                else
                {
                    b = t + height;
                }
            }
            else
            {
                // 横方向が変わってない場合は、横方向を高さに合わせます。
                if (EnumUtil.HasFlag(this.edge, WindowEdge.Top))
                {
                    t = screenPos.Y - this.relativePoint.Y;
                    t = Math.Min(t, b - 32);
                }
                else if (EnumUtil.HasFlag(this.edge, WindowEdge.Bottom))
                {
                    b = screenPos.Y - this.relativePoint.Y;
                    b = Math.Max(b, t + 32);
                }

                r = l + (int)((b - t) * this.aspectRatio);
            }

            SetBounds(l, t, r, b);
        }
    }
}
