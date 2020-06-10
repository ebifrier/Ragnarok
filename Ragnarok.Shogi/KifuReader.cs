using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Hnx8.ReadJEnc;

namespace Ragnarok.Shogi
{
    using File;
    using Net;

    /// <summary>
    /// 棋譜の読み込みを行います。
    /// </summary>
    public static class KifuReader
    {
        /// <summary>
        /// 棋譜をファイルから読み込みます。
        /// </summary>
        public static KifuObject Load(Uri url, Encoding encoding = null)
        {
            var GetBytes = new Func<byte[]>(() =>
            {
                switch (url.Scheme)
                {
                    case "file":
                        return System.IO.File.ReadAllBytes(url.LocalPath);
                    case "http":
                    case "https":
                        return WebUtil.RequestHttp(url.ToString(), null);
                    default:
                        throw new ArgumentException($"{url}: 不明なスキームです。");
                }
            });

            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            return LoadFrom(GetBytes(), encoding);
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
