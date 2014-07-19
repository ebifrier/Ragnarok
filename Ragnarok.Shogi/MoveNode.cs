using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi
{
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
        public BoardMove Move
        {
            get;
            set;
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
            set;
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
            get { return NextNodes.Count(); }
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
                    str, new string(' ', Math.Max(0, 14 - hanlen)));
            }

            if (NextNode != null)
            {
                NextNode.MakeString(sb, nmoves + 1);
            }

            for (var i = 1; i < NextNodes.Count(); ++i)
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
            // 変化の重複を削除します。
            for (var i = 0; i < NextNodes.Count(); ++i)
            {
                for (var j = i + 1; j < NextNodes.Count(); )
                {
                    var baseNode = NextNodes[i];
                    var compNode = NextNodes[j];

                    if (baseNode.Move == compNode.Move)
                    {
                        // もし同じ指し手の変化があれば、子の指し手をマージします。
                        // 子の重複チェックはこの後行うので、
                        // ここで重複があっても構いません。
                        baseNode.NextNodes.AddRange(compNode.NextNodes);

                        NextNodes.RemoveAt(j);
                    }
                    else
                    {
                        ++j;
                    }
                }
            }

            // 手数の再設定
            MoveCount = moveCount;

            // 子ノードに関してもチェックを行います。
            foreach (var node in NextNodes)
            {
                node.Regulalize(moveCount + 1);
            }
        }

        /// <summary>
        /// ノード全体を比較します。
        /// </summary>
        public bool NodeEquals(MoveNode other)
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

            if (NextNodeCount != other.NextNodeCount)
            {
                return false;
            }

            for (var i = 0; i < NextNodeCount; ++i)
            {
                if (!NextNodes[i].NodeEquals(other.NextNodes[i]))
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
        }
    }
}
