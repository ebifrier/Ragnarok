using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading;

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
            public long TotalSize;
            public long ReceivedSize;
            public bool IsCompleted;
            public DownloadDataCompletedEventHandler Callback;
        }

        private List<DownloadItem> itemList = new List<DownloadItem>();

        /// <summary>
        /// ダウンロードアイテムの数を取得します。
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
        /// ダウンロード中のアイテム数を取得します。
        /// </summary>
        [DependOnProperty("Count")]
        public int LeaveCount
        {
            get
            {
                using (LazyLock())
                {
                    return this.itemList
                        .Where(_ => !_.IsCompleted)
                        .Count();
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

        void client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            var item = (DownloadItem)e.UserState;

            if (e.Error != null)
            {
                Log.ErrorException(e.Error,
                    "データのダウンロードに失敗しました。");
            }
            else
            {
                Log.Debug("ファイルのダウンロードに成功しました。");
            }

            // itemをリストから削除して、ダウンロードの完了とします。
            using (LazyLock())
            {
                //this.itemList.Remove(item);
                item.IsCompleted = true;

                UpdateProgressPercentage();
                this.RaisePropertyChanged("LeaveCount");
            }

            // コールバックは最後に呼びます。
            // この中でDownloaderの他のメソッドが呼ばれる可能性があるためです。
            if (item.Callback != null)
            {
                Util.SafeCall(() =>
                    item.Callback(this, e));
            }
        }

        /// <summary>
        /// ダウンロードを開始します。
        /// </summary>
        public void BeginDownload(Uri address,
                                  DownloadDataCompletedEventHandler callback)
        {
            var client = new WebClient();
            client.DownloadProgressChanged += client_DownloadProgressChanged;
            client.DownloadDataCompleted += client_DownloadDataCompleted;

            // ダウンロード管理用オブジェクト
            var item = new DownloadItem
            {
                Client = client,
                Callback = callback,
            };

            // ここで例外が返る可能性がある。
            Log.Debug("ダウンロードを開始します");
            ThreadPool.QueueUserWorkItem(_ =>
                client.DownloadDataAsync(address, item));

            using (LazyLock())
            {
                this.itemList.Add(item);

                this.RaisePropertyChanged("Count");
            }
        }

        /// <summary>
        /// すべてのダウンロードを停止します。
        /// </summary>
        public void CancelAll()
        {
            List<DownloadItem> clonedList;

            // lock中にCancelすると、そこでDownloadXxxCompletedが呼ばれるため
            // もしかするとデッドロックするかもしれません。
            using (LazyLock())
            {
                if (!this.itemList.Any())
                {
                    return;
                }

                clonedList = this.itemList;
                this.itemList = new List<DownloadItem>();
            }

            Log.Debug("ダウンロードはすべてキャンセルされます。");

            foreach (var item in clonedList)
            {
                item.Client.CancelAsync();
            }

            UpdateProgressPercentage();
        }
    }
}
