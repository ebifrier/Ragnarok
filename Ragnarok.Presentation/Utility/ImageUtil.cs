using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ragnarok.Presentation.Utility
{
    /// <summary>
    /// 画像処理に関する機能を提供します。
    /// </summary>
    public static class ImageUtil
    {
        /// <summary>
        /// 画像ファイルを読み込みます。
        /// </summary>
        public static BitmapSource LoadImage(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                var enc = BitmapDecoder.Create(
                    stream,
                    BitmapCreateOptions.None,
                    BitmapCacheOption.Default);

                return enc.Frames[0];
            }
        }

        /// <summary>
        /// 画像ファイルを保存します。
        /// </summary>
        public static void SaveImage(string filename, BitmapSource source)
        {
            var enc = new PngBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(source));

            using (var stream = new FileStream(filename, FileMode.Create))
            {
                enc.Save(stream);
            }
        }
    }
}
