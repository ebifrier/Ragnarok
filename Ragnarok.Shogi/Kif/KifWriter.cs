using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Ragnarok.Shogi.Kif
{
    using File;

    /// <summary>
    /// kifファイルの書き出しを行います。
    /// </summary>
    internal sealed class KifWriter : IKifuWriter
    {
        /// <summary>
        /// kif形式で出力するか取得します。(でなければki2形式)
        /// </summary>
        public bool IsKif
        {
            get;
            private set;
        }

        /// <summary>
        /// ヘッダ部分を出力します。
        /// </summary>
        private void WriteHeader(TextWriter writer, KifuObject kifu)
        {
            writer.WriteLine("# ----  棋譜ファイル  ----");

            foreach (var item in kifu.Header)
            {
                var name = KifUtil.GetHeaderName(item.Key);
                if (name == null)
                {
                    continue;
                }

                writer.WriteLine("{0}：{1}", name, item.Value);
            }
        }

        /// <summary>
        /// 局面を出力します。
        /// </summary>
        private void WriteBoard(TextWriter writer, Board board)
        {
            writer.WriteLine(board.ToBod());

            if (IsKif)
            {
                writer.WriteLine("手数----指手---------消費時間--");
            }
        }

        #region ki2
        /// <summary>
        /// ki2形式の指し手を出力します。
        /// </summary>
        private void WriteMoveNodeKi2(TextWriter writer, MoveNode node, Board board)
        {
            var moveList = KifuObject.Convert2List(node);
            var lineList = board.ConvertMove(moveList, false)
                .Where(_ => _ != null && _.Validate())
                .Select(_ => Stringizer.ToString(_, MoveTextStyle.Normal))
                .TakeBy(6)
                .Select(_ => string.Join("　", _.ToArray()));

            // 各指し手行を出力します。
            foreach (var line in lineList)
            {
                writer.WriteLine(line);
            }
        }
        #endregion

        #region kif
        /// <summary>
        /// 変化の分岐を含めて出力します。
        /// </summary>
        private void WriteMoveNodeKif(TextWriter writer, MoveNode node,
                                      Board board, bool hasVariation)
        {
            if (node == null)
            {
                return;
            }

            // とりあえず指し手を書きます。
            if (node.Move != null)
            {
                WriteMakeKif(writer, node, board, hasVariation);

                if (!board.DoMove(node.Move))
                {
                    Log.Error("{0}の指し手が正しくありません。", node.Move);
                    return;
                }
            }

            // ついでにコメント行も出力します。
            // 先頭ノードは指し手がありませんが
            // コメントは存在することがあります。
            WriteCommentKif(writer, node);

            // 次の指し手があればそれも出力します。
            for (var i = 0; i < node.NextNodes.Count(); ++i)
            {
                var child = node.NextNodes[i];
                var hasVariationNext = (i < node.NextNodes.Count() - 1);

                if (i > 0)
                {
                    writer.WriteLine();
                    writer.WriteLine();
                    writer.WriteLine("変化：{0}手", child.MoveCount);
                }

                WriteMoveNodeKif(writer, child, board, hasVariationNext);
            }

            if (node.Move != null)
            {
                board.Undo();
            }
        }

        /// <summary>
        /// 指し手行を出力します。
        /// </summary>
        private void WriteMakeKif(TextWriter writer, MoveNode node,
                                  Board board, bool hasVariation)
        {
            var move = board.ConvertMove(node.Move, true);
            if (move == null || !move.Validate())
            {
                Log.Error("指し手'{0}'を出力できませんでした。", node.Move);
                return;
            }

            // 半角文字相当の文字数で空白の数を計算します。
            var moveText = Stringizer.ToString(move, MoveTextStyle.KifFile);
            var hanLen = moveText.HankakuLength();

            writer.WriteLine(
                @"{0,4} {1}{2} ({3:mm\:ss} / {4:hh\:mm\:ss}){5}",
                node.MoveCount,
                moveText,
                new string(' ', Math.Max(0, 14 - hanLen)),
                node.Duration,
                node.TotalDuration,
                (hasVariation ? "+" : ""));
        }

        /// <summary>
        /// コメント行を出力します。
        /// </summary>
        private void WriteCommentKif(TextWriter writer, MoveNode node)
        {
            foreach (var variationInfo in node.VariationInfoList)
            {
                if (variationInfo == null || variationInfo.MoveList == null)
                {
                    continue;
                }

                writer.WriteLine("**{0} {1}",
                    variationInfo.Value,
                    string.Join("",
                        variationInfo.MoveList.Select(_ => _.ToString())));
            }

            foreach (var comment in node.CommentList)
            {
                writer.WriteLine("*{0}", comment);
            }
        }
        #endregion

        /// <summary>
        /// 局面と差し手をファイルに保存します。
        /// </summary>
        public void Save(TextWriter writer, KifuObject kifu)
        {
            WriteHeader(writer, kifu);
            WriteBoard(writer, kifu.StartBoard);

            if (IsKif)
            {
                WriteMoveNodeKif(writer, kifu.RootNode, kifu.StartBoard, false);
            }
            else
            {
                WriteMoveNodeKi2(writer, kifu.RootNode, kifu.StartBoard);
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KifWriter(bool isKif)
        {
            IsKif = isKif;
        }
    }
}
