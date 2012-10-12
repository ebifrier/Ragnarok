using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Net
{
    /// <summary>
    /// サーバーやクライアントとの接続の切断理由です。
    /// </summary>
    public enum DisconnectReason
    {
        /// <summary>
        /// こちらからのコネクション切断。
        /// </summary>
        Disconnected,
        /// <summary>
        /// 相手側からのコネクション切断。
        /// </summary>
        DisconnectedByOpposite,
        /// <summary>
        /// 通信エラー。
        /// </summary>
        Error,
    }

    /// <summary>
    /// コネクション接続時のイベント引数です。
    /// </summary>
    public class ConnectEventArgs : EventArgs
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConnectEventArgs()
        {
        }
    }

    /// <summary>
    /// コネクション切断のイベント引数です。
    /// </summary>
    public class DisconnectEventArgs : EventArgs
    {
        /// <summary>
        /// コネクションの切断理由を取得します。
        /// </summary>
        public DisconnectReason Reason
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DisconnectEventArgs(DisconnectReason reason)
        {
            this.Reason = reason;
        }
    }

    /// <summary>
    /// データの送受信イベントの引数です。
    /// </summary>
    public class DataEventArgs : EventArgs
    {
        /// <summary>
        /// 送受信されたデータを取得します。
        /// </summary>
        public byte[] Data
        {
            get;
            private set;
        }

        /// <summary>
        /// 送受信されたデータの長さを取得します。
        /// </summary>
        public int DataLength
        {
            get;
            private set;
        }

        /// <summary>
        /// 発生した例外があればそれを取得します。
        /// </summary>
        public Exception Error
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataEventArgs(byte[] data, Exception error)
        {
            Data = data;
            DataLength = (data != null ? data.Length : 0);
            Error = error;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataEventArgs(byte[] data, int length, Exception error)
        {
            Data = data;
            DataLength = length;
            Error = error;
        }
    }
}
