using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.NicoNico.Provider
{
    /// <summary>
    /// アップロード中の動画の状態を識別します。
    /// </summary>
    public enum UploadedVideoStatus
    {
        /// <summary>
        /// エラーが発生しました。
        /// </summary>
        Error,
        /// <summary>
        /// アップロード中です。
        /// </summary>
        Uploading,
        /// <summary>
        /// アップロードに成功しました。
        /// </summary>
        Success,
    }

    /// <summary>
    /// アップロード中の動画の情報を保持します。
    /// </summary>
    public sealed class ChannelUploadedVideoData
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ChannelUploadedVideoData(UploadedVideoStatus status, string id,
                                        string title)
        {
            Status = status;
            Id = id;
            Title = title;
        }

        /// <summary>
        /// アップロード状態を取得します。
        /// </summary>
        public UploadedVideoStatus Status
        {
            get;
            private set;
        }

        /// <summary>
        /// IDを取得します。
        /// </summary>
        public string Id
        {
            get;
            private set;
        }

        /// <summary>
        /// 動画タイトルを取得します。
        /// </summary>
        public string Title
        {
            get;
            private set;
        }
    }
}
