using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ragnarok.Utility;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 特殊な指し手の識別子です。
    /// </summary>
    /// <remarks>
    /// 中断|投了|持将棋|千日手|切れ負け|反則|詰み
    /// など
    /// 
    /// BoardMoveのSerializeの関係で16種類まで
    /// </remarks>
    public enum SpecialMoveType
    {
        /// <summary>
        /// 特になし
        /// </summary>
        [LabelDescription(Label = "なし")]
        None,
        /// <summary>
        /// 中断
        /// </summary>
        [LabelDescription(Label = "中断")]
        Interrupt,
        /// <summary>
        /// 投了
        /// </summary>
        [LabelDescription(Label = "投了")]
        Resign,
        /// <summary>
        /// 持将棋
        /// </summary>
        [LabelDescription(Label = "持将棋")]
        Jishogi,
        /// <summary>
        /// 千日手
        /// </summary>
        [LabelDescription(Label = "千日手")]
        Sennichite,
        /// <summary>
        /// 王手の千日手
        /// </summary>
        [LabelDescription(Label = "王手の千日手")]
        OuteSennichite,
        /// <summary>
        /// 時間切れ
        /// </summary>
        [LabelDescription(Label = "時間切れ")]
        TimeUp,
        /// <summary>
        /// 反則
        /// </summary>
        [LabelDescription(Label = "反則")]
        IllegalMove,
        /// <summary>
        /// 詰み
        /// </summary>
        [LabelDescription(Label = "詰み")]
        CheckMate,
        /// <summary>
        /// 封じ手
        /// </summary>
        [LabelDescription(Label = "封じ手")]
        SealedMove,
        /// <summary>
        /// エラー
        /// </summary>
        [LabelDescription(Label = "エラー")]
        Error,
    }
}
