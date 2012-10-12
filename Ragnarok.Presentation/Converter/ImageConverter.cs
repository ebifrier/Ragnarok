using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Globalization;

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
        /// URLから画像を読み込みます。
        /// </summary>
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            // URLを取り出します。
            var uri = value as Uri;
            if (uri == null)
            {
                var str = value as string;
                if (str == null)
                {
                    return (parameter as BitmapSource);
                }

                uri = new Uri(str, UriKind.RelativeOrAbsolute);
            }

            try
            {
                if (!uri.IsAbsoluteUri || uri.IsLoopback || uri.Scheme == "pack")
                {
                    // ローカル環境ならそのまま読み込めます。
                    return new BitmapImage(uri);
                }
                else if (uri.Scheme == "http")
                {
                    // ネットワーク先にあるときは自前で読み込む必要があります。
                    var data = WebUtil.RequestHttp(uri.ToString(), null);
                    var image = new BitmapImage();

                    image.BeginInit();
                    image.StreamSource = new MemoryStream(data);
                    image.EndInit();
                    return image;
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
