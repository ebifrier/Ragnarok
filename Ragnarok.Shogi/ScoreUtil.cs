using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 評価値用のユーティリティクラスです。
    /// </summary>
    public static class ScoreUtil
    {
        /// <summary>
        /// ScoreBoundTypeの値を反転させます。
        /// </summary>
        public static ScoreBound Flip(this ScoreBound bound)
        {
            return (
                bound == ScoreBound.Lower ? ScoreBound.Upper :
                bound == ScoreBound.Upper ? ScoreBound.Lower :
                ScoreBound.Exact);
        }
    }
}
