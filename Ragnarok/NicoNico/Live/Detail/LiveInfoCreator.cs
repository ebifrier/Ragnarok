using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.NicoNico.Live.Detail
{
    using Provider;

    /// <summary>
    /// 共通インターフェースです。
    /// </summary>
    public interface ILiveInfoCreator
    {
        /// <summary>
        /// 全コメントルームの情報などを取得します。
        /// </summary>
        CommentRoomInfo[] GetAllRoomInfo(PlayerStatus playerStatus,
                                         int communityLevel);
    }

    /// <summary>
    /// ILiveInfoCreator用の便利クラスです。
    /// </summary>
    public static class LiveInfoCreatorUtil
    {
        /// <summary>
        /// <paramref name="type"/>に合うInfoCreatorを作成します。
        /// </summary>
        public static ILiveInfoCreator CreateCreator(ProviderType type)
        {
            switch (type)
            {
                case ProviderType.Community:
                    return new UserLiveInfoCreator();
                case ProviderType.Channel:
                    return new ChannelInfoCreator();
                case ProviderType.Official:
                    return new OfficialLiveInfoCreator();
            }

            throw new NicoLiveException(
                "不明な放送提供元です。");
        }
    }
}
