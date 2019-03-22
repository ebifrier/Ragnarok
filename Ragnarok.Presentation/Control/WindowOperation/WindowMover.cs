using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Ragnarok.Presentation.Control.WindowOperation
{
    /// <summary>
    /// ウィンドウの移動を開始します。
    /// </summary>
    class WindowMoverStarter : IWindowOperationStarter
    {
        private readonly MovableWindow window;

        public override Cursor GetCursor(Point wp)
        {
            return Cursors.Hand;
        }

        public override WindowOperationBase BeginOperation(Point wp)
        {
            var operation = new WindowMover(this.window);

            operation.Begin(wp);
            return operation;
        }

        public WindowMoverStarter(MovableWindow window)
        {
            this.window = window;
        }
    }

    /// <summary>
    /// ウィンドウの移動を行います。
    /// </summary>
    class WindowMover : WindowOperationBase
    {
        private Point relativePoint;

        protected override void OnBegin(Point wp)
        {
            this.relativePoint = wp;
        }

        protected override void OnOperate(Point wp)
        {
            var devicePos = Window.PointToScreen(wp);
            var screenPos = WPFUtil.LogicalFromDevice(devicePos, Window);
            var leftTop = new Point(
                screenPos.X - this.relativePoint.X,
                screenPos.Y - this.relativePoint.Y);

            Window.Left = leftTop.X;
            Window.Top = leftTop.Y;
        }

        public WindowMover(MovableWindow window)
            : base(window)
        {
        }
    }
}
