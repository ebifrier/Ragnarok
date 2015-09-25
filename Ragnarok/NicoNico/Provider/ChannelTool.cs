using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ragnarok.NicoNico.Provider
{
    using Net;
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
    /// ニコニコチャンネルで動画編集や動画アップロードを行うためのクラスです。
    /// </summary>
    public static class ChannelTool
    {
        public readonly static string VideoUrl =
            @"http://chtool.nicovideo.jp/video/video.php";
        public readonly static string VideoEditUrl =
            @"http://chtool.nicovideo.jp/video/video_edit.php";

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
                @"{0}?channel_id={1}&fileid={2}&pageID=",
                VideoEditUrl, channelId, videoId);
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
        public static CookieContainer Login(string loginId, string mail,
                                            string password)
        {
            var param = new Dictionary<string, object>();
            param["next_url"] = loginId;
            param["mail"] = mail;
            param["password"] = password;
            param["g"] = "dashboard";

            var cc = new CookieContainer();
            var text = WebUtil.RequestHttpText(
                @"https://secure.nicovideo.jp/secure/login?site=chtool",
                param, cc, Encoding.UTF8);

            Save("login.html", text);

            // エラー表示用の文字列が表示されていればエラーとします。
            if (string.IsNullOrEmpty(text))
            {
                throw new ChannelToolException(
                    "chtoolへのログインに失敗しました。");
            }

            return cc;
        }

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

            // 公開or非公開 (公開時は0)
            param["hide_flag"] = 0;

            // タグの編集モード
            // 0: 誰でも編集できる
            // 1: チャンネル会員のみが編集できる
            // 2: ユーザーによるタグ編集を禁止する
            param["tag_edit_flag"] = 0;

            // 1: 誰でも視聴可能
            // 4: チャンネル会員のみ視聴可能
            param["permission"] = 4; // 固定値？

            param["title_url"] = "";
            param["post_by_mobile"] = "";
            param["ppv_permission"] = 1;
            param["ppv_type"] = 0;
            param["ad_flag"] = 1;
            param["comment"] = "allow";
            param["display_flag"] = 0; // コメントの表示方法 (0:通常、1:動画の裏)

            param["retrieval_flag"] = 1; // 動画検索結果に表示させる
            param["uad_maintenance"] = 0; // ニコニ広告で宣伝させる
            param["uad_flag"] = 0;
            param["homeland_flag"] = 0; // 公開地域を日本国内のみに限定する
            param["commons_materials"] = "1"; // 使用したニコニコモンズ作品

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

            param["ignore_nicodic"] = 0; // 大百科の記事を表示させない
            param["out_flag"] = 0; // niconico外部プレイヤー で再生させない
            param["sexual_flag"] = 0; // R18かどうか
            param["market_flag"] = 0; // ニコニコ市場を表示する(する場合は0)
            param["mobile_item_max"] = 3;
            param["general_item_max"] = 10;
            param["disabled_ngs"] = 0; // 共有NGを無効にする
            param["account_linkage"] = 0; // 用途不明
            param["vast_enabled"] = 0; // 用途不明
            param["countries"] = ""; // 用途不明
            param["nicolanguage_code"] = ""; // 用途不明

            param["licensed_music[0][artist]"] = "";
            param["licensed_music[0][title]"] = "";
            param["licensed_music[0][rightsController]"] = "";
            param["licensed_music[0][opus_code]"] = "";
            param["licensed_music[0][lyricist]"] = "";
            param["licensed_music[0][composer]"] = "";
            param["licensed_music[0][uid]"] = "";

            return param;
        }

        /// <summary>
        /// 動画情報の編集リクエストを発行します。
        /// </summary>
        public static string RequestEdit(int channelId, int movieId,
                                         Dictionary<string, object> param,
                                         CookieContainer cc)
        {
            // 動画編集用のURLです。
            var url = MakeVideoEditUrl(channelId, movieId);

            var chKey = GetSmileChKey(cc, url);
            if (string.IsNullOrEmpty(chKey))
            {
                chKey = RequestSmileChKey(cc, url);
                if (string.IsNullOrEmpty(chKey))
                {
                    return null;
                }
            }


            
            // 動画更新時に必要になる必須パラメータを設定します。
            var cparam = new Dictionary<string, object>(param);
            cparam["smile_ch_key"] = chKey;
            cparam["mode"] = "edit";
            cparam["channel_id"] = channelId;
            cparam["fileid"] = movieId;

            // 実際に動画情報の更新リクエストを発行します。
            var result = WebUtil.RequestHttpText(url, cparam, cc, Encoding.UTF8);
            if (string.IsNullOrEmpty(result))
            {
                return result;
            }

            return result;
        }

        /// <summary>
        /// smile_ch_keyをネット上から取得します。
        /// </summary>
        public static string RequestSmileChKey(CookieContainer cc, string url)
        {
            Log.Debug("Get smile_ch_key try...");

            // smile_ch_keyを取得するために動画ページにアクセスします。
            var text = WebUtil.RequestHttpText(url, null, cc, Encoding.UTF8);
            if (string.IsNullOrEmpty(text))
            {
                throw new ChannelToolException(
                    "smile_ch_keyの取得に失敗しました。");
            }

            // デバッグ用にページを出力し、正しくsmile_ch_keyを取得できているか調べます。
            //Save("smile_ch_key.html", text);

            var chKey = GetSmileChKey(cc, url);
            if (string.IsNullOrEmpty(chKey))
            {
                throw new ChannelToolException(
                    "smile_ch_keyの取得に失敗しました。");
            }

            return chKey;
        }

        /// <summary>
        /// smile_ch_keyを<paramref name="cc"/>から探します。
        /// </summary>
        private static string GetSmileChKey(CookieContainer cc, string url)
        {
            // CookieContainerからsmile_ch_keyの情報を抜き出します。
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

        /// <summary>
        /// チャンネルツール内でキーワードによる検索結果をhtmlで取得します。
        /// </summary>
        public static string RequestSearch(CookieContainer cc, int channelId, string keyword,
                                           int pageId = 1, int limit = 20)
        {
            var url = MakeVideoUrl(channelId);

            // Cookieを取るために、指定のサイトに事前にログインしておく。
            var chKey = GetSmileChKey(cc, url);
            if (string.IsNullOrEmpty(chKey))
            {
                chKey = RequestSmileChKey(cc, url);
                if (string.IsNullOrEmpty(chKey))
                {
                    return null;
                }
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
            param["order"] = "_visible_start_time";
            param["keyword"] = keyword;

            // パラメータはGET用のエンコードを行います。
            var encodedParam = WebUtil.EncodeParam(param);
            var searchUrl = string.Format("{0}&{1}", url, encodedParam);
            return WebUtil.RequestHttpText(searchUrl, null, cc, Encoding.UTF8);
        }
        
        /// <summary>
        /// チャンネルページ上から動画の検索を行います。
        /// </summary>
        public static IEnumerable<VideoData> Search(CookieContainer cc,
                                                    int channelId,
                                                    string keyword, 
                                                    int pageId = 1,
                                                    int limit = 20)
        {
            if (cc == null)
            {
                throw new ArgumentNullException("cc");
            }

            var text = ChannelTool.RequestSearch(
                cc, channelId, keyword, pageId, limit);
            if (string.IsNullOrEmpty(text))
            {
                return new List<VideoData>();
            }

            return VideoData.FromChannelToolSearchResults(text);
        }

        /// <summary>
        /// 指定の期間に表示されたすべての動画を取得します。
        /// </summary>
        public static IEnumerable<VideoData> SearchPeriod(CookieContainer cc,
                                                          int channelId, 
                                                          string keyword,
                                                          DateTime? start,
                                                          DateTime? end)
        {
            const int MaxCount = 20;
            int pageId = 0;
            int count = 0;

            do
            {
                var movies = Search(cc, channelId, keyword, ++pageId, MaxCount);
                foreach (var movie in movies)
                {
                    if (start != null && movie.StartTime < start.Value)
                    {
                        yield break;
                    }

                    if ((start == null || start.Value <= movie.StartTime) &&
                        (end == null || movie.StartTime < end.Value))
                    {
                        yield return movie;
                    }
                }

                count = movies.Count();
            } while (count == MaxCount);
        }
    }
}
