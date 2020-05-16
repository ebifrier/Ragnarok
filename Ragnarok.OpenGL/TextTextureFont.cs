using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Ragnarok.OpenGL
{
    public sealed class TextTextureFont : IEquatable<TextTextureFont>
    {
        public static Font DefaultFont
        {
            get;
        } = new Font(FontFamily.GenericSansSerif, 40);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TextTextureFont()
        {
            FontFamily = DefaultFont.FontFamily;
            FontStyle = DefaultFont.Style;
            Size = DefaultFont.SizeInPoints;
            Color = Color.White;
            EdgeColor = Color.Black;
            EdgeLength = 1.0;
        }

        /// <summary>
        /// 描画するフォントを取得または設定します。
        /// </summary>
        public Font Font
        {
            get { return new Font(FontFamily, (float)Size, FontStyle); }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                FontFamily = value.FontFamily;
                FontStyle = value.Style;
                Size = value.SizeInPoints;
            }
        }

        /// <summary>
        /// フォントファミリーを取得または設定します。
        /// </summary>
        public FontFamily FontFamily
        {
            get;
            set;
        }

        /// <summary>
        /// フォントファミリー名を取得または設定します。
        /// </summary>
        public string FontFamilyName
        {
            get { return FontFamily.Name; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(FontFamilyName));
                }

                // フォントファミリーを検索し、もしなければ無視します。
                var family = FontFamily.Families
                    .FirstOrDefault(_ => _.Name == value);
                if (family == null)
                {
                    Log.Error($"{value}: フォントファミリーが見つかりません。");
                    return;
                }

                FontFamily = family;
            }
        }

        /// <summary>
        /// フォントスタイルを取得または設定します。
        /// </summary>
        public FontStyle FontStyle
        {
            get;
            set;
        }

        /// <summary>
        /// フォントサイズをemサイズ(Point数)で取得または設定します。
        /// </summary>
        public double Size
        {
            get;
            set;
        }

        /// <summary>
        /// 文字列の色を取得または設定します。
        /// </summary>
        public Color Color
        {
            get;
            set;
        }

        /// <summary>
        /// 縁取りの色を取得または設定します。
        /// </summary>
        public Color EdgeColor
        {
            get;
            set;
        }

        /// <summary>
        /// 縁取りの色を取得または設定します。
        /// </summary>
        public double EdgeLength
        {
            get;
            set;
        }

        /// <summary>
        /// 外側の空白を削除し、ビットマップいっぱいに文字を
        /// 描画するかどうかを取得または設定します。
        /// </summary>
        public bool IsStretchSize
        {
            get;
            set;
        }

        /// <summary>
        /// オブジェクトが同じものか比較します。
        /// </summary>
        public override bool Equals(object obj)
        {
            var result = this.PreEquals(obj);
            if (result != null)
            {
                return result.Value;
            }

            return Equals((TextTextureFont)obj);
        }

        /// <summary>
        /// オブジェクトが同じものか比較します。
        /// </summary>
        public bool Equals(TextTextureFont other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return (
                FontFamilyName == other.FontFamilyName &&
                FontStyle == other.FontStyle &&
                Size == other.Size &&
                Color == other.Color &&
                EdgeColor == other.EdgeColor &&
                EdgeLength == other.EdgeLength &&
                IsStretchSize == other.IsStretchSize);
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public static bool operator==(TextTextureFont x, TextTextureFont y)
        {
            return Util.GenericEquals(x, y);
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public static bool operator!=(TextTextureFont x, TextTextureFont y)
        {
            return !(x == y);
        }

        /// <summary>
        /// ハッシュ値を計算します。
        /// </summary>
        public override int GetHashCode()
        {
            return (
                FontFamilyName.GetHashCode() ^
                FontStyle.GetHashCode() ^
                Size.GetHashCode() ^
                Color.GetHashCode() ^
                EdgeColor.GetHashCode() ^
                EdgeLength.GetHashCode() ^
                IsStretchSize.GetHashCode());
        }
    }
}
