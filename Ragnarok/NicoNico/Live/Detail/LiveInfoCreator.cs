using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.NicoNico.Live.Detail
{
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

    public static class LiveInfoCreatorUtil
    {
        public static ILiveInfoCreator CreateCreator(ProviderType type)
        {
            switch (type)
            {
                case ProviderType.Community:
                    return new UserLiveInfoCreator();
                case ProviderType.Official:
                    return new OfficialLiveInfoCreator();
                case ProviderType.Channel:
                    throw new NotImplementedException(
                        "実装されていません。");
            }

            throw new NicoLiveException(
                "不明な放送提供元です。");
        }
    }
}
