using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Ragnarok.Net
{
    /// <summary>
    /// データ送信完了時に呼ばれるデリゲート型です。
    /// </summary>
    public delegate void SentDataHandler(SendData sendData, Exception ex);

    /// <summary>
    /// 送信用のデータです。
    /// </summary>
    public class SendData
    {
        /// <summary>
        /// 送信ソケットを取得または設定します。
        /// </summary>
        public Socket Socket { get; set; }

        /// <summary>
        /// 送信データを取得または設定します。
        /// </summary>
        public List<ArraySegment<byte>> Buffers { get; private set; }

        /// <summary>
        /// 送信予定時刻を取得または設定します。
        /// </summary>
        public DateTime ScheduleTime { get; set; }

        /// <summary>
        /// 送信完了時に呼ばれるコールバックを取得または設定します。
        /// </summary>
        public event SentDataHandler Callback;

        /// <summary>
        /// 送信完了時のイベントを呼び出します。
        /// </summary>
        internal void OnSent(Exception ex)
        {
            var handler = Callback;

            if (handler != null)
            {
                handler(this, ex);
            }
        }

        /// <summary>
        /// データを追加します。
        /// </summary>
        public void AddBuffer(byte[] buffer)
        {
            if (buffer == null)
            {
                return;
            }

            AddBuffer(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// データを追加します。
        /// </summary>
        public void AddBuffer(byte[] buffer, int index, int length)
        {
            if (buffer == null)
            {
                return;
            }

            Buffers.Add(new ArraySegment<byte>(buffer, index, length));
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SendData()
        {
            Buffers = new List<ArraySegment<byte>>();
        }
    }
}
