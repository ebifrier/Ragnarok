using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi
{
    using File;

    /// <summary>
    /// 記譜の読み込みを行います。
    /// </summary>
    public static class KifuReader
    {
        /// <summary>
        /// sjisが基本。
        /// </summary>
        private static Encoding DefaultEncoding =
            Encoding.GetEncoding("Shift_JIS");

        /// <summary>
        /// ファイル名から棋譜ファイルを読み込みます。
        /// </summary>
        public static KifuObject LoadFile(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new ArgumentNullException("filepath");
            }

            using (var stream = new FileStream(filepath, FileMode.Open))
            {
                var text = Util.ReadToEnd(stream, DefaultEncoding);

                return LoadFrom(text);
            }
        }

        /// <summary>
        /// 記譜ファイルの読み込みます。
        /// </summary>
        public static KifuObject LoadFrom(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }

            var readers = new IKifuReader[]
            {
                new KifReader(),
                new Ki2Reader(),
            };

            // すべての形式のファイルを読み込んでみます。
            foreach (var reader in readers)
            {
                try
                {
                    var obj = reader.LoadFrom(text);
                    if (obj != null)
                    {
                        return obj;
                    }
                }
                catch
                {
                    // 違う形式のファイルだった場合
                }
            }

            throw new ArgumentException(
                "記譜ファイルの読み込みに失敗しました。");
        }
    }
}
