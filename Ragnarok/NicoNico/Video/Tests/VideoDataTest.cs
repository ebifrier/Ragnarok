#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;

namespace Ragnarok.NicoNico.Video.Tests
{
    [TestFixture()]
    internal sealed class VideoDataTest
    {
        enum RemoveTagType
        {
            None,
            All,
            ATagOnly,
        }

        /// <summary>
        /// htmlのタグを削除します。
        /// </summary>
        private string RemoveTag(RemoveTagType removeTagType, string html)
        {
            switch (removeTagType)
            {
                case RemoveTagType.None:
                    return html;
                    
                case RemoveTagType.All:
                    var result = Regex.Replace(html, @"<\w+[^>]*?>", "");
                    return Regex.Replace(result, @"</?\w+( /)?>", "");

                case RemoveTagType.ATagOnly:
                    return Regex.Replace(html, @"<a[^>]*?>((sm|mylist|user|watch).*?)</a>", "$1");
            }

            return string.Empty;
        }

        /// <summary>
        /// VideoDataオブジェトの基本的なチェックを行います。
        /// </summary>
        private void AssertBase(VideoData video)
        {
            var now = DateTime.Now;
            var span = TimeSpan.FromSeconds(60);

            Assert.NotNull(video);
            Assert.True(now - span <= video.Timestamp && video.Timestamp <= now);
        }

        private void TestSM9(VideoData video, RemoveTagType removeTagType)
        {
            AssertBase(video);

            Assert.AreEqual("sm9", video.IdString);
            Assert.AreEqual(null, video.ThreadId);
            Assert.AreEqual("新・豪血寺一族 -煩悩解放 - レッツゴー！陰陽師", video.Title);
            Assert.AreEqual("レッツゴー！陰陽師（フルコーラスバージョン）", video.Description);
            Assert.AreEqual(DateTime.Parse("2007-03-06T00:33:00+09:00"), video.StartTime);
            Assert.AreEqual(319, video.LengthSeconds);
            Assert.AreEqual("http://tn-skr2.smilevideo.jp/smile?i=9", video.ThumbnailUrl);
            Assert.NotNull(video.IsVisible);
            Assert.True(video.IsVisible.Value);
            if (removeTagType == RemoveTagType.None)
            {
                Assert.NotNull(video.IsMemberOnly);
                Assert.False(video.IsMemberOnly.Value);
            }
            else
            {
                Assert.Null(video.IsMemberOnly);
            }
            Assert.GreaterOrEqual(video.ViewCounter, 15104451);
            Assert.GreaterOrEqual(video.CommentCounter, 4320605);
            Assert.GreaterOrEqual(video.MylistCounter, 159164);

            var tags = new string[]
            {
                "陰陽師", "レッツゴー！陰陽師", "公式", "音楽", "ゲーム"
            };
            Assert.LessOrEqual(tags.Count(), video.TagList.Count());
            tags.ForEach(_ => Assert.True(video.TagList.Contains(_)));
        }

        private void TestSM941537(VideoData video, RemoveTagType removeTagType)
        {
            AssertBase(video);

            Assert.AreEqual("sm941537", video.IdString);
            Assert.AreEqual(null, video.ThreadId);
            Assert.AreEqual("ボーカロイド　初音ミク　デモソング", video.Title);
            Assert.AreEqual(RemoveTag(removeTagType,
                "クリプトン開発のVOCALOID。つまり音声合成ソフトです　　　" +
                "　　　　　　　　　　　　　　　　　　　　　　　　　　　　" +
                "　　　　　　　　　CV：藤田咲（主な出演作＊ TVアニメ「と" +
                "きめきメモリアルOnly Love」弥生水奈役ＴＶアニメ「がくえんゆー" +
                "とぴあ まなびストレート！」小鳥桃葉役 TVアニメ「つよきすCool×Sweet」" +
                "　蟹沢きぬ、 TVアニメ「吉永さん家のガーゴイル」　など）　　　　　" +
                "詳しくはこちらhttp://www.crypton.co.jp/mp/pages/prod/vocaloid/cv01.jsp　" +
                "私もミクで遊んでみました " +
                "<a href=\"http://www.nicovideo.jp/mylist/4883031\" target=\"_blank\">mylist/4883031</a>"),
                video.Description);
            Assert.AreEqual(DateTime.Parse("2007-08-29T14:02:39+09:00"), video.StartTime);
            Assert.AreEqual(49, video.LengthSeconds);
            Assert.AreEqual("http://tn-skr2.smilevideo.jp/smile?i=941537", video.ThumbnailUrl);
            Assert.NotNull(video.IsVisible);
            Assert.True(video.IsVisible.Value);
            if (removeTagType == RemoveTagType.None)
            {
                Assert.NotNull(video.IsMemberOnly);
                Assert.False(video.IsMemberOnly.Value);
            }
            else
            {
                Assert.Null(video.IsMemberOnly);
            }
            Assert.GreaterOrEqual(video.ViewCounter, 165419);
            Assert.GreaterOrEqual(video.CommentCounter, 1940);
            Assert.GreaterOrEqual(video.MylistCounter, 2363);

            var tags = new string[]
            {
                "音楽", "初音ミク", "公式デモ", "VOCALOID"
            };
            Assert.LessOrEqual(tags.Count(), video.TagList.Count());
            tags.ForEach(_ => Assert.True(video.TagList.Contains(_)));
        }

        private void TestSM500873(VideoData video, RemoveTagType removeTagType)
        {
            AssertBase(video);

            Assert.AreEqual("sm500873", video.IdString);
            Assert.AreEqual(null, video.ThreadId);
            Assert.AreEqual("組曲『ニコニコ動画』 ", video.Title);
            Assert.AreEqual(RemoveTag(removeTagType,
                "<font size=\"+2\">700万再生、ありがとうございました。<br />" +
                "記念動画公開中です ⇒ (<a href=\"http://www.nicovideo.jp/watch/sm14242201\" class=\"watch\">sm14242201</a>)<br />" +
                "</font><br />" +
                "ニコニコ動画(β・γ)で人気のあった曲などを繋いでひとつの曲にしてみました(2度目)。全33曲。<br />" +
                "<font size=\"-2\">※多くの方を誤解させてしまっているようですが(申し訳ないです)、厳密には「組曲」ではなく「メドレー」です。<br />" +
                "「組曲という名前のメドレー」だと思ってください。</font><br /><br />" +
                "<a href=\"http://www.nicovideo.jp/mylist/1535765\" target=\"_blank\">mylist/1535765</a><br />" +
                "<a href=\"http://www.nicovideo.jp/user/145217\" target=\"_blank\">user/145217</a>"),
                video.Description);
            Assert.AreEqual(DateTime.Parse("2007-06-23T18:27:06+09:00"), video.StartTime);
            Assert.AreEqual(648, video.LengthSeconds);
            Assert.AreEqual("http://tn-skr2.smilevideo.jp/smile?i=500873", video.ThumbnailUrl);
            Assert.NotNull(video.IsVisible);
            Assert.True(video.IsVisible.Value);
            if (removeTagType == RemoveTagType.None)
            {
                Assert.NotNull(video.IsMemberOnly);
                Assert.False(video.IsMemberOnly.Value);
            }
            else
            {
                Assert.Null(video.IsMemberOnly);
            }
            Assert.GreaterOrEqual(video.ViewCounter, 8882231);
            Assert.GreaterOrEqual(video.CommentCounter, 4412089);
            Assert.GreaterOrEqual(video.MylistCounter, 130981);

            var tags = new string[]
            {
                "音楽", "アレンジ", "組曲『ニコニコ動画』", "空気の読めるWMP",
                "ニコニコオールスター",
            };
            Assert.LessOrEqual(tags.Count(), video.TagList.Count());
            tags.ForEach(_ => Assert.True(video.TagList.Contains(_)));
        }

        private void TestSM1949063(VideoData video, RemoveTagType removeTagType)
        {
            AssertBase(video);

            Assert.AreEqual("sm1949063", video.IdString);
            Assert.AreEqual(null, video.ThreadId);
            Assert.AreEqual("【作業用ＢＧＭ】切なくなる綺麗な曲達【無節操オタ神曲】", video.Title);
            Assert.AreEqual(RemoveTag(removeTagType,
                "【曲名と関連ワード】1.調和～oto～with reflection《KOKIA》(銀色の髪のアギト)" + 
                "2.迷夢《志方あきこ》(緑の森で眠ル鳥)3.伝承の詩―vox―Languet anima mea《いとうかなこ》(Lamento)" +
                "4.賛えし闘いの詩《いとうかなこ》(Lamento)5.とある竜の恋の歌《いとうかなこ》(竜†恋)"+
                "6.愛の神、魂狩る者《NAKI》(らぶデス2)7.brilliant azure《ちっち》(夜明け前より瑠璃色な)"+
                "8.月のワルツ《諫山実生》9.ふたりの場所《片霧 烈火》(神無ノ鳥)" +
                "10.サヨナラ・ヘブン《猫叉Master》(pop'n14)11.shoot《関智一》(友人に捧ぐ)" +
                "12.Unknown, little Scarlet(東方UNオーエンは彼女なのか？)13.Granado Espada《KimJS》" +
                "14.雫piano ver《あさき 蜉蝣之羽》(pop'n12)15.花帰葬交響曲(志方あきこ)" +
                "16.東方萃夢想《上海アリス幻樂団》(黄昏フロンティア)" +
                "マイリスト→<a href=\"http://www.nicovideo.jp/mylist/4068517\" target=\"_blank\">mylist/4068517</a>。" +
                "テンション上げたい時用ＢＧＭ→<a href=\"http://www.nicovideo.jp/watch/sm1939013\" class=\"watch\">sm1939013</a>"),
                video.Description);
            Assert.AreEqual(DateTime.Parse("2008-01-04T22:56:55+09:00"), video.StartTime);
            Assert.AreEqual(61*60+43, video.LengthSeconds);
            Assert.AreEqual("http://tn-skr4.smilevideo.jp/smile?i=1949063", video.ThumbnailUrl);
            Assert.NotNull(video.IsVisible);
            Assert.True(video.IsVisible.Value);
            if (removeTagType == RemoveTagType.None)
            {
                Assert.NotNull(video.IsMemberOnly);
                Assert.False(video.IsMemberOnly.Value);
            }
            else
            {
                Assert.Null(video.IsMemberOnly);
            }
            Assert.GreaterOrEqual(video.ViewCounter, 166000);
            Assert.GreaterOrEqual(video.CommentCounter, 3700);
            Assert.GreaterOrEqual(video.MylistCounter, 4800);

            var tags = new string[]
            {
                "音楽", "作業用BGM", "最初からクライマックス", "作業妨害用BGM",
                "志方あきこ", "KOKIA", "いとうかなこ", "片霧烈火",
            };
            Assert.LessOrEqual(tags.Count(), video.TagList.Count());
            tags.ForEach(_ => Assert.True(video.TagList.Contains(_)));
        }

        private void Test1441099865(VideoData video, RemoveTagType removeTagType)
        {
            AssertBase(video);

            Assert.AreEqual("so27063885", video.IdString);
            Assert.AreEqual(removeTagType == RemoveTagType.None ? "1441099865" : null, video.ThreadId);
            Assert.AreEqual("【9/30まで】将棋プレミアム【無料トライアルキャンペーン実施中】", video.Title);
            Assert.AreEqual(RemoveTag(removeTagType,
                "囲碁・将棋チャンネルの新会員サービス【将棋プレミアム】が8月10日(月)よりスタート！<br>" +
                "いつでもどこでも見られるオンデマンドサービスをはじめ、会員イベントやプレミアムグッズプレゼントなど将棋ファン必見のサービスです！<br><br>" +
                "9月30日(水)まで無料メルマガ会員登録を行うと、すべてのコンテンツが見放題となる<br>" +
                "「無料トライアルキャンペーン 」も実施しています。<br><br>" +
                "詳しくは将棋プレミアム(<a href=\"http://www.igoshogi.net/shogipremium/\" target=\"_blank\">http://www.igoshogi.net/shogipremium/</a>)へ今すぐアクセス☆"),
                video.Description);
            Assert.AreEqual(DateTime.Parse("2015-09-01T18:30:00+09:00"), video.StartTime);
            Assert.AreEqual(30, video.LengthSeconds);
            Assert.AreEqual("http://tn-skr2.smilevideo.jp/smile?i=27063885", video.ThumbnailUrl);
            Assert.NotNull(video.IsVisible);
            Assert.True(video.IsVisible.Value);
            if (removeTagType == RemoveTagType.None)
            {
                Assert.NotNull(video.IsMemberOnly);
                Assert.False(video.IsMemberOnly.Value);
            }
            else
            {
                Assert.Null(video.IsMemberOnly);
            }
            Assert.GreaterOrEqual(video.ViewCounter, 190);
            Assert.GreaterOrEqual(video.CommentCounter, 0);
            Assert.GreaterOrEqual(video.MylistCounter, 0);

            var tags = new string[]
            {
                "ゲーム", "将棋", "生放送", "CM", "実験動画", "糸谷哲郎",
                "将棋プレミアム", "囲碁将棋チャンネル"
            };
            Assert.AreEqual(tags.Count(), video.TagList.Count());
            tags.ForEach(_ => Assert.True(video.TagList.Contains(_)));
        }

        private void TestMemberOnly(VideoData video, RemoveTagType removeTagType)
        {
            AssertBase(video);

            Assert.AreEqual("so23569455", video.IdString);
            Assert.AreEqual(removeTagType == RemoveTagType.None ? "1400284158" : null, video.ThreadId);
            Assert.AreEqual("祷陽子の囲碁講座「負けない置き碁の形」第2回 9子局の打ち方②", video.Title);
            Assert.AreEqual(RemoveTag(removeTagType,
                "【番組内容】<br>" +
                "負けない置き碁の形<br>" +
                "講師：祷 陽子（現姓 桑原）<br>" +
                "アシスタント：永山美穂<br>" +
                "詳細：置き碁をテーマにした入門講座です。<br>" +
                "　　　是非ご覧いただき、皆様の置き碁対局でお役立てください！<br>" +
                "【第1回は無料視聴できます】<br><br>" +
                "前回【<a href=\"http://www.nicovideo.jp/watch/1400283915\" class=\"watch\">watch/1400283915</a>】 次回【<a href=\"http://www.nicovideo.jp/watch/1400284278\" class=\"watch\">watch/1400284278</a>】<br>" +
                "第1回【<a href=\"http://www.nicovideo.jp/watch/1400283915\" class=\"watch\">watch/1400283915</a>】<br>" +
                "囲碁講座第1回マイリスト(全動画無料)【<a href=\"http://www.nicovideo.jp/mylist/50433520\" target=\"_blank\">mylist/50433520</a>】<br><br>" +
                "万波佳奈の囲碁講座「打ち込み＆荒らし大作戦」【<a href=\"http://www.nicovideo.jp/watch/1390348239\" class=\"watch\">watch/1390348239</a>】"),
                video.Description);
            Assert.AreEqual(DateTime.Parse("2014-05-17T10:00:00+09:00"), video.StartTime);
            Assert.AreEqual(604, video.LengthSeconds);
            Assert.AreEqual("http://tn-skr4.smilevideo.jp/smile?i=23569455", video.ThumbnailUrl);
            Assert.NotNull(video.IsVisible);
            Assert.True(video.IsVisible.Value);
            if (removeTagType == RemoveTagType.None)
            {
                Assert.NotNull(video.IsMemberOnly);
                Assert.True(video.IsMemberOnly.Value);
            }
            else
            {
                Assert.Null(video.IsMemberOnly);
            }
            Assert.GreaterOrEqual(video.ViewCounter, 110);
            Assert.GreaterOrEqual(video.CommentCounter, 0);
            Assert.GreaterOrEqual(video.MylistCounter, 0);

            var tags = new string[]
            {
                "ゲーム", "囲碁", "祷陽子", "講座", "置き碁", "永山美穂",
                "投稿者コメント",
            };
            Assert.AreEqual(tags.Count(), video.TagList.Count());
            tags.ForEach(_ => Assert.True(video.TagList.Contains(_)));
        }

        private void TestHidden(VideoData video, RemoveTagType removeTagType)
        {
            AssertBase(video);

            Assert.AreEqual("so27077196", video.IdString);
            Assert.AreEqual(removeTagType == RemoveTagType.None ? "1441274407" : null, video.ThreadId);
            Assert.AreEqual("【囲碁・無料】第40期 名人戦 挑戦手合七番勝負 第1局 井山裕太名人 vs 高尾紳路天元 1日目朝【取材映像・短縮版】", video.Title);
            Assert.AreEqual(RemoveTag(removeTagType,
                "【動画内容】<br>" +
                "■日時：2015年9月3～4日<br>" +
                "■対局者：井山裕太名人 vs 高尾紳路天元<br>" +
                "■持ち時間：各8時間（秒読み10分前）<br>" +
                "■対局場：ホテル椿山荘東京<br>" +
                "■主催：朝日新聞社、日本棋院、関西棋院<br><br>" +
                "第40期名人戦 挑戦手合七番勝負 第1局 1日目朝の対局開始前映像です。<br><br>" +
                "◆日本棋院 ＞ 第40期名人戦<br><a href=\"http://www.nihonkiin.or.jp/match/meijin/040.html\" target=\"_blank\">http://www.nihonkiin.or.jp/match/meijin/040.html</a><br><br><br>" +
                "【囲碁プレミアム】<br>" +
                "８月１０日からプレオープンしたオンデマンドサービス「囲碁プレミアム」では、<br>" +
                "各棋戦やイベントの取材映像を毎週お届けします！<br><br>" +
                "さらに現在、囲碁プレミアムでは「無料トライアルキャンペーン」を実施しております。<br>" +
                "無料メルマガ会員登録をして頂いたお客様は全員、サービスを期間中すべて無料でご利用頂けます。<br><br>" +
                "今対局の終局映像などもメルマガ会員登録をして頂ければ無料でご視聴できますので、ぜひ皆様ご登録下さい！！<br><br>" +
                "◆トップページ<br><a href=\"http://www.igoshogi.net/igopremium/\" target=\"_blank\">http://www.igoshogi.net/igopremium/</a><br>" +
                "◆無料トライアルキャンペーン<br><a href=\"http://www.igoshogi.net/igopremium/event/\" target=\"_blank\">http://www.igoshogi.net/igopremium/event/</a>"),
                video.Description);
            Assert.AreEqual(DateTime.Parse("2015-09-03T19:00:00+09:00"), video.StartTime);
            Assert.AreEqual(60, video.LengthSeconds);
            Assert.AreEqual("http://tn-skr1.smilevideo.jp/smile?i=27077196", video.ThumbnailUrl);
            Assert.NotNull(video.IsVisible);
            Assert.False(video.IsVisible.Value);
            if (removeTagType == RemoveTagType.None)
            {
                Assert.NotNull(video.IsMemberOnly);
                Assert.False(video.IsMemberOnly.Value);
            }
            else
            {
                Assert.Null(video.IsMemberOnly);
            }
            Assert.GreaterOrEqual(video.ViewCounter, 120);
            Assert.GreaterOrEqual(video.CommentCounter, 1);
            Assert.GreaterOrEqual(video.MylistCounter, 2);

            var tags = new string[]
            {
                "ゲーム", "囲碁", "名人戦", "第40期", "井山裕太", "高尾紳路",
                "囲碁将棋チャンネル", "実験動画", "囲碁プレミアム", "取材映像",
            };
            Assert.AreEqual(tags.Count(), video.TagList.Count());
            tags.ForEach(_ => Assert.True(video.TagList.Contains(_)));
        }

        /// <summary>
        /// CreateFromApiのテスト
        /// </summary>
        [Test()]
        public void CreateFromApiTest()
        {
            TestSM9(VideoData.CreateFromApi("sm9"), RemoveTagType.All);
            TestSM941537(VideoData.CreateFromApi("sm941537"), RemoveTagType.All);
            TestSM500873(VideoData.CreateFromApi("sm500873"), RemoveTagType.All);
            TestSM1949063(VideoData.CreateFromApi("sm1949063"), RemoveTagType.All);
            Test1441099865(VideoData.CreateFromApi(
                "http://www.nicovideo.jp/watch/1441099865?eco=1"), RemoveTagType.All);
            TestMemberOnly(VideoData.CreateFromApi("1400284158"), RemoveTagType.All);

            // 非表示の動画は取れない
            Assert.Catch(() => VideoData.CreateFromApi("1441274407"));
            Assert.Catch(() => VideoData.CreateFromApi("sm44422222222222222"));
            Assert.Catch(() => VideoData.CreateFromApi("134444444444444444"));
        }

        private static string[] GetEmailPassword(string filename)
        {
            var dir = Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments);
            var path = System.IO.Path.Combine(dir, filename);

            return System.IO.File.ReadAllLines(path);
        }

        /// <summary>
        /// CreateFromPageのテスト
        /// </summary>
        [Test()]
        public void CreateFromPageTest()
        {
            var data = GetEmailPassword(".niconico.txt");
            var cc = Login.Loginer.LoginDirect(data[0], data[1]);
            Assert.IsNotNull(cc);

            TestSM9(VideoData.CreateFromPage("sm9", cc), RemoveTagType.None);
            Console.WriteLine("Waiting ...");
            Thread.Sleep(5000);
            TestSM941537(VideoData.CreateFromPage("sm941537", cc), RemoveTagType.None);
            Console.WriteLine("Waiting ...");
            Thread.Sleep(5000);
            TestSM500873(VideoData.CreateFromPage("sm500873", cc), RemoveTagType.None);
            Console.WriteLine("Waiting ...");
            Thread.Sleep(5000);
            TestSM1949063(VideoData.CreateFromPage("sm1949063", cc), RemoveTagType.None);
            Console.WriteLine("Waiting ...");
            Thread.Sleep(5000);
            Test1441099865(VideoData.CreateFromPage(
                "http://www.nicovideo.jp/watch/1441099865?eco=1", cc), RemoveTagType.None);
            Console.WriteLine("Waiting ...");
            Thread.Sleep(5000);
            TestMemberOnly(VideoData.CreateFromPage("1400284158", cc), RemoveTagType.None);
            Console.WriteLine("Waiting ...");
            Thread.Sleep(5000);
            TestHidden(VideoData.CreateFromPage("1441274407", cc), RemoveTagType.None);
            Console.WriteLine("Waiting ...");
            Thread.Sleep(5000);

            Assert.Catch(() =>
                VideoData.CreateFromPage("sm44422222222222222", cc));
            Console.WriteLine("Waiting ...");
            Thread.Sleep(5000);
            Assert.Catch(() =>
                VideoData.CreateFromPage("134444444444444444", cc));
        }

        /// <summary>
        /// SnapshotApiのテスト
        /// </summary>
        [Test()]
        public void SnapshotApiTest()
        {
            var vs = SnapshotApi.Search("レッツゴー！陰陽師（フルコーラスバージョン）");
            TestSM9(vs.OrderByDescending(_ => _.ViewCounter).FirstOrDefault(), RemoveTagType.ATagOnly);

            vs = SnapshotApi.Search("クリプトン開発のVOCALOID。つまり音声合成ソフトです");
            TestSM941537(vs.OrderByDescending(_ => _.ViewCounter).FirstOrDefault(), RemoveTagType.ATagOnly);

            vs = SnapshotApi.Search("ニコニコ動画(β・γ)で人気のあった曲などを繋いで");
            TestSM500873(vs.OrderByDescending(_ => _.ViewCounter).FirstOrDefault(), RemoveTagType.ATagOnly);

            vs = SnapshotApi.Search("【作業用ＢＧＭ】切なくなる綺麗な曲達【無節操オタ神曲】");
            TestSM1949063(vs.OrderByDescending(_ => _.ViewCounter).FirstOrDefault(), RemoveTagType.ATagOnly);

            vs = SnapshotApi.Search("CM 実験動画 囲碁将棋チャンネル 糸谷哲郎", false);
            Test1441099865(vs.OrderByDescending(_ => _.ViewCounter).FirstOrDefault(), RemoveTagType.ATagOnly);

            vs = SnapshotApi.Search("祷陽子の囲碁講座「負けない置き碁の形」第2回 9子局の打ち方②", true);
            TestMemberOnly(vs.FirstOrDefault(), RemoveTagType.ATagOnly);

            vs = SnapshotApi.Search("どｊどういｔｓｄｋｊぁ", false);
            Assert.AreEqual(0, vs.Count());
        }
    }
}
#endif
