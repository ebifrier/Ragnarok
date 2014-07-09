using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi
{
    using File;

    public enum KifuFormat
    {
        Kif,
        Ki2,
        Bod,
    }

    /// <summary>
    /// 棋譜の書き込みを行います。
    /// </summary>
    public static class KifuWriter
    {
        private static IKifuWriter GetWriter(KifuFormat format)
        {
            switch (format)
            {
                case KifuFormat.Kif:
                    return new KifWriter();
                case KifuFormat.Ki2:
                    return new KifWriter();
                case KifuFormat.Bod:
                    return new KifWriter();
            }

            return null;
        }

        /// <summary>
        /// 棋譜を文字列として書き込みます。
        /// </summary>
        public static string WriteTo(KifuObject kifuObj, KifuFormat format)
        {
            using (var writer = new StringWriter())
            {
                Save(writer, kifuObj, format);

                return writer.ToString();
            }
        }

        /// <summary>
        /// 棋譜ファイルに保存します。
        /// </summary>
        public static void SaveFile(string filepath, KifuObject kifuObj,
                                    KifuFormat? format)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new ArgumentNullException("filepath");
            }

            using (var stream = new FileStream(filepath, FileMode.Create))
            using (var writer = new StreamWriter(stream, KifuObject.DefaultEncoding))
            {
                Save(writer, kifuObj, format.Value);
            }
        }

        /// <summary>
        /// 棋譜を指定の出力先に書き込みます。
        /// </summary>
        public static void Save(TextWriter writer, KifuObject kifuObj,
                                KifuFormat format)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (kifuObj == null)
            {
                throw new ArgumentNullException("kifuObj");
            }

            var kifuWriter = GetWriter(format);
            kifuWriter.Save(writer, kifuObj);
        }
    }
}
