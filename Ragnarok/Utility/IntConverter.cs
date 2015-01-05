using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Utility
{
    /// <summary>
    /// 数値の種類です。
    /// </summary>
    public enum NumberType
    {
        /// <summary>
        /// 半角数字です。
        /// </summary>
        Normal,
        /// <summary>
        /// 全角数字です。
        /// </summary>
        Big,
        /// <summary>
        /// 漢数字です。
        /// </summary>
        Kanji,
        /// <summary>
        /// 段位などです。(1級 → 初級)
        /// </summary>
        Grade,
    };

    /// <summary>
    /// 数値を文字列に変換します。
    /// </summary>
    public static class IntConverter
    {
        private static readonly string[] NormalNumberTable = new string[]
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "-",
        };

        private static readonly string[] BigNumberTable = new string[]
        {
            "０", "１", "２", "３", "４", "５", "６", "７", "８", "９", "－",
        };

        private static readonly string[] KanjiNumberTable = new string[]
        {
            "零", "一", "二", "三", "四", "五", "六", "七", "八", "九", "－",
        };

        /// <summary>
        /// 数字を文字列に変換します。
        /// </summary>
        public static string Convert(NumberType type, int num)
        {
            var result = new StringBuilder();
            string[] numberTable;

            switch (type)
            {
                case NumberType.Normal:
                    numberTable = NormalNumberTable;
                    break;
                case NumberType.Big:
                    numberTable = BigNumberTable;
                    break;
                case NumberType.Kanji:
                    numberTable = KanjiNumberTable;
                    break;
                case NumberType.Grade:
                    numberTable = NormalNumberTable;
                    if (num == 1)
                    {
                        return "初";
                    }
                    break;
                default:
                    return null;
            }

            if (num == 0)
            {
                return numberTable[0];
            }

            // 負数ならマイナス記号をつけます。
            var original = num;
            num = (num > 0 ? num : -num);

            while (num > 0)
            {
                result.Insert(0, numberTable[num % 10]);
                num /= 10;
            }

            if (original < 0)
            {
                result.Insert(0, numberTable[10]);
            }

            return result.ToString();
        }
    }
}
