using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.NicoNico.Provider
{
    /// <summary>
    /// 各コミュニティレベルにおける参加人数などを保持します。
    /// </summary>
    public class CommunityLevelInfo
    {
        /// <summary>
        /// コミュニティレベルを取得します。
        /// </summary>
        public int Level
        {
            get;
            private set;
        }

        /// <summary>
        /// プレミアム会員の参加総数を取得します。
        /// </summary>
        public int NumberOfPremiums
        {
            get;
            private set;
        }

        /// <summary>
        /// 参加できる最大会員数を取得します。
        /// </summary>
        public int MaximumNumberOfMembers
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommunityLevelInfo(int level, int numberOfPremiums,
                                  int maximumNumberOfMembers)
        {
            this.Level = level;
            this.NumberOfPremiums = numberOfPremiums;
            this.MaximumNumberOfMembers = maximumNumberOfMembers;
        }
    }
}
