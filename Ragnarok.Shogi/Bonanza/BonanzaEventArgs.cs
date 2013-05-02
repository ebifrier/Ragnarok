using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi.Bonanza
{
    /// <summary>
    /// ボナンザが止まった理由です。
    /// </summary>
    public enum AbortReason
    {
        /// <summary>
        /// 指示により停止しました。
        /// </summary>
        Aborted,
        /// <summary>
        /// エラーにより停止しました。
        /// </summary>
        Error,
        /// <summary>
        /// 致命的なエラーにより停止しました。
        /// </summary>
        FatalError,
    }

    /// <summary>
    /// ボナンザ停止時に使われるイベント引数です。
    /// </summary>
    public class BonanzaAbortedEventArgs : EventArgs
    {
        /// <summary>
        /// ボナンザが停止した理由を取得または設定します。
        /// </summary>
        public AbortReason Reason
        {
            get;
            set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BonanzaAbortedEventArgs(AbortReason reason)
        {
            Reason = reason;
        }
    }

    /// <summary>
    /// コマンド受信時に使われるイベント引数です。
    /// </summary>
    public class BonanzaReceivedCommandEventArgs : EventArgs
    {
        /// <summary>
        /// 受信したコマンドを取得または設定します。
        /// </summary>
        public string Command
        {
            get;
            set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BonanzaReceivedCommandEventArgs(string command)
        {
            Command = command;
        }
    }
}
