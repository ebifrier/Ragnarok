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
            writer.WriteLine(BodBoard.ToBod(board));

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
                .Select(_ => _.ToString())
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
                                      bool hasVariation)
        {
            if (node == null)
            {
                return;
            }

            // とりあえず指し手を書きます。
            if (node.Move != null)
            {
                WriteMakeKif(writer, node, hasVariation);
            }

            // ついでにコメント行も出力します。
            // 先頭ノードは指し手がありませんが
            // コメントは存在することがあります。
            if (!string.IsNullOrEmpty(node.Comment))
            {
                WriteCommentKif(writer, node);
            }

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

                WriteMoveNodeKif(writer, child, hasVariationNext);
            }
        }

        /// <summary>
        /// 指し手行を出力します。
        /// </summary>
        private void WriteMakeKif(TextWriter writer, MoveNode node, bool hasVariation)
        {
            // 半角文字相当の文字数で空白の数を計算します。
            var moveText = node.Move.ToString();
            var hanLen = moveText.HankakuLength();

            writer.WriteLine(
                @"{0,4} {1}{2} ({3:mm\:ss}/{4:hh\:mm\:ss}){5}",
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
            var commentList = node.Comment.Split(
                new char[] { '\n' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var comment in commentList)
            {
                writer.WriteLine("* {0}", comment);
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
                WriteMoveNodeKif(writer, kifu.RootNode, false);
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
