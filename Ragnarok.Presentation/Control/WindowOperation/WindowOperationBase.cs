using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Ragnarok.Presentation.Control.WindowOperation
{
    /// <summary>
    /// ウィンドウの移動などを開始するための基底クラスです。
    /// </summary>
    abstract class IWindowOperationStarter
    {
        /// <summary>
        /// そこでのカーソルを取得します。
        /// </summary>
        public abstract Cursor GetCursor(Point wp);

        /// <summary>
        /// オペレーションを開始します。
        /// </summary>
        public abstract WindowOperationBase BeginOperation(Point wp);

        protected IWindowOperationStarter()
        {
        }
    }

    /// <summary>
    /// ウィンドウの移動を行うための基底クラスです。
    /// </summary>
    class WindowOperationBase
    {
        private readonly MovableWindow window;
        private bool begin = false;
        private bool end = false;
        private bool captured = false;

        protected virtual void OnBegin(Point wp) { }
        protected virtual void OnOperate(Point wp) { }
        protected virtual void OnEnd() { }

        /// <summary>
        /// 対象となるウィンドウを取得します。
        /// </summary>
        public MovableWindow Window
        {
            get { return this.window; }
        }

        /// <summary>
        /// オペレーションを開始します。
        /// </summary>
        public void Begin(Point wp)
        {
            if (!this.begin && !this.end)
            {
                OnBegin(wp);

                this.begin = true;
                this.captured = Window.CaptureMouse();
            }
        }

        /// <summary>
        /// オペレーションを行います。
        /// </summary>
        public void Operate(Point wp)
        {
            if (this.begin && !this.end)
            {
                OnOperate(wp);
            }
        }

        /// <summary>
        /// オペレーションを終了します。
        /// </summary>
        public void End()
        {
            if (this.begin && !this.end)
            {
                OnEnd();

                if (this.captured)
                {
                    Window.ReleaseMouseCapture();
                    this.captured = false;
                }

                this.end = true;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WindowOperationBase(MovableWindow window)
        {
            this.window = window;
        }
    }
}
