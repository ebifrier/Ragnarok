using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi.File
{
    /// <summary>
    /// ファイルフォーマット用の例外クラスです。
    /// </summary>
    public class FileFormatException : ShogiException
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FileFormatException()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FileFormatException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FileFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FileFormatException(int lineNumber, string message)
            : base(message)
        {
            LineNumber = lineNumber;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FileFormatException(int lineNumber, string message,
                                   Exception innerException)
            : base(message, innerException)
        {
            LineNumber = lineNumber;
        }

        /// <summary>
        /// エラーがあった行数を取得します。
        /// </summary>
        public int? LineNumber
        {
            get;
            private set;
        }
    }
}
