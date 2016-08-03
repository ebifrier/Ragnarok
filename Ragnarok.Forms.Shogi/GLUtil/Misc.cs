using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
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
                var raw = GL.GetString(StringName.Extensions);
                var splitter = new string[] { " " };

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

        private struct TextureFormat
        {
            public PixelInternalFormat pif;
            public PixelFormat pf;
            public PixelType pt;

            public TextureFormat(PixelInternalFormat _pif, PixelFormat _pf, PixelType _pt)
            {
                pif = _pif;
                pf = _pf;
                pt = _pt;
            }
        }

        private static TextureFormat[] TextureFormats = new TextureFormat[]
        {
            new TextureFormat( PixelInternalFormat.Alpha, PixelFormat.Alpha, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.Alpha4, PixelFormat.Alpha, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.Alpha8, PixelFormat.Alpha, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.Alpha12, PixelFormat.Alpha, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.Alpha16, PixelFormat.Alpha, PixelType.UnsignedByte),
            new TextureFormat( (PixelInternalFormat)All.Alpha16fArb, PixelFormat.Alpha, PixelType.HalfFloat),
            new TextureFormat( (PixelInternalFormat)All.Alpha32fArb, PixelFormat.Alpha, PixelType.Float),

            new TextureFormat( PixelInternalFormat.DepthComponent, PixelFormat.DepthComponent, PixelType.Int),
            new TextureFormat( PixelInternalFormat.DepthComponent16, PixelFormat.DepthComponent, PixelType.Float),
            new TextureFormat( PixelInternalFormat.DepthComponent24, PixelFormat.DepthComponent, PixelType.Float),
            new TextureFormat( PixelInternalFormat.DepthComponent32, PixelFormat.DepthComponent, PixelType.Float),
            new TextureFormat( PixelInternalFormat.DepthComponent32f, PixelFormat.DepthComponent, PixelType.Float),
            new TextureFormat( PixelInternalFormat.DepthStencil, PixelFormat.DepthStencil, PixelType.UnsignedInt248),
            new TextureFormat( PixelInternalFormat.Depth24Stencil8, PixelFormat.DepthStencil, PixelType.UnsignedInt248),
            new TextureFormat( PixelInternalFormat.Depth32fStencil8, PixelFormat.DepthStencil, PixelType.Float32UnsignedInt248Rev),

            new TextureFormat( PixelInternalFormat.One, PixelFormat.Red, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.Two, PixelFormat.Rg, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte),

            new TextureFormat( PixelInternalFormat.Srgb, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.SrgbAlpha, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.Srgb8, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.Srgb8Alpha8, PixelFormat.Rgba, PixelType.UnsignedByte),

            new TextureFormat( PixelInternalFormat.R16f, PixelFormat.Red, PixelType.HalfFloat),
            new TextureFormat( PixelInternalFormat.Rg16f, PixelFormat.Rg, PixelType.HalfFloat),
            new TextureFormat( PixelInternalFormat.Rgb16f, PixelFormat.Rgb, PixelType.HalfFloat),
            new TextureFormat( PixelInternalFormat.Rgba16f, PixelFormat.Rgba, PixelType.HalfFloat),
            new TextureFormat( PixelInternalFormat.R32f, PixelFormat.Red, PixelType.Float),
            new TextureFormat( PixelInternalFormat.Rg32f, PixelFormat.Rg, PixelType.Float),
            new TextureFormat( PixelInternalFormat.Rgb32f, PixelFormat.Rgb, PixelType.Float),
            new TextureFormat( PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float),

            new TextureFormat( PixelInternalFormat.R8, PixelFormat.Red, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.Rg8, PixelFormat.Rg, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.Rgb8, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.Rgba8, PixelFormat.Rgba, PixelType.UnsignedByte),

            new TextureFormat( PixelInternalFormat.R8ui, PixelFormat.Red, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.Rg8ui, PixelFormat.Rg, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.Rgb8ui, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.Rgba8ui, PixelFormat.Rgba, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.R16ui, PixelFormat.Red, PixelType.UnsignedShort),
            new TextureFormat( PixelInternalFormat.Rg16ui, PixelFormat.Rg, PixelType.UnsignedShort),
            new TextureFormat( PixelInternalFormat.Rgb16ui, PixelFormat.Rgb, PixelType.UnsignedShort),
            new TextureFormat( PixelInternalFormat.Rgba16ui, PixelFormat.Rgba, PixelType.UnsignedShort),
            new TextureFormat( PixelInternalFormat.R32ui, PixelFormat.Red, PixelType.UnsignedInt),
            new TextureFormat( PixelInternalFormat.Rg32ui, PixelFormat.Rg, PixelType.UnsignedInt),
            new TextureFormat( PixelInternalFormat.Rgb32ui, PixelFormat.Rgb, PixelType.UnsignedInt),
            new TextureFormat( PixelInternalFormat.Rgba32ui, PixelFormat.Rgba, PixelType.UnsignedInt),

            new TextureFormat( PixelInternalFormat.R8i, PixelFormat.Red, PixelType.Byte),
            new TextureFormat( PixelInternalFormat.Rg8i, PixelFormat.Rg, PixelType.Byte),
            new TextureFormat( PixelInternalFormat.Rgb8i, PixelFormat.Rgb, PixelType.Byte),
            new TextureFormat( PixelInternalFormat.Rgba8i, PixelFormat.Rgba, PixelType.Byte),
            new TextureFormat( PixelInternalFormat.R16i, PixelFormat.Red, PixelType.Short),
            new TextureFormat( PixelInternalFormat.Rg16i, PixelFormat.Rg, PixelType.Short),
            new TextureFormat( PixelInternalFormat.Rgb16i, PixelFormat.Rgb, PixelType.Short),
            new TextureFormat( PixelInternalFormat.Rgba16i, PixelFormat.Rgba, PixelType.Short),
            new TextureFormat( PixelInternalFormat.R32i, PixelFormat.Red, PixelType.Int),
            new TextureFormat( PixelInternalFormat.Rg32i, PixelFormat.Rg, PixelType.Int),
            new TextureFormat( PixelInternalFormat.Rgb32i, PixelFormat.Rgb, PixelType.Int),
            new TextureFormat( PixelInternalFormat.Rgba32i, PixelFormat.Rgba, PixelType.Int),

            new TextureFormat( PixelInternalFormat.R3G3B2, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.Rgb10A2, PixelFormat.Rgba, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.Rgb5A1, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.Rgb9E5, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.R11fG11fB10f, PixelFormat.Rgb, PixelType.UnsignedByte),

            new TextureFormat( PixelInternalFormat.CompressedAlpha, PixelFormat.Alpha, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.CompressedIntensity, PixelFormat.Luminance, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.CompressedLuminance, PixelFormat.Luminance, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.CompressedLuminanceAlpha, PixelFormat.LuminanceAlpha, PixelType.UnsignedByte),
            new TextureFormat( (PixelInternalFormat)All.CompressedLuminanceLatc1Ext, PixelFormat.Luminance, PixelType.UnsignedByte),
            new TextureFormat( (PixelInternalFormat)All.CompressedLuminanceAlphaLatc2Ext, PixelFormat.LuminanceAlpha, PixelType.UnsignedByte),

            new TextureFormat( PixelInternalFormat.CompressedRed, PixelFormat.Red, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.CompressedRedRgtc1, PixelFormat.Red, PixelType.UnsignedByte),
            new TextureFormat( (PixelInternalFormat)All.CompressedRedGreenRgtc2Ext, PixelFormat.Rg, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.CompressedRg, PixelFormat.Rg, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.CompressedRgRgtc2, PixelFormat.Rg, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.CompressedRgb, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TextureFormat( (PixelInternalFormat)All.CompressedRgbFxt13Dfx, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.CompressedRgba, PixelFormat.Rgba, PixelType.UnsignedByte),
            new TextureFormat( (PixelInternalFormat)All.CompressedRgbaFxt13Dfx, PixelFormat.Rgba, PixelType.UnsignedByte),

            new TextureFormat( (PixelInternalFormat)All.CompressedSignedLuminanceAlphaLatc2Ext, PixelFormat.LuminanceAlpha, PixelType.UnsignedByte),
            new TextureFormat( (PixelInternalFormat)All.CompressedSignedLuminanceLatc1Ext, PixelFormat.Luminance, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.CompressedSignedRedRgtc1, PixelFormat.Red, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.CompressedSignedRgRgtc2, PixelFormat.Rg, PixelType.UnsignedByte),

            new TextureFormat( PixelInternalFormat.CompressedSluminance, PixelFormat.Luminance, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.CompressedSluminanceAlpha, PixelFormat.LuminanceAlpha, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.CompressedSrgb, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.CompressedSrgbAlpha, PixelFormat.Rgba, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.CompressedSrgbS3tcDxt1Ext, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.CompressedSrgbAlphaS3tcDxt1Ext, PixelFormat.Rgba, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.CompressedSrgbAlphaS3tcDxt3Ext, PixelFormat.Rgba, PixelType.UnsignedByte),
            new TextureFormat( PixelInternalFormat.CompressedSrgbAlphaS3tcDxt5Ext, PixelFormat.Rgba, PixelType.UnsignedByte),

            new TextureFormat( (PixelInternalFormat)All.CompressedRgbS3tcDxt1Ext, PixelFormat.Rgb, PixelType.UnsignedByte),
            new TextureFormat( (PixelInternalFormat)All.CompressedRgbaS3tcDxt1Ext, PixelFormat.Rgba, PixelType.UnsignedByte),
            new TextureFormat( (PixelInternalFormat)All.CompressedRgbaS3tcDxt3Ext, PixelFormat.Rgba, PixelType.UnsignedByte),
            new TextureFormat( (PixelInternalFormat)All.CompressedRgbaS3tcDxt5Ext, PixelFormat.Rgba, PixelType.UnsignedByte),
        };

        private enum GLType
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

        /// <summary>
        /// OpenGLのベンダー情報などを表示します。
        /// </summary>
        public static string ShowAnalyze()
        {
            var sb = new StringBuilder();

            string Renderer = GL.GetString(StringName.Renderer);
            string GLSLang = GL.GetString(StringName.ShadingLanguageVersion);
            string Vendor = GL.GetString(StringName.Vendor);
            string Version = GL.GetString(StringName.Version);

            string ExtensionsRaw = GL.GetString(StringName.Extensions);
            string[] splitter = new string[] { " " };
            string[] Extensions = ExtensionsRaw.Split(splitter, StringSplitOptions.None);

            using (new Section(sb, "OpenGL Basic Information"))
            {
                sb.AppendLine("Vendor: " + Vendor);
                sb.AppendLine("Renderer: " + Renderer);
                sb.AppendLine("GL Version: " + Version);
                sb.AppendLine(Analyze(GetPName.MajorVersion, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MinorVersion, GLType.Int));
                sb.AppendLine("GLSL Version: " + GLSLang);
                sb.AppendLine("Extensions: ");
                Extensions
                    .Where(_ => !string.IsNullOrEmpty(_))
                    .ForEach(_ => sb.AppendLine(_));
            }
            
            // returns 0 formats, driver broken?
            int CompressedTextureFormatCount;
            GL.GetInteger(GetPName.NumCompressedTextureFormats, out CompressedTextureFormatCount);
            if (CompressedTextureFormatCount > 0)
            {
                var CompressedTextureFormats = new int[CompressedTextureFormatCount];
                GL.GetInteger(GetPName.CompressedTextureFormats, CompressedTextureFormats);

                using (new Section(sb, "Supported compressed Texture formats"))
                {
                    for (int i = 0; i < CompressedTextureFormats.Length; ++i)
                    {
                        sb.AppendLine((All)CompressedTextureFormats[i] + "");
                    }
                }
            }

            using (new Section(sb, "Framebuffer"))
            {
                sb.AppendLine(Analyze(GetPName.Doublebuffer, GLType.Boolean));
                sb.AppendLine(Analyze(GetPName.MaxColorAttachments, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxDrawBuffers, GLType.Int));
                sb.AppendLine(Analyze(GetPName.AuxBuffers, GLType.Int));
                sb.AppendLine(Analyze(GetPName.DrawBuffer, GLType.IntEnum));
                sb.AppendLine(Analyze(GetPName.MaxSamples, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxViewportDims, GLType.IntArray2));
                sb.AppendLine(Analyze(GetPName.Viewport, GLType.IntArray4));
            }

            using (new Section(sb, "Framebuffer channels"))
            {
                sb.AppendLine(Analyze(GetPName.RedBits, GLType.Int));
                sb.AppendLine(Analyze(GetPName.GreenBits, GLType.Int));
                sb.AppendLine(Analyze(GetPName.BlueBits, GLType.Int));
                sb.AppendLine(Analyze(GetPName.AlphaBits, GLType.Int));
                sb.AppendLine(Analyze(GetPName.DepthBits, GLType.Int));
                sb.AppendLine(Analyze(GetPName.StencilBits, GLType.Int));

                sb.AppendLine(Analyze(GetPName.AccumRedBits, GLType.Int));
                sb.AppendLine(Analyze(GetPName.AccumGreenBits, GLType.Int));
                sb.AppendLine(Analyze(GetPName.AccumBlueBits, GLType.Int));
                sb.AppendLine(Analyze(GetPName.AccumAlphaBits, GLType.Int));
            }

            using (new Section(sb, "Textures"))
            {
                sb.AppendLine(Analyze(GetPName.MaxCombinedTextureImageUnits, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxVertexTextureImageUnits, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxTextureImageUnits, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxTextureUnits, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxTextureSize, GLType.Int));
                sb.AppendLine(Analyze(GetPName.Max3DTextureSize, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxCubeMapTextureSize, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxRenderbufferSize, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxTextureLodBias, GLType.Int));
            }

            var Supported = new Queue<TextureFormat>();
            var Unsupported = new Queue<TextureFormat>();

            // 実際に様々なフォーマットのテクスチャを作ることで、
            // 対応状況を調べます。
            foreach (TextureFormat t in TextureFormats)
            {
                uint DummyTexture = 0;
                try
                {
                    GL.GenTextures(1, out DummyTexture);
                    GL.BindTexture(TextureTarget.Texture2D, DummyTexture);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, t.pif, 4, 4, 0, t.pf, t.pt, IntPtr.Zero);

                    if (GL.GetError() == ErrorCode.NoError)
                        Supported.Enqueue(t);
                    else
                        Unsupported.Enqueue(t);
                }
                catch
                {
                    if (DummyTexture != 0)
                    {
                        GL.DeleteTextures(1, ref DummyTexture);
                    }
                }
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);

            using (new Section(sb, "Supported Texture formats"))
            {
                while (Supported.Count > 0)
                {
                    TextureFormat tex = Supported.Dequeue();
                    sb.AppendLine($"{(All)tex.pif,-32} {tex.pf,-16} {tex.pt}");
                }
            }

            using (new Section(sb, "UN-Supported Texture formats"))
            {
                while (Unsupported.Count > 0)
                {
                    TextureFormat tex = Unsupported.Dequeue();
                    sb.AppendLine((All)tex.pif + "");
                }
            }

            using (new Section(sb, "Point&Line volumes"))
            {
                sb.AppendLine(Analyze(GetPName.AliasedPointSizeRange, GLType.FloatArray2));
                sb.AppendLine(Analyze(GetPName.PointSizeMin, GLType.Float));
                sb.AppendLine(Analyze(GetPName.PointSizeMax, GLType.Float));
                sb.AppendLine(Analyze(GetPName.PointSizeGranularity, GLType.Float));
                sb.AppendLine(Analyze(GetPName.PointSizeRange, GLType.FloatArray2));

                sb.AppendLine(Analyze(GetPName.AliasedLineWidthRange, GLType.FloatArray2));
                sb.AppendLine(Analyze(GetPName.LineWidthGranularity, GLType.Float));
                sb.AppendLine(Analyze(GetPName.LineWidthRange, GLType.FloatArray2));
            }

            using (new Section(sb, "VBO"))
            {
                sb.AppendLine(Analyze(GetPName.MaxElementsIndices, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxElementsVertices, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxVertexAttribs, GLType.Int));
            }

            using (new Section(sb, "GLSL"))
            {
                sb.AppendLine(Analyze(GetPName.MaxCombinedFragmentUniformComponents, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxCombinedGeometryUniformComponents, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxCombinedVertexUniformComponents, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxFragmentUniformComponents, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxVertexUniformComponents, GLType.Int));

                sb.AppendLine(Analyze(GetPName.MaxCombinedUniformBlocks, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxFragmentUniformBlocks, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxGeometryUniformBlocks, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxVertexUniformBlocks, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxUniformBlockSize, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxUniformBufferBindings, GLType.Int));

                sb.AppendLine(Analyze(GetPName.MaxVaryingFloats, GLType.Int));
            }

            using (new Section(sb, "Transform Feedback"))
            {
                sb.AppendLine(Analyze(GetPName.MaxTransformFeedbackInterleavedComponents, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxTransformFeedbackSeparateAttribs, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxTransformFeedbackSeparateComponents, GLType.Int));
            }

            using (new Section(sb, "Fixed-Func Stacks, GL.Push* and GL.Pop*"))
            {
                sb.AppendLine(Analyze(GetPName.MaxClientAttribStackDepth, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxAttribStackDepth, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxProjectionStackDepth, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxModelviewStackDepth, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxTextureStackDepth, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxNameStackDepth, GLType.Int));
            }

            using (new Section(sb, "Fixed-Func misc. stuff"))
            {
                sb.AppendLine(Analyze(GetPName.MaxEvalOrder, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxClipPlanes, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxArrayTextureLayers, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxListNesting, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxLights, GLType.Int));
                sb.AppendLine(Analyze(GetPName.MaxTextureCoords, GLType.Int));
            }

            return sb.ToString();
        }

        private sealed class Section : Ragnarok.Utility.ActionOnDispose
        {
            public Section(StringBuilder sb, string label)
                : base(new Action(() => sb.AppendLine()))
            {
                sb.AppendLine($"--- {label} ---");
            }
        }

        /// <summary>
        /// OpenGLのプロパティを取得します。
        /// </summary>
        private static string Analyze(GetPName pname, GLType type)
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
                case GLType.Boolean:
                    GL.GetBoolean(pname, out result1b);
                    output = pname + ": " + result1b;
                    break;
                case GLType.Int:
                    GL.GetInteger(pname, out result1i);
                    output = pname + ": " + result1i;
                    break;
                case GLType.IntEnum:
                    GL.GetInteger(pname, out result1i);
                    output = pname + ": " + (All)result1i;
                    break;
                case GLType.IntArray2:
                    GL.GetInteger(pname, result2i);
                    output = pname + ": ( " + result2i[0] + ", " + result2i[1] + " )";
                    break;
                case GLType.IntArray4:
                    GL.GetInteger(pname, result4i);
                    output = pname + ": ( " + result4i[0] + ", " + result4i[1] + " ) ( " + result4i[2] + ", " + result4i[3] + " )";
                    break;
                case GLType.Float:
                    GL.GetFloat(pname, out result1f);
                    output = pname + ": " + result1f;
                    break;
                case GLType.FloatArray2:
                    GL.GetFloat(pname, out result2f);
                    output = pname + ": ( " + result2f.X + ", " + result2f.Y + " )";
                    break;
                case GLType.FloatArray4:
                    GL.GetFloat(pname, out result4f);
                    output = pname + ": ( " + result4f.X + ", " + result4f.Y + ", " + result4f.Z + ", " + result4f.W + " )";
                    break;
                default:
                    throw new NotImplementedException();
            }

            var err = GL.GetError();
            if (err != ErrorCode.NoError)
            {
                Log.Error("Unsupported Token: " + pname);
            }

            return output;
        }
    }
}
