using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.Utility
{
    /// <summary>
    /// 文字列を正規化するときのオプションです。
    /// </summary>
    [Flags()]
    public enum NormalizeTextOption
    {
        /// <summary>
        /// なにもしません。
        /// </summary>
        None = 0x00,
        /// <summary>
        /// 大文字数字や漢数字を半角数字に直します。
        /// </summary>
        Number = 0x01,
        /// <summary>
        /// アルファベットを半角大文字に直します。
        /// </summary>
        Alphabet = 0x02,
        /// <summary>
        /// ひらがな・(半角/全角)カタカナをひらがなに直します。
        /// </summary>
        Kana = 0x04,
        /// <summary>
        /// シンボルを半角文字に直します。
        /// </summary>
        Symbol = 0x08,
        /// <summary>
        /// 十・百・千など桁を示す漢字の変換を行います。
        /// </summary>
        KanjiDigit = 0x10,
        /// <summary>
        /// 全変換を行います。
        /// </summary>
        All = (Number | Alphabet | Kana | Symbol | KanjiDigit)
    }

    /// <summary>
    /// 文字列を正規化します。
    /// </summary>
    public static class StringNormalizer
    {
        /// <summary>
        /// 文字列を正規化します。
        /// </summary>
        /// <remarks>
        /// ここでいう'正規化'の定義は恣意的なものであり、
        /// ・ローマ数字/漢数字など → 半角数字
        /// ・かな文字 → 全角ひらがな
        /// ・半角/全角アルファベット → 半角大文字アルファベット
        /// に変換します。
        /// </remarks>
        public static string NormalizeText(string text, NormalizeTextOption option)
        {
            // なにもしません。
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            if ((option & NormalizeTextOption.Number) != 0)
            {
                var kanjiDigit = option.HasFlag(NormalizeTextOption.KanjiDigit);
                text = NormalizeNumber(text, kanjiDigit);
            }

            if ((option & NormalizeTextOption.Alphabet) != 0)
            {
                text = NormalizeAlphabet(text);
            }

            if ((option & NormalizeTextOption.Kana) != 0)
            {
                text = NormalizeKana(text);
            }

            if ((option & NormalizeTextOption.Symbol) != 0)
            {
                text = NormalizeSymbol(text);
            }

            return text;
        }

        /// <summary>
        /// すべての正規化を行います。
        /// </summary>
        public static string NormalizeText(string text)
        {
            return NormalizeText(text, NormalizeTextOption.All);
        }

        /// <summary>
        /// 数字文字列を数字に変換するときに使います。
        /// </summary>
        private static readonly Dictionary<string, long> numberTable =
            new Dictionary<string, long>()
        {
            {"0", 0}, {"０", 0}, {"零", 0},
            {"1", 1}, {"１", 1}, {"一", 1}, {"Ⅰ", 1},
            {"2", 2}, {"２", 2}, {"二", 2}, {"Ⅱ", 2},
            {"3", 3}, {"３", 3}, {"三", 3}, {"Ⅲ", 3},
            {"4", 4}, {"４", 4}, {"四", 4}, {"Ⅳ", 4},
            {"5", 5}, {"５", 5}, {"五", 5}, {"Ⅴ", 5},
            {"6", 6}, {"６", 6}, {"六", 6}, {"Ⅵ", 6},
            {"7", 7}, {"７", 7}, {"七", 7}, {"Ⅶ", 7},
            {"8", 8}, {"８", 8}, {"八", 8}, {"Ⅷ", 8},
            {"9", 9}, {"９", 9}, {"九", 9}, {"Ⅸ", 9},
        };

        /// <summary>
        /// ①、②などの変換テーブルです。
        /// </summary>
        private static readonly Dictionary<string, string> circleNumberTable =
            new Dictionary<string, string>()
        {
            {"①", "1"}, {"②", "2"}, {"③", "3"}, {"④", "4"}, {"⑤", "5"},
            {"⑥", "6"}, {"⑦", "7"}, {"⑧", "8"}, {"⑨", "9"}, {"⑩", "10"},
            {"⑪", "11"}, {"⑫", "12"}, {"⑬", "13"}, {"⑭", "14"}, {"⑮", "15"},
            {"⑯", "16"}, {"⑰", "17"}, {"⑱", "18"}, {"⑲", "19"}, {"⑳", "20"},

            // 以下、環境依存文字
            {"㉑", "21"}, {"㉒", "22"}, {"㉓", "23"}, {"㉔", "24"}, {"㉕", "25"},
            {"㉖", "26"}, {"㉗", "27"}, {"㉘", "28"}, {"㉙", "29"}, {"㉚", "30"},
        };

        /// <summary>
        /// 十百千を判別します。
        /// </summary>
        private static readonly Dictionary<char, long> ketaTable1 =
            new Dictionary<char, long>()
        {
            {'十', 10},
            {'百', 100},
            {'千', 1000},
        };

        /// <summary>
        /// 万億などを判別します。
        /// </summary>
        private static readonly Dictionary<char, long> ketaTable2 =
            new Dictionary<char, long>()
        {
            {'万', 10000L},
            {'億', 10000 * 10000L},
            {'兆', 10000 * 10000 * 10000L},
        };

        /// <summary>
        /// 含まれる漢数字などを半角数字に直します。
        /// </summary>
        public static string NormalizeNumber(string text, bool normalizeKanjiDigit)
        {
            var result = new StringBuilder();
            long? allNum = null;
            long? prevNum = null;

            // まず単純な文字列置き換えを行います。
            text = circleNumberTable.Aggregate(text,
                (current, target) => current.Replace(target.Key, target.Value));

            // 二十五 → 25
            // 三千四百万六百十二 → 34000612

            foreach (var c in text)
            {
                long n = 0;

                if (normalizeKanjiDigit && ketaTable1.TryGetValue(c, out n))
                {
                    // 十、百、千のどれかなら
                    allNum = (allNum ?? 0) + ((prevNum ?? 1) * n);
                    prevNum = null;
                }
                else if (normalizeKanjiDigit && ketaTable2.TryGetValue(c, out n))
                {
                    // 万、億、などなら
                    allNum = ((allNum ?? 1) + (prevNum ?? 0)) * n;
                    prevNum = null;
                }
                else if (numberTable.TryGetValue(c.ToString(), out n))
                {
                    // 普通の数字の場合
                    if (prevNum != null)
                    {
                        allNum = ((allNum ?? 0) + prevNum) * 10;
                    }
                    prevNum = n;
                }
                else
                {
                    if (prevNum != null)
                    {
                        allNum = (allNum ?? 0) + prevNum;
                    }
                    if (allNum != null)
                    {
                        result.Append(allNum);
                    }
                    result.Append(c);

                    allNum = null;
                    prevNum = null;
                }
            }

            if (prevNum != null)
            {
                allNum = (allNum ?? 0) + prevNum;
            }
            if (allNum != null)
            {
                result.Append(allNum);
            }

            return result.ToString();
        }

        /// <summary>
        /// アルファベットを半角大文字に直します。
        /// </summary>
        public static string NormalizeAlphabet(string text)
        {
            // 全角大文字アルファベット
            text = Regex.Replace(
                text, "[\uFF21-\uFF3A]",
                (match) =>
                    ((char)((match.Value[0] - 'Ａ') + 'A')).ToString());

            // 全角小文字アルファベット
            text = Regex.Replace(
                text, "[\uFF41-\uFF5A]",
                (match) =>
                    ((char)((match.Value[0] - 'ａ') + 'A')).ToString());

            return text.ToUpper();
        }

        /// <summary>
        /// 半角/全角カタカナをひらがなに直します。
        /// </summary>
        public static string NormalizeKana(string text)
        {
            const string hiragana =
                "あいうえおかきくけこさしすせそたちつてとなにぬねの" +
                "はひふへほまみむめもやゆよらりるれろわおん" +
                "ぁぃぅぇぉゃゅょっゎ" +
                "がぎぐげござじずぜぞだぢづでどばびぶべぼ" +
                "ぱぴぷぺぽ";
            const string katakana =
                "アイウエオカキクケコサシスセソタチツテトナニヌネノ" +
                "ハヒフヘホマミムメモヤユヨラリルレロワオン" +
                "ァィゥェォャュョッヮ" +
                "ガギグゲゴザジズゼゾダヂヅデドバビブベボ" +
                "パピプペポ";
            const string hankaku =
                "ｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉ" +
                "ﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜｵﾝ" +
                "ｧｨｩｪｫｬｭｮｯ";

            var result = new StringBuilder();
            foreach (var c in text)
            {
                var index = 0;

                if ((index = katakana.IndexOf(c)) >= 0)
                {
                    result.Append(hiragana[index]);
                }
                else if ((index = hankaku.IndexOf(c)) >= 0)
                {
                    result.Append(hiragana[index]);
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// シンボルを半角文字にします。
        /// </summary>
        public static string NormalizeSymbol(string text)
        {
            const string source = "＃＄％＆＠￥＊！？＋＝｜／＿，．" +
                "；：－｀＾’”＜＞（）｛｝［］「」、。・" +
                "～";
            const string destination = "#$%&@\\*!?+=|/_,." +
                ";:-`^'\"<>(){}[]｢｣､｡･" +
                "ー";

            var result = new StringBuilder();
            foreach (var c in text)
            {
                var index = 0;

                result.Append(
                    (index = source.IndexOf(c)) >= 0 ?
                    destination[index] : c);
            }

            return result.ToString();
        }
    }
}
