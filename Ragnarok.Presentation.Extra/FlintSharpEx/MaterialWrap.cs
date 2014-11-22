using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using FlintSharp;

namespace Ragnarok.Presentation.Extra.FlintSharpEx
{
    /// <summary>
    /// The wrap class of DiffuseMaterial and EmissiveMaterial.
    /// </summary>
    internal sealed class MaterialWrap
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MaterialWrap()
        {
            MaterialType = MaterialType.Emissive;
        }

        /// <summary>
        /// The material type.
        /// </summary>
        public MaterialType MaterialType
        {
            get;
            set;
        }

        /// <summary>
        /// The brush of the material object.
        /// </summary>
        public Brush Brush
        {
            get;
            set;
        }

        /// <summary>
        /// The color of the material object.
        /// </summary>
        public Color Color
        {
            get;
            set;
        }

        /// <summary>
        /// Create the material of WPF.
        /// </summary>
        public Material Create()
        {
            if (MaterialType == MaterialType.Diffuse)
            {
                return new DiffuseMaterial
                {
                    Brush = Brush,
                    Color = Color,
                };
            }
            else if (MaterialType == MaterialType.Emissive)
            {
                return new EmissiveMaterial
                {
                    Brush = Brush,
                    Color = Color,
                };
            }

            throw new InvalidOperationException(
                "Invalid material type.");
        }
    }
}
