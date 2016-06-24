using System;
using System.Collections.Generic;
using System.Linq;

using FlintSharp;

namespace Ragnarok.Forms.Shogi.FlintSharpEx
{
    /// <summary>
    /// OpenGL用のイメージ読み込みクラスです。
    /// </summary>
    /// <remarks>
    /// OpenGLの場合、テクスチャの読み込みには固有のOpenGLオブジェクトが
    /// 必要になりますが、このクラスではそれを得ることができないので、
    /// 実際にテクスチャ読み込みは他の場所で行うことにします。
    /// </remarks>
    internal sealed class GLImageLoader : IImageLoader
    {
        /// <summary>
        /// 画像データを読み込みます。
        /// </summary>
        public IImageData LoadImage(string imagePath, MaterialType materialType)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                throw new ArgumentException("imagePath");
            }

            return new GLImageData
            {
                ImagePath = imagePath,
                MaterialType = materialType,
            };
        }

        /// <summary>
        /// パーティクルを読み込みます。
        /// </summary>
        public IImageData LoadParticleImage(bool isSingle, MaterialType materialType)
        {
            return new GLImageData
            {
                IsParticle = true,
                IsDoubleParticle = !isSingle,
                MaterialType = materialType,
            };
        }
    }
}
