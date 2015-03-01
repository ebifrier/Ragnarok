using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Ragnarok.Forms.Shogi.GLUtil
{
    /// <summary>
    /// OpenGLのユーティリティクラスです。
    /// </summary>
    public static class Misc
    {
        private static string[] extensions;

        static Misc()
        {
            try
            {
                string raw = GL.GetString(StringName.Extensions);
                string[] splitter = new string[] { " " };

                extensions = raw.Split(splitter,
                    StringSplitOptions.RemoveEmptyEntries);
            }
            catch (Exception ex)
            {
                extensions = new string[0];
                Log.ErrorException(ex,
                    "GL.GetStringに失敗しました。");
            }
        }

        /// <summary>
        /// OpenGL拡張のリストを取得します。
        /// </summary>
        public static string[] Extensions
        {
            get { return extensions; }
        }

        /// <summary>
        /// 指定のOpenGL拡張が使用可能か調べます。
        /// </summary>
        public static bool HasExtension(string name)
        {
            return extensions.Any(_ => _ == name);
        }

        private struct TexFormat
        {
            public PixelInternalFormat pif;
            public PixelFormat pf;
            public PixelType pt;

            public TexFormat(PixelInternalFormat _pif, PixelFormat _pf, PixelType _pt)
            {
                pif = _pif;
                pf = _pf;
                pt = _pt;
            }
        }

        private static TexFormat[] TextureFormats = new TexFormat[]
        {        
            new TexFormat( PixelInternalFormat.Alpha, PixelFormat.Alpha, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.Alpha4, PixelFormat.Alpha, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.Alpha8, PixelFormat.Alpha, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.Alpha12, PixelFormat.Alpha, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.Alpha16, PixelFormat.Alpha, PixelType.UnsignedByte),
            new TexFormat( (PixelInternalFormat)All.Alpha16fArb, PixelFormat.Alpha, PixelType.HalfFloat),
            new TexFormat( (PixelInternalFormat)All.Alpha32fArb, PixelFormat.Alpha, PixelType.Float),

            new TexFormat( PixelInternalFormat.DepthComponent, PixelFormat.DepthComponent, PixelType.Int),
            new TexFormat( PixelInternalFormat.DepthComponent16, PixelFormat.DepthComponent, PixelType.Float),
            new TexFormat( PixelInternalFormat.DepthComponent24, PixelFormat.DepthComponent, PixelType.Float),
            new TexFormat( PixelInternalFormat.DepthComponent32, PixelFormat.DepthComponent, PixelType.Float), 
            new TexFormat( PixelInternalFormat.DepthComponent32f, PixelFormat.DepthComponent, PixelType.Float),  
            new TexFormat( PixelInternalFormat.DepthStencil, PixelFormat.DepthStencil, PixelType.UnsignedInt248),
            new TexFormat( PixelInternalFormat.Depth24Stencil8, PixelFormat.DepthStencil, PixelType.UnsignedInt248),
            new TexFormat( PixelInternalFormat.Depth32fStencil8, PixelFormat.DepthStencil, PixelType.Float32UnsignedInt248Rev),

            new TexFormat( PixelInternalFormat.One, PixelFormat.Red, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.Two, PixelFormat.Rg, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte),

            new TexFormat( PixelInternalFormat.Srgb, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.SrgbAlpha, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.Srgb8, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.Srgb8Alpha8, PixelFormat.Rgba, PixelType.UnsignedByte),

            new TexFormat( PixelInternalFormat.R16f, PixelFormat.Red, PixelType.HalfFloat),
            new TexFormat( PixelInternalFormat.Rg16f, PixelFormat.Rg, PixelType.HalfFloat),
            new TexFormat( PixelInternalFormat.Rgb16f, PixelFormat.Rgb, PixelType.HalfFloat),
            new TexFormat( PixelInternalFormat.Rgba16f, PixelFormat.Rgba, PixelType.HalfFloat),
            new TexFormat( PixelInternalFormat.R32f, PixelFormat.Red, PixelType.Float),
            new TexFormat( PixelInternalFormat.Rg32f, PixelFormat.Rg, PixelType.Float),
            new TexFormat( PixelInternalFormat.Rgb32f, PixelFormat.Rgb, PixelType.Float),
            new TexFormat( PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float), 

            new TexFormat( PixelInternalFormat.R8, PixelFormat.Red, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.Rg8, PixelFormat.Rg, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.Rgb8, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.Rgba8, PixelFormat.Rgba, PixelType.UnsignedByte),

            new TexFormat( PixelInternalFormat.R8ui, PixelFormat.Red, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.Rg8ui, PixelFormat.Rg, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.Rgb8ui, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.Rgba8ui, PixelFormat.Rgba, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.R16ui, PixelFormat.Red, PixelType.UnsignedShort),
            new TexFormat( PixelInternalFormat.Rg16ui, PixelFormat.Rg, PixelType.UnsignedShort),
            new TexFormat( PixelInternalFormat.Rgb16ui, PixelFormat.Rgb, PixelType.UnsignedShort),
            new TexFormat( PixelInternalFormat.Rgba16ui, PixelFormat.Rgba, PixelType.UnsignedShort),
            new TexFormat( PixelInternalFormat.R32ui, PixelFormat.Red, PixelType.UnsignedInt),
            new TexFormat( PixelInternalFormat.Rg32ui, PixelFormat.Rg, PixelType.UnsignedInt),
            new TexFormat( PixelInternalFormat.Rgb32ui, PixelFormat.Rgb, PixelType.UnsignedInt),
            new TexFormat( PixelInternalFormat.Rgba32ui, PixelFormat.Rgba, PixelType.UnsignedInt),

            new TexFormat( PixelInternalFormat.R8i, PixelFormat.Red, PixelType.Byte),
            new TexFormat( PixelInternalFormat.Rg8i, PixelFormat.Rg, PixelType.Byte),
            new TexFormat( PixelInternalFormat.Rgb8i, PixelFormat.Rgb, PixelType.Byte),
            new TexFormat( PixelInternalFormat.Rgba8i, PixelFormat.Rgba, PixelType.Byte),
            new TexFormat( PixelInternalFormat.R16i, PixelFormat.Red, PixelType.Short),
            new TexFormat( PixelInternalFormat.Rg16i, PixelFormat.Rg, PixelType.Short),
            new TexFormat( PixelInternalFormat.Rgb16i, PixelFormat.Rgb, PixelType.Short),
            new TexFormat( PixelInternalFormat.Rgba16i, PixelFormat.Rgba, PixelType.Short),
            new TexFormat( PixelInternalFormat.R32i, PixelFormat.Red, PixelType.Int),
            new TexFormat( PixelInternalFormat.Rg32i, PixelFormat.Rg, PixelType.Int),
            new TexFormat( PixelInternalFormat.Rgb32i, PixelFormat.Rgb, PixelType.Int),
            new TexFormat( PixelInternalFormat.Rgba32i, PixelFormat.Rgba, PixelType.Int),

            new TexFormat( PixelInternalFormat.R3G3B2, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.Rgb10A2, PixelFormat.Rgba, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.Rgb5A1, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.Rgb9E5, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.R11fG11fB10f, PixelFormat.Rgb, PixelType.UnsignedByte),

            new TexFormat( PixelInternalFormat.CompressedAlpha, PixelFormat.Alpha, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.CompressedIntensity, PixelFormat.Luminance, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.CompressedLuminance, PixelFormat.Luminance, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.CompressedLuminanceAlpha, PixelFormat.LuminanceAlpha, PixelType.UnsignedByte),
            new TexFormat( (PixelInternalFormat)All.CompressedLuminanceLatc1Ext, PixelFormat.Luminance, PixelType.UnsignedByte),
            new TexFormat( (PixelInternalFormat)All.CompressedLuminanceAlphaLatc2Ext, PixelFormat.LuminanceAlpha, PixelType.UnsignedByte),

            new TexFormat( PixelInternalFormat.CompressedRed, PixelFormat.Red, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.CompressedRedRgtc1, PixelFormat.Red, PixelType.UnsignedByte),
            new TexFormat( (PixelInternalFormat)All.CompressedRedGreenRgtc2Ext, PixelFormat.Rg, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.CompressedRg, PixelFormat.Rg, PixelType.UnsignedByte), 
            new TexFormat( PixelInternalFormat.CompressedRgRgtc2, PixelFormat.Rg, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.CompressedRgb, PixelFormat.Rgb, PixelType.UnsignedByte), 
            new TexFormat( (PixelInternalFormat)All.CompressedRgbFxt13Dfx, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.CompressedRgba, PixelFormat.Rgba, PixelType.UnsignedByte),
            new TexFormat( (PixelInternalFormat)All.CompressedRgbaFxt13Dfx, PixelFormat.Rgba, PixelType.UnsignedByte),

            new TexFormat( (PixelInternalFormat)All.CompressedSignedLuminanceAlphaLatc2Ext, PixelFormat.LuminanceAlpha, PixelType.UnsignedByte),
            new TexFormat( (PixelInternalFormat)All.CompressedSignedLuminanceLatc1Ext, PixelFormat.Luminance, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.CompressedSignedRedRgtc1, PixelFormat.Red, PixelType.UnsignedByte),  
            new TexFormat( PixelInternalFormat.CompressedSignedRgRgtc2, PixelFormat.Rg, PixelType.UnsignedByte),

            new TexFormat( PixelInternalFormat.CompressedSluminance, PixelFormat.Luminance, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.CompressedSluminanceAlpha, PixelFormat.LuminanceAlpha, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.CompressedSrgb, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.CompressedSrgbAlpha, PixelFormat.Rgba, PixelType.UnsignedByte), 
            new TexFormat( PixelInternalFormat.CompressedSrgbS3tcDxt1Ext, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.CompressedSrgbAlphaS3tcDxt1Ext, PixelFormat.Rgba, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.CompressedSrgbAlphaS3tcDxt3Ext, PixelFormat.Rgba, PixelType.UnsignedByte),
            new TexFormat( PixelInternalFormat.CompressedSrgbAlphaS3tcDxt5Ext, PixelFormat.Rgba, PixelType.UnsignedByte),

            new TexFormat( (PixelInternalFormat)All.CompressedRgbS3tcDxt1Ext, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TexFormat( (PixelInternalFormat)All.CompressedRgbaS3tcDxt1Ext, PixelFormat.Rgba, PixelType.UnsignedByte),
            new TexFormat( (PixelInternalFormat)All.CompressedRgbaS3tcDxt3Ext, PixelFormat.Rgba, PixelType.UnsignedByte),
            new TexFormat( (PixelInternalFormat)All.CompressedRgbaS3tcDxt5Ext, PixelFormat.Rgba, PixelType.UnsignedByte),
          
        };

        public enum eType
        {
            Boolean,
            Int,
            IntEnum,
            IntArray2,
            IntArray4,
            Float,
            FloatArray2,
            FloatArray4,
        }

        public static string Analyze(GetPName pname, eType type)
        {
            bool result1b;
            int result1i;
            int[] result2i = new int[2];
            int[] result4i = new int[4];
            float result1f;
            Vector2 result2f;
            Vector4 result4f;
            string output;

            switch (type)
            {
                case eType.Boolean:
                    GL.GetBoolean(pname, out result1b);
                    output = pname + ": " + result1b;
                    break;
                case eType.Int:
                    GL.GetInteger(pname, out result1i);
                    output = pname + ": " + result1i;
                    break;
                case eType.IntEnum:
                    GL.GetInteger(pname, out result1i);
                    output = pname + ": " + (All)result1i;
                    break;
                case eType.IntArray2:
                    GL.GetInteger(pname, result2i);
                    output = pname + ": ( " + result2i[0] + ", " + result2i[1] + " )";
                    break;
                case eType.IntArray4:
                    GL.GetInteger(pname, result4i);
                    output = pname + ": ( " + result4i[0] + ", " + result4i[1] + " ) ( " + result4i[2] + ", " + result4i[3] + " )";
                    break;
                case eType.Float:
                    GL.GetFloat(pname, out result1f);
                    output = pname + ": " + result1f;
                    break;
                case eType.FloatArray2:
                    GL.GetFloat(pname, out result2f);
                    output = pname + ": ( " + result2f.X + ", " + result2f.Y + " )";
                    break;
                case eType.FloatArray4:
                    GL.GetFloat(pname, out result4f);
                    output = pname + ": ( " + result4f.X + ", " + result4f.Y + ", " + result4f.Z + ", " + result4f.W + " )";
                    break;
                default:
                    throw new NotImplementedException();
            }

            ErrorCode err = GL.GetError();
            if (err != ErrorCode.NoError)
            {
                Log.Error("Unsupported Token: " + pname);
            }

            return output;
        }

        /// <summary>
        /// Load resources here.
        /// </summary>
        public static string ShowAnalyze()
        {
            var sb = new StringBuilder();

            // returns 0 formats, driver broken?
            /*
            int CompressedTextureFormatCount;
            GL.GetInteger(GetPName.NumCompressedTextureFormats, out CompressedTextureFormatCount);
            if (CompressedTextureFormatCount > 0)
            {
                int[] CompressedTextureFormats = new int[CompressedTextureFormatCount];
                GL.GetInteger(GetPName.CompressedTextureFormats, CompressedTextureFormats);
                Trace.WriteLine("Supported compressed Texture formats:");
                for (int i = 0; i < CompressedTextureFormats.Length; i++)
                    Trace.Write((All)CompressedTextureFormats[i] + ", ");
            }
             */

            string Renderer = GL.GetString(StringName.Renderer);
            string GLSLang = GL.GetString(StringName.ShadingLanguageVersion);
            string Vendor = GL.GetString(StringName.Vendor);
            string Version = GL.GetString(StringName.Version);

            string ExtensionsRaw = GL.GetString(StringName.Extensions);
            string[] splitter = new string[] { " " };
            string[] Extensions = ExtensionsRaw.Split(splitter, StringSplitOptions.None);

            sb.AppendLine("Vendor: " + Vendor);
            sb.AppendLine("Renderer: " + Renderer);
            sb.AppendLine("GL Version: " + Version);
            sb.AppendLine(Analyze(GetPName.MajorVersion, eType.Int));
            sb.AppendLine(Analyze(GetPName.MinorVersion, eType.Int));
            sb.AppendLine("GLSL Version: " + GLSLang);
            sb.AppendLine("Extensions: ");
            for (int i = 0; i < Extensions.Length; i++)
                sb.AppendLine(Extensions[i]);

            sb.AppendLine("--- Framebuffer ---");
            sb.AppendLine(Analyze(GetPName.Doublebuffer, eType.Boolean));
            sb.AppendLine(Analyze(GetPName.MaxColorAttachments, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxDrawBuffers, eType.Int));
            sb.AppendLine(Analyze(GetPName.AuxBuffers, eType.Int));
            sb.AppendLine(Analyze(GetPName.DrawBuffer, eType.IntEnum));
            sb.AppendLine(Analyze(GetPName.MaxSamples, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxViewportDims, eType.IntArray2));
            sb.AppendLine(Analyze(GetPName.Viewport, eType.IntArray4));

            sb.Append("--- Framebuffer channels ---");
            sb.AppendLine(Analyze(GetPName.RedBits, eType.Int));
            sb.AppendLine(Analyze(GetPName.GreenBits, eType.Int));
            sb.AppendLine(Analyze(GetPName.BlueBits, eType.Int));
            sb.AppendLine(Analyze(GetPName.AlphaBits, eType.Int));
            sb.AppendLine(Analyze(GetPName.DepthBits, eType.Int));
            sb.AppendLine(Analyze(GetPName.StencilBits, eType.Int));

            sb.AppendLine(Analyze(GetPName.AccumRedBits, eType.Int));
            sb.AppendLine(Analyze(GetPName.AccumGreenBits, eType.Int));
            sb.AppendLine(Analyze(GetPName.AccumBlueBits, eType.Int));
            sb.AppendLine(Analyze(GetPName.AccumAlphaBits, eType.Int));

            sb.AppendLine("--- Textures ---");
            sb.AppendLine(Analyze(GetPName.MaxCombinedTextureImageUnits, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxVertexTextureImageUnits, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxTextureImageUnits, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxTextureUnits, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxTextureSize, eType.Int));
            sb.AppendLine(Analyze(GetPName.Max3DTextureSize, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxCubeMapTextureSize, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxRenderbufferSize, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxTextureLodBias, eType.Int));

            Queue<TexFormat> Supported = new Queue<TexFormat>();
            Queue<TexFormat> Unsupported = new Queue<TexFormat>();

            uint DummyTexture;
            foreach (TexFormat t in TextureFormats)
            {
                GL.GenTextures(1, out DummyTexture);
                GL.BindTexture(TextureTarget.Texture2D, DummyTexture);
                GL.TexImage2D(TextureTarget.Texture2D, 0, t.pif, 4, 4, 0, t.pf, t.pt, IntPtr.Zero);
                if (GL.GetError() == ErrorCode.NoError)
                    Supported.Enqueue(t);
                else
                    Unsupported.Enqueue(t);
                GL.DeleteTextures(1, ref DummyTexture);
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);

            sb.AppendLine("--- UN-supported Texture formats ---");
            while (Unsupported.Count > 0)
            {
                TexFormat tex = Unsupported.Dequeue();
                sb.AppendLine((All)tex.pif + ", ");
            }
            sb.AppendLine(" ");

            sb.AppendLine("--- SUPPORTED Texture formats ---");
            while (Supported.Count > 0)
            {
                TexFormat tex = Supported.Dequeue();
                sb.AppendLine((All)tex.pif + "   " + tex.pf + "  " + tex.pt);
            }
            sb.AppendLine(" ");

            sb.AppendLine("--- Point&Line volumes ---");
            sb.AppendLine(Analyze(GetPName.AliasedPointSizeRange, eType.FloatArray2));
            sb.AppendLine(Analyze(GetPName.PointSizeMin, eType.Float));
            sb.AppendLine(Analyze(GetPName.PointSizeMax, eType.Float));
            sb.AppendLine(Analyze(GetPName.PointSizeGranularity, eType.Float));
            sb.AppendLine(Analyze(GetPName.PointSizeRange, eType.FloatArray2));

            sb.AppendLine(Analyze(GetPName.AliasedLineWidthRange, eType.FloatArray2));
            sb.AppendLine(Analyze(GetPName.LineWidthGranularity, eType.Float));
            sb.AppendLine(Analyze(GetPName.LineWidthRange, eType.FloatArray2));

            sb.AppendLine("--- VBO ---");
            sb.AppendLine(Analyze(GetPName.MaxElementsIndices, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxElementsVertices, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxVertexAttribs, eType.Int));

            sb.AppendLine("--- GLSL ---");
            sb.AppendLine(Analyze(GetPName.MaxCombinedFragmentUniformComponents, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxCombinedGeometryUniformComponents, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxCombinedVertexUniformComponents, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxFragmentUniformComponents, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxVertexUniformComponents, eType.Int));

            sb.AppendLine(Analyze(GetPName.MaxCombinedUniformBlocks, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxFragmentUniformBlocks, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxGeometryUniformBlocks, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxVertexUniformBlocks, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxUniformBlockSize, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxUniformBufferBindings, eType.Int));

            sb.AppendLine(Analyze(GetPName.MaxVaryingFloats, eType.Int));

            sb.AppendLine("--- Transform Feedback ---");
            sb.AppendLine(Analyze(GetPName.MaxTransformFeedbackInterleavedComponents, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxTransformFeedbackSeparateAttribs, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxTransformFeedbackSeparateComponents, eType.Int));

            sb.AppendLine("--- Fixed-Func Stacks, GL.Push* and GL.Pop* ---");
            sb.AppendLine(Analyze(GetPName.MaxClientAttribStackDepth, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxAttribStackDepth, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxProjectionStackDepth, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxModelviewStackDepth, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxTextureStackDepth, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxNameStackDepth, eType.Int));

            sb.AppendLine("--- Fixed-Func misc. stuff ---");
            sb.AppendLine(Analyze(GetPName.MaxEvalOrder, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxClipPlanes, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxArrayTextureLayers, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxListNesting, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxLights, eType.Int));
            sb.AppendLine(Analyze(GetPName.MaxTextureCoords, eType.Int));

            return sb.ToString();
        }
    }
}
