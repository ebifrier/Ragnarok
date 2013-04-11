using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Presentation.Shogi.Xaml
{
    /// <summary>
    /// 将棋プラグインのグローバルオブジェクトです。
    /// </summary>
    internal static class BoardText
    {
        private readonly static string[] fileTextList = new string[]
        {
            "９", "８", "７", "６", "５", "４", "３", "２", "１"
        };

        private readonly static string[] rankTextList = new string[]
        {
            "一", "二", "三", "四", "五", "六", "七", "八", "九"
        };

        /// <summary>
        /// 列の各数字を取得または設定します。
        /// </summary>
        public static string[] FileTextList
        {
            get { return fileTextList; }
        }

        /// <summary>
        /// 段の各数字を取得または設定します。
        /// </summary>
        public static string[] RankTextList
        {
            get { return rankTextList; }
        }
    }
}
