using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Ragnarok.Forms.Controls.WindowOperation
{
    /// <summary>
    /// ウィンドウの移動などを開始するための基底クラスです。
    /// </summary>
    interface IWindowOperationStarter
    {
        /// <summary>
        /// そこでのカーソルを取得します。
        /// </summary>
        Cursor GetCursor(Point wp);

        /// <summary>
        /// オペレーションを開始します。
        /// </summary>
        WindowOperationBase BeginOperation(Point wp);
    }

    /// <summary>
    /// ウィンドウの移動を行うための基底クラスです。
    /// </summary>
    class WindowOperationBase
    {
        private bool begin = false;
        private bool end = false;
        private bool captured = false;

        protected virtual void OnBegin(Point wp) { }
        protected virtual void OnOperate(Point wp) { }
        protected virtual void OnEnd() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WindowOperationBase(MovableForm form)
        {
            Form = form;
        }

        /// <summary>
        /// 対象となるウィンドウを取得します。
        /// </summary>
        public MovableForm Form
        {
            get;
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
                this.captured = Form.Capture;
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
                    Form.Capture = false;
                    this.captured = false;
                }

                this.end = true;
            }
        }
    }
}
