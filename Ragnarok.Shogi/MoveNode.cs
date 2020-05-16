using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 変化と評価値は１セットのため、まとめて管理しています。
    /// </summary>
    public sealed class PVInfo
    {
        /// <summary>
        /// 解析された評価値を取得または設定します。
        /// </summary>
        public int Value
        {
            get;
            set;
        }

        /// <summary>
        /// 解析された変化を取得または設定します。
        /// </summary>
        public List<Move> MoveList
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 変化を木構造で表すためのクラス
    /// </summary>
    /// <remarks>
    /// このクラスは指し手としてBoardMoveオブジェクトを持ちます。
    /// </remarks>
    public sealed class MoveNode
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
        /// 総消費時間を取得します。
        /// </summary>
        public TimeSpan TotalDuration
        {
            get
            {
                // 自分の手番の消費時間のみを考えます。
                if (ParentNode == null || ParentNode.ParentNode == null)
                {
                    return Duration;
                }
                else
                {
                    return (ParentNode.ParentNode.TotalDuration + Duration);
                }
            }
        }

        /// <summary>
        /// ソフトが出力した変化とその評価値のリストを取得します。
        /// </summary>
        public List<PVInfo> PVInfoList
        {
            get;
            set;
        }

        /// <summary>
        /// このノードの評価値を取得します。
        /// </summary>
        public int? Value
        {
            get
            {
                return (PVInfoList != null && PVInfoList.Any() ?
                    PVInfoList.First().Value : (int?)null);
            }
        }

        /// <summary>
        /// この手のコメント行ごとに取得します。
        /// </summary>
        public List<string> CommentList
        {
            get;
            set;
        }

        /// <summary>
        /// 一手前の指し手ノードを取得します。
        /// </summary>
        public MoveNode ParentNode
        {
            get;
            private set;
        }

        /// <summary>
        /// 次の指し手を取得または設定します。
        /// </summary>
        /// <remarks>
        /// index=0が本譜、それ以外は違う変化の手となります。
        /// </remarks>
        public List<MoveNode> NextNodes
        {
            get;
            private set;
        }

        /// <summary>
        /// 本譜における次の指し手を取得します。
        /// </summary>
        public MoveNode NextNode
        {
            get { return NextNodes.FirstOrDefault(); }
        }

        /// <summary>
        /// 次の指し手がいくつあるか取得します。
        /// </summary>
        public int NextNodeCount
        {
            get { return NextNodes.Count; }
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
        /// コメント行の追加を行います。
        /// </summary>
        public void AddComment(string comment)
        {
            if (string.IsNullOrEmpty(comment))
            {
                return;
            }

            CommentList.Add(comment);
        }

        /// <summary>
        /// 次の指し手ノードを追加します。
        /// </summary>
        public void AddNextNode(MoveNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (node.ParentNode != null)
            {
                throw new InvalidOperationException(
                    "すでに親ノードが登録されています。");
            }

            NextNodes.Add(node);
            node.ParentNode = this;
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
                    str, new string(' ', Math.Max(0, 14 - hanlen)));
            }

            if (NextNode != null)
            {
                NextNode.MakeString(sb, nmoves + 1);
            }

            for (var i = 1; i < NextNodes.Count; ++i)
            {
                sb.AppendLine();
                sb.Append(new string(' ', 17 * nmoves));

                NextNodes[i].MakeString(sb, nmoves);
            }
        }

        /// <summary>
        /// ノード全体を正規化し、変化の重複などをなくします。
        /// </summary>
        public void Regulalize(int moveCount = 0)
        {
            RegulalizeStatic(this, moveCount);
        }

        /// <summary>
        /// ノード全体を正規化し、変化の重複などをなくします。
        /// </summary>
        private static void RegulalizeStatic(MoveNode node, int moveCount)
        {
            while (node != null)
            {
                // 変化の重複を削除します。
                for (var i = 0; i < node.NextNodes.Count; ++i)
                {
                    for (var j = i + 1; j < node.NextNodes.Count; )
                    {
                        var baseNode = node.NextNodes[i];
                        var compNode = node.NextNodes[j];

                        if (baseNode.Move == compNode.Move)
                        {
                            // もし同じ指し手の変化があれば、子の指し手をマージします。
                            // 子の重複チェックはこの後行うので、
                            // ここで重複があっても構いません。
                            compNode.NextNodes.ForEach(_ => _.ParentNode = null);
                            compNode.NextNodes.ForEach(_ => baseNode.AddNextNode(_));

                            node.NextNodes.RemoveAt(j);
                        }
                        else
                        {
                            ++j;
                        }
                    }
                }

                // 手数の再設定
                node.MoveCount = moveCount;

                if (node.NextNodeCount == 1)
                {
                    // 手数が長くなったときスタックオーバーフローになるため、
                    // 子ノードが一つの場合はループ内で回します。
                    node = node.NextNode;
                    moveCount += 1;
                }
                else
                {
                    // 子ノードに関してもチェックを行います。
                    foreach (var child in node.NextNodes)
                    {
                        RegulalizeStatic(child, moveCount + 1);
                    }

                    return;
                }
            }
        }

        /// <summary>
        /// ノード全体を比較します。
        /// </summary>
        public bool NodeEquals(MoveNode other, bool compareComment)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (MoveCount != other.MoveCount)
            {
                return false;
            }

            if (Move != other.Move)
            {
                return false;
            }

            if (Duration != other.Duration)
            {
                return false;
            }

            if (NextNodeCount != other.NextNodeCount)
            {
                return false;
            }

            if (compareComment && !CommentList.SequenceEqual(other.CommentList))
            {
                return false;
            }

            for (var i = 0; i < NextNodeCount; ++i)
            {
                if (!NextNodes[i].NodeEquals(other.NextNodes[i], compareComment))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MoveNode()
        {
            NextNodes = new List<MoveNode>();
            PVInfoList = new List<PVInfo>();
            CommentList = new List<string>();
        }
    }
}
