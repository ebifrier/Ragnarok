using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Ragnarok.Shogi.File
{
    /// <summary>
    /// kifファイルの書き出しを行います。
    /// </summary>
    internal sealed class KifWriter : IKifuWriter
    {
        /// <summary>
        /// ヘッダ部分を出力します。
        /// </summary>
        private void WriteHeader(TextWriter writer, KifuObject kifu)
        {
            writer.WriteLine("# ----  投票将棋 棋譜ファイル  ----");

            foreach (var header in kifu.Headers)
            {
                writer.WriteLine("{0}：{1}", header.Key, header.Value);
            }

            writer.WriteLine("手数----指手---------消費時間--");
        }

        /// <summary>
        /// 指し手行、１行分を作成します。
        /// </summary>
        private string MakeLine(MoveNode node, bool hasVariation)
        {
            // 半角文字相当の文字数で空白の数を計算します。
            var moveText = node.Move.ToString();
            var hanLen = moveText.HankakuLength();

            return string.Format("{0,4} {1}{2} ( 0:00/00:00:00){3}",
                node.MoveCount,
                moveText,
                new string(' ', Math.Max(0, 14 - hanLen)),
                (hasVariation ? "+" : ""));
        }

        /// <summary>
        /// 変化の分岐を含めて出力します。
        /// </summary>
        private void WriteMoveNode(TextWriter writer, MoveNode node,
                                   bool hasVariation)
        {
            if (node == null || node.Move == null)
            {
                return;
            }

            // とりあえず指し手を書きます。
            writer.WriteLine(MakeLine(node, hasVariation));

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

                WriteMoveNode(writer, child, hasVariationNext);
            }
        }

        /// <summary>
        /// 局面と差し手をファイルに保存します。
        /// </summary>
        public void Save(TextWriter writer, KifuObject kifu)
        {
            WriteHeader(writer, kifu);
            WriteMoveNode(writer, kifu.RootNode, false);
        }
    }
}
