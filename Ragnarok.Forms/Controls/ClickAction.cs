﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Forms.Controls
{
    /// <summary>
    /// ToolStripExやMenuExを非アクティブ状態からクリックしたときの挙動を示す定数。
    /// </summary>
    public enum ClickAction : int
    {
        /// <summary>
        /// ウィンドウをアクティブにし、マウスのメッセージを破棄しません。
        /// </summary>
        MA_ACTIVATE = 1,

        /// <summary>
        /// ウィンドウをアクティブにし、マウスメッセージを破棄します。
        /// </summary>
        MA_ACTIVATEANDEAT = 2,

        /// <summary>
        /// ウィンドウをアクティブにせず、マウスのメッセージを破棄しません。
        /// </summary>
        MA_NOACTIVATE = 3,

        /// <summary>
        /// ウィンドウをアクティブにせず、マウスメッセージを破棄します。
        /// </summary>
        MA_NOACTIVATEANDEAT = 4,
    }
}
