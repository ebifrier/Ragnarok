using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

using Ragnarok.Net;

namespace Ragnarok.NicoNico.Live
{
    /// <summary>
    /// 生放送に関する情報をまとめたクラスです。
    /// </summary>
    public class LiveStreamInfo
    {
        /// <summary>
        /// playerstatusを取得または設定します。
        /// </summary>
        public PlayerStatus PlayerStatus
        {
            get;
            set;
        }

        /// <summary>
        /// publishstatusを取得または設定します。
        /// </summary>
        public PublishStatus PublishStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 生放送ページから得られた情報を取得または設定します。
        /// </summary>
        public LiveInfo LiveInfo
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 生放送関連の共通コードを持ちます。
    /// </summary>
    public static class LiveUtil
    {
        /// <summary>
        /// 放送URLや他の文字列からlvXXXXを含む放送IDを取得します。
        /// </summary>
        /// <remarks>
        /// 放送URLはcoXXXXで記述されることがあります。
        /// </remarks>
        public static long GetLiveId(string liveStr)
        {
            if (string.IsNullOrEmpty(liveStr))
            {
                return -1;
            }

            // lvXXXX が含まれていればその放送IDをそのまま使います。
            var m = Regex.Match(liveStr, "lv([0-9]+)");
            if (m.Success)
            {
                return long.Parse(m.Groups[1].Value);
            }

            // 一度放送ページを取得して、そこから放送IDを探します。
            // 放送URLはcoXXXXを含むURLのことがあります。
            var page = WebUtil.RequestHttpText(
                liveStr,
                null,
                null,
                Encoding.UTF8);
            if (string.IsNullOrEmpty(page))
            {
                return -1;
            }

            // 放送IDを検索します。
            m = Regex.Match(page, "番組ID:lv([0-9]+)");
            if (!m.Success)
            {
                return -1;
            }

            return long.Parse(m.Groups[1].Value);
        }

        /// <summary>
        /// コメント投稿時に使うポストキーを取得します。
        /// </summary>
        public static string GetPostKey(int threadId, int blockNo,
                                        CookieContainer cc)
        {
            var responseText = WebUtil.RequestHttpText(
                NicoString.GetPostKeyUrl(threadId, blockNo),
                null,
                cc,
                Encoding.UTF8);

            if (string.IsNullOrEmpty(responseText))
            {
                return null;
            }

            // postkeyは=の後に入っています。
            var index = responseText.IndexOf('=');
            if (index < 0 || index + 1 >= responseText.Length)
            {
                return null;
            }

            return responseText.Substring(index + 1);
        }

        /// <summary>
        /// 過去コメントの取得に使うWaybackKeyを取得します。
        /// </summary>
        public static string GetWaybackKey(int threadId, CookieContainer cc)
        {
            var responseText = WebUtil.RequestHttpText(
                NicoString.GetWaybackKeyUrl(threadId),
                null,
                cc,
                Encoding.UTF8);

            if (string.IsNullOrEmpty(responseText))
            {
                return null;
            }

            // waybackkeyは=の後に入っています。
            var index = responseText.IndexOf('=');
            if (index < 0 || index + 1 >= responseText.Length)
            {
                return null;
            }

            return responseText.Substring(index + 1);
        }

        /// <summary>
        /// コミュニティで放送中であれば、そのURLを取得します。
        /// </summary>
        public static string GetCurrentLiveUrl(int communityId,
                                               CookieContainer cc)
        {
            if (communityId <= 0)
            {
                throw new ArgumentException(
                    "コミュニティＩＤが正しくありません。",
                    "communityId");
            }

            // コミュニティページの方が放送中アドレスの更新が早いため、
            // まずはこちらから検索します。
            // ただし、クローズコミュの場合は失敗します。
            var liveUrl = GetCurrentLiveUrlFromCommunity(
                communityId, cc);
            if (!string.IsNullOrEmpty(liveUrl))
            {
                return liveUrl;
            }

            return GetCurrentLiveUrlFromLive(communityId, cc);
        }

        private static readonly Regex CurrentLiveCommunityPageRegex = new Regex(
            @"<div class=""now_item cfix"">\s*" +
            @"<h2><a href=""http://video.nicovideo.jp/watch/lv(\d+)\?ref=community""\s*" +
            @"class=""community"">");

        /// <summary>
        /// 現在放送中の放送アドレスをコミュニティから取得します。
        /// </summary>
        private static string GetCurrentLiveUrlFromCommunity(int communityId,
                                                             CookieContainer cc)
        {
            try
            {
                var responseText = WebUtil.RequestHttpText(
                    NicoString.GetCommunityUrl(communityId),
                    null,
                    cc,
                    Encoding.UTF8);
                if (string.IsNullOrEmpty(responseText))
                {
                    return null;
                }

                var m = CurrentLiveCommunityPageRegex.Match(responseText);
                if (!m.Success)
                {
                    return null;
                }

                var liveId = long.Parse(m.Groups[1].Value);
                return NicoString.GetLiveUrl(liveId);
            }
            catch (WebException)
            {
                // クローズコミュニティの場合はこの例外が発生します。
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);

                Log.ErrorException(ex,
                    "co{0}: コミュニティページの取得に失敗しました。",
                    communityId);
            }

            return null;
        }

        private static readonly Regex CurrentLiveRegexLivePage = new Regex(
            @"<script type=""text/javascript""><!--" +
            @"\s*var Video = [{]" +
            @"\s*v:\s*'lv(\d+)',");
        private static readonly Regex NowOnAirRegex = new Regex(
            @"<div id=""flvplayer_container"">\s*" +
            @"<div class=""dummy_box""></div>");

        /// <summary>
        /// 現在放送中の放送アドレスを放送ページから取得します。
        /// </summary>
        private static string GetCurrentLiveUrlFromLive(int communityId,
                                                        CookieContainer cc)
        {
            try
            {
                var responseText = WebUtil.RequestHttpText(
                    NicoString.GetLiveUrl("co" + communityId),
                    null,
                    cc,
                    Encoding.UTF8);
                if (string.IsNullOrEmpty(responseText))
                {
                    return null;
                }

                if (!NowOnAirRegex.IsMatch(responseText))
                {
                    // 放送中ではありません。
                    return null;
                }

                var m = CurrentLiveRegexLivePage.Match(responseText);
                if (!m.Success)
                {
                    // 放送IDが見つかりません。
                    return null;
                }

                var liveId = long.Parse(m.Groups[1].Value);
                return NicoString.GetLiveUrl(liveId);
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);

                Log.ErrorException(ex,
                    "co{0}: 放送ページの取得に失敗しました。",
                    communityId);
            }

            return null;
        }

        /// <summary>
        /// 放送情報などを保持します。
        /// </summary>
        private class InternalData
        {
            public CookieContainer Cookie;
            public readonly LiveStreamInfo LiveStreamInfo = new LiveStreamInfo();
            public Exception Exception = null;
        }

        /// <summary>
        /// 放送関連情報を同期的に取得します。
        /// </summary>
        public static LiveStreamInfo GetLiveStreamInfoSync(string liveUrl,
                                                           CookieContainer cc)
        {
            var playerStatus = PlayerStatus.Create(liveUrl, cc);
            var id = playerStatus.Stream.Id;

            // publishstatusは放送主しか取得することが出来ません。
            PublishStatus publishStatus = null;
            if (playerStatus.Stream.IsOwner)
            {
                publishStatus = PublishStatus.Create(id, cc);
            }

            // 放送情報を取得します。
            var liveInfo = LiveInfo.Create(id, cc);

            return new LiveStreamInfo()
            {
                PlayerStatus = playerStatus,
                PublishStatus = publishStatus,
                LiveInfo = liveInfo,
            };
        }

        /// <summary>
        /// 生放送に必要な情報を非同期でまとめて取得します。
        /// </summary>
        public static LiveStreamInfo GetLiveStreamInfo(string liveUrl,
                                                       CookieContainer cc)
        {
            var playerStatus = PlayerStatus.Create(liveUrl, cc);

            return GetLiveStreamInfo(playerStatus, cc);
        }

        /// <summary>
        /// 生放送に必要な情報を非同期でまとめて取得します。
        /// </summary>
        public static LiveStreamInfo GetLiveStreamInfo(PlayerStatus playerStatus,
                                                       CookieContainer cc)
        {
            if (playerStatus == null)
            {
                throw new ArgumentNullException("playerStatus");
            }

            if (cc == null)
            {
                throw new ArgumentNullException("cc");
            }

            // 結果などを保存するオブジェクトです。
            var internalData = new InternalData()
            {
                Cookie = cc,
            };

            var eventList = new AutoResetEvent[3]
            {
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false),
            };            

            // playerStatusを取得します。
            internalData.LiveStreamInfo.PlayerStatus = playerStatus;
            eventList[0].Set();

            // publishstatusを取得します。
            // これは放送主にしか取得できません。
            if (playerStatus.Stream.IsOwner)
            {
                BeginGetPublishStatus(
                    playerStatus.Stream.Id,
                    internalData,
                    eventList[1]);
            }
            else
            {
                eventList[1].Set();
            }

            // 放送ページの情報を取得します。
            // playerstatusなどでは放送タイトルなどが一定の文字数で
            // 省略されてしまうためです。
            BeginGetLiveInfo(
                playerStatus.Stream.Id,
                internalData,
                eventList[2]);

            // タイムアウト時間を取得します。
            // (デフォルトで３０秒)
            var timeout = WebUtil.DefaultTimeout;
            if (timeout < 0)
            {
                timeout = 30 * 1000;
            }

            // 各イベントが終了するのを待ちます。
            foreach (var ev in eventList)
            {
                if (!ev.WaitOne(TimeSpan.FromMilliseconds(timeout)))
                {
                    throw new TimeoutException(
                        "放送情報の取得がタイムアウトしました。");
                }

                // 例外があればそれをここで投げ返します。
                var ex = internalData.Exception;
                if (ex != null)
                {
                    throw new NicoLiveException(
                        "GetLiveStreamInfoでエラーが発生しました。", ex);
                }
            }

            return internalData.LiveStreamInfo;
        }

        /// <summary>
        /// playerstatusの取得を開始します。
        /// </summary>
        private static void BeginGetPlayerStatus(long liveId,
                                                 InternalData internalData,
                                                 AutoResetEvent ev)
        {
            WebUtil.RequestHttpAsync(
                NicoString.GetPlayerStatusUrl(liveId),
                null,
                internalData.Cookie,
                (result, data) =>
                {
                    try
                    {
                        // ステータスがおかしければエラーとします。
                        var v = PlayerStatus.CreateFromXml(liveId, NicoUtil.GetXml(data));

                        internalData.LiveStreamInfo.PlayerStatus = v;
                    }
                    catch (Exception ex)
                    {
                        internalData.Exception = ex;
                    }

                    ev.Set();
                });
        }

        /// <summary>
        /// publishstatusの取得を開始します。
        /// </summary>
        private static void BeginGetPublishStatus(long liveId,
                                                  InternalData internalData,
                                                  AutoResetEvent ev)
        {
            WebUtil.RequestHttpAsync(
                NicoString.GetPublishStatusUrl(liveId),
                null,
                internalData.Cookie,
                (result, data) =>
                {
                    try
                    {
                        // publishstatusは放送主でないと取得できません。
                        var v = PublishStatus.CreateFromXml(liveId, NicoUtil.GetXml(data));

                        internalData.LiveStreamInfo.PublishStatus = v;
                    }
                    catch (Exception ex)
                    {
                        internalData.Exception = ex;
                    }

                    ev.Set();
                });
        }

        /// <summary>
        /// 放送情報の取得を開始します。
        /// </summary>
        private static void BeginGetLiveInfo(long liveId,
                                             InternalData internalData,
                                             AutoResetEvent ev)
        {
            WebUtil.RequestHttpTextAsync(
                NicoString.GetLiveUrl(liveId),
                null,
                internalData.Cookie,
                Encoding.UTF8,
                (result, text) =>
                {
                    try
                    {
                        // 放送情報が取得できなければエラーとします。
                        var v = LiveInfo.CreateFromHtml(
                            string.Format("lv{0}", liveId),
                            text);
                        
                        internalData.LiveStreamInfo.LiveInfo = v;
                    }
                    catch (Exception ex)
                    {
                        internalData.Exception = ex;
                    }

                    ev.Set();
                });
        }
    }
}
