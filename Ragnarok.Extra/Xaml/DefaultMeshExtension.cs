using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace Ragnarok.Extra.Xaml
{
    using Effect;

    /// <summary>
    /// デフォルトのメッシュを作成する拡張構文です。
    /// </summary>
    [MarkupExtensionReturnType(typeof(Mesh))]
    public sealed class DefaultMeshExtension : MarkupExtension
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DefaultMeshExtension()
        {
            Width = 1.0;
            Height = 1.0;
            ImageWidth = 0.0;
            ImageHeight = 0.0;
        }

        /// <summary>
        /// メッシュの幅を取得または設定します。
        /// </summary>
        public double Width
        {
            get;
            set;
        }

        /// <summary>
        /// メッシュの高さを取得または設定します。
        /// </summary>
        public double Height
        {
            get;
            set;
        }

        /// <summary>
        /// サブピクセルを調整するためのイメージの幅を取得または設定します。
        /// </summary>
        public double ImageWidth
        {
            get;
            set;
        }

        /// <summary>
        /// サブピクセルを調整するためのイメージの高さを取得または設定します。
        /// </summary>
        public double ImageHeight
        {
            get;
            set;
        }

        /// <summary>
        /// デフォルトのメッシュを作成します。
        /// </summary>
        public override object ProvideValue(IServiceProvider service)
        {
            return Mesh.CreateDefault(
                Width, Height, ImageWidth, ImageHeight);
        }
    }
}
