using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

using Ragnarok.Net;

namespace Ragnarok.Presentation.Converter
{
    /// <summary>
    /// 画像のURIからイメージに変換します。
    /// </summary>
    [ValueConversion(typeof(string), typeof(BitmapSource))]
    [ValueConversion(typeof(Uri), typeof(BitmapSource))]
    public class ImageConverter : IValueConverter
    {
        /// <summary>
        /// デフォルトのコンバーターを取得します。
        /// </summary>
        public static readonly ImageConverter Default = new();

        /// <summary>
        /// デフォルトのキャッシュオプションを取得または設定します。
        /// </summary>
        public BitmapCacheOption CacheOption = BitmapCacheOption.OnLoad;

        /// <summary>
        /// URLから画像を読み込みます。
        /// </summary>
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            // URLを取り出します。
            var uri = value as Uri;
            if (uri == null)
            {
                if (value is not string str)
                {
                    return parameter as BitmapSource;
                }

                uri = new Uri(str, UriKind.RelativeOrAbsolute);
            }

            try
            {
                if (!uri.IsAbsoluteUri || uri.IsLoopback ||
                    uri.Scheme == "pack" || uri.Scheme == "file")
                {
                    // ローカル環境ならそのまま読み込めます。
                    var bitmap = new BitmapImage();

                    bitmap.BeginInit();
                    bitmap.UriSource = uri;
                    bitmap.CacheOption = CacheOption;
                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
                else if (uri.Scheme == "http")
                {
                    // ネットワーク先にあるときは自前で読み込む必要があります。
                    var data = WebUtil.RequestHttp(uri.ToString(), null);
                    var bitmap = new BitmapImage();

                    bitmap.BeginInit();
                    bitmap.StreamSource = new MemoryStream(data);
                    bitmap.CacheOption = CacheOption;
                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
                else
                {
                    return parameter as BitmapSource;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "ImageConverter: '{0}'の画像読み込みに失敗しました。",
                    uri);

                return parameter as BitmapSource;
            }
        }

        /// <summary>
        /// 実装されていません。
        /// </summary>
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
