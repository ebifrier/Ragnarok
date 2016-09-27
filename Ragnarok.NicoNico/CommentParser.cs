using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.NicoNico
{
    /// <summary>
    /// コメントのMailを解析します。
    /// </summary>
    public static class CommentParser
    {
        /// <summary>
        /// 色とその文字列をセットに保存しています。
        /// </summary>
        private static readonly SortedList<string, CommentColor> colorTable =
            new SortedList<string, CommentColor>()
            {
                {"white", CommentColor.White},
                {"red", CommentColor.Red},
                {"pink", CommentColor.Pink},
                {"orange", CommentColor.Orange},
                {"yellow", CommentColor.Yellow},
                {"green", CommentColor.Green},
                {"cyan", CommentColor.Cyan},
                {"blue", CommentColor.Blue},
                {"purple", CommentColor.Purple},
                {"black", CommentColor.Black},

                {"#cccc99", CommentColor.White2},
                {"#cc0033", CommentColor.Red2},
                {"#ff33cc", CommentColor.Pink2},
                {"#ff6600", CommentColor.Orange2},
                {"#999900", CommentColor.Yellow2},
                {"#00cc66", CommentColor.Green2},
                {"#00cccc", CommentColor.Cyan2},
                {"#3399ff", CommentColor.Blue2},
                {"#6633cc", CommentColor.Purple2},
                {"#666666", CommentColor.Black2},

                {"white2", CommentColor.White2},
                {"red2", CommentColor.Red2},
                {"pink2", CommentColor.Pink2},
                {"orange2", CommentColor.Orange2},
                {"yellow2", CommentColor.Yellow2},
                {"green2", CommentColor.Green2},
                {"cyan2", CommentColor.Cyan2},
                {"blue2", CommentColor.Blue2},
                {"purple2", CommentColor.Purple2},
                {"black2", CommentColor.Black2},

                {"niconicowhite", CommentColor.White2},
                {"truered", CommentColor.Red2},
                {"passionorange", CommentColor.Orange2},
                {"madyellow", CommentColor.Yellow2},
                {"elementalgreen", CommentColor.Green2},
                {"marineblue", CommentColor.Blue2},
                {"noblepurple", CommentColor.Purple2},
            };

        /// <summary>
        /// コメントサイズとその文字列をセットに保存しています。
        /// </summary>
        private static readonly SortedList<string, CommentSize> sizeTable =
            new SortedList<string, CommentSize>()
            {
                {"big", CommentSize.Big},
                {"small", CommentSize.Small},
                {"medium", CommentSize.Medium},
            };

        /// <summary>
        /// コメント位置とその文字列をセットに保存しています。
        /// </summary>
        private static readonly SortedList<string, CommentPosition> positionTable =
            new SortedList<string, CommentPosition>()
            {
                {"ue", CommentPosition.Ue},
                {"shita", CommentPosition.Shita},
                {"naka", CommentPosition.Naka},
            };

        /// <summary>
        /// 与えられた単語を含むかどうか調べます。
        /// </summary>
        private static bool HasWord(string text, string word)
        {
            var r = new Regex("\\b" + word + "\\b");
            var m = r.Match(text);

            return m.Success;
        }

        /// <summary>
        /// 匿名コメントかどうか調べます。
        /// </summary>
        public static bool IsAnonymous(string mail)
        {
            if (string.IsNullOrEmpty(mail))
            {
                return false;
            }

            return HasWord(mail, "184");
        }

        /// <summary>
        /// コメント属性を解析し色属性があればそれを返します。
        /// </summary>
        public static CommentColor ParseColor(string mail)
        {
            if (string.IsNullOrEmpty(mail))
            {
                return CommentColor.Default;
            }

            var downcase = mail.ToLower();
            foreach (var elem in colorTable)
            {
                if (HasWord(downcase, elem.Key))
                {
                    return elem.Value;
                }
            }

            var r = new Regex("\\b#([0-9a-fA-F]{6})\\b");
            var m = r.Match(downcase);
            if (m.Success)
            {
                return CommentColor.Custom;
            }

            return CommentColor.Default;
        }

        /// <summary>
        /// コメント属性を解析しサイズ属性があればそれを返します。
        /// </summary>
        public static CommentSize ParseSize(string mail)
        {
            if (string.IsNullOrEmpty(mail))
            {
                return CommentSize.Default;
            }

            var downcase = mail.ToLower();
            foreach (var elem in sizeTable)
            {
                if (HasWord(downcase, elem.Key))
                {
                    return elem.Value;
                }
            }

            return CommentSize.Default;
        }

        /// <summary>
        /// コメント属性を解析し位置属性があればそれを返します。
        /// </summary>
        public static CommentPosition ParsePosition(string mail)
        {
            if (string.IsNullOrEmpty(mail))
            {
                return CommentPosition.Default;
            }

            var downcase = mail.ToLower();
            foreach (var elem in positionTable)
            {
                if (HasWord(downcase, elem.Key))
                {
                    return elem.Value;
                }
            }

            return CommentPosition.Default;
        }
    }
}
