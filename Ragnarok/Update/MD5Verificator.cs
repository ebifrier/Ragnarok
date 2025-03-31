using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Ragnarok.Update
{
    /// <summary>
    /// MD5によるバイナリ認証を行います。
    /// </summary>
    public static class MD5Verificator
    {
        /// <summary>
        /// MD5を計算します。
        /// </summary>
        public static string ComputeMD5(string filename)
        {
            using (var stream = File.OpenRead(filename))
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(stream);

                var sb = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                {
                    sb.Append(result[i].ToString("x2", CultureInfo.InvariantCulture));
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// MD5を文字列として比較します。
        /// </summary>
        public static bool CompareMD5(string x, string y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if ((object)x == null || (object)y == null || x.Length != y.Length)
            {
                return false;
            }

            // 大文字・小文字は区別しない。
            return (string.Compare(x, 0, y, 0, x.Length, true, CultureInfo.InvariantCulture) == 0);
        }
    }
}
