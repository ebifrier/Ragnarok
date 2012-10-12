using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Media3D;

namespace Ragnarok.Shogi.ViewModel
{
    /// <summary>
    /// メッシュを作成します。
    /// </summary>
    [MarkupExtensionReturnType(typeof(MeshGeometry3D))]
    public class MakeMeshExtension : MarkupExtension
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MakeMeshExtension()
        {
            Width = 1.0;
            Height = 1.0;
        }

        /// <summary>
        /// メッシュの幅を取得または設定します。
        /// </summary>
        [DefaultValue(1.0)]
        public double Width
        {
            get;
            set;
        }

        /// <summary>
        /// メッシュの高さを取得または設定します。
        /// </summary>
        [DefaultValue(1.0)]
        public double Height
        {
            get;
            set;
        }

        /// <summary>
        /// バインディングした値を返します。
        /// </summary>
        public override object ProvideValue(IServiceProvider service)
        {
            return Util3D.CreateDefaultMesh(Width, Height, 0, 0);
        }
    }
}
