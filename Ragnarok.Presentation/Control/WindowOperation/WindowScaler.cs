using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Ragnarok.Presentation.Control.WindowOperation
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
        private readonly MovableWindow window;
        private readonly bool saveAspect;
        private readonly bool? withShift;

        /// <summary>
        /// ウィンドウ上のエッジ位置を取得します。
        /// </summary>
        private WindowEdge GetWindowEdge(Point wp)
        {
            var edge = WindowEdge.None;
            var width = this.window.Width;
            var height = this.window.Height;

            if (Math.Abs(wp.X) < this.window.EdgeLength)
            {
                edge |= WindowEdge.Left;
            }
            else if (Math.Abs(width - wp.X) < this.window.EdgeLength)
            {
                edge |= WindowEdge.Right;
            }

            if (Math.Abs(wp.Y) < this.window.EdgeLength)
            {
                edge |= WindowEdge.Top;
            }
            else if (Math.Abs(height - wp.Y) < this.window.EdgeLength)
            {
                edge |= WindowEdge.Bottom;
            }

            return edge;
        }

        public override Cursor GetCursor(Point wp)
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

        public override WindowOperationBase BeginOperation(Point wp)
        {
            // shiftキーが押されている必要がある場合
            if (this.withShift != null)
            {
                var isShiftDown = Keyboard.IsKeyDown(Key.LeftShift) |
                                  Keyboard.IsKeyDown(Key.RightShift);

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

            var operation = new WindowScaler(this.window, this.saveAspect, edge);
            operation.Begin(wp);
            return operation;
        }

        public WindowScalerStarter(MovableWindow window, bool saveAspect, bool? withShift=null)
        {
            this.window = window;
            this.saveAspect = saveAspect;
            this.withShift = withShift;
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

        protected override void OnBegin(Point wp)
        {
            this.aspectRatio = Window.Width / Window.Height;
            this.relativePoint = wp;

            if ((this.edge & WindowEdge.Right) != 0)
            {
                this.relativePoint.X = wp.X - Window.Width;
            }

            if ((this.edge & WindowEdge.Bottom) != 0)
            {
                this.relativePoint.Y = wp.Y - Window.Height;
            }
        }

        protected override void OnOperate(Point wp)
        {
            var devicePos = Window.PointToScreen(wp);
            var screenPos = WPFUtil.LogicalFromDevice(devicePos, Window);

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
        /// アスペクト比に関係なくウィンドウをリサイズします。
        /// </summary>
        private void ResizeFreeAspect(Point screenPos)
        {
            var l = Window.Left;
            var t = Window.Top;
            var r = l + Window.Width;
            var b = t + Window.Height;

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
            var l = Window.Left;
            var t = Window.Top;
            var r = l + Window.Width;
            var b = t + Window.Height;
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
                var height = (r - l) / this.aspectRatio;

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

                r = l + (b - t) * this.aspectRatio;
            }

            SetBounds(l, t, r, b);
        }

        /// <summary>
        /// ウィンドウの上下左右の位置を設定します。
        /// </summary>
        private void SetBounds(double l, double t, double r, double b)
        {
            Window.Left = l;
            Window.Top = t;
            Window.Width = r - l;
            Window.Height = b - t;
        }

        public WindowScaler(MovableWindow window, bool saveAspect, WindowEdge edge)
            : base(window)
        {
            this.saveAspect = saveAspect;
            this.edge = edge;
        }
    }
}
