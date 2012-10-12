using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace Ragnarok.Net
{
    /// <summary>
    /// 送信開始処理に時間がかかることがあるため、
    /// 専用スレッドで送信開始処理を行います。
    /// </summary>
    internal static class SendThread
    {
        /// <summary>
        /// 送信スレッドの数です。
        /// </summary>
        private const int ThreadCount = 5;

        private static readonly Thread[] threads;
        private static readonly Queue<SendData> sendDataQueue = new Queue<SendData>();

        /// <summary>
        /// データを送信データキューに追加します。
        /// </summary>
        internal static void AddSendData(SendData sendData)
        {
            if (sendData == null || !sendData.Buffers.Any() ||
                sendData.Socket == null)
            {
                return;
            }

            lock (sendDataQueue)
            {
                sendDataQueue.Enqueue(sendData);

                Monitor.PulseAll(sendDataQueue);
            }
        }

        /// <summary>
        /// 送信データキューからデータを一つ取り出します。
        /// </summary>
        internal static SendData GetNextSendDataWait()
        {
            lock (sendDataQueue)
            {
                while (!sendDataQueue.Any())
                {
                    Monitor.Wait(sendDataQueue);
                }

                return sendDataQueue.Dequeue();
            }
        }

#if MONO
        /// <summary>
        /// BeginSend(IList&lt;ArraySegment&lt;byte&gt;&gt;, ...)が使えない
        /// 環境があるので、その場合は複数のデータを一つのバッファにまとめます。
        /// </summary>
        private static byte[] MergeBuffer(SendData sendData, out int length)
        {
            // バッファサイズを計算します。
            length = 0;
            foreach (var data in sendData.Buffers)
            {
                length += data.Count - data.Offset;
            }

            // 実際に確保されたバッファサイズと、送信サイズは違う可能性が
            // 考えられます。
            using (var stream = new MemoryStream(length))
            {
                foreach (var data in sendData.Buffers)
                {
                    stream.Write(data.Array, data.Offset, data.Count);
                }

                stream.Flush();
                return stream.GetBuffer();
            }
        }
#endif

        /// <summary>
        /// もし送信データがあれば、非同期の送信処理を行います。
        /// </summary>
        private static void UpdateSendData()
        {
            var retry = false;

            // 送信可能なデータがあるか調べます。
            var sendData = GetNextSendDataWait();
            if (sendData == null)
            {
                return;
            }

            // メッセージを送信します。
            try
            {
#if MONO
                int length = 0;
                var mergedBuffer = MergeBuffer(sendData, out length);
#endif

                // コメントの投稿処理を開始します。
                sendData.Socket.BeginSend(
#if MONO
                    mergedBuffer, 0, length,
#else
                    sendData.Buffers,
#endif
                    SocketFlags.None,
                    ar => SendDataDone(sendData, ar),
                    null);

                Log.Trace(
                    "データ送信を開始しました。");
            }
            catch (Exception ex)
            {
                // 送信の失敗報告をします。
                RaiseSent(sendData, ex);

                retry = true;
            }

            // 必要なら再度送信処理を実行します。
            // これはロックの外側で行います。
            if (retry)
            {
                UpdateSendData();
            }
        }

        /// <summary>
        /// 非同期のメッセージ送信処理終了後に呼ばれます。
        /// </summary>
        private static void SendDataDone(SendData sendData, IAsyncResult result)
        {
            try
            {
                sendData.Socket.EndSend(result);

                foreach (var buffer in sendData.Buffers)
                {
                    Log.Trace(
                        "データの送信を完了しました。({0}bytes)",
                        buffer.Count);
                }

                RaiseSent(sendData, null);
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "データの送信に失敗しました。");

                RaiseSent(sendData, ex);
            }

            UpdateSendData();
        }

        /// <summary>
        /// 送信完了(失敗)をコールバックを通して伝達します。
        /// </summary>
        private static void RaiseSent(SendData sendData, Exception ex)
        {
            sendData.OnSent(ex);
        }

        /// <summary>
        /// 送信スレッドを初期化し、送信処理を開始します。
        /// </summary>
        static SendThread()
        {
            threads = new Thread[ThreadCount];

            for (var i = 0; i < threads.Count(); ++i)
            {
                var th = new Thread(UpdateSendData)
                {
                    Priority = ThreadPriority.BelowNormal,
                    Name = "SendThread " + i.ToString(),
                    IsBackground = true,
                };

                th.Start();

                threads[i] = th;
            }
        }
    }
}
