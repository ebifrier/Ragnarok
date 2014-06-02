using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ragnarok.Presentation.Extra.Effect
{
    /// <summary>
    /// 画像オブジェクトをキャッシュします。
    /// </summary>
    /// <remarks>
    /// エフェクトはアニメーションするため、画像が縦や横に連なって保存されています。
    /// スクリーンより大きい画像はビデオカードによっては表示できないことがあるため、
    /// このクラスでは各画像をクロッピングして管理します。
    /// </remarks>
    public static class EffectImageCache
    {
        private readonly static object SyncRoot = new object();
        private readonly static Dictionary<Uri, List<BitmapSource>> Cache =
            new Dictionary<Uri, List<BitmapSource>>();
        private readonly static Dictionary<Color, BitmapSource> ParticleCache =
            new Dictionary<Color, BitmapSource>();

        /// <summary>
        /// イメージを読み込み、キャッシュにも追加します。
        /// </summary>
        private static List<BitmapSource> LoadImage(Uri imageUri, int count)
        {
            try
            {
                var image = new BitmapImage();

                // まず画像を読み込みます。
                image.BeginInit();
                image.UriSource = imageUri;
                image.EndInit();

                List<BitmapSource> list = null;
                if (count <= 1)
                {
                    list = new List<BitmapSource>() { image };
                }
                else
                {
                    // 次に各画像をクロッピングします。
                    var w = image.PixelWidth / count;
                    var h = image.PixelHeight;

                    list = (from i in Enumerable.Range(0, count)
                            select (BitmapSource)
                                new CroppedBitmap(image, new Int32Rect(w * i, 0, w, h))
                                    .Apply(_ => _.Freeze()))
                           .ToList();
                }

                lock (SyncRoot)
                {
                    Cache[imageUri] = list;
                }

                return list;
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "画像の読み込みに失敗しました。");

                return null;
            }
        }

        /// <summary>
        /// 画像をキャッシュから探し、もしなければ読み込みます。
        /// </summary>
        public static List<BitmapSource> GetImageList(Uri imageUri, int count)
        {
            lock (SyncRoot)
            {
                List<BitmapSource> imageList;
                if (Cache.TryGetValue(imageUri, out imageList))
                {
                    return imageList;
                }

                return LoadImage(imageUri, count);
            }
        }
    }
}
