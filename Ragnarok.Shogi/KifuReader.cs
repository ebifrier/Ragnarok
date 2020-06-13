using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Hnx8.ReadJEnc;

namespace Ragnarok.Shogi
{
    using File;

    /// <summary>
    /// 棋譜の読み込みを行います。
    /// </summary>
    public static class KifuReader
    {
        /// <summary>
        /// ファイルから棋譜を読み込みます。
        /// </summary>
        public static KifuObject Load(string filepath, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new ArgumentNullException(nameof(filepath));
            }

            var data = System.IO.File.ReadAllBytes(filepath);
            return LoadFrom(data, encoding);
        }

        /// <summary>
        /// エンコーディングを自動で判別し、Streamから棋譜を読み込みます。
        /// </summary>
        public static KifuObject LoadFrom(Stream stream, Encoding encoding = null)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return LoadFrom(memoryStream.ToArray(), encoding);
            }
        }

        /// <summary>
        /// エンコーディングを自動で判別し、棋譜を読み込みます。
        /// </summary>
        public static KifuObject LoadFrom(byte[] data, Encoding encoding = null)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            string text;
            if (encoding != null)
            {
                text = encoding.GetString(data);
            }
            else
            {
                ReadJEnc.JP.GetEncoding(data, data.Length, out text);
            }

            return LoadFrom(text);
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
