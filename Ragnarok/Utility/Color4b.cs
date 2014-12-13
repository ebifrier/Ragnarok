using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime;
using System.Windows;

namespace Ragnarok.Utility
{
    /// <summary>
    /// α、赤、青、緑の要素を持った色を管理します。
    /// </summary>
    [TypeConverter(typeof(Color4bConverter))]
    public struct Color4b : IEquatable<Color4b>
    {
        /// <summary>
        /// 色のARGB成分から新たな色を作成します。
        /// </summary>
        public static Color4b FromArgb(int a, int r, int g, int b)
        {
            return new Color4b
            {
                A = (byte)a,
                R = (byte)r,
                G = (byte)g,
                B = (byte)b,
            };
        }

        /// <summary>
        /// 元となる色と新たなα成分から、新たな色を作成します。
        /// </summary>
        public static Color4b FromArgb(int a, Color4b protoColor)
        {
            return new Color4b
            {
                A = (byte)a,
                R = protoColor.R,
                G = protoColor.G,
                B = protoColor.B,
            };
        }

        /// <summary>
        /// 色のRGB成分から新たな色を作成します。
        /// </summary>
        public static Color4b FromRgb(int r, int g, int b)
        {
            return new Color4b
            {
                A = 255,
                R = (byte)r,
                G = (byte)g,
                B = (byte)b,
            };
        }

        /// <summary>
        /// 色成分が上位ビットからARGB順に並んでいる値から色を作成します。
        /// </summary>
        [CLSCompliant(false)]
        public static Color4b FromValue(uint value)
        {
            return Color4b.FromArgb(
                (int)((value >> 24) & 0xff),
                (int)((value >> 16) & 0xff),
                (int)((value >> 8) & 0xff),
                (int)((value >> 0) & 0xff));
        }

        /// <summary>
        /// アルファ成分を取得または設定します。
        /// </summary>
        public byte A
        {
            get;
            set;
        }

        /// <summary>
        /// 色の赤成分を取得または設定します。
        /// </summary>
        public byte R
        {
            get;
            set;
        }

        /// <summary>
        /// 色の緑成分を取得または設定します。
        /// </summary>
        public byte G
        {
            get;
            set;
        }

        /// <summary>
        /// 色の青成分を取得または設定します。
        /// </summary>
        public byte B
        {
            get;
            set;
        }

        /// <summary>
        /// ARGBの成分が含まれたuint型として値を取得します。
        /// </summary>
        [CLSCompliant(false)]
        public uint Value
        {
            get
            {
                return (
                    ((uint)A << 24) | ((uint)R << 16) |
                    ((uint)G <<  8) | ((uint)B <<  0));
            }
        }

        /// <summary>
        /// 色の ScRGB アルファ チャネルの値を取得または設定します。
        /// </summary>
        public float ScA
        {
            get { return ((float)A / 255.0f);}
            set { A = (byte)MathEx.Between(0, 255, (int)(value * 255.0f)); }
        }

        /// <summary>
        /// 色の ScRGB 赤チャネルの値を取得または設定します。
        /// </summary>
        public float ScR
        {
            get { return ((float)R / 255.0f); }
            set { R = (byte)MathEx.Between(0, 255, (int)(value * 255.0f)); }
        }
        
        /// <summary>
        /// 色の ScRGB 緑チャネルの値を取得または設定します。
        /// </summary>
        public float ScG
        {
            get { return ((float)G / 255.0f); }
            set { G = (byte)MathEx.Between(0, 255, (int)(value * 255.0f)); }
        }

        /// <summary>
        /// 色の ScRGB 青チャネルの値を取得または設定します。
        /// </summary>
        public float ScB
        {
            get { return ((float)B / 255.0f); }
            set { B = (byte)MathEx.Between(0, 255, (int)(value * 255.0f)); }
        }

        /// <summary>
        /// 色同士が近い色かどうかを取得します。
        /// </summary>
        public static bool AreClose(Color4b color1, Color4b color2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 色同士を加算を行い、新たな色を作成します。
        /// </summary>
        public static Color4b Add(Color4b color1, Color4b color2)
        {
            return Color4b.FromArgb(
                Math.Min((int)color1.A + color2.A, 255),
                Math.Min((int)color1.R + color2.R, 255),
                Math.Min((int)color1.G + color2.G, 255),
                Math.Min((int)color1.B + color2.B, 255));
        }       

        /// <summary>
        /// 色同士の減算を行い、新たな色を作成します。
        /// </summary>
        public static Color4b Subtract(Color4b color1, Color4b color2)
        {
            return Color4b.FromArgb(
                Math.Max((int)color1.A - color2.A, 0),
                Math.Max((int)color1.R - color2.R, 0),
                Math.Max((int)color1.G - color2.G, 0),
                Math.Max((int)color1.B - color2.B, 0));
        }

        /// <summary>
        /// 色に係数を掛け、新たな色を作成します。
        /// </summary>
        public static Color4b Multiply(Color4b Color4b, float coefficient)
        {
            return Color4b.FromArgb(
                MathEx.Between(0, 255, (int)(Color4b.A * coefficient)),
                MathEx.Between(0, 255, (int)(Color4b.R * coefficient)),
                MathEx.Between(0, 255, (int)(Color4b.G * coefficient)),
                MathEx.Between(0, 255, (int)(Color4b.B * coefficient)));
        }

        /// <summary>
        /// 色同士の加算を行い、新たな色を作成します。
        /// </summary>
        public static Color4b operator +(Color4b color1, Color4b color2)
        {
            return Color4b.Add(color1, color2);
        }

        /// <summary>
        /// 色同士の減算を行い、新たな色を作成します。
        /// </summary>
        public static Color4b operator -(Color4b color1, Color4b color2)
        {
            return Color4b.Subtract(color1, color2);
        }

        /// <summary>
        /// 色に係数を掛け、新たな色を作成します。
        /// </summary>
        public static Color4b operator *(Color4b Color4b, float coefficient)
        {
            return Color4b.Multiply(Color4b, coefficient);
        }

        /// <summary>
        /// 色同士が同じ色を示しているかどうかを調べます。
        /// </summary>
        public override bool Equals(object obj)
        {
            var result = this.PreEquals(obj);
            if (result != null)
            {
                return result.Value;
            }

            return Equals((Color4b)obj);
        }

        /// <summary>
        /// 色同士が同じ色を示しているかどうかを調べます。
        /// </summary>
        public bool Equals(Color4b Color4b)
        {
            return (
                A == Color4b.A &&
                R == Color4b.R &&
                G == Color4b.G &&
                B == Color4b.B);
        }

        /// <summary>
        /// 同じ色かどうかを調べます。
        /// </summary>
        public static bool operator ==(Color4b color1, Color4b color2)
        {
            return Util.GenericEquals(color1, color2);
        }

        /// <summary>
        /// 同じでない色かどうかを調べます。
        /// </summary>
        public static bool operator !=(Color4b color1, Color4b color2)
        {
            return !(color1 == color2);
        }

        /// <summary>
        /// ハッシュ値を計算します。
        /// </summary>
        public override int GetHashCode()
        {
            return (
                A.GetHashCode() ^
                R.GetHashCode() ^
                G.GetHashCode() ^
                B.GetHashCode());
        }

        /// <summary>
        /// 文字列をColor4b型に変換します。
        /// </summary>
        public static Color4b Parse(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException("source");
            }

            var trimmedSource = source.Trim();
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException("source");
            }

            if (trimmedSource[0] == '#')
            {
                var substr = trimmedSource.Substring(1);
                var value = uint.Parse(substr, NumberStyles.AllowHexSpecifier);

                switch (substr.Length)
                {
                    case 8:
                        // ARGB : 4*2byte
                        return Color4b.FromValue(value);
                    case 6:
                        // RGB : 3*2byte
                        return Color4b.FromValue(((uint)0xff << 24) + value);
                    case 4:
                        // ARGB : 4*1byte
                        return Color4b.FromArgb(
                            ((int)(value >> 12) & 0xf) * 0x11,
                            ((int)(value >> 8) & 0xf) * 0x11,
                            ((int)(value >> 4) & 0xf) * 0x11,
                            ((int)(value >> 0) & 0xf) * 0x11);
                    case 3:
                        // RGB : 3*1byte
                        return Color4b.FromArgb(
                            255,
                            ((int)(value >> 8) & 0xf) * 0x11,
                            ((int)(value >> 4) & 0xf) * 0x11,
                            ((int)(value >> 0) & 0xf) * 0x11);
                }
            }
            else
            {
                // 組み込み色
                Color4b color;
                if (Color4bs.RegisteredColorsDic.TryGetValue(
                    trimmedSource, out color))
                {
                    return color;
                }
            }

            throw new FormatException(
                source + ": 色の指定が正しくありません。");
        }

        /// <summary>
        /// 文字列化します。
        /// </summary>
        public override string ToString()
        {
            // 色が組み込み色と一致する場合は、その名前を返します。
            foreach (var pair in Color4bs.RegisteredColorsDic)
            {
                if (this.Equals(pair.Value))
                {
                    return pair.Key;
                }
            }

            // ARGB形式の出力
            return string.Format(
                "#{0:X2}{1:X2}{2:X2}{3:X3}",
                A, R, G, B);
        }
    }
}
