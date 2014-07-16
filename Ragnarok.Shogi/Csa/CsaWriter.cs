using System;
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
        private void WriteHeader(TextWriter writer, KifuObject kifu)
        {
            writer.WriteLine("' ----  Ragnarok 棋譜ファイル  ----");

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

                writer.WriteLine("{0}:{1}", name, item.Value);
            }
        }

        /// <summary>
        /// 局面を出力します。
        /// </summary>
        private void WriteBoard(TextWriter writer, Board board)
        {
            writer.WriteLine(CsaBoard.ToCsa(board));
        }

        /// <summary>
        /// CSA形式の指し手を出力します。
        /// </summary>
        private void WriteMoveNode(TextWriter writer, MoveNode node)
        {
            var csaList = KifuObject.Convert2List(node)
                .Select(_ => _.ToCsa());

            // 各指し手行を出力します。
            foreach (var csa in csaList)
            {
                writer.WriteLine(csa);
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
