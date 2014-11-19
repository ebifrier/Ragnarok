using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Ragnarok.Extra.Sound
{
    /// <summary>
    /// 音声ファイルを再生します。
    /// </summary>
    public class SoundManager : MarshalByRefObject
    {
        private readonly object SyncRoot = new object();
        private readonly Dictionary<string, DateTime> lastPlayedTime =
            new Dictionary<string, DateTime>();
        private readonly Backend.SoundManagerBackend backend = null;

        private TimeSpan playInterval = TimeSpan.FromSeconds(2);
        private string defaultPath = string.Empty;

        /// <summary>
        /// 音声を再生できるかどうかを取得します。
        /// </summary>
        public bool CanUseSound
        {
            get
            {
                lock (SyncRoot)
                {
                    return (this.backend != null);
                }
            }
        }

        /// <summary>
        /// SE再生の最小間隔を取得または設定します。
        /// </summary>
        public TimeSpan PlayInterval
        {
            get
            {
                lock (SyncRoot)
                {
                    return this.playInterval;
                }
            }
            set
            {
                lock (SyncRoot)
                {
                    this.playInterval = value;
                }
            }
        }

        /// <summary>
        /// ボリュームを0-100の間で取得または設定します。
        /// </summary>
        public double Volume
        {
            get
            {
                try
                {
                    lock (SyncRoot)
                    {
                        if (this.backend == null)
                        {
                            return 0;
                        }

                        return this.backend.Volume;
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorException(ex,
                        "サウンドの音量取得に失敗しました。");

                    return 0;
                }
            }
            set
            {
                try
                {
                    lock (SyncRoot)
                    {
                        if (this.backend == null)
                        {
                            return;
                        }

                        this.backend.Volume = value;
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorException(ex,
                        "サウンドの音量設定に失敗しました。");
                }
            }
        }

        /// <summary>
        /// EntryAssemblyのパスを取得します。
        /// </summary>
        public static string AssemblyLocation
        {
            get
            {
                var thisAsm = Assembly.GetEntryAssembly();

                return Path.GetDirectoryName(thisAsm.Location);
            }
        }

        /// <summary>
        /// 音楽ファイルのある基本パスを取得または設定します。
        /// </summary>
        public string DefaultPath
        {
            get
            {
                lock (SyncRoot)
                {
                    return this.defaultPath;
                }
            }
            set
            {
                lock (SyncRoot)
                {
                    this.defaultPath = value;
                }
            }
        }

        /// <summary>
        /// 音声ファイルのパスを取得します。
        /// </summary>
        public string GetSoundFilePath(string filename)
        {
            lock (SyncRoot)
            {
                if (string.IsNullOrEmpty(DefaultPath))
                {
                    return null;
                }

                // filenameが既にフルパスの場合は、filenameそのものが返ります。
                return Path.Combine(DefaultPath, filename);
            }
        }

        /// <summary>
        /// SEが再生可能か調べます。
        /// </summary>
        private bool CanPlaySE(string fullpath)
        {
            lock (this.lastPlayedTime)
            {
                DateTime date;
                var now = DateTime.Now;
                
                // 時刻がなければ、必ず真となるような値となります。
                if (!this.lastPlayedTime.TryGetValue(fullpath, out date))
                {
                    this.lastPlayedTime[fullpath] = now;
                    return true;
                }

                // 再生時刻を更新します。
                if (now >= date + PlayInterval)
                {
                    this.lastPlayedTime[fullpath] = now;
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// SEを再生します。再生中のファイルがあればその再生の直後に鳴らします。
        /// </summary>
        private SoundObject PlaySEInternal(string filename, double volume, bool checkTime)
        {
            lock (SyncRoot)
            {
                if (this.backend == null)
                {
                    return null;
                }

                var fullpath = GetSoundFilePath(filename);
                if (checkTime && !CanPlaySE(fullpath))
                {
                    return null;
                }

                var bsound = this.backend.PlaySE(fullpath, volume);
                if (bsound == null)
                {
                    return null;
                }

                return new SoundObject(bsound);
            }
        }

        /// <summary>
        /// 音声ファイルの事前読み込みを行います。
        /// </summary>
        public void PreLoad(string filename)
        {
            try
            {
                PlaySEInternal(filename, 0.0, false);
            }
            catch (FileNotFoundException)
            {
                // サウンドシステムの初期化に失敗すると、
                // 恒常的にこの例外が出るようになります。
                return;
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "{0}サウンドの準備に失敗しました。",
                    Path.GetFileName(filename));
            }
        }

        /// <summary>
        /// SEを再生します。再生中のファイルがあればその再生の直後に鳴らします。
        /// </summary>
        public SoundObject PlaySE(string filename, double volume = 1.0,
                                  bool checkTime = true, bool ignoreError = false)
        {
            try
            {
                return PlaySEInternal(filename, volume, checkTime);
            }
            catch (FileNotFoundException)
            {
                // サウンドシステムの初期化に失敗すると、
                // 恒常的にこの例外が出るようになります。
            }
            catch (Exception ex)
            {
                if (!ignoreError)
                {
                    Log.ErrorException(ex,
                        "{0}サウンドの再生に失敗しました。",
                        Path.GetFileName(filename));
                }
            }

            return null;
        }
        
        /// <summary>
        /// 音声プレイヤーオブジェクトを初期化します。
        /// </summary>
        public SoundManager()
        {
            try
            {
                this.backend = new Backend.SoundManagerBackend();
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "サウンドオブジェクトの初期化に失敗しました。");

                this.backend = null;
            }
        }
    }
}
