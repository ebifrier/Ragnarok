using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi
{
    using File;

    /// <summary>
    /// 棋譜の読み込みを行います。
    /// </summary>
    public static class KifuReader
    {
        /// <summary>
        /// 棋譜をファイルから読み込みます。
        /// </summary>
        public static KifuObject LoadFile(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new ArgumentNullException(nameof(filepath));
            }

            using (var reader = new StreamReader(filepath, KifuObject.DefaultEncoding))
            {
                return Load(reader);
            }
        }

        /// <summary>
        /// 棋譜を文字列から読み込みます。
        /// </summary>
        public static KifuObject Load(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            return LoadFrom(reader.ReadToEnd());
        }

        /// <summary>
        /// 棋譜の読み込みます。
        /// </summary>
        public static KifuObject LoadFrom(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            var kifuReaders = new IKifuReader[]
            {
                new Kif.KifReader(),
                new Csa.CsaReader(),
            };

            // すべての形式のファイルを読み込んでみます。
            foreach (var kifuReader in kifuReaders)
            {
                // readerは読み込みごとに再作成します。
                using (var reader = new StringReader(text))
                {
                    if (!kifuReader.CanHandle(reader))
                    {
                        continue;
                    }
                }

                using (var reader = new StringReader(text))
                {
                    return kifuReader.Load(reader);
                }
            }

            throw new ShogiException(
                "不明なフォーマットの棋譜です。");
        }
    }
}
