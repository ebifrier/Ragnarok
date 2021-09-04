using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Ragnarok.Presentation.Control.ColorPicker
{
    /// <summary>
    /// 色関連のユーティリティクラスです。
    /// </summary>
    internal static class ColorUtils
    {
        /// <summary>
        /// 色変更を通知するイベントを発行します。
        /// </summary>
        public static void FireSelectedColorChangedEvent(UIElement issuer,
                                                         RoutedEvent routedEvent,
                                                         Color oldColor,
                                                         Color newColor)
        {
            var e = new RoutedPropertyChangedEventArgs<Color>(oldColor, newColor)
            {
                RoutedEvent = routedEvent,
            };

            issuer.RaiseEvent(e);
        }

        /// <summary>
        /// Colorオブジェクトを作成します。
        /// </summary>
        private static Color BuildColor(double red, double green, double blue,
                                        double m)
        {
            return Color.FromArgb(
                255, 
                (byte)((red + m) * 255 + 0.5), 
                (byte)((green + m) * 255 + 0.5), 
                (byte)((blue + m) * 255 + 0.5));
        }

        /// <summary>
        /// RGBをHSVに変換します。
        /// </summary>
        public static void ConvertRgbToHsv(Color color, out double hue,
                                           out double saturation,
                                           out double value)
        {
            double red   = color.R / 255.0;
            double green = color.G / 255.0;
            double blue  = color.B / 255.0;
            double min = Math.Min(red, Math.Min(green, blue));
            double max = Math.Max(red, Math.Max(green, blue));
            double delta = max - min;

            value = max;
            saturation = (max == 0 ? 0 : delta / max);

            // Hueを計算します。
            if (saturation == 0)
            {
                hue = 0;
            }
            else
            {
                if (red == max)
                {
                    hue = (green - blue) / delta;
                }
                else if (green == max)
                {
                    hue = 2 + (blue - red) / delta;
                }
                else // blue == max
                {
                    hue = 4 + (red - green) / delta;
                }
            }

            hue *= 60;
            if (hue < 0)
            {
                hue += 360;
            }
        }

        /// <summary>
        /// HSVをRGBに変換します。
        /// </summary>
        public static Color ConvertHsvToRgb(double hue, double saturation, double value)
        {
            double chroma = value * saturation;

            if (hue >= 360)
            {
                hue -= 360;
            }

            double hueTag = hue / 60;
            double x = chroma * (1 - Math.Abs(hueTag % 2 - 1));
            double m = value - chroma;
            switch ((int)hueTag)
            {
                case 0:
                    return BuildColor(chroma, x, 0, m);
                case 1:
                    return BuildColor(x, chroma, 0, m);
                case 2:
                    return BuildColor(0, chroma, x, m);
                case 3:
                    return BuildColor(0, x, chroma, m);
                case 4:
                    return BuildColor(x, 0, chroma, m);
                default:
                    return BuildColor(chroma, 0, x, m);
            }
        }

        /// <summary>
        /// スペクトルの色をまとめて取得します。
        /// </summary>
        public static Color[] GetSpectrumColors(int colorCount)
        {
            var spectrumColors = new Color[colorCount];

            for (int i = 0; i < colorCount; ++i)
            {
                double hue = (i * 360.0) / colorCount;

                spectrumColors[i] = ConvertHsvToRgb(
                    hue,
                    1.0,   /* saturation */
                    1.0);  /* value */
            }

            return spectrumColors;
        }
    }
}
