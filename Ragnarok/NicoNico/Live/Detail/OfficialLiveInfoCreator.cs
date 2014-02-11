using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.NicoNico.Live.Detail
{
    /// <summary>
    /// 公式生放送の各部屋に割り当てられた名前などを取得します。
    /// </summary>
    public sealed class OfficialLiveRoomData
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
        public OfficialLiveRoomData(string name, int offset)
        {
            Name = name;
            Offset = offset;
        }
    }

    /// <summary>
    /// 公式生放送の各ルームの接続情報を管理します。
    /// </summary>
    /// <remarks>
    /// 公式生放送ではコメントサーバーの使用において、
    /// アリーナ最前列とその他の部屋の関係性が以下のようになっています。
    /// 
    /// * アリーナ最前列に対して、アリーナはアドレス番号が１多い。
    ///  また、アドレス番号は101～104の間をループしており、
    ///  もし繰り上げが発生するとポート番号が１増える。
    ///  
    /// * 立ち見Bの場合は２多く、Cの場合は3多い。
    /// </remarks>
    public sealed class OfficialLiveInfoCreator : ILiveInfoCreator
    {
        public static readonly List<OfficialLiveRoomData>
            LiveRoomDataList = new List<OfficialLiveRoomData>
        {
            new OfficialLiveRoomData("アリーナ最前列", 0),
            new OfficialLiveRoomData("アリーナ", 1),
            new OfficialLiveRoomData("裏アリーナ", 2),

            new OfficialLiveRoomData("1F中央 最前列", 3),
            new OfficialLiveRoomData("1F中央 前方", 4),
            new OfficialLiveRoomData("1F中央 後方", 5),
            new OfficialLiveRoomData("1F右 前方", 6),
            new OfficialLiveRoomData("1F右 後方", 7),
            new OfficialLiveRoomData("1F左 前方", 8),
            new OfficialLiveRoomData("1F左 後方", 9),

            new OfficialLiveRoomData("2F中央 最前列", 10),
            new OfficialLiveRoomData("2F中央 前方", 11),
            new OfficialLiveRoomData("2F右 Aブロック", 12),
            new OfficialLiveRoomData("2F右 Bブロック", 13),
            new OfficialLiveRoomData("2F右 Cブロック", 14),
            new OfficialLiveRoomData("2F右 Dブロック", 15),
            new OfficialLiveRoomData("2F左 Aブロック", 16),
            new OfficialLiveRoomData("2F左 Bブロック", 17),
            new OfficialLiveRoomData("2F左 Cブロック", 18),
            new OfficialLiveRoomData("2F左 Dブロック", 19),
        };

        public static readonly Dictionary<string, OfficialLiveRoomData>
            LiveRoomDataDic;

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static OfficialLiveInfoCreator()
        {
            LiveRoomDataDic = LiveRoomDataList.ToDictionary(_ => _.Name);            
        }

        /// <summary>
        /// コメントサーバーはアドレス番号が101-104をループする。
        /// </summary>
        public string CommentServerAddress(int number, out int carry)
        {
            carry = 0;

            while (number < 101)
            {
                carry = -1;
                number += 4;
            }

            while (number > 104)
            {
                carry = +1;
                number -= 4;
            }

            return NicoString.GetOfficialMessageServerAddress(number);
        }

        /// <summary>
        /// コメントサーバーはポート番号が2805-2814をループする。
        /// </summary>
        public int CommentServerPort(int port)
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

            if (playerStatus.Stream.ProviderType != ProviderType.Official)
            {
                throw new NicoLiveException(
                    "公式生放送以外には対応していません。");
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

            // アリーナへのオフセットを取得します。
            OfficialLiveRoomData data;
            if (!LiveRoomDataDic.TryGetValue(roomLabel, out data))
            {
                return new CommentRoomInfo(
                    roomLabel,
                    ms.Address,
                    ms.Port,
                    ms.Thread);
            }

            var carry = 0;
            var msAddr = CommentServerAddress(msAddrNum - data.Offset, out carry);
            var msPort = CommentServerPort(ms.Port + carry);

            // サーバーポート番号はアドレス番号のオーバーフローによって
            // 上下します。
            return new CommentRoomInfo(
                "先頭のルーム",
                msAddr,
                msPort,
                ms.Thread - data.Offset);
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

            if (arenaInfo.RoomLabel != "先頭のルーム")
            {
                return new CommentRoomInfo[] { arenaInfo };
            }

            return LiveRoomDataList.Select(data =>
            {
                var carry = 0;
                var msAddr = CommentServerAddress(
                    arenaAddrNum + data.Offset, out carry);
                var msPort = CommentServerPort(arenaInfo.Port + carry);

                return new CommentRoomInfo(
                    data.Name,
                    msAddr,
                    msPort,
                    arenaInfo.Thread + data.Offset);
            }).ToArray();
        }
    }
}
