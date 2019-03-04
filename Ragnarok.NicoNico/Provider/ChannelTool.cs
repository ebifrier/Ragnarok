using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ragnarok.NicoNico.Provider
{
    using Net;
    using Utility;
    using Video;

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
    /// 検索の表示順を指定します。
    /// </summary>
    public enum SearchOrder
    {
        [LabelDescription(Description = "_fileid")]
        BiggerFileId,
        [LabelDescription(Description = "fileid")]
        SmallerFileId,
        [LabelDescription(Description = "title")]
        Title,
        [LabelDescription(Description = "_title")]
        TitleReverse,
        [LabelDescription(Description = "_insertdate")]
        NewerInsertDate,
        [LabelDescription(Description = "insertdate")]
        OlderInsertDate,
        [LabelDescription(Description = "_uploaddate")]
        NewerUploadDate,
        [LabelDescription(Description = "uploaddate")]
        OlderUploadDate,
        [LabelDescription(Description = "_visible_start_time")]
        NewerVisibleStartTime,
        [LabelDescription(Description = "visible_start_time")]
        OlderVisibleStartTime,
    }

    /// <summary>
    /// ニコニコチャンネルで動画編集や動画アップロードを行うためのクラスです。
    /// </summary>
    public static class ChannelTool
    {
        public readonly static string BaseUrl = @"http://chtool.nicovideo.jp/";
        public readonly static string UploadedVideosUrl = BaseUrl + @"video/uploaded_videos";
        public readonly static string VideoUrl = BaseUrl + @"video/video.php";
        public readonly static string VideoEditUrl = BaseUrl + @"video/video_edit";
        public readonly static string VideoDeleteUrl = BaseUrl + @"video/video_delete.php";

        /// <summary>
        /// 動画編集用のサイトURLを作成します。
        /// </summary>
        public static string MakeUploadedVideosUrl(int channelId)
        {
            return string.Format(
                @"{0}?channel_id={1}",
                UploadedVideosUrl, channelId);
        }

        /// <summary>
        /// 動画用のサイトURLを作成します。
        /// </summary>
        public static string MakeVideoUrl(int channelId)
        {
            return string.Format(
                @"{0}?channel_id={1}",
                VideoUrl, channelId);
        }

        /// <summary>
        /// 動画編集用のサイトURLを作成します。
        /// </summary>
        public static string MakeVideoEditUrl(int channelId, int videoId)
        {
            return string.Format(
                @"{0}?channel_id={1}&fileid={2}&pageID=1",
                VideoEditUrl, channelId, videoId);
        }

        /// <summary>
        /// 動画削除用のサイトURLを作成します。
        /// </summary>
        public static string MakeVideoDeleteUrl(int channelId, int videoId)
        {
            return string.Format(
                @"{0}?channel_id={1}&fileid={2}&pageID=1",
                VideoDeleteUrl, channelId, videoId);
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

        #region smile_ch_key
        /// <summary>
        /// smile_ch_keyを取得します。
        /// </summary>
        public static string GetOrRequestSmileChKey(CookieContainer cc,
                                                    int channelId)
        {
            var chKey = GetSmileChKey(cc, channelId);
            if (string.IsNullOrEmpty(chKey))
            {
                chKey = RequestSmileChKey(cc, channelId);
            }

            return chKey;
        }

        /// <summary>
        /// smile_ch_keyをネット上から取得します。
        /// </summary>
        private static string RequestSmileChKey(CookieContainer cc, int channelId)
        {
            Log.Debug("get smile_ch_key try...");

            // smile_ch_keyを取得するために動画ページにアクセスします。
            var url = MakeVideoUrl(channelId);
            var text = WebUtil.RequestHttpText(url, null, cc, Encoding.UTF8);
            if (string.IsNullOrEmpty(text))
            {
                throw new ChannelToolException(
                    "smile_ch_keyの取得に失敗しました。");
            }

            // デバッグ用にページを出力し、正しくsmile_ch_keyを取得できているか調べます。
            //Save("smile_ch_key.html", text);

            // smile_ch_keyの取得を行います。
            var chKey = GetSmileChKey(cc, channelId);
            if (string.IsNullOrEmpty(chKey))
            {
                throw new ChannelToolException(
                    "smile_ch_keyの取得に失敗しました。");
            }

            Log.Debug("smile_ch_key is '{0}'.", chKey);
            return chKey;
        }

        /// <summary>
        /// smile_ch_keyを<paramref name="cc"/>から探します。
        /// </summary>
        private static string GetSmileChKey(CookieContainer cc, int channelId)
        {
            // CookieContainerからsmile_ch_keyの情報を抜き出します。
            var url = MakeVideoUrl(channelId);
            var collection = cc.GetCookies(new Uri(url));
            if (collection == null)
            {
                return null;
            }

            // smile_ch_keyの存在を確認します。
            var chKey = collection["smile_ch_key"];
            if (chKey == null || string.IsNullOrEmpty(chKey.Value))
            {
                return null;
            }

            return chKey.Value;
        }
        #endregion

        #region VideoList
        private readonly static Regex VideoRegex = new Regex(
            @"<li>\s*<div class=""videoListItems__thumbnail"">\s*" +
            @"(?<content>[\s\S]+?)\s*" +
            @"</div>\s*</div>\s*</li>");
        
        private static readonly Regex IdRegex = new Regex(
            @"<li data-gaEventTrackTarget=""videoList_[\w]+VideoMetadata_videoId"">\s*(.+)\s*</li>");
        private static readonly Regex ThreadIdRegex = new Regex(
            @"<li data-gaEventTrackTarget=""videoList_[\w]+VideoMetadata_threadId"">\s*watch/(\d+)\s*</li>");
        private static readonly Regex TitleRegex = new Regex(
            @"<h3 class=""videoListItems__metadata__controls__title"">\s*([\s\S]+?)\s*</h3>");
        private static readonly Regex DataRegex = new Regex(
            @"<div class=""videoListItems__metadata__date"">[\s\S]+?\s*<span>\s*(.*?)\s*公開\s*</span>");
        private static readonly Regex StatusRegex = new Regex(
            @"<div class=""videoListItems__metadata__date"">\s*<span>\s*(.*?)\s*</span>");

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
            var hidden = "videoListItems__metadata__label__status__private";
            result.IsVisible = !htmlContent.Contains(hidden);

            // 会員限定／全員公開
            var memberOnly = "videoListItems__metadata__label__status__memberOnly";
            result.IsMemberOnly = htmlContent.Contains(memberOnly);

            return result;
        }
        #endregion

        #region UploadedVideoList
        /// <summary>
        /// アップロードされた動画のリストを取得します。
        /// </summary>
        public static List<ChannelUploadedVideoData> GetUploadedVideoList(int channelId,
                                                                          CookieContainer cc)
        {
            var url = MakeUploadedVideosUrl(channelId);
            var text = WebUtil.RequestHttpText(url, null, cc, Encoding.UTF8);

            //File.WriteAllText("channelpage.html", text);

            return ParseUploadedVideoList(text);
        }

        /// <summary>
        /// アップロードされた動画のリストを取得します。
        /// </summary>
        public static List<ChannelUploadedVideoData> ParseUploadedVideoList(string html)
        {
            if (html == null)
            {
                throw new ArgumentNullException("html");
            }

            return VideoRegex
                .Matches(html)
                .OfType<Match>()
                .Where(_ => _.Success)
                .Select(_ => CreateUploadedVideoData(_))
                .ToList();
        }

        private static ChannelUploadedVideoData CreateUploadedVideoData(Match m)
        {
            var content = m.Groups["content"].Value;

            var sm = IdRegex.Match(content);
            if (!sm.Success)
            {
                return null;
            }
            var id = sm.Groups[1].Value;

            sm = TitleRegex.Match(content);
            if (!sm.Success)
            {
                return null;
            }
            var title = sm.Groups[1].Value;

            sm = StatusRegex.Match(content);
            if (!sm.Success)
            {
                return null;
            }
            var statusText = sm.Groups[1].Value;

            var status = (
                statusText.Contains("処理順番待ち") || statusText.Contains("エンコード中") ?
                    UploadedVideoStatus.Uploading :
                statusText.IsWhiteSpaceOnly() ? UploadedVideoStatus.Success :
                UploadedVideoStatus.Error);

            return new ChannelUploadedVideoData(status, id, title);
        }
        #endregion

        #region Edit
        /// <summary>
        /// 動画の情報更新時に使うデフォルトのPOSTパラメータを作成します。
        /// </summary>
        public static Dictionary<string, object> CreateDefaultPostParam()
        {
            var param = new Dictionary<string, object>();

            //param["visible_start_time"] = "2015-09-10 12:15:00";
            //param["auto_end_time"] = "on";
            //param["uploaddate"] = "2015-09-10 12:15:00";
            //param["distribute_as"] = "member_only";
            //param["is_nicos"] = 0; // 再生終了時にページ移動
            //param["nicos_jump"] = "";

            // すでに動画が初期化済みかどうか
            param["initialized"] = 1;
            param["mode"] = "edit";
            param["submit_edit"] = "";

            // 公開or非公開 (公開時は0)
            param["hide_flag"] = 0;

            // タグの編集モード
            // 0: 誰でも編集できる
            // 1: チャンネル会員のみが編集できる
            // 2: ユーザーによるタグ編集を禁止する
            param["tag_edit_flag"] = 2;

            // 1: 誰でも視聴可能
            // 4: チャンネル会員のみ視聴可能
            param["permission"] = (int)DistributeRange.MemberOnly;

            param["title_url"] = "";
            param["specify_uploaddate"] = "";
            param["ppv_type"] = 0;
            param["ad_flag"] = 1;
            param["display_flag"] = 0; // コメントの表示方法 (0:通常、1:動画の裏)
            param["nicos_jump"] = ""; // ジャンプ先動画

            param["retrieval_flag"] = 1; // 動画検索結果に表示させる
            param["uad_maintenance"] = 0; // ニコニ広告で宣伝させる
            param["uad_flag"] = 0;
            param["homeland_flag"] = 0; // 公開地域を日本国内のみに限定する

            param["market_flag"] = 0;
            param["commons_materials"] = "";
            param["option_flag_dmc_hls_encryption_method"] = 0;

            // NG関連の指定
            param["deny_mobile"] = 0;
            param["mobile_ng_docomo"] = 0;
            param["mobile_ng_au"] = 0;
            param["mobile_ng_softbank"] = 0;
            param["mobile_ng_apple"] = 0;
            param["mobile_ng_other"] = 0;
            param["ng_tv"] = 0;
            param["ng_nintendo"] = 0;
            param["ng_boqz"] = 0;
            param["ng_dolce"] = 0;
            param["ng_sun"] = 0;
            param["ng_xboxone"] = 0;
            param["ng_nicoswitch"] = 0;
            param["ng_nicobox"] = 0;

            param["ignore_nicodic"] = 0; // 大百科の記事を表示させない
            param["out_flag"] = 0; // niconico外部プレイヤー で再生させない
            param["sexual_flag"] = 0; // R18かどうか
            param["mobile_item_max"] = 3;
            param["general_item_max"] = 10;
            param["disabled_ngs"] = 0; // 共有NGを無効にする
            param["account_linkage"] = ""; // 用途不明
            param["vast_enabled"] = 0; // 用途不明
            param["countries"] = ""; // 用途不明
            param["nicolanguage_code"] = ""; // 用途不明
            param["comment_visibility"] = 0;

            return param;
        }

        /// <summary>
        /// 動画情報の編集リクエストを発行します。
        /// </summary>
        public static string RequestEdit(int channelId, string videoId,
                                         Dictionary<string, object> param,
                                         CookieContainer cc)
        {
            if (string.IsNullOrEmpty(videoId) || !videoId.StartsWith("so"))
            {
                throw new ArgumentException(
                    $"videoId is invalid value: {videoId}", "videoId");
            }

            if (cc == null)
            {
                throw new ArgumentNullException("cc");
            }

            var fileId = int.Parse(videoId.Substring(2));

            // smile_ch_keyは過去のオペレーションで削除されていることがあるため、
            // 必ず新規に取得します。
            var chKey = RequestSmileChKey(cc, channelId);
            if (string.IsNullOrEmpty(chKey))
            {
                return null;
            }

            // 詳細編集前に専用のクッキーを仕込んでおく。
            // これをやらないとエラーになって処理が失敗する。
            cc.Add(new Uri(BaseUrl), new Cookie("smile_ch_key", chKey));

            // 動画更新時に必要になる必須パラメータを設定します。
            var cparam = new Dictionary<string, object>(param);
            cparam["smile_ch_key"] = chKey;
            cparam["channel_id"] = channelId;
            cparam["fileid"] = fileId;
            cparam["mode"] = "edit";

            // 実際に動画情報の更新リクエストを発行します。
            var url = MakeVideoEditUrl(channelId, fileId);
            var req = WebUtil.MakeNormalRequest(url, cc);

            var oldValue = WebUtil.IsConvertPostParamSpaceToPlus;
            WebUtil.IsConvertPostParamSpaceToPlus = true;
            WebUtil.WritePostData(req, cparam);
            WebUtil.IsConvertPostParamSpaceToPlus = oldValue;

            var buffer = WebUtil.RequestHttp(req);
            if (buffer == null)
            {
                return null;
            }

            var result = Encoding.UTF8.GetString(buffer);
            if (string.IsNullOrEmpty(result))
            {
                return result;
            }

            //Save(@"E:\programs\develop\Ragnarok\index.html", result);
            return result;
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
                    $"videoId is invalid value: {videoId}", "videoId");
            }

            if (cc == null)
            {
                throw new ArgumentNullException("cc");
            }

            var fileId = int.Parse(videoId.Substring(2));

            // 動画更新時に必要になる必須パラメータを設定します。
            var param = new Dictionary<string, object>();
            param["channel_id"] = channelId;
            param["fileid"] = fileId;

            // 実際に動画情報の更新リクエストを発行します。
            var url = MakeVideoDeleteUrl(channelId, fileId);
            var result = WebUtil.RequestHttpText(url, param, cc, Encoding.UTF8);
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
                                           SearchOrder order = SearchOrder.NewerVisibleStartTime,
                                           int pageId = 1, int limit = 20)
        {
            if (cc == null)
            {
                throw new ArgumentNullException("cc");
            }

            // Cookieを取るために、指定のサイトに事前にログインしておく。
            var chKey = GetOrRequestSmileChKey(cc, channelId);
            if (string.IsNullOrEmpty(chKey))
            {
                return null;
            }

            // 検索用のリクエストを作成します。
            var param = new Dictionary<string, object>();
            param["channel_id"] = channelId;
            param["smile_ch_key"] = chKey;
            param["pageID"] = pageId;
            param["post_by_mobile"] = 0;
            param["limit"] = limit;
            param["ppv_type"] = "all";
            param["hide_flag"] = "";
            param["permission"] = "1,2,3,4,5";
            param["order"] = EnumEx.GetDescription(order);
            param["keyword"] = keyword;

            // パラメータはGET用のエンコードを行います。
            var url = MakeVideoUrl(channelId);
            var encodedParam = WebUtil.EncodeParam(param);
            var searchUrl = string.Format("{0}&{1}", url, encodedParam);

            return WebUtil.RequestHttpText(searchUrl, null, cc, Encoding.UTF8);
        }
        
        /// <summary>
        /// チャンネルページ上から動画の検索を行います。
        /// </summary>
        public static IEnumerable<ChannelVideoData> Search(CookieContainer cc,
                                                           int channelId,
                                                           string keyword,
                                                           SearchOrder order = SearchOrder.NewerVisibleStartTime,
                                                           int pageId = 1,
                                                           int limit = 20)
        {
            if (cc == null)
            {
                throw new ArgumentNullException("cc");
            }

            var text = RequestSearch(
                cc, channelId, keyword, order, pageId, limit);
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
