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
        public KifuHeader Header
        {
            get;
            private set;
        }

        /// <summary>
        /// 開始局面を取得します。
        /// </summary>
        public Board StartBoard
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
        public IEnumerable<BoardMove> MoveList
        {
            get;
            private set;
        }

        /// <summary>
        /// 棋譜にあったエラーを取得します。
        /// </summary>
        public Exception Error
        {
            get;
            private set;
        }

        /// <summary>
        /// 指し手ツリーをリスト形式に変換します。
        /// </summary>
        public static IEnumerable<BoardMove> Convert2List(MoveNode root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            for (var node = root.NextNode; node != null; node = node.NextNode)
            {
                yield return node.Move;
            }
        }

        /// <summary>
        /// 指し手リストをツリー形式に変換します。
        /// </summary>
        public static MoveNode Convert2Node(IEnumerable<BoardMove> moveList,
                                            int firstMoveCount)
        {
            if (moveList == null)
            {
                throw new ArgumentNullException("moveList");
            }

            var root = new MoveNode();
            var last = root;

            foreach (var move in moveList)
            {
                var node = new MoveNode()
                {
                    MoveCount = firstMoveCount++,
                    Move = move,
                };

                last.NextNodes.Add(node);
                last = node;
            }

            return root;
        }

        /// <summary>
        /// ヘッダを設定します。
        /// </summary>
        public void SetHeader(KifuHeader header)
        {
            Header = header ?? new KifuHeader();
        }

        /// <summary>
        /// 局面を設定し、局面に指し手がある場合はそれを指し手リストに追加します。
        /// </summary>
        /// <remarks>
        /// <paramref name="isStartBoard"/>が真の場合は開始局面のみ
        /// （手を全く指していない＝手数が０）しか受け入れません。
        /// </remarks>
        public void SetBoard(Board board, bool isStartBoard)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }

            if (isStartBoard)
            {
                if (board.MoveCount > 0)
                {
                    throw new ShogiException(
                        "SetBoardには開始局面を設定してください。");
                }

                StartBoard = board;
            }
            else
            {
                // 開始局面でなくてもよい場合は、指し手を別に取得します。
                var moveList = new List<BoardMove>();
                for (var move = board.Undo(); move != null; move = board.Undo())
                {
                    moveList.Insert(0, move);
                }

                StartBoard = board;
                SetMoveList(moveList);
            }
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
        public void SetMoveList(IEnumerable<BoardMove> moveList)
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
            var board = StartBoard.Clone();

            MoveList.ForEachWithIndex((move, n) =>
            {
                if (!board.DoMove(move))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "{0}手目: 差し手が正しくありません。",
                            n + 1));
                }
            });

            return board;
        }

        /// <summary>
        /// 棋譜の確認と指し手の正規化を行います。
        /// </summary>
        public void Validate()
        {
            if (Header == null)
            {
                throw new ShogiException(
                    "ヘッダーが設定されていません。");
            }

            if (RootNode == null)
            {
                throw new ShogiException(
                    "指し手が設定されていません。");
            }

            if (StartBoard == null || !StartBoard.Validate())
            {
                throw new ShogiException(
                    "開始局面が正しくありません。");
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KifuObject(KifuHeader header,
                          Board startBoard,
                          Exception error = null)
        {
            MoveList = new List<BoardMove>();
            RootNode = new MoveNode();
            Error = error;

            SetHeader(header);
            SetBoard(startBoard, false);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KifuObject(KifuHeader header,
                         Board startBoard,
                         IEnumerable<BoardMove> moveList,
                         Exception error = null)
        {
            Error = error;

            SetHeader(header);
            SetBoard(startBoard, true);
            SetMoveList(moveList);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KifuObject(KifuHeader header,
                          Board startBoard,
                          MoveNode root, Exception error = null)
        {
            Error = error;

            SetHeader(header);
            SetBoard(startBoard, true);
            SetRootNode(root);
        }
    }
}
