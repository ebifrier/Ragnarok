using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 評価値のタイプです。
    /// </summary>
    public enum ScoreType
    {
        /// <summary>
        /// 通常の評価値です。
        /// </summary>
        Value,
        /// <summary>
        /// 詰みの手数を示します。
        /// </summary>
        Mate,
    }

    /// <summary>
    /// 評価値を文字列で扱う為のクラスです。
    /// </summary>
    public sealed class Score
    {
        /// <summary>
        /// 詰みの時の評価値を取得します。
        /// </summary>
        public static readonly int MateScore = 99999;
        private string name;

        /// <summary>
        /// 通常の評価値をパースします。
        /// </summary>
        /// <example>
        /// 0
        /// -98
        /// +456↑
        /// </example>
        public static Score ParseValue(string text, BWType turn)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }

            var trimmedText = text.Trim();
            var value = StringToInt(trimmedText);

            return new Score(turn, trimmedText, value);
        }

        /// <summary>
        /// 詰みになったときの手数をパースします。
        /// </summary>
        /// <example>
        /// +
        /// -10
        /// +5↑
        /// </example>
        public static Score ParseMate(string text, BWType turn)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }

            var trimmedText = text.Trim();
            var value = StringToInt(trimmedText);

            if (value == 0)
            {
                if (trimmedText[0] == '+')
                {
                    return new Score(turn, trimmedText, 0, true);
                }
                else if (trimmedText[0] == '-')
                {
                    return new Score(turn, trimmedText, 0, false);
                }
                else
                {
                    //throw new ShogiException(
                    //    trimmedText + ": メイト手数が正しくありません。");

                    // 本来は先頭に+/-が必要ですが、そうなっていないソフトも多いので
                    // ここでは現状に合わせてエラーにはしないことにします。
                    return new Score(turn, trimmedText, 0, true);
                }
            }

            return new Score(turn, trimmedText, Math.Abs(value), (value > 0));
        }

        /// <summary>
        /// 数値に変換可能な部分のみを数値に直します。
        /// </summary>
        private static int StringToInt(string text)
        {
            var isNegative = false;
            var startIndex = 0;
            var result = 0L;

            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }

            if (text[0] == '-')
            {
                isNegative = true;
                startIndex = 1;
            }
            else if (text[0] == '+')
            {
                startIndex = 1;
            }

            for (var i = startIndex; i < text.Length; i++)
            {
                if (char.IsWhiteSpace(text[i]))
                {
                    continue;
                }

                if ('0' <= text[i] && text[i] <= '9')
                {
                    var n = text[i] - '0';

                    result = result * 10 + n;
                    if (result > int.MaxValue || result < int.MinValue)
                    {
                        throw new OverflowException(
                            text + ": 評価値がオーバーフローしました。");
                    }
                }
                else
                {
                    if (i == startIndex)
                    {
                        throw new ArgumentException(
                            text + ": 評価値が正しくありません。");
                    }

                    break;
                }
            }

            return (int)(result * (isNegative ? -1 : +1));
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Score()
        {
            ScoreType = ScoreType.Value;
            Text = "0";
            Value = 0;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Score(BWType turn, string text = "", int value = 0)
        {
            ScoreType = ScoreType.Value;
            Turn = turn;
            Text = text;
            Value = value;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Score(BWType turn, string text, int mate, bool isMateWin)
        {
            ScoreType = ScoreType.Mate;
            Turn = turn;
            Text = text;
            Mate = mate;
            IsMateWin = isMateWin;
            Value = (isMateWin ? MateScore : -MateScore);
        }

        /// <summary>
        /// 評価値の種類を取得します。
        /// </summary>
        public ScoreType ScoreType
        {
            get;
            private set;
        }

        /// <summary>
        /// 評価値が先後どちらを基準にしているかを取得します。
        /// </summary>
        public BWType Turn
        {
            get;
            private set;
        }

        /// <summary>
        /// 評価値文字列を取得します。
        /// </summary>
        public string Text
        {
            get;
            private set;
        }

        /// <summary>
        /// 評価値を取得します。
        /// </summary>
        public int Value
        {
            get;
            private set;
        }

        /// <summary>
        /// 先手を＋、後手を－とした評価値を取得します。
        /// </summary>
        public Score Absolute
        {
            get
            {
                return (Turn == BWType.White ? -this : this);
            }
        }

        /// <summary>
        /// 詰みの手数を取得します。
        /// </summary>
        public int Mate
        {
            get;
            private set;
        }

        /// <summary>
        /// 詰みの場合、自分が勝ちかどうかを取得します。
        /// </summary>
        public bool IsMateWin
        {
            get;
            private set;
        }

        /// <summary>
        /// この評価値の表示名を取得または設定します。
        /// </summary>
        public string Name
        {
            get
            {
                if (this.name != null)
                {
                    return this.name;
                }

                return (
                    Turn == BWType.Black ? "先手" :
                    Turn == BWType.White ? "後手" : "");
            }
            set { this.name = value; }
        }

        /// <summary>
        /// オブジェクトのクローンを作成します。
        /// </summary>
        public Score Clone()
        {
            return (Score)this.MemberwiseClone();
        }

        /// <summary>
        /// 符号を反転します。
        /// </summary>
        public void Neg()
        {
            if (ScoreType == ScoreType.Value)
            {
                if (Value == 0)
                {
                    return;
                }

                Text = (
                    Text[0] == '-' ? Text.Substring(1) :
                    Text[0] == '+' ? '-' + Text.Substring(1) :
                    '-' + Text);
            }
            else
            {
                // 符号を必ずつけます。
                Text = (
                    Text[0] == '-' ? '+' + Text.Substring(1) :
                    Text[0] == '+' ? '-' + Text.Substring(1) :
                    '-' + Text);
                IsMateWin = !IsMateWin;
            }

            Value *= -1;
            Turn = Turn.Flip();
        }

        /// <summary>
        /// 評価値の基準となる手番を入れ替えます。
        /// </summary>
        public void FlipTurn()
        {
            Turn = Turn.Flip();
        }

        /// <summary>
        /// 単項+演算子
        /// </summary>
        public static Score operator +(Score score)
        {
            return score;
        }

        /// <summary>
        /// 単項-演算子
        /// </summary>
        public static Score operator -(Score score)
        {
            var clone = score.Clone();
            clone.Neg();
            return clone;
        }
    }
}
