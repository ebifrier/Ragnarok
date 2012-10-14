using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 変化を木構造で表すためのクラスです。
    /// </summary>
    public sealed class VariationNode
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
        /// 次の指し手を取得または設定します。
        /// </summary>
        public VariationNode NextChild
        {
            get;
            set;
        }

        /// <summary>
        /// 次の変化の手を取得または設定します。
        /// </summary>
        public VariationNode NextVariation
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 指し手や変化など棋譜ファイルに必要な要素を保持します。
    /// </summary>
    public sealed class KifuObject
    {
        /// <summary>
        /// ヘッダ部分の情報を取得します。
        /// </summary>
        public Dictionary<string, string> Headers
        {
            get;
            private set;
        }

        /// <summary>
        /// 変化を含んだ指し手の開始ノードを取得します。
        /// </summary>
        public VariationNode RootNode
        {
            get;
            private set;
        }

        /// <summary>
        /// 差し手リストを取得します。
        /// </summary>
        public IEnumerable<Move> MoveList
        {
            get;
            private set;
        }

        /// <summary>
        /// 指し手ツリーをリスト形式に変換します。
        /// </summary>
        public static IEnumerable<Move> Convert2List(VariationNode root)
        {
            for (var node = root; node != null; node = node.NextChild)
            {
                yield return node.Move;
            }
        }

        /// <summary>
        /// 指し手リストをツリー形式に変換します。
        /// </summary>
        public static VariationNode Convert2Node(IEnumerable<Move> moveList,
                                                 int firstMoveCount)
        {
            if (moveList == null || !moveList.Any())
            {
                return null;
            }

            VariationNode root = null;
            var beginNumber = firstMoveCount + moveList.Count();

            foreach (var move in moveList.Reverse())
            {
                var node = new VariationNode()
                {
                    MoveCount = --beginNumber,
                    Move = move,
                    NextChild = root,
                };

                root = node;
            }

            return root;
        }

        /// <summary>
        /// ヘッダを設定します。
        /// </summary>
        public void SetHeader(Dictionary<string, string> header)
        {
            Headers = header ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// 変化ツリーのルートノードを設定します。
        /// </summary>
        public void SetRootNode(VariationNode root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            RootNode = root;
            MoveList = Convert2List(root).ToList();
        }

        /// <summary>
        /// 変化リストを設定します。
        /// </summary>
        public void SetMoveList(IEnumerable<Move> moveList)
        {
            if (moveList == null)
            {
                throw new ArgumentNullException("moveList");
            }

            MoveList = moveList.ToList();
            RootNode = Convert2Node(moveList, 1);
        }

        /// <summary>
        /// 現在読み込まれている差し手から、局面を作成します。
        /// </summary>
        public Board CreateBoard()
        {
            var board = new Board();

            var boardMoveList = board.ConvertMove(MoveList);
            if (boardMoveList.Count != MoveList.Count())
            {
                throw new InvalidOperationException(
                    string.Format(
                        "{0}手目: 差し手が正しくないため、局面の作成に失敗しました。",
                        boardMoveList.Count + 1));
            }

            var num = 1;
            foreach (var boardMove in boardMoveList)
            {
                if (!board.DoMove(boardMove))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "{0}手目: 差し手が正しくありません。",
                            num));
                }

                num += 1;
            }

            return board;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KifuObject()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KifuObject(Dictionary<string, string> header,
                         IEnumerable<Move> moveList)
        {
            SetHeader(header);
            SetMoveList(moveList);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KifuObject(Dictionary<string, string> header,
                         VariationNode root)
        {
            SetHeader(header);
            SetRootNode(root);
        }
    }
}
