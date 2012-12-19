using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;

using Ragnarok.ObjectModel;

namespace Ragnarok.Update
{
    /// <summary>
    /// ファイルダウンロード完了時に使われます。
    /// </summary>
    public class DownloadFileCompletedEventArgs : AsyncCompletedEventArgs
    {
        /// <summary>
        /// ファイル名を取得します。
        /// </summary>
        public string FileName
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DownloadFileCompletedEventArgs(string filename, Exception error,
                                              bool cancelled)
            : base(error, cancelled, null)
        {
            FileName = filename;
        }
    }

    /// <summary>
    ///  ファイルダウンロード完了時に使われます。
    /// </summary>
    public delegate void DownloadFileCompletedHandler(
        object sender, DownloadFileCompletedEventArgs e);

    /// <summary>
    /// ファイルのダウンロード、その進捗管理を行います。
    /// </summary>
    public sealed class Downloader : NotifyObject
    {
        /// <summary>
        /// ダウンロード中の各アイテムを管理します。
        /// </summary>
        private sealed class DownloadItem
        {
            public WebClient Client;
            public string FileName;
            public long TotalSize;
            public long ReceivedSize;
            public DownloadFileCompletedHandler Callback;
        }

        private List<DownloadItem> itemList = new List<DownloadItem>();

        /// <summary>
        /// ダウンロード中のアイテムがあるか取得します。
        /// </summary>
        public int Count
        {
            get
            {
                using (LazyLock())
                {
                    return this.itemList.Count();
                }
            }
        }

        /// <summary>
        /// ダウンロードの進捗割合を0.0～1.0の範囲で取得します。
        /// </summary>
        public double ProgressRate
        {
            get
            {
                return GetValue<double>("ProgressRate");
            }
            private set
            {
                value = MathEx.Between(0.0, 1.0, value);
                SetValue("ProgressRate", value);
            }
        }

        /// <summary>
        /// ダウンロードの進捗割合をパーセンテージで取得します。
        /// </summary>
        [DependOnProperty("ProgressRate")]
        public int ProgressPercentage
        {
            get
            {
                var value = (int)(ProgressRate * 100);

                return MathEx.Between(0, 100, value);
            }
        }

        /// <summary>
        /// 進捗割合を更新します。
        /// </summary>
        private void UpdateProgressPercentage()
        {
            using (LazyLock())
            {
                var totalSize = 0L;
                var receivedSize = 0L;

                foreach (var item in this.itemList)
                {
                    totalSize += item.TotalSize;
                    receivedSize += item.ReceivedSize;
                }

                ProgressRate = (totalSize == 0 ?
                    0.0 :
                    ((double)receivedSize / totalSize));
            }
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var item = (DownloadItem)e.UserState;

            // 受信バイト数を更新します。
            item.TotalSize = e.TotalBytesToReceive;
            item.ReceivedSize = e.BytesReceived;

            UpdateProgressPercentage();
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var item = (DownloadItem)e.UserState;

            if (e.Error != null)
            {
                Log.ErrorException(e.Error,
                    "ファイルのダウンロードに失敗しました: {0}",
                    item.FileName);
            }
            else
            {
                Log.Debug("ファイルのダウンロードに成功しました。");
            }

            // itemをリストから削除して、ダウンロードの完了とします。
            using (LazyLock())
            {
                this.itemList.Remove(item);

                UpdateProgressPercentage();
            }

            // コールバックは最後に呼びます。
            // この中でDownloaderの他のメソッドが呼ばれる可能性があるためです。
            if (item.Callback != null)
            {
                var e_ = new DownloadFileCompletedEventArgs(
                    item.FileName,
                    e.Error,
                    e.Cancelled);

                Util.SafeCall(() =>
                    item.Callback(this, e_));
            }
        }

        /// <summary>
        /// ダウンロードを開始します。
        /// </summary>
        public void BeginDownload(Uri address, string filename,
                                  DownloadFileCompletedHandler callback)
        {
            var client = new WebClient();
            client.DownloadProgressChanged += client_DownloadProgressChanged;
            client.DownloadFileCompleted += client_DownloadFileCompleted;

            // ダウンロード管理用オブジェクト
            var item = new DownloadItem
            {
                Client = client,
                FileName = filename,
                Callback = callback,
            };

            // ここで例外が返る可能性がある。
            Log.Debug("ダウンロードを開始します: {0}", filename);
            client.DownloadFileAsync(address, filename, item);

            using (LazyLock())
            {
                this.itemList.Add(item);
            }
        }

        /// <summary>
        /// すべてのダウンロードを停止します。
        /// </summary>
        public void CancelAll()
        {
            List<DownloadItem> clonedList;

            Log.Debug("ダウンロードはすべてキャンセルされました。");

            // lock中にCancelすると、そこでDownloadXxxCompletedが呼ばれるため
            // もしかするとデッドロックするかもしれません。
            using (LazyLock())
            {
                clonedList = this.itemList;
                this.itemList = new List<DownloadItem>();
            }

            foreach (var item in clonedList)
            {
                item.Client.CancelAsync();
            }

            UpdateProgressPercentage();
        }
    }
}
