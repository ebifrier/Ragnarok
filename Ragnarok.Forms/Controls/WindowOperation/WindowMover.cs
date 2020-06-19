using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Ragnarok.Forms.Controls.WindowOperation
{
    /// <summary>
    /// ウィンドウの移動を開始します。
    /// </summary>
    class WindowMoverStarter : IWindowOperationStarter
    {
        private readonly MovableForm form;

        public WindowMoverStarter(MovableForm form)
        {
            this.form = form;
        }

        public Cursor GetCursor(Point wp)
        {
            return Cursors.Hand;
        }

        public WindowOperationBase BeginOperation(Point wp)
        {
            var operation = new WindowMover(this.form);

            operation.Begin(wp);
            return operation;
        }
    }

    /// <summary>
    /// ウィンドウの移動を行います。
    /// </summary>
    class WindowMover : WindowOperationBase
    {
        private Point relativePoint;

        public WindowMover(MovableForm form)
            : base(form)
        {
        }

        protected override void OnBegin(Point wp)
        {
            this.relativePoint = wp;
        }

        protected override void OnOperate(Point wp)
        {
            var screenPos = Form.PointToScreen(wp);
            var leftTop = new Point(
                screenPos.X - this.relativePoint.X,
                screenPos.Y - this.relativePoint.Y);

            Form.Left = leftTop.X;
            Form.Top = leftTop.Y;
        }
    }
}
