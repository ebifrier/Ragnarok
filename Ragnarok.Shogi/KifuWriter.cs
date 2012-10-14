using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi
{
    using File;

    /// <summary>
    /// 棋譜の書き込みを行います。
    /// </summary>
    public static class KifuWriter
    {
        /// <summary>
        /// sjisが基本。
        /// </summary>
        private static Encoding DefaultEncoding =
            Encoding.GetEncoding("Shift_JIS");

        /// <summary>
        /// 棋譜ファイルに保存します。
        /// </summary>
        public static void SaveFile(string filepath, KifuObject kifuObj)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new ArgumentNullException("filepath");
            }

            using (var stream = new FileStream(filepath, FileMode.Create))
            using (var writer = new StreamWriter(stream, DefaultEncoding))
            {
                Save(writer, kifuObj);
            }
        }

        /// <summary>
        /// 棋譜を文字列として書き込みます。
        /// </summary>
        public static string SaveTo(KifuObject kifuObj)
        {
            using (var writer = new StringWriter())
            {
                Save(writer, kifuObj);

                return writer.ToString();
            }
        }

        /// <summary>
        /// 棋譜を指定の出力先に書き込みます。
        /// </summary>
        public static void Save(TextWriter writer, KifuObject kifuObj)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (kifuObj == null)
            {
                throw new ArgumentNullException("kifuObj");
            }

            var kifuWriters = new IKifuWriter[]
            {
                new KifWriter(),
            };

            // すべての形式のファイルを読み込んでみます。
            foreach (var kifuWriter in kifuWriters)
            {
                try
                {
                    kifuWriter.Save(writer, kifuObj);
                    return;
                }
                catch
                {
                    // 違う形式のファイルだった場合
                }
            }

            throw new ArgumentException(
                "棋譜の書き込みに失敗しました。");
        }
    }
}
