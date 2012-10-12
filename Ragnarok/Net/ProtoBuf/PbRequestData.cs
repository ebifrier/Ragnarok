using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ragnarok.Net.ProtoBuf
{
    /// <summary>
    /// リクエストされたデータのレスポンスを処理します。
    /// 実際にこのオブジェクトが送られるわけではありません。
    /// </summary>
    /// <remarks>
    /// 一定時間中にレスポンスが返って来なかった場合は、
    /// コールバックにエラーが返ります。
    /// 
    /// タイムアウト判定にはタイマーを使っているため、タイムアウト時の処理に
    /// 時間がかかる場合は予期しない動作を招くことがあります。
    /// </remarks>
    internal abstract class PbRequestData : MarshalByRefObject, IDisposable
    {
        private readonly object SyncRoot = new object();
        /// <summary>
        /// タイムアウトを判定するためのタイマーです。
        /// </summary>
        private Timer timer;
        private bool disposed = false;

        /// <summary>
        /// レスポンスハンドラを呼び出します。
        /// </summary>
        public abstract void OnResponseReceived(IPbResponse response);

        /// <summary>
        /// リクエストIDを取得または設定します。
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// レスポンス受信イベント発行時のthisオブジェクトを
        /// 取得または設定します。
        /// </summary>
        public PbConnection Connection
        {
            get;
            set;
        }

        /// <summary>
        /// レスポンス到着までのタイムアウト時間を設定します。
        /// </summary>
        public void SetTimeout(TimeSpan timeout)
        {
            if (timeout != TimeSpan.MaxValue)
            {
                StartTimeoutTimer(timeout);
            }
        }

        /// <summary>
        /// タイムアウト時の処理を行います。
        /// </summary>
        private void HandleTimeout(object state)
        {
            this.OnResponseReceived(
                new PbResponse<PbDummy>()
                {
                    ErrorCode = PbErrorCode.Timeout,
                    Response = null,
                });
        }

        /// <summary>
        /// タイムアウト検出用タイマを開始します。
        /// </summary>
        protected void StartTimeoutTimer(TimeSpan timeout)
        {
            lock (SyncRoot)
            {
                StopTimeoutTimer();

                // ２度は呼ばれないようにします。
                this.timer = new Timer(
                    HandleTimeout,
                    null,
                    timeout,
                    TimeSpan.FromMilliseconds(-1));
            }
        }

        /// <summary>
        /// タイムアウト検出用タイマを停止します。
        /// </summary>
        protected void StopTimeoutTimer()
        {
            lock (SyncRoot)
            {
                if (this.timer != null)
                {
                    this.timer.Dispose();
                    this.timer = null;
                }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected PbRequestData()
        {
            Id = -1;
        }

        /// <summary>
        /// ファイナライザ
        /// </summary>
        ~PbRequestData()
        {
            Dispose(false);
        }

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        protected void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    StopTimeoutTimer();
                }

                this.disposed = true;
            }
        }
    }

    /// <summary>
    /// 型が不明なレスポンスを、指定の型にして処理します。
    /// </summary>
    internal sealed class PbRequestData<TReq, TRes> : PbRequestData
    {
        /// <summary>
        /// レスポンスが返ってきたときに呼ばれます。
        /// </summary>
        public PbResponseHandler<TRes> ResponseReceived;

        /// <summary>
        /// レスポンスハンドラを呼び出します。
        /// </summary>
        public override void OnResponseReceived(IPbResponse response)
        {
            // タイムアウト検出用のタイマはとめます。
            StopTimeoutTimer();

            // ハンドラを二回以上呼ばないようにこうしています。
            // 一応、念のために。
            var handler = Interlocked.Exchange(ref ResponseReceived, null);

            // タイマーを使っているため、タイムアウト時の処理に
            // 時間がかかる場合は予期しない動作を招くことがあります。
            if (handler != null)
            {
                // ここで対象のレスポンス型にキャストします。
                var e = new PbResponseEventArgs<TRes>(
                    Id,
                    response.ErrorCode,
                    (TRes)response.Response);

                handler(Connection, e);
            }
        }
    }
}
