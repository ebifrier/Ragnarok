using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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
        public const int ThreadCount = 1;

        private static readonly object SyncObject = new object();
        private static readonly Thread[] threads = new Thread[ThreadCount];
        private static readonly List<SendData> sendDataList = new List<SendData>();
        /*private static readonly HashSet<Socket> sendingSockets =
            new HashSet<Socket>(new EqualityComparer());*/
        private static int sendingCount = 0;

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

            lock (SyncObject)
            {
                sendDataList.Add(sendData);

                Monitor.PulseAll(SyncObject);
            }
        }

        /// <summary>
        /// 送信データキューからデータを一つ取り出します。
        /// </summary>
        internal static SendData GetNextSendDataWait()
        {
            lock (SyncObject)
            {
                while (!sendDataList.Any() || sendingCount > 5)
                {
                    Monitor.Wait(SyncObject);
                }

                var sendData = sendDataList[0];
                sendDataList.RemoveAt(0);
                return sendData;
            }
        }

        private static void AddSendingSocket()
        {
            lock (SyncObject)
            {
                sendingCount += 1;
            }
        }

        private static void RemoveSendingSocket()
        {
            lock (SyncObject)
            {
                sendingCount -= 1;
                Log.Debug("Sending Count: {0}", sendingCount);

                Monitor.PulseAll(SyncObject);
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
            while (true)
            {
                // 送信可能なデータがあるか調べます。
                var sendData = GetNextSendDataWait();
                if (sendData == null)
                {
                    continue;
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

                    // 送信中ソケットを追加します。
                    AddSendingSocket();

                    Log.Trace(
                        "データ送信を開始しました。");
                }
                catch (Exception ex)
                {
                    // 送信の失敗報告をします。
                    RaiseSent(sendData, ex);
                }
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
            catch (ObjectDisposedException ex)
            {
                // ログは出しません。
                RaiseSent(sendData, ex);
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode != SocketError.Shutdown)
                {
                    Log.ErrorException(ex,
                        "データの送信に失敗しました。");
                }

                // ログは出しません。
                RaiseSent(sendData, ex);
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "データの送信に失敗しました。");

                RaiseSent(sendData, ex);
            }

            // 最後にソケットリストから送信中ソケットを外します。
            RemoveSendingSocket();
        }

        /// <summary>
        /// 送信完了(失敗)をコールバックを通して伝達します。
        /// </summary>
        private static void RaiseSent(SendData sendData, Exception ex)
        {
            Util.SafeCall(() =>
                sendData.OnSent(ex));
        }

        /// <summary>
        /// 送信スレッドを初期化し、送信処理を開始します。
        /// </summary>
        static SendThread()
        {
            for (var i = 0; i < threads.Count(); ++i)
            {
                var th = new Thread(UpdateSendData)
                {
                    Priority = ThreadPriority.Normal,
                    Name = string.Format("SendThread {0}", i),
                    IsBackground = true,
                };

                th.Start();
                threads[i] = th;
            }
        }
    }
}
