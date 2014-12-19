using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Ragnarok.NicoNico.Live.Detail
{
    /// <summary>
    /// チャンネル生放送の各部屋に割り当てられた名前などを取得します。
    /// </summary>
    public sealed class ChannelLiveRoomData
    {
        /// <summary>
        /// 部屋名を取得します。
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// アドレス番号などのオフセットを取得します。
        /// </summary>
        public int Offset
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ChannelLiveRoomData(string name, int offset)
        {
            Name = name;
            Offset = offset;
        }
    }

    /// <summary>
    /// チャンネル放送の各ルームの接続情報を管理します。
    /// </summary>
    /// <remarks>
    /// チャンネル放送のコメントサーバー仕様は基本的に公式生と同じです。
    /// </remarks>
    public sealed class ChannelInfoCreator : ILiveInfoCreator
    {
        /*public static readonly List<ChannelLiveRoomData>
            LiveRoomDataList = new List<ChannelLiveRoomData>
        {
            new ChannelLiveRoomData("アリーナ 最前列", 0),
            new ChannelLiveRoomData("アリーナ", 1),
            new ChannelLiveRoomData("裏アリーナ", 2),

            new ChannelLiveRoomData("1F中央 最前列", 3),
            new ChannelLiveRoomData("1F中央 前方", 4),
            new ChannelLiveRoomData("1F中央 後方", 5),
            new ChannelLiveRoomData("1F右 前方", 6),
            new ChannelLiveRoomData("1F右 後方", 7),
            new ChannelLiveRoomData("1F左 前方", 8),
            new ChannelLiveRoomData("1F左 後方", 9),

            new ChannelLiveRoomData("2F中央 最前列", 10),
            new ChannelLiveRoomData("2F中央 前方", 11),
        };

        public static readonly Dictionary<string, ChannelLiveRoomData>
            LiveRoomDataDic;*/

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static ChannelInfoCreator()
        {
            //LiveRoomDataDic = LiveRoomDataList.ToDictionary(_ => _.Name);            
        }

        /// <summary>
        /// コメントサーバーはアドレス番号が101-104をループする。
        /// </summary>
        public static string CommentServerAddress(int number)
        {
            int carry;

            return CommentServerAddress(number, out carry);
        }

        /// <summary>
        /// コメントサーバーはアドレス番号が101-104をループする。
        /// </summary>
        public static string CommentServerAddress(int number, out int carry)
        {
            carry = 0;

            while (number < 101)
            {
                carry -= 1;
                number += 4;
            }

            while (number > 104)
            {
                carry += 1;
                number -= 4;
            }

            return NicoString.GetOfficialMessageServerAddress(number);
        }

        /// <summary>
        /// コメントサーバーはポート番号が2805-2814をループする。
        /// </summary>
        public static int CommentServerPort(int port)
        {
            while (port < 2805)
            {
                port += 10;
            }

            while (port > 2814)
            {
                port -= 10;
            }

            return port;
        }

        /// <summary>
        /// アリーナのコメントサーバー情報を取得します。
        /// </summary>
        public CommentRoomInfo GetArenaInfo(PlayerStatus playerStatus)
        {
            var roomLabel = playerStatus.User.RoomLabel;
            var ms = playerStatus.MS;

            if (playerStatus.Stream.ProviderType != ProviderType.Channel)
            {
                throw new NicoLiveException(
                    "チャンネル生放送以外には対応していません。");
            }

            // メッセージサーバーのURLから番号を取得します。
            var msAddrNum = NicoString.GetMessageServerNumber(ms.Address);
            if (msAddrNum < 0)
            {
                return new CommentRoomInfo(
                    roomLabel,
                    ms.Address,
                    ms.Port,
                    ms.Thread);
            }

            return new CommentRoomInfo(
                roomLabel,
                ms.Address,
                ms.Port,
                ms.Thread);
        }

        /// <summary>
        /// 全コメントルームのポート情報などを取得します。
        /// </summary>
        public CommentRoomInfo[] GetAllRoomInfo(PlayerStatus playerStatus,
                                                int communityLevel)
        {
            var arenaInfo = GetArenaInfo(playerStatus);
            var arenaAddrNum = NicoString.GetMessageServerNumber(
                arenaInfo.Address);

            return new[] { arenaInfo };
        }
    }
}
