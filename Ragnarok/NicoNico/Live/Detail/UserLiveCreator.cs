using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.NicoNico.Live.Detail
{
    /// <summary>
    /// ユーザー生放送の各ルームのの接続情報を管理します。
    /// </summary>
    /// <remarks>
    /// ユーザー生放送ではコメントサーバーの使用において、
    /// アリーナとその他の部屋の関係性が以下のようになっています。
    /// 
    /// * アリーナに対して、立ち見Aはポート番号が１多い。
    ///  また、ポート番号は2805～2814の間をループしており、
    ///  もし繰り上げが発生するとアドレス番号が１増える。
    ///  
    /// * 立ち見Bの場合は２多く、Cの場合は3多い。
    /// </remarks>
    public sealed class UserLiveInfoCreator : ILiveInfoCreator
    {
        /// <summary>
        /// ニコ生のコメントサーバーはアドレス番号が101-104をループする。
        /// </summary>
        public string CommentServerAddress(int number)
        {
            while (number < 101)
            {
                number += 4;
            }

            while (number > 104)
            {
                number -= 4;
            }

            return NicoString.GetCommunityMessageServerAddress(number);
        }

        /// <summary>
        /// ニコ生のコメントサーバーはポート番号が2805-2814をループする。
        /// </summary>
        public int CommentServerPort(int port, out int carry)
        {
            carry = 0;
            while (port < 2805)
            {
                carry = -1;
                port += 10;
            }

            while (port > 2814)
            {
                carry = +1;
                port -= 10;
            }

            return port;
        }

        /// <summary>
        /// アリーナのコメントサーバー情報を取得します。
        /// </summary>
        public CommentRoomInfo GetArenaInfo(PlayerStatus playerStatus)
        {
            var arenaName = playerStatus.Stream.DefaultCommunity;
            var roomLabel = playerStatus.User.RoomLabel;
            var ms = playerStatus.MS;
            var offset = 0;

            if (playerStatus.Stream.ProviderType != ProviderType.Community)
            {
                throw new NicoLiveException(
                    "ユーザー生放送以外には対応していません。");
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

            // アリーナ席へのオフセットを取得します。
            if (roomLabel.IndexOf("A") >= 0)
            {
                offset = 1;
            }
            else if (roomLabel.IndexOf("B") >= 0)
            {
                offset = 2;
            }
            else if (roomLabel.IndexOf("C") >= 0)
            {
                offset = 3;
            }
            else
            {
                offset = 0;
            }

            var carry = 0;
            var msPort = CommentServerPort(ms.Port - offset, out carry);
            var msAddr = CommentServerAddress(msAddrNum + carry);

            // サーバーアドレス番号はポート番号のオーバーフローによって
            // 上下します。
            // - ポート番号が2805以下なら、アドレス番号は１下がる
            // - ポート番号が2814以上なら、アドレス番号は１上がる
            return new CommentRoomInfo(
                arenaName,
                msAddr,
                msPort,
                ms.Thread - offset);
        }

        /// <summary>
        /// 全コメントルームのポート情報などを取得します。
        /// </summary>
        public CommentRoomInfo[] GetAllRoomInfo(PlayerStatus playerStatus,
                                                int communityLevel)
        {
            var result = new List<CommentRoomInfo>();
            var arenaInfo = GetArenaInfo(playerStatus);

            // ルーム数を取得します。
            var roomCount = (
                communityLevel > 0 ?
                CommunityLevelTable.GetNumberOfSeet(communityLevel) / 500 :
                1);

            result.Add(arenaInfo);

            var arenaInfoAddrNum = NicoString.GetMessageServerNumber(
                arenaInfo.Address);
            for (var i = 1; i < roomCount; ++i)
            {
                var carry = 0;
                var msPort = CommentServerPort(arenaInfo.Port + i, out carry);
                var msAddr = CommentServerAddress(arenaInfoAddrNum + carry);

                // 各立ち見席の情報を設定します。
                var roomInfo = new CommentRoomInfo(
                    "立ち見" + (char)((int)'A' + (i - 1)) + "列",
                    msAddr,
                    msPort,
                    arenaInfo.Thread + i);

                result.Add(roomInfo);
            }

            return result.ToArray();
        }
    }
}
