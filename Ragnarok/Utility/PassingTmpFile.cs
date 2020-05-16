using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Ragnarok.Utility
{
    /// <summary>
    /// 一時ファイルとしてファイルを作成し、それが成功したら
    /// 元のファイルに置き換えます。
    /// </summary>
    /// <remarks>
    /// こうすることでファイルが中途半端な状態になることを防ぎます。
    /// </remarks>
    public sealed class PassingTmpFile : IDisposable
    {
        private volatile bool success;

        /// <summary>
        /// オリジナルのファイル名を取得します。
        /// </summary>
        public string OriginalFileName
        {
            get;
            private set;
        }

        /// <summary>
        /// 途中で作成する一時ファイルの名前を取得します。
        /// </summary>
        public string TmpFileName
        {
            get;
            private set;
        }

        /// <summary>
        /// 一時ファイル名を作成し、そのファイルがあればそれを削除します。
        /// </summary>
        public PassingTmpFile(string filename)
        {
            var tmpFilename = filename + ".temporary";

            // 一度tempファイルに設定内容を出力し、
            // 正しく出力したことを確認した後ファイルを置き換えます。
            // こうしないと失敗時にファイル内容が中途半端な
            // 状態で残されてしまいます。
            if (File.Exists(tmpFilename))
            {
                File.Delete(tmpFilename);
            }

            OriginalFileName = filename;
            TmpFileName = tmpFilename;
        }

        /// <summary>
        /// ファイル作成成功時に呼んでください。
        /// </summary>
        public void Success()
        {
            this.success = true;
        }

        ~PassingTmpFile()
        {
            Dispose(false);
        }
        /// <summary>
        /// ファイル作成に成功したら、一時ファイルを元のファイルに置き換えます。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// ファイル作成に成功したら、一時ファイルを元のファイルに置き換えます。
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.success)
                {
                    // 元のファイルを削除し、tmpファイルを規定の名前に変えます。
                    if (File.Exists(OriginalFileName))
                    {
                        File.Delete(OriginalFileName);
                    }

                    File.Move(TmpFileName, OriginalFileName);
                }
                else
                {
                    try
                    {
                        if (File.Exists(TmpFileName))
                        {
                            File.Delete(TmpFileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorException(ex,
                            "一時ファイルの削除に失敗しました。");
                    }
                }
            }
        }
    }
}
