using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi.Csa
{
    public enum GameResult
    {
        [CsaCommand("引き分け", "DRAW")]
        Draw,
        [CsaCommand("勝ち", "WIN")]
        Win,
        [CsaCommand("負け", "LOSE")]
        Lose,
    }

    public enum GameEndReason
    {
        [CsaCommand("千日手", "SENNICHITE")]
        Sennichite,
        [CsaCommand("連続王手の千日手", "OUTE_SENNICHITE")]
        OuteSennichite,
        [CsaCommand("不正な文字列", "ILLEGAL_MOVE")]
        IllegalMove,
        [CsaCommand("時間切れ", "TIME_UP")]
        TimeUp,
        [CsaCommand("投了", "RESIGN")]
        Resign,
        [CsaCommand("持将棋", "JISHOGI")]
        Jishogi,
    }

    /// <summary>
    /// CSA将棋サーバーにおける、指し手や勝敗などを伝えるデータを保持します。
    /// </summary>
    public sealed class CsaGameCommand
    {
        public CsaMove Move
        {
            get;
            set;
        }

        public int MoveTime
        {
            get;
            set;
        }

        public GameEndReason? EndReason
        {
            get;
            set;
        }

        public GameResult? Result
        {
            get;
            set;
        }

        public string Error
        {
            get;
            set;
        }
    }
}
