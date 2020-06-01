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
    /// 評価値探索の種別を示します。
    /// </summary>
    public enum ScoreBound
    {
        /// <summary>
        /// 通常の値
        /// </summary>
        Exact,
        /// <summary>
        /// この値より上の評価値になる。
        /// </summary>
        Lower,
        /// <summary>
        /// この値より下の評価値になる。
        /// </summary>
        Upper,
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
        /// コンストラクタ
        /// </summary>
        public Score()
        {
            ScoreType = ScoreType.Value;
            ScoreBound = ScoreBound.Exact;
            Value = 0;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public static Score CreateValue(Colour turn, int value = 0,
                                        ScoreBound bound = ScoreBound.Exact)
        {
            return new Score()
            {
                ScoreType = ScoreType.Value,
                ScoreBound = bound,
                Turn = turn,
                Value = value
            };
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public static Score CreateMate(Colour turn, int mate,
                                       bool isMateWin,
                                       ScoreBound bound = ScoreBound.Exact)
        {
            return new Score()
            {
                ScoreType = ScoreType.Mate,
                ScoreBound = bound,
                Turn = turn,
                Mate = mate,
                IsMateWin = isMateWin,
                Value = (isMateWin
                    ? MateScore - mate
                    : -MateScore + mate)
            };
        }

        /// <summary>
        /// 通常の評価値をパースします。
        /// </summary>
        /// <example>
        /// 0
        /// -98
        /// +456↑
        /// </example>
        public static Score ParseValue(string text, Colour turn,
                                       ScoreBound bound = ScoreBound.Exact)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            var trimmedText = text.Trim();
            var value = StringToInt(trimmedText);

            // 評価値の最後に↑や↓があれば評価値タイプを上書き
            var last = trimmedText.LastOrDefault();
            bound = (
                last == '↑' ? ScoreBound.Lower :
                last == '↓' ? ScoreBound.Upper :
                bound);

            return Score.CreateValue(turn, value, bound);
        }

        /// <summary>
        /// 詰みになったときの手数をパースします。
        /// </summary>
        /// <example>
        /// +
        /// -10
        /// +5↑
        /// </example>
        public static Score ParseMate(string text, Colour turn,
                                      ScoreBound bound = ScoreBound.Exact)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            var trimmedText = text.Trim();
            var value = StringToInt(trimmedText);

            if (value == 0)
            {
                if (trimmedText.FirstOrDefault() == '-')
                {
                    return CreateMate(turn, 0, false, bound);
                }
                else
                {
                    // 本来は先頭に+/-が必要ですが、そうなっていないソフトも多いので
                    // ここでは現状に合わせてエラーにはしないことにします。
                    return CreateMate(turn, 0, true, bound);
                }
            }

            return CreateMate(turn, Math.Abs(value), value > 0, bound);
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
                throw new ArgumentNullException(nameof(text));
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
        public Colour Turn
        {
            get;
            private set;
        }

        /// <summary>
        /// 評価値を取得します。(詰みの場合はその手数となります)
        /// </summary>
        public int Value
        {
            get;
            private set;
        }

        /// <summary>
        /// 評価値の特性を取得します。
        /// </summary>
        public ScoreBound ScoreBound
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
                return (Turn == Colour.White ? -this : this);
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
                    Turn == Colour.Black ? "先手" :
                    Turn == Colour.White ? "後手" : "");
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
            if (ScoreType == ScoreType.Mate)
            {
                IsMateWin = !IsMateWin;
            }

            Value *= -1;
            ScoreBound = ScoreBound.Flip();
            Turn = Turn.Flip();
        }

        /// <summary>
        /// 評価値の手番のみを反転します。
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
            if (score == null)
            {
                throw new ArgumentNullException(nameof(score));
            }

            var clone = score.Clone();
            clone.Neg();
            return clone;
        }
    }
}
