using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;

namespace Ragnarok.Forms.Shogi.GL
{
    public sealed class TextTextureFont : IEquatable<TextTextureFont>
    {
        private Font font = new Font(FontFamily.GenericSansSerif, 40);
        private Color color = Color.White;
        private Color edgeColor = Color.Black;
        private double edgeLength = 1.0;

        /// <summary>
        /// 描画するフォントを取得または設定します。
        /// </summary>
        public Font Font
        {
            get { return this.font; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.font = value;
            }
        }

        /// <summary>
        /// 文字列の色を取得または設定します。
        /// </summary>
        public Color Color
        {
            get { return this.color; }
            set { this.color = value; }
        }

        /// <summary>
        /// 縁取りの色を取得または設定します。
        /// </summary>
        public Color EdgeColor
        {
            get { return this.edgeColor; }
            set { this.edgeColor = value; }
        }

        /// <summary>
        /// 縁取りの色を取得または設定します。
        /// </summary>
        public double EdgeLength
        {
            get { return this.edgeLength; }
            set { this.edgeLength = value; }
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
                Font == other.Font &&
                Color == other.Color &&
                EdgeColor == other.EdgeColor &&
                EdgeLength == other.EdgeLength);
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
                Font.GetHashCode() ^
                Color.GetHashCode() ^
                EdgeColor.GetHashCode() ^
                EdgeLength.GetHashCode());
        }
    }
}
