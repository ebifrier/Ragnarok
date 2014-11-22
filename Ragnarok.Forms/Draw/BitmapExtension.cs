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
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.DrawImage(bitmap, 0, 0, width, height); 
            }

            return target;
        }

        /// <summary>
        /// 画像の一部を綺麗に切り抜きます。
        /// </summary>
        public static Bitmap CropHighQuality(this Bitmap bitmap, int x, int y,
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
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.DrawImage(bitmap,
                    new Rectangle(0, 0, width, height), // dst
                    new Rectangle(x, y, width, height), // src
                    GraphicsUnit.Pixel);
            }

            return target;
        }
    }
}
