using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using FlintSharp;
using FlintSharp.Particles;

namespace Ragnarok.Presentation.Extra.FlintSharpEx
{
    /// <summary>
    /// WPF用のイメージデータを保持します。
    /// </summary>
    internal sealed class WPFImageData : IImageData
    {
        /// <summary>
        /// オブジェクトの破棄時に呼ばれます。
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// 描画用のモデルを取得または設定します。
        /// </summary>
        public GeometryModel3D Model
        {
            get;
            set;
        }

        /// <summary>
        /// マテリアルを取得または設定します。
        /// </summary>
        public Material Material
        {
            get;
            set;
        }

        /// <summary>
        /// 描画用のブラシを取得または設定します。
        /// </summary>
        public Brush Brush
        {
            get;
            set;
        }
    }
}
