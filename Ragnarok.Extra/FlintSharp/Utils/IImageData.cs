using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlintSharp
{
    /// <summary>
    /// イメージ情報を保持します。
    /// </summary>
    public interface IImageData : IDisposable
    {
        /// <summary>
        /// オブジェクトの破棄時に呼ばれます。
        /// </summary>
        void Dispose();
    }
}
