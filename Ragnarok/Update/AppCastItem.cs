using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Update
{
    /// <summary>
    /// インストールするアプリの
    /// </summary>
    public sealed class AppCastItem : IComparable<AppCastItem>
    {
        /// <summary>
        /// アプリ名を取得または設定します。
        /// </summary>
        public string AppName
        {
            get;
            set;
        }

        /// <summary>
        /// インストールするアプリのバージョンを取得または設定します。
        /// </summary>
        public string Version
        {
            get;
            set;
        }

        /// <summary>
        /// 更新処理を行う実行ファイルのリンクを取得または設定します。
        /// </summary>
        public string UpdatePackLink
        {
            get;
            set;
        }

        /// <summary>
        /// リリースノートへのリンクを取得または設定します。
        /// </summary>
        public string ReleaseNotesLink
        {
            get;
            set;
        }

        /// <summary>
        /// アプリのダウンロードリンクを取得または設定します。
        /// </summary>
        public string DownloadLink
        {
            get;
            set;
        }

        /// <summary>
        /// アプリのMD5値を取得または設定します。
        /// </summary>
        public string MD5Signature
        {
            get;
            set;
        }

        /// <summary>
        /// MD5のチェックを行います。
        /// </summary>
        public bool VerifyMD5(string filename)
        {
            // check if we have a md5 signature
            if (string.IsNullOrEmpty(MD5Signature))
            {
                Log.Info("No MD5 check needed");
                return true;
            }

            var md5 = MD5Verificator.ComputeMD5(filename);
            if (!MD5Verificator.CompareMD5(md5, MD5Signature))
            {
                Log.Info("MD5 check failed.");
                return false;
            }

            Log.Info("MD5 check succeeded.");
            return true;
        }

        /// <summary>
        /// バージョンの前後比較を行います。
        /// </summary>
        public int CompareTo(AppCastItem other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var v1 = new Version(this.Version);
            var v2 = new Version(other.Version);

            return v1.CompareTo(v2);            
        }
    }    
}
