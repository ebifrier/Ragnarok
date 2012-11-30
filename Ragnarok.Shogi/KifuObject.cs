using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 指し手や変化など棋譜ファイルに必要な要素を保持します。
    /// </summary>
    public sealed class KifuObject
    {
        /// <summary>
        /// sjisが基本。
        /// </summary>
        public static readonly Encoding DefaultEncoding =
            Encoding.GetEncoding("Shift_JIS");

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
        public MoveNode RootNode
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
        public static IEnumerable<Move> Convert2List(MoveNode root)
        {
            for (var node = root; node != null; node = node.NextChild)
            {
                yield return node.Move;
            }
        }

        /// <summary>
        /// 指し手リストをツリー形式に変換します。
        /// </summary>
        public static MoveNode Convert2Node(IEnumerable<Move> moveList,
                                                 int firstMoveCount)
        {
            if (moveList == null || !moveList.Any())
            {
                return null;
            }

            var clonedMoveList = moveList.ToList();
            var beginNumber = firstMoveCount + clonedMoveList.Count();
            MoveNode root = null;

            clonedMoveList.Reverse();
            foreach (var move in clonedMoveList)
            {
                var node = new MoveNode()
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
        public void SetRootNode(MoveNode root)
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
        /// 棋譜の確認と指し手の正規化を行います。
        /// </summary>
        public void Normalize()
        {
            if (Headers == null || RootNode == null)
            {
                throw new ShogiException(
                    "ヘッダーかノードが設定されていません。");
            }

            RootNode.Normalize(new Board());
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
                         MoveNode root)
        {
            SetHeader(header);
            SetRootNode(root);
        }
    }
}
