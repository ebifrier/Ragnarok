using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace Ragnarok.Forms.Draw
{
    /// <summary>
    /// Bitmapクラス用の拡張メソッドを実装します。
    /// </summary>
    public static class BitmapExtension
    {
        /// <summary>
        /// 画像を綺麗に拡大縮小します。
        /// </summary>
        public static Bitmap ResizeHighQuality(this Bitmap bitmap,
                                               int width, int height)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException("bitmap");
            }

            var target = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(target))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.DrawImage(bitmap, 0, 0, width, height); 
            }

            return target;
        }
    }
}
