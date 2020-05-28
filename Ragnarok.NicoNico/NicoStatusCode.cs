using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.NicoNico
{
    using Utility;

    /// <summary>
    /// ニコニコAPI全般に使われるステータスコードです。
    /// </summary>
    public enum NicoStatusCode
    {
        /// <summary>
        /// 異常はありません。
        /// </summary>
        [Description("エラーはありません。")]
        Ok,

        /// <summary>
        /// 不明なエラーです。
        /// </summary>
        [Description("不明なエラーです。")]
        UnknownError,
        /// <summary>
        /// 放送ＩＤが正しくありません。
        /// </summary>
        [Description("放送ＩＤが正しくありません。")]
        InvalidLiveId,
        /// <summary>
        /// 取得したXMLのパースに失敗しました。
        /// </summary>
        [Description("受信したXmlデータに誤りがありました。")]
        XmlParseError,
        /// <summary>
        /// ネットワークエラーです。
        /// </summary>
        [Description("ネットワーク障害により通信できません。")]
        NetworkError,


        /// <summary>
        /// ビジー状態です。
        /// </summary>
        [Description("ビジー状態です。")]
        Busy,
        /// <summary>
        /// ログインしていません。
        /// </summary>
        [Description("ログインしていません。")]
        NotLogin,
        /// <summary>
        /// 放送が見つかりません。
        /// </summary>
        [Description("放送が見つかりません。")]
        NotFound,
        /// <summary>
        /// 放送はすでに終了しています。
        /// </summary>
        [Description("放送はすでに終了しています。")]
        AlreadyClosed,
        /// <summary>
        /// 権限がありません。
        /// </summary>
        [Description("アクセスする権限がありません。")]
        NoAuthentification,
        /// <summary>
        /// アクセスは拒否されました。
        /// </summary>
        [Description("アクセスが拒否されました。")]
        PermissionDenied,
        /// <summary>
        /// まもなく放送が始まります。
        /// </summary>
        [Description("放送が始まるまでしばらくお待ちください。")]
        ComingSoon,
        /// <summary>
        /// 放送はすでに人が多すぎです。
        /// </summary>
        [Description("放送はすでに満員です。")]
        Full,
        /// <summary>
        /// コミュニティに参加する必要があります。
        /// </summary>
        [Description("コミュニティのメンバーである必要があります。")]
        RequireCommunityMember,
        /// <summary>
        /// スレッドが見つかりません。
        /// </summary>
        [Description("スレッドが見つかりません。")]
        NotFoundThread,
        /// <summary>
        /// スロットが見つかりません。
        /// </summary>
        /// <remarks>
        /// heartbeat取得時によく現れます。
        /// </remarks>
        [Description("スロットが見つかりません。")]
        NotFoundSlot,
        /// <summary>
        /// スロットがありません。
        /// </summary>
        /// <remarks>
        /// heartbeat取得時によく現れます。
        /// </remarks>
        [Description("スロットがありません。")]
        NotExistSlot,
        /// <summary>
        /// 削除されました。
        /// </summary>
        [Description("削除されました。")]
        Deleted,
        /// <summary>
        /// ユーザーにより削除されました。
        /// </summary>
        [Description("ユーザーにより削除されました。")]
        DeletedByUser,
        /// <summary>
        /// 管理者により削除されました。
        /// </summary>
        [Description("管理者により削除されました。")]
        DeletedByVisor,
        /// <summary>
        /// プレミアム会員のみが視聴できます。
        /// </summary>
        [Description("プレミアム会員のみが視聴できます。")]
        PremiumOnly,
        /// <summary>
        /// 権利侵害などの理由で削除されました。
        /// </summary>
        [Description("権利侵害などの理由で削除されました。")]
        Violated,
    }

    /// <summary>
    /// ステータスコードを扱います。
    /// </summary>
    public static class NicoStatusCodeUtil
    {
        /// <summary>
        /// ステータスコードを取得します。
        /// </summary>
        public static NicoStatusCode GetCode(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return NicoStatusCode.UnknownError;
            }

            switch (code.ToUpper())
            {
                case "BUSY":
                    return NicoStatusCode.Busy;
                case "NOTLOGIN":
                case "NOT_LOGIN":
                    return NicoStatusCode.NotLogin;
                case "NOTFOUND":
                case "NOT_FOUND":
                    return NicoStatusCode.NotFound;
                case "CLOSED":
                    return NicoStatusCode.AlreadyClosed;
                case "NOAUTH":
                    return NicoStatusCode.NoAuthentification;
                case "PERMISSION_DENIED":
                    return NicoStatusCode.PermissionDenied;
                case "COMINGSOON":
                    return NicoStatusCode.ComingSoon;
                case "FULL":
                    return NicoStatusCode.Full;
                case "REQUIRE_COMMUNITY_MEMBER":
                    return NicoStatusCode.RequireCommunityMember;
                case "NOTFOUND_THREAD":
                    return NicoStatusCode.NotFoundThread;
                case "NOTFOUND_SLOT":
                    return NicoStatusCode.NotFoundSlot;
                case "NOTEXIST_SLOT":
                    return NicoStatusCode.NotExistSlot;
                case "DELETED":
                    return NicoStatusCode.Deleted;
                case "DELETEDBYUSER":
                    return NicoStatusCode.DeletedByUser;
                case "DELETEDBYVISOR":
                    return NicoStatusCode.DeletedByVisor;
                case "PREMIUM_ONLY":
                    return NicoStatusCode.PremiumOnly;
                case "VIOLATED":
                    return NicoStatusCode.Violated;
                case "UNKNOWN":
                case "UNKNOWN_ERROR":
                    return NicoStatusCode.UnknownError;
                default:
                    Log.Error("Unknown error: code={0}", code);
                    return NicoStatusCode.UnknownError;
            }
        }

        /// <summary>
        /// エラー文字列を取得します。
        /// </summary>
        public static string GetDescription(this NicoStatusCode code)
        {
            var description = EnumEx.GetDescription(code);
            if (string.IsNullOrEmpty(description))
            {
                return string.Format(
                    "{0}: エラーコードが正しくありません。", code);
            }

            return description;
        }
    }
}
