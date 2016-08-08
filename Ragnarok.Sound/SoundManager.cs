using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CSCore;

namespace Ragnarok.Sound
{
    public sealed class PlayData
    {
        public PlayData()
        {
            Volume = 1.0;
            UsePlayInterval = true;
        }

        public string FileName
        {
            get;
            set;
        }

        public double Volume
        {
            get;
            set;
        }

        public bool UsePlayInterval
        {
            get;
            set;
        }

        public bool IgnoreError
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 音声ファイルを再生します。
    /// </summary>
    public class SoundManager : MarshalByRefObject
    {
        private readonly object SyncRoot = new object();
        private readonly Backend.ISoundManagerBackend backend = null;

        private readonly Dictionary<string, DateTime> lastPlayedTime =
            new Dictionary<string, DateTime>();
        private TimeSpan playInterval = TimeSpan.FromSeconds(2);
        private string defaultPath = string.Empty;

        /// <summary>
        /// 音声プレイヤーオブジェクトを初期化します。
        /// </summary>
        public SoundManager()
        {
            try
            {
                this.backend =
#if USE_SOUND_IRRKLANG
                    new Backend.SoundManagerBackend_IrrKlang();
#else
                    new Backend.SoundManagerBackend_Dummy();
#endif
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "サウンドオブジェクトの初期化に失敗しました。");

                this.backend = null;
            }
        }

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
        /// ボリュームを0.0-1.0の間で取得または設定します。
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
        /// SEの最小再生間隔を取得または設定します。
        /// </summary>
        public TimeSpan PlayInterval
        {
            get { return this.playInterval; }
            set { this.playInterval = value; }
        }

        /// <summary>
        /// 音楽ファイルのある基本パスを取得または設定します。
        /// </summary>
        public string DefaultPath
        {
            get { return this.defaultPath; }
            set { this.defaultPath = value; }
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
        private bool CheckPlayInterval(string fullpath)
        {
            lock (SyncRoot)
            {
                var now = DateTime.Now;

                // 時刻がなければ、必ず真となるような値となります。
                DateTime time;
                if (!this.lastPlayedTime.TryGetValue(fullpath, out time))
                {
                    this.lastPlayedTime[fullpath] = now;
                    return true;
                }

                // 再生時刻を更新します。
                if (now >= time + PlayInterval)
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
        private SoundObject PlaySEInternal(PlayData data)
        {
            lock (SyncRoot)
            {
                if (this.backend == null)
                {
                    return null;
                }

                var fullpath = GetSoundFilePath(data.FileName);
                if (data.UsePlayInterval && !CheckPlayInterval(fullpath))
                {
                    return null;
                }

                var bsound = this.backend.Play(fullpath, data.Volume);
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
            PlaySE(filename, 0.0, false);
        }

        /// <summary>
        /// SEを再生します。再生中のファイルがあればその再生の直後に鳴らします。
        /// </summary>
        public SoundObject PlaySE(string filename, double volume = 1.0,
                                  bool usePlayInterval = true, bool ignoreError = false)
        {
            try
            {
                var data = new PlayData
                {
                    FileName = filename,
                    Volume = volume,
                    UsePlayInterval = usePlayInterval,
                    IgnoreError = ignoreError,
                };

                return PlaySEInternal(data);
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
    }
}
