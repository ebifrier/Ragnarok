using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlintSharp
{
    /// <summary>
    /// flintsharp用のイメージデータを読み込むためのインターフェースです。
    /// </summary>
    public interface IImageLoader
    {
        /// <summary>
        /// 通常のイメージを読み込みます。
        /// </summary>
        IImageData LoadImage(Uri imageUri, MaterialType materialType);

        /// <summary>
        /// パーティクル用のイメージを読み込みます。
        /// </summary>
        IImageData LoadParticleImage(bool isSingle, MaterialType materialType);
    }
}
