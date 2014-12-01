using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ragnarok.Utility;

namespace Ragnarok.Forms.Shogi
{
    /// <summary>
    /// 将棋ウィンドウの編集モードを識別します。
    /// </summary>
    public enum EditMode
    {
        /// <summary>
        /// 手番と一致する側の駒のみを可能な位置にのみ動かせます。
        /// </summary>
        Normal,
        /// <summary>
        /// どの駒も自由に動かせます。
        /// </summary>
        Editing,
        /// <summary>
        /// 駒を動かすことが出来ません。
        /// </summary>
        NoEdit,
    }

    /// <summary>
    /// 変化の再生状態を示します。
    /// </summary>
    public enum AutoPlayState
    {
        /// <summary>
        /// 何もしていません。
        /// </summary>
        None,
        /// <summary>
        /// 変化再生中です。
        /// </summary>
        Playing,
    }
}
