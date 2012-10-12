using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// ハンゲーム中の棋力です。
    /// </summary>
    public enum HangameSkill
    {
        /// <summary>
        /// 一般人
        /// </summary>
        NormalPeople,
        /// <summary>
        /// 将棋ファン
        /// </summary>
        ShogiFun,
        /// <summary>
        /// 将棋通
        /// </summary>
        ShogiKnower,
        /// <summary>
        /// 棋士見習い
        /// </summary>
        Apprenticeship,
        /// <summary>
        /// 一般棋士
        /// </summary>
        LowPlayer,
        /// <summary>
        /// 中級棋士
        /// </summary>
        MiddlePlayer,
        /// <summary>
        /// 上級棋士
        /// </summary>
        SeniorPlayer,
        /// <summary>
        /// 師範
        /// </summary>
        Master,
        /// <summary>
        /// 達人棋士
        /// </summary>
        ExpertPlayer,
        /// <summary>
        /// 名人棋士
        /// </summary>
        ProfessionalPlayer,
        /// <summary>
        /// 銀将
        /// </summary>
        SilverGeneral,
        /// <summary>
        /// 金将
        /// </summary>
        GoldGeneral,
        /// <summary>
        /// 角行
        /// </summary>
        Bishop,
        /// <summary>
        /// 飛将
        /// </summary>
        Rook,
        /// <summary>
        /// 王将
        /// </summary>
        King,
    }
}
