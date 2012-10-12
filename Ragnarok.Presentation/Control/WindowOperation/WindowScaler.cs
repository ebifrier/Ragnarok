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
            var edge = GetWindowEdge(wp);
            if (edge == WindowEdge.None)
            {
                return null;
            }

            var operation = new WindowScaler(this.window, edge);
            operation.Begin(wp);
            return operation;
        }

        public WindowScalerStarter(MovableWindow window)
        {
            this.window = window;
        }
    }

    /// <summary>
    /// ウィンドウのリサイズを行います。
    /// </summary>
    class WindowScaler : WindowOperationBase
    {
        private readonly WindowEdge edge;
        private Point relativePoint;

        protected override void OnBegin(Point wp)
        {
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
            var screenPos = Window.PointToScreen(wp);
            var l = Window.Left;
            var t = Window.Top;
            var r = l + Window.Width;
            var b = t + Window.Height;

            if ((this.edge & WindowEdge.Left) != 0)
            {
                l = screenPos.X - this.relativePoint.X;

                if (r - l < 32)
                {
                    l = r - 32;
                }
            }
            else if ((this.edge & WindowEdge.Right) != 0)
            {
                r = screenPos.X - this.relativePoint.X;

                if (r - l < 32)
                {
                    r = l + 32;
                }
            }

            if ((this.edge & WindowEdge.Top) != 0)
            {
                t = screenPos.Y - this.relativePoint.Y;

                if (b - t < 32)
                {
                    t = b - 32;
                }
            }
            else if ((this.edge & WindowEdge.Bottom) != 0)
            {
                b = screenPos.Y - this.relativePoint.Y;

                if (b - t < 32)
                {
                    b = t + 32;
                }
            }

            Window.Left = l;
            Window.Top = t;
            Window.Width = r - l;
            Window.Height = b - t;
        }

        public WindowScaler(MovableWindow window, WindowEdge edge)
            : base(window)
        {
            this.edge = edge;
        }
    }
}
