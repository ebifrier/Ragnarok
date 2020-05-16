using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Ragnarok.Shogi
{
    using File;

    public enum KifuFormat
    {
        Kif,
        Ki2,
        Csa,
    }

    /// <summary>
    /// 棋譜の書き込みを行います。
    /// </summary>
    public static class KifuWriter
    {
        /// <summary>
        /// ファイル名から棋譜のフォーマットを取得します。
        /// </summary>
        public static KifuFormat GetFormat(string filepath)
        {
            // 棋譜フォーマットを取得
            var ext = Path.GetExtension(filepath);
            if (string.IsNullOrEmpty(ext))
            {
                throw new ArgumentNullException(nameof(filepath));
            }

            switch (ext.ToUpperInvariant())
            {
                case ".KIF": return KifuFormat.Kif;
                case ".KI2": return KifuFormat.Ki2;
                case ".CSA": return KifuFormat.Csa;
            }

            // 分からない場合はkif形式に設定する。
            return KifuFormat.Kif;
        }

        /// <summary>
        /// 各フォーマットに対応するライターを取得します。
        /// </summary>
        private static IKifuWriter GetWriter(KifuFormat format)
        {
            switch (format)
            {
                case KifuFormat.Kif:
                    return new Kif.KifWriter(true);
                case KifuFormat.Ki2:
                    return new Kif.KifWriter(false);
                case KifuFormat.Csa:
                    return new Csa.CsaWriter();
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
        /// <remarks>
        /// <paramref name="format"/>がnullの場合は
        /// ファイル名からデフォルトの値に設定します。
        /// </remarks>
        public static void SaveFile(string filepath, KifuObject kifuObj,
                                    KifuFormat? format = null)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new ArgumentNullException(nameof(filepath));
            }

            if (format == null)
            {
                format = GetFormat(filepath);
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
                throw new ArgumentNullException(nameof(writer));
            }

            if (kifuObj == null)
            {
                throw new ArgumentNullException(nameof(kifuObj));
            }

            var kifuWriter = GetWriter(format);
            if (kifuWriter == null)
            {
                throw new ShogiException(
                    format + ": このフォーマットの棋譜出力は未対応です。");
            }

            kifuWriter.Save(writer, kifuObj);
        }
    }
}
