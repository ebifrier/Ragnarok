using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.NicoNico.Live
{
    using Utility;

    /// <summary>
    /// 生放送API全般に使われるステータスコードです。
    /// </summary>
    public enum LiveStatusCode
    {
        /// <summary>
        /// 異常はありません。
        /// </summary>
        [LabelDescription("エラーはありません。")]
        Ok,

        /// <summary>
        /// 不明なエラーです。
        /// </summary>
        [LabelDescription("不明なエラーです。")]
        UnknownError,
        /// <summary>
        /// 放送ＩＤが正しくありません。
        /// </summary>
        [LabelDescription("放送ＩＤが正しくありません。")]
        InvalidLiveId,
        /// <summary>
        /// 取得したXMLのパースに失敗しました。
        /// </summary>
        [LabelDescription("受信したXmlデータに誤りがありました。")]
        XmlParseError,
        /// <summary>
        /// ネットワークエラーです。
        /// </summary>
        [LabelDescription("ネットワーク障害により通信できません。")]
        NetworkError,

        /// <summary>
        /// ログインしていません。
        /// </summary>
        [LabelDescription("ログインしていません。")]
        NotLogin,
        /// <summary>
        /// 放送が見つかりません。
        /// </summary>
        [LabelDescription("放送が見つかりません。")]
        NotFound,
        /// <summary>
        /// 放送はすでに終了しています。
        /// </summary>
        [LabelDescription("放送はすでに終了しています。")]
        AlreadyClosed,
        /// <summary>
        /// 権限がありません。
        /// </summary>
        [LabelDescription("アクセスする権限がありません。")]
        NoAuthentification,
        /// <summary>
        /// アクセスは拒否されました。
        /// </summary>
        [LabelDescription("アクセスが拒否されました。")]
        PermissionDenied,
        /// <summary>
        /// まもなく放送が始まります。
        /// </summary>
        [LabelDescription("放送が始まるまでしばらくお待ちください。")]
        ComingSoon,
        /// <summary>
        /// 放送はすでに人が多すぎです。
        /// </summary>
        [LabelDescription("放送はすでに満員です。")]
        Full,
        /// <summary>
        /// コミュニティに参加する必要があります。
        /// </summary>
        [LabelDescription("コミュニティのメンバーである必要があります。")]
        RequireCommunityMember,
        /// <summary>
        /// スレッドが見つかりません。
        /// </summary>
        [LabelDescription("スレッドが見つかりません。")]
        NotFoundThread,
        /// <summary>
        /// スロットが見つかりません。
        /// </summary>
        /// <remarks>
        /// heartbeat取得時によく現れます。
        /// </remarks>
        [LabelDescription("スロットが見つかりません。")]
        NotFoundSlot,
        /// <summary>
        /// ユーザーにより削除されました。
        /// </summary>
        [LabelDescription("ユーザーにより削除されました。")]
        DeletedByUser,
        /// <summary>
        /// 管理者により削除されました。
        /// </summary>
        [LabelDescription("管理者により削除されました。")]
        DeletedByVisor,
        /// <summary>
        /// プレミアム会員のみが視聴できます。
        /// </summary>
        [LabelDescription("プレミアム会員のみが視聴できます。")]
        PremiumOnly,
        /// <summary>
        /// 権利侵害などの理由で削除されました。
        /// </summary>
        [LabelDescription("権利侵害などの理由で削除されました。")]
        Violated,
    }

    /// <summary>
    /// ステータスコードを扱います。
    /// </summary>
    public static class LiveStatusCodeUtil
    {
        /// <summary>
        /// ステータスコードを取得します。
        /// </summary>
        public static LiveStatusCode GetCode(string code)
        {
            switch (code)
            {
                case "notlogin":
                    return LiveStatusCode.NotLogin;
                case "notfound":
                    return LiveStatusCode.NotFound;
                case "closed":
                    return LiveStatusCode.AlreadyClosed;
                case "noauth":
                    return LiveStatusCode.NoAuthentification;
                case "permission_denied":
                    return LiveStatusCode.PermissionDenied;
                case "comingsoon":
                    return LiveStatusCode.ComingSoon;
                case "full":
                    return LiveStatusCode.Full;
                case "require_community_member":
                    return LiveStatusCode.RequireCommunityMember;
                case "NOTFOUND_THREAD":
                    return LiveStatusCode.NotFoundThread;
                case "NOTFOUND_SLOT":
                    return LiveStatusCode.NotFoundSlot;
                case "deletedbyuser":
                    return LiveStatusCode.DeletedByUser;
                case "deletedbyvisor":
                    return LiveStatusCode.DeletedByVisor;
                case "premium_only":
                    return LiveStatusCode.PremiumOnly;
                case "violated":
                    return LiveStatusCode.Violated;
                case "unknown":
                case "unknown_error":
                    return LiveStatusCode.UnknownError;
                default:
                    Log.Error("Unknown error code: {0}", code);
                    return LiveStatusCode.UnknownError;
            }
        }

        /// <summary>
        /// エラー文字列を取得します。
        /// </summary>
        public static string GetDescription(LiveStatusCode code)
        {
            var description = EnumEx.GetEnumDescription(code);
            if (string.IsNullOrEmpty(description))
            {
                return string.Format(
                    "{0}: エラーコードが正しくありません。", code);
            }

            return description;
        }
    }
}
