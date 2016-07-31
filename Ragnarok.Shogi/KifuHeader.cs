using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 棋譜ヘッダの各要素を識別します。
    /// </summary>
    public enum KifuHeaderType
    {
        /// <summary>
        /// 先手番の対局者名です。
        /// </summary>
        BlackName,
        /// <summary>
        /// 後手番の対局者名です。
        /// </summary>
        WhiteName,
        /// <summary>
        /// 棋戦名
        /// </summary>
        Event,
        /// <summary>
        /// 対局場所
        /// </summary>
        Site,
        /// <summary>
        /// 開始時刻
        /// </summary>
        StartTime,
        /// <summary>
        /// 終了時刻
        /// </summary>
        EndTime,
        /// <summary>
        /// 持ち時間
        /// </summary>
        TimeLimit,
        /// <summary>
        /// 戦型
        /// </summary>
        Opening,
        /// <summary>
        /// 評価値タイプ
        /// </summary>
        NodeScoreType,
    }

    /// <summary>
    /// 棋譜のヘッダを管理します。
    /// </summary>
    public sealed class KifuHeader : IEnumerable<KeyValuePair<KifuHeaderType, string>>
    {
        private readonly object SyncRoot = new object();
        private readonly Dictionary<KifuHeaderType, string> headerItems =
            new Dictionary<KifuHeaderType, string>();

        /// <summary>
        /// Enumeratorを取得します。
        /// </summary>
        public IEnumerator<KeyValuePair<KifuHeaderType, string>> GetEnumerator()
        {
            return this.headerItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.headerItems.GetEnumerator();
        }

        /// <summary>
        /// 指定の種類のヘッダ情報を取得または設定します。
        /// </summary>
        public string this[KifuHeaderType type]
        {
            get
            {
                lock (SyncRoot)
                {
                    string result;
                    if (!this.headerItems.TryGetValue(type, out result))
                    {
                        return null;
                    }

                    return result;
                }
            }
            set
            {
                lock (SyncRoot)
                {
                    this.headerItems[type] = value;
                }
            }
        }

        /// <summary>
        /// 指定のタイプのヘッダ情報が含まれるか確認します。
        /// </summary>
        public bool Contains(KifuHeaderType type)
        {
            lock (SyncRoot)
            {
                return this.headerItems.ContainsKey(type);
            }
        }
    }
}
