using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlintSharp
{
    public interface IImageLoader
    {
        IImageData LoadImage(Uri imageUri, MaterialType matType);

        IImageData LoadParticleImage(bool isSingle, MaterialType materialType);
    }
}
