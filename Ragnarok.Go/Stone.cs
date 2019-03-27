using System;
using System.Collections.Generic;
using System.Linq;

using Ragnarok.Utility;

namespace Ragnarok.Go
{
    /// <summary>
    /// 石の色
    /// </summary>
    public enum Stone
    {
        /// <summary>
        /// 特になし
        /// </summary>
        [Label("不明")]
        Empty = 0,
        /// <summary>
        /// 黒
        /// </summary>
        [Label("黒")]
        Black = 1,
        /// <summary>
        /// 白
        /// </summary>
        [Label("白")]
        White = 2,
        /// <summary>
        /// エラー
        /// </summary>
        [Label("エラー")]
        Error = 3,
        /// <summary>
        /// 盤外
        /// </summary>
        [Label("盤外")]
        Wall = 4,
    }

    public static class StoneColorUtil
    {
        /// <summary>
        /// 石の色を反転します。
        /// </summary>
        public static Stone Inv(this Stone stone)
        {
            return (
                stone == Stone.Black ? Stone.White :
                stone == Stone.White ? Stone.Black :
                stone);
        }
    }
}
