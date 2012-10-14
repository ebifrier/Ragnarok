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
        /// 指し手を出力します。
        /// </summary>
        private string MakeMoveLine(Move move, int number, bool hasVariation)
        {
            // kif形式では、不成りは何も書きません。
            if (move.ActionType == ActionType.Unpromote)
            {
                move = move.Clone();
                move.ActionType = ActionType.None;
            }

            // 半角文字相当の文字数で空白を計算します。
            var moveText = Stringizer.ToString(move, MoveTextStyle.KifFile);
            var hanLen = moveText.HankakuLength();

            return string.Format("{0,4} {1}{2} ( 0:00/00:00:00){3}",
                number,
                moveText,
                new string(' ', Math.Max(0, 14 - hanLen)),
                (hasVariation ? "+" : ""));
        }

        /// <summary>
        /// 変化の分岐を含めて出力します。
        /// </summary>
        private void WriteMoveNode(TextWriter writer, VariationNode node)
        {
            if (node == null || node.Move == null)
            {
                return;
            }

            // とりあえず指し手を書きます。
            var moveStr = MakeMoveLine(
                node.Move,
                node.MoveCount,
                (node.NextVariation != null));
            writer.WriteLine(moveStr);

            // 次の指し手があればそれも出力します。
            if (node.NextChild != null)
            {
                WriteMoveNode(writer, node.NextChild);
            }

            var child = node.NextVariation;
            if (child != null)
            {
                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine("変化：{0}手", child.MoveCount);

                WriteMoveNode(writer, child);
            }
        }

        /// <summary>
        /// 局面と差し手をファイルに保存します。
        /// </summary>
        public void Save(TextWriter writer, KifuObject kifu)
        {
            WriteHeader(writer, kifu);
            WriteMoveNode(writer, kifu.RootNode);
        }
    }
}
