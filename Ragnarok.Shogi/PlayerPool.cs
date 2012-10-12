using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// プレイヤーの参加が行われた時刻を保持します。
    /// </summary>
    public class RegistereredPlayer
    {
        /// <summary>
        /// 参加したプレイヤーを取得または設定します。
        /// </summary>
        public ShogiPlayer Player
        {
            get;
            set;
        }

        /// <summary>
        /// 参加時刻を取得または設定します。
        /// </summary>
        public DateTime Timestamp
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 参加棋士の情報を保持します。
    /// </summary>
    public class PlayerPool //: IEnumerable<KeyValuePair<string, ShogiPlayer>>
    {
        private readonly Dictionary<string, RegistereredPlayer> playerDic =
            new Dictionary<string, RegistereredPlayer>();

        /*/// <summary>
        /// プレイヤーリストのコピーを取得します。
        /// </summary>
        public Dictionary<string, RegistereredPlayer> Players
        {
            get
            {
                lock (this.playerDic)
                {
                    return new Dictionary<string, RegistereredPlayer>(
                        this.playerDic);
                }
            }
        }*/

        /// <summary>
        /// プレイヤーを追加します。
        /// </summary>
        public void Add(ShogiPlayer player, DateTime timestamp)
        {
            if (player == null || !player.Validate())
            {
                return;
            }

            lock (this.playerDic)
            {
                if (Contains(player))
                {
                    this.playerDic.Remove(player.PlayerId);
                }

                this.playerDic[player.PlayerId] = new RegistereredPlayer()
                {
                    Player = player,
                    Timestamp = timestamp,
                };
            }
        }

        /// <summary>
        /// 与えられたプレイヤーが含まれているか調べます。
        /// </summary>
        public bool Contains(ShogiPlayer player)
        {
            if (player == null)
            {
                return false;
            }

            return Contains(player.PlayerId);
        }

        /// <summary>
        /// 与えられたＩＤを持つプレイヤーが含まれているか調べます。
        /// </summary>
        public bool Contains(string playerId)
        {
            if (string.IsNullOrEmpty(playerId))
            {
                return false;
            }

            lock (this.playerDic)
            {
                return this.playerDic.ContainsKey(playerId);
            }
        }

        /// <summary>
        /// 指定のＩＤを持つプレイヤーを探します。
        /// </summary>
        public RegistereredPlayer Get(string playerId)
        {
            lock (this.playerDic)
            {
                if (!Contains(playerId))
                {
                    return null;
                }

                return this.playerDic[playerId];
            }
        }
    }
}
