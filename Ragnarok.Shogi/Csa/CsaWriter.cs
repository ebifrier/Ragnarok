﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Ragnarok.Shogi.Csa
{
    using File;

    /// <summary>
    /// CSAファイルの書き出しを行います。
    /// </summary>
    internal sealed class CsaWriter : IKifuWriter
    {
        /// <summary>
        /// ヘッダ部分を出力します。
        /// </summary>
        private static void WriteHeader(TextWriter writer, KifuObject kifu)
        {
            writer.WriteLine("' ----  Ragnarok 棋譜ファイル  ----");
            writer.WriteLine("V2.2");

            // 対局者名は別腹で行きます。
            var value = kifu.Header[KifuHeaderType.BlackName];
            if (value != null)
            {
                writer.WriteLine("N+{0}", value);
            }

            value = kifu.Header[KifuHeaderType.WhiteName];
            if (value != null)
            {
                writer.WriteLine("N-{0}", value);
            }

            // 他のヘッダアイテムを書き出します。
            foreach (var item in kifu.Header)
            {
                var name = CsaUtil.GetHeaderName(item.Key);
                if (name == null)
                {
                    continue;
                }

                writer.WriteLine("${0}:{1}", name, item.Value);
            }
        }

        /// <summary>
        /// 局面を出力します。
        /// </summary>
        private static void WriteBoard(TextWriter writer, Board board)
        {
            writer.WriteLine(board.ToCsa());
        }

        /// <summary>
        /// CSA形式の指し手を出力します。
        /// </summary>
        private static void WriteMoveNode(TextWriter writer, MoveNode node)
        {
            // 各指し手行を出力します。
            for (node = node.NextNode; node != null; node = node.NextNode)
            {
                writer.WriteLine(node.Move.ToCsa());
                writer.WriteLine("T" + node.DurationSeconds);
            }
        }

        /// <summary>
        /// 局面と差し手をファイルに保存します。
        /// </summary>
        public void Save(TextWriter writer, KifuObject kifu)
        {
            WriteHeader(writer, kifu);
            WriteBoard(writer, kifu.StartBoard);
            WriteMoveNode(writer, kifu.RootNode);
        }
    }
}
