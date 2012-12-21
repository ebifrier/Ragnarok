using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Update
{
    /// <summary>
    /// Everytime when netsparkle detects an update the 
    /// consumer can decide what should happen as next with the help 
    /// of the UpdateDatected event
    /// </summary>
    public enum NextUpdateAction
    {
        ContinueToUpdate,
        ProhibitUpdate,
    }

    /// <summary>
    /// アプリの新バージョンが見つかった時に呼ばれるイベント引数です。
    /// </summary>
    public class UpdateDetectedEventArgs : EventArgs
    {
        /// <summary>
        /// 次の処理を取得または設定します。
        /// </summary>
        public NextUpdateAction NextAction
        {
            get;
            set;
        }

        /// <summary>
        /// アプリのコンフィグ情報を取得または設定します。
        /// </summary>
        public Configuration ApplicationConfig
        {
            get;
            set;
        }

        /// <summary>
        /// 最新バージョンを取得または設定します。
        /// </summary>
        public AppCastItem LatestVersion
        {
            get;
            set;
        }
    }

    /// <summary>
    /// ダウンロードが完了したときに呼ばれるイベント引数です。
    /// </summary>
    public class DownloadDoneEventArgs : EventArgs
    {
        /// <summary>
        /// 更新処理を行うかどうかを取得または設定します。
        /// </summary>
        public bool IsUpdate
        {
            get;
            set;
        }

        /// <summary>
        /// ダウンロードがキャンセルされたかどうかを取得または設定します。
        /// </summary>
        public bool IsCancelled
        {
            get;
            set;
        }

        /// <summary>
        /// 投げられた例外を取得または設定します。
        /// </summary>
        public Exception Error
        {
            get;
            set;
        }

        /// <summary>
        /// アプリのコンフィグ情報を取得または設定します。
        /// </summary>
        public Configuration ApplicationConfig
        {
            get;
            set;
        }

        /// <summary>
        /// 最新バージョンを取得または設定します。
        /// </summary>
        public AppCastItem LatestVersion
        {
            get;
            set;
        }
    }
}
