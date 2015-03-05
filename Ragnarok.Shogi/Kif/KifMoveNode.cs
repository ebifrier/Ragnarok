using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.Shogi
{
    using File;

    /// <summary>
    /// 変化のあるkif形式を読み取るときに使います。
    /// </summary>
    public sealed class KifMoveNode
    {
        /// <summary>
        /// 手数を取得または設定します。
        /// </summary>
        public int MoveCount
        {
            get;
            set;
        }

        /// <summary>
        /// 指し手があるファイルの行数を取得または設定します。
        /// </summary>
        public int LineNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 指し手を取得または設定します。
        /// </summary>
        public Move Move
        {
            get;
            set;
        }

        /// <summary>
        /// 着手にかかった時間を取得または設定します。
        /// </summary>
        public TimeSpan Duration
        {
            get;
            set;
        }

        /// <summary>
        /// 着手にかかった時間を秒単位で取得または設定します。
        /// </summary>
        public int DurationSeconds
        {
            get { return (int)Duration.TotalSeconds; }
            set { Duration = TimeSpan.FromSeconds(value); }
        }

        /// <summary>
        /// コメント行を取得または設定します。
        /// </summary>
        public string Comment
        {
            get;
            set;
        }

        /// <summary>
        /// 解析された評価値を取得または設定します。
        /// </summary>
        public int? EvaluationValue
        {
            get;
            set;
        }

        /// <summary>
        /// 解析された変化を取得または設定します。
        /// </summary>
        public List<BoardMove> AnalyzedVariation
        {
            get;
            set;
        }

        /// <summary>
        /// 本譜における次の指し手を取得または設定します。
        /// </summary>
        public KifMoveNode Next
        {
            get;
            set;
        }

        /// <summary>
        /// 次の変化を取得します。
        /// </summary>
        public KifMoveNode Variation
        {
            get;
            set;
        }

        /// <summary>
        /// 次の指し手に本譜以外の変化があるか調べます。
        /// </summary>
        public bool HasVariation
        {
            get;
            set;
        }

        /// <summary>
        /// ノード全体を文字列化します。
        /// </summary>
        public string DebugText
        {
            get
            {
                var sb = new StringBuilder();

                MakeString(sb, 0);
                return sb.ToString();
            }
        }

        /// <summary>
        /// ノード全体を文字列化します。
        /// </summary>
        private void MakeString(StringBuilder sb, int nmoves)
        {
            if (Move != null)
            {
                var str = Move.ToString();
                var hanlen = str.HankakuLength();

                sb.AppendFormat(" - {0}{1}",
                    str, new string(' ', 8 - hanlen));
            }

            if (Next != null)
            {
                Next.MakeString(sb, nmoves + 1);
            }

            if (Variation != null)
            {
                sb.AppendLine();
                sb.Append(new string(' ', 11 * nmoves));

                Variation.MakeString(sb, nmoves);
            }
        }

        /// <summary>
        /// 解析された変化を示す正規表現です。
        /// </summary>
        private static readonly Regex KifAnalyzedVariationRegex = new Regex(
            @"(\d+) (\w+)");

        /// <summary>
        /// コメント文字列を追加します。
        /// </summary>
        public void AddComment(string comment)
        {
            if (string.IsNullOrEmpty(comment))
            {
                return;
            }

            var m = KifAnalyzedVariationRegex.Match(comment);
            if (m.Success)
            {
                // 解析された変化や評価値を設定します。
            }
            else if (string.IsNullOrEmpty(Comment))
            {
                Comment = comment;
            }
            else
            {
                Comment += "\n" + comment;
            }
        }

        /// <summary>
        /// KifMoveNodeからMoveNodeへ構造を変換します。
        /// </summary>
        public MoveNode ConvertToMoveNode(Board board, KifMoveNode head,
                                          out Exception error)
        {
            var errors = new List<Exception>();
            var root =  new MoveNode
            {
                MoveCount = head.MoveCount,
                Duration = head.Duration,
                Comment = head.Comment,
            };
            // これでrootの子要素に指し手ツリーが設定されます。
            ConvertToMoveNode(board, root, errors);

            error = (
                !errors.Any() ? null :
                errors.Count() == 1 ? errors.FirstOrDefault() :
                new AggregateException(errors));

            return root;
        }

        /// <summary>
        /// KifMoveNodeからMoveNodeへ構造を変換します。
        /// 結果は<paramref name="root"/>以下に設定されます。
        /// </summary>
        private void ConvertToMoveNode(Board board, MoveNode root,
                                       List<Exception> errors)
        {
            for (var node = this; node != null; node = node.Variation)
            {
                var cloned = board.Clone();

                // 指し手を実際に指してみます。
                var bmove = node.MakeMove(cloned, errors);
                if (bmove == null)
                {
                    continue;
                }

                var moveNode = new MoveNode
                {
                    Move = bmove,
                    MoveCount = node.MoveCount,
                    Duration = node.Duration,
                    Comment = node.Comment,
                };

                // 次の指し手とその変化を変換します。
                if (node.Next != null)
                {
                    node.Next.ConvertToMoveNode(cloned, moveNode, errors);
                }

                root.AddNext(moveNode);
            }
        }

        /// <summary>
        /// このノードの手を実際に指してみて、着手可能か確認します。
        /// </summary>
        private BoardMove MakeMove(Board board, List<Exception> errors)
        {
            var bmove = board.ConvertMove(Move, true);
            if (bmove == null || !bmove.Validate())
            {
                errors.Add(new FileFormatException(
                    LineNumber,
                    string.Format(
                        "{0}手目: 指し手が正しくありません。",
                        MoveCount)));
                return null;
            }

            // 局面を進めます。
            if (!board.DoMove(bmove))
            {
                errors.Add(new FileFormatException(
                    LineNumber,
                    string.Format(
                        "{0}手目の手を指すことができませんでした。",
                        MoveCount)));
                return null;
            }

            return bmove;
        }
    }
}
