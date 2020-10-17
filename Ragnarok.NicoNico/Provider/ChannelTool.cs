using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.NicoNico.Provider
{
    using Net;
    using Utility;

    /// <summary>
    /// 動画の公開範囲を設定します。
    /// </summary>
    public enum DistributeRange
    {
        /// <summary>
        /// 誰でも視聴可能
        /// </summary>
        Everyone = 1,
        /// <summary>
        /// メンバーのみ視聴可能
        /// </summary>
        MemberOnly = 4,
    }

    /// <summary>
    /// 検索の順序を決める属性を指定します。
    /// </summary>
    public enum SearchSort
    {
        [Description(Description = "registered_at")]
        RegisteredAt,
        [Description(Description = "view_count")]
        ViewCount,
        [Description(Description = "last_comment_time")]
        LastCommentTime,
        [Description(Description = "comment_count")]
        CommentCount,
        [Description(Description = "mylist_count")]
        MylistCount,
        [Description(Description = "duration")]
        Duration,
        [Description(Description = "visible_start_time")]
        VisibleStartTime,
    }

    /// <summary>
    /// 検索の降順・昇順を指定します。
    /// </summary>
    public enum SearchOrder
    {
        [Description(Description = "desc")]
        Desc,
        [Description(Description = "asc")]
        Asc,
    }

    /// <summary>
    /// 動画ステータスを保持します。
    /// </summary>
    public sealed class VideoStatus
    {
        /// <summary>
        /// 動画の投稿状態を取得します。
        /// </summary>
        /// <remarks>
        /// PREPARING-REGISTER
        /// REGISTERING
        /// REGISTERED
        /// DELETED
        /// </remarks>
        public string PostStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 動画エンコードの状態を取得します。
        /// </summary>
        /// <remarks>
        /// ACCEPTED
        /// TRANSCODING
        /// UNDEFINED
        /// TRANSFERED
        /// TRANSFER-FAILED
        /// </remarks>
        public string VideoFileStatus
        {
            get;
            set;
        }

        /// <summary>
        /// サムネイルの状態を取得します。
        /// </summary>
        /// <remarks>
        /// NOT-AVAILABLE
        /// </remarks>
        public string ThumbnailStatus
        {
            get;
            set;
        }
    }

    /// <summary>
    /// ニコニコチャンネルで動画編集や動画アップロードを行うためのクラスです。
    /// </summary>
    public static class ChannelTool
    {
        public readonly static string UrlBase = @"https://chtool.nicovideo.jp/video";
        public readonly static string UploaUrldBase = @"https://www.upload.nicovideo.jp";

        /// <summary>
        /// 動画IDの取得用URLを作成します。
        /// </summary>
        public static string MakeGetVideoIdUrl(int channelId)
        {
            return $"{UploaUrldBase}/v2/channels/ch{channelId}/videos";
        }

        /// <summary>
        /// 動画用のサイトURLを作成します。
        /// </summary>
        public static string MakeVideoUrl(int channelId, string videoId, string subPath = null)
        {
            return $@"{UploaUrldBase}/v2/channels/ch{channelId}/videos/{videoId}{subPath}";
        }

        /// <summary>
        /// 動画の状態を取得するURLを作成します。
        /// </summary>
        public static string MakeVideoStatusUrl(int channelId, string videoId)
        {
            return MakeVideoUrl(channelId, videoId, "/status");
        }

        /// <summary>
        /// 動画のアップロードURLを取得するためのURLを作成します。
        /// </summary>
        public static string MakeGetUploadChunkUrl(int channelId, string videoId)
        {
            return MakeVideoUrl(channelId, videoId, "/upload-chunk-stream");
        }

        /// <summary>
        /// 動画のサムネイル時間を設定するURLを作成します。
        /// </summary>
        public static string MakeThumbnailTimeUrl(int channelId, string videoId, int thumbnailId = 0)
        {
            return MakeVideoUrl(channelId, videoId, $"/scene-thumbnails/{thumbnailId}");
        }

        /// <summary>
        /// 動画登録用のサイトURLを作成します。
        /// </summary>
        public static string MakeVideoRegisterUrl(int channelId, string videoId)
        {
            return MakeVideoUrl(channelId, videoId, "/register");
        }

        /// <summary>
        /// 動画検索用のサイトURLを作成します。
        /// </summary>
        public static string MakeSearchUrl(int channelId)
        {
            return $"{UrlBase}/video?channel_id={channelId}";
        }

        /// <summary>
        /// 動画削除用のサイトURLを作成します。
        /// </summary>
        public static string MakeVideoDeleteUrl(int channelId, string videoId)
        {
            return $"{UrlBase}/video_delete2?channel_id={channelId}&video_id={videoId}";
        }

        /// <summary>
        /// コンテンツ内容をファイルに保存します。
        /// </summary>
        public static void Save(string filename, string text)
        {
            text = text.Replace("href=\"/", "href=\"http://chtool.nicovideo.jp/");
            text = text.Replace("src=\"/", "src=\"http://chtool.nicovideo.jp/");

            using (var stream = new FileStream(filename, FileMode.Create))
            {
                var bytes = Encoding.UTF8.GetBytes(text);
                stream.Write(bytes, 0, bytes.Count());
            }
        }

        /// <summary>
        /// chtoolにログインし、そのクッキーを返します。
        /// </summary>
        public static CookieContainer Login(int channelId, string mail,
                                            string password)
        {
            var postParam = new Dictionary<string, object>
            {
                { "mail_tel", mail },
                { "password", password }
            };

            var getParam = new Dictionary<string, object>
            {
                { "show_button_twitter", 1 },
                { "site", "chtool_2" },
                { "show_button_facebook", 1 },
                { "next_url", $"channel_id={channelId}" }
            };
            var getQuery = WebUtil.EncodeParam(getParam);

            var cc = new CookieContainer();
            var text = WebUtil.RequestHttpText(
                $@"https://account.nicovideo.jp/api/v1/login?{getQuery}",
                postParam, cc, Encoding.UTF8);

            //Save("login.html", text);

            // エラー表示用の文字列が表示されていればエラーとします。
            if (string.IsNullOrEmpty(text))
            {
                throw new ChannelToolException(
                    "chtoolへのログインに失敗しました。");
            }

            return cc;
        }

        /// <summary>
        /// 動画の状態を取得します。
        /// </summary>
        public static VideoStatus RequestStatus(int channelId, string videoId,
                                                CookieContainer cc)
        {
            var url = MakeVideoStatusUrl(channelId, videoId);
            var req = WebUtil.MakeJsonRequest(url, cc);
            req.Method = "GET";
            req.Headers["X-Frontend-Id"] = "23";
            req.Headers["X-Frontend-Version"] = "1.0.0";
            req.Headers["X-Request-With"] = "N-garage";

            var result = WebUtil.RequestJson(req);
            return new VideoStatus
            {
                PostStatus = result.data.postStatus,
                VideoFileStatus = result.data.videoFileStatus,
                ThumbnailStatus = result.data.thumbnailStatus,
            };
        }

        #region VideoList
        private readonly static Regex VideoRegex = new Regex(
            @"<li class=""item"">\s*<div class=""item__thumbnail"">\s*" +
            @"(?<content>[\s\S]+?)\s*" +
            @"</li>\s*</ul>\s*</li>");
        
        private static readonly Regex IdRegex = new Regex(
            @"<li class=""videoId"" data-ga-event-track-target=""videoList_[\w]+VideoMetadata_videoId"">\s*(.+?)\s*</li>");
        private static readonly Regex ThreadIdRegex = new Regex(
            @"<li class=""threadId"" data-ga-event-track-target=""videoList_[\w]+VideoMetadata_threadId"">\s*watch/(\d+)\s*</li>");
        private static readonly Regex TitleRegex = new Regex(
            @"<h3 class=""item__metadata__title"">\s*([\s\S]+?)\s*</h3>");
        private static readonly Regex DataRegex = new Regex(
            @"<div class=""item__metadata__date"">[\s\S]+?\s*<span>\s*(.*?)\s*公開\s*</span>");
        private static readonly Regex StatusRegex = new Regex(
            @"<div class=""item__metadata__date"">\s*<span>\s*([\s\S]*?)\s*</span>\s*</div>");

        /// <summary>
        /// 動画のリストを取得します。
        /// </summary>
        public static List<ChannelVideoData> ParseVideoList(string html)
        {
            if (html == null)
            {
                throw new ArgumentNullException("html");
            }

            return VideoRegex
                .Matches(html)
                .OfType<Match>()
                .Where(_ => _.Success)
                .Select(_ => ParseVideoData(_.Groups["content"].Value))
                .ToList();
        }

        /// <summary>
        /// チャンネルツールのHTML情報から、動画情報を作成します。
        /// </summary>
        private static ChannelVideoData ParseVideoData(string htmlContent)
        {
            var result = new ChannelVideoData
            {
                Timestamp = DateTime.Now,
            };

            var m = IdRegex.Match(htmlContent);
            if (!m.Success)
            {
                return null;
            }
            result.Id = m.Groups[1].Value;

            m = ThreadIdRegex.Match(htmlContent);
            if (!m.Success)
            {
                return null;
            }
            result.ThreadId = m.Groups[1].Value;

            m = TitleRegex.Match(htmlContent);
            if (!m.Success)
            {
                return null;
            }
            result.Title = m.Groups[1].Value;

            // 動画の公開日時
            m = DataRegex.Match(htmlContent);
            if (!m.Success)
            {
                return null;
            }
            result.StartTime = DateTime.Parse(m.Groups[1].Value);

            // 表示／非表示
            var hidden = "isPrivate";
            result.IsVisible = !htmlContent.Contains(hidden);

            // 会員限定／全員公開
            var memberOnly = "isMemberOnly";
            result.IsMemberOnly = htmlContent.Contains(memberOnly);

            return result;
        }
        #endregion

        #region Upload
        /// <summary>
        /// 新しいvideoidを要求します。
        /// </summary>
        public static string RequestNewVideoId(int channelId, string filename,
                                               CookieContainer cc)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            var randInt1 = MathEx.MathUtil.RandInt();
            var randInt2 = MathEx.MathUtil.RandInt();
            var filekey = $"{filename}-{randInt1}-{randInt2}-video/mp4";
            var param = new Dictionary<string, object>
            {
                ["file_key"] = filekey
            };

            var url = MakeGetVideoIdUrl(channelId);
            var req = WebUtil.MakeJsonRequest(url, cc);
            req.Headers["X-Frontend-Id"] = "23";
            req.Headers["X-Frontend-Version"] = "1.0.0";
            req.Headers["X-Request-With"] = "N-garage";
            WebUtil.WriteJsonData(req, param);

            var result = WebUtil.RequestJson(req);
            return result.data.videoId;
        }

        /// <summary>
        /// 動画アップロード用のURLを取得します。
        /// </summary>
        public static string RequestUploadChunkUrl(int channelId, string videoId,
                                                   CookieContainer cc)
        {
            var url = MakeGetUploadChunkUrl(channelId, videoId);
            var req = WebUtil.MakeJsonRequest(url, cc);
            req.Headers["X-Frontend-Id"] = "23";
            req.Headers["X-Frontend-Version"] = "1.0.0";
            req.Headers["X-Request-With"] = "N-garage";

            var result = WebUtil.RequestJson(req);
            var subPath = result.data.url;
            return $"{UploaUrldBase}{subPath}";
        }

        /// <summary>
        /// 動画のサムネイル時刻を設定します。
        /// </summary>
        public static void RequestSetThumbnailTime(int channelId, string videoId,
                                                   TimeSpan thumbnailTime,
                                                   CookieContainer cc)
        {
            var url = MakeThumbnailTimeUrl(channelId, videoId);
            var req = WebUtil.MakeNormalRequest(url, cc);
            req.Headers["X-Frontend-Id"] = "23";
            req.Headers["X-Frontend-Version"] = "1.0.0";
            req.Headers["X-Request-With"] = "N-garage";

            WebUtil.WritePostData(req, new Dictionary<string, object>
            {
                ["timing"] = (int)thumbnailTime.TotalMilliseconds
            });

            var text = WebUtil.RequestHttpText(req);
        }
        #endregion

        #region Edit
        /// <summary>
        /// 動画の情報更新時に使うデフォルトのパラメータを作成します。
        /// </summary>
        public static Dictionary<string, object> CreateDefaultParam()
        {
            return new Dictionary<string, object>
            {
                ["genre"] = new Dictionary<string, object>
                {
                    ["key"] = "game"
                },

                // 公開or非公開 (公開時はtrue)
                ["publish"] = true,

                ["commentDisplayType"] = "normal",
                ["initialCommentDisplayHidden"] = false,
                ["commentType"] = "member_only",
                ["distributionPlan"] = "member_only",
                ["tagEditableUserType"] = "nobody", // タグの編集モード
                ["adultVideo"] = false,

                ["permissionSettings"] = new Dictionary<string, object>
                {
                    ["allowUadFlag"] = true,
                    ["allowNgShareFlag"] = true,
                    ["allowIchibaFlag"] = true,
                    ["allowVideoSearchFlag"] = true,
                    ["onlyHomelandFlag"] = false,
                    ["enableSecureHlsFlag"] = false,
                    ["enableVastFlag"] = false,
                    ["deviceSettings"] = new Dictionary<string, object>
                    {
                        ["allowMobileAuFlag"] = true,
                        ["allowMobileDocomoFlag"] = true,
                        ["allowMobileSoftbankFlag"] = true,
                        ["allowMobileWillcomFlag"] = true,
                        ["allowAppleFlag"] = true,
                        ["allowAndroidAndOtherFlag"] = true,
                        ["allowSwitchFlag"] = true,
                        ["allow3dsFlag"] = true,
                        ["allowNicoboxFlag"] = false,
                        ["allowExternalPlayerFlag"] = true,
                    }
                }
            };
        }

        /// <summary>
        /// 動画情報更新時に使うJSONデータを作成します。
        /// </summary>
        public static Dictionary<string, object> CreateUpdateJson(string title, string description,
                                                                  DateTime startAt, DateTime? endAt = null,
                                                                  bool isPublish = true)
        {
            var param = CreateDefaultParam();
            param["title"] = title;
            param["description"] = description;
            param["publish"] = isPublish;

            var visibleSpan = new Dictionary<string, object>();
            visibleSpan["visibleStartAt"] = startAt.ToString("yyyy-MM-ddTHH:mm:ss.fffzzzz");
            if (endAt != null)
            {
                visibleSpan["visibleEndAt"] = endAt.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffzzzz");
            }
            param["visibleSpan"] = visibleSpan;

            return param;
        }

        /// <summary>
        /// 動画情報更新時に使うJSONデータを作成します。
        /// </summary>
        public static Dictionary<string, object> CreateRegisterJson(string title, string description,
                                                                    List<string> tags,
                                                                    DateTime startAt, DateTime? endAt = null,
                                                                    bool isPublish = true)
        {
            var param = CreateUpdateJson(title, description, startAt, endAt, isPublish);
            param["tags"] = tags;
            param["thumbnail"] = new Dictionary<string, object>
            {
                ["selectThumbnailIndex"] = "0",
            };

            return param;
        }

        /// <summary>
        /// 動画情報の編集リクエストを発行します。
        /// </summary>
        public static string RequestEdit(int channelId, string videoId,
                                         Dictionary<string, object> param,
                                         CookieContainer cc, bool isFirst = true)
        {
            if (string.IsNullOrEmpty(videoId) || !videoId.StartsWith("so"))
            {
                throw new ArgumentException(
                    $"videoId is invalid value: {videoId}", nameof(videoId));
            }

            if (cc == null)
            {
                throw new ArgumentNullException(nameof(cc));
            }

            // 動画更新時に必要になる必須パラメータを設定します。
            var cparam = new Dictionary<string, object>(param);
            cparam["videoId"] = videoId;
            cparam["channelId"] = $"ch{channelId}";

            // 実際に動画情報の更新リクエストを発行します。
            var url = MakeVideoUrl(channelId, videoId);
            var req = WebUtil.MakeJsonRequest(url, cc);

            req.Method = (isFirst ? "POST" : "PUT");
            req.Headers["X-Frontend-Id"] = "23";
            req.Headers["X-Frontend-Version"] = "1.0.0";
            req.Headers["X-Request-With"] = "N-garage";
            WebUtil.WriteJsonData(req, cparam);

            return WebUtil.RequestHttpText(req);
        }

        /// <summary>
        /// 動画情報の登録リクエストを発行します。
        /// </summary>
        public static void RequestRegister(int channelId, string videoId,
                                           CookieContainer cc)
        {
            if (string.IsNullOrEmpty(videoId) || !videoId.StartsWith("so"))
            {
                throw new ArgumentException(
                    $"videoId is invalid value: {videoId}", nameof(videoId));
            }

            if (cc == null)
            {
                throw new ArgumentNullException(nameof(cc));
            }

            // 実際に動画情報の更新リクエストを発行します。
            var url = MakeVideoRegisterUrl(channelId, videoId);
            var req = WebUtil.MakeNormalRequest(url, cc);

            req.Method = "POST";
            req.Headers["X-Frontend-Id"] = "23";
            req.Headers["X-Frontend-Version"] = "1.0.0";
            req.Headers["X-Request-With"] = "N-garage";

            WebUtil.RequestHttp(req);
        }
        #endregion

        #region Delete
        /// <summary>
        /// 動画の削除リクエストを発行します。
        /// </summary>
        public static string RequestDelete(int channelId, string videoId,
                                           CookieContainer cc)
        {
            if (string.IsNullOrEmpty(videoId) || !videoId.StartsWith("so"))
            {
                throw new ArgumentException(
                    $"videoId is invalid value: {videoId}", nameof(videoId));
            }

            if (cc == null)
            {
                throw new ArgumentNullException(nameof(cc));
            }

            var url = MakeVideoDeleteUrl(channelId, videoId);

            // CSRFを取得するためのリクエストを発行します。
            var result = WebUtil.RequestHttpText(url, null, cc);
            if (string.IsNullOrEmpty(result))
            {
                return result;
            }

            var regex = new Regex("<input type=\"hidden\" name=\"_csrf\" value=\"([^\"]+)\">");
            var m = regex.Match(result);
            if (!m.Success)
            {
                return null;
            }

            var param = new Dictionary<string, object>()
            {
                ["_csrf"] = m.Groups[1].Value
            };

            // 実際に動画情報の更新リクエストを発行します。
            result = WebUtil.RequestHttpText(url, param, cc);
            if (string.IsNullOrEmpty(result))
            {
                return result;
            }

            return result;
        }
        #endregion

        #region Search
        /// <summary>
        /// チャンネルツール内でキーワードによる検索結果をhtmlで取得します。
        /// </summary>
        public static string RequestSearch(CookieContainer cc, int channelId, string keyword,
                                           SearchSort sort = SearchSort.VisibleStartTime,
                                           SearchOrder order = SearchOrder.Desc,
                                           int page = 1, int size = 20)
        {
            if (cc == null)
            {
                throw new ArgumentNullException("cc");
            }

            // 検索用のリクエストを作成します。
            var param = new Dictionary<string, object>();
            param["channel_id"] = channelId;
            param["page"] = page;
            param["size"] = size;
            param["sort"] = EnumUtil.GetDescription(sort);
            param["order"] = EnumUtil.GetDescription(order);
            param["keyword"] = keyword;

            // パラメータはGET用のエンコードを行います。
            var url = MakeSearchUrl(channelId);
            var encodedParam = WebUtil.EncodeParam(param);
            var searchUrl = $"{url}&{encodedParam}";

            return WebUtil.RequestHttpText(searchUrl, null, cc, Encoding.UTF8);
        }
        
        /// <summary>
        /// チャンネルページ上から動画の検索を行います。
        /// </summary>
        public static IEnumerable<ChannelVideoData> Search(CookieContainer cc,
                                                           int channelId,
                                                           string keyword,
                                                           SearchSort sort = SearchSort.RegisteredAt,
                                                           SearchOrder order = SearchOrder.Desc,
                                                           int page = 1,
                                                           int size = 20)
        {
            if (cc == null)
            {
                throw new ArgumentNullException(nameof(cc));
            }

            var text = RequestSearch(
                cc, channelId, keyword, sort, order, page, size);
            if (string.IsNullOrEmpty(text))
            {
                return new List<ChannelVideoData>();
            }

            //Save("searcg.html", text);

            return ParseVideoList(text);
        }
        #endregion
    }
}
