using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ragnarok.Utility
{
    /// <summary>
    /// <see cref="MemoryStream"/>を拡張します。
    /// </summary>
    public static class MemoryStreamExtensions
    {
        /// <summary>
        /// 読み込みようとして使っている<see cref="MemoryStream"/>
        /// の最後尾にデータを書き込みます。
        /// </summary>
        /// <remarks>
        /// <see cref="MemoryStream"/>はPositionを書き込み位置と読み込み位置の
        /// 両方をを示す値として使っているので、読み込みながら書き込みをする場合は
        /// 少し注意がいります。
        /// </remarks>
        public static void WriteToReadStream(this MemoryStream stream, byte[] buffer,
                                             int offset, int count)
        {
            var oldPosition = -1L;

            try
            {
                // 現在の読み込み位置を保存してから、データを書き込みます。
                oldPosition = stream.Position;

                // データを最後尾に書き込みます。
                stream.Seek(0, SeekOrigin.End);
                stream.Write(buffer, 0, count);
            }
            finally
            {
                if (oldPosition >= 0)
                {
                    stream.Position = oldPosition;
                }
            }
        }        
    }
}
