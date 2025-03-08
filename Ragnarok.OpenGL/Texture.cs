using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;

using Ragnarok.Utility;

namespace Ragnarok.OpenGL
{
    using Imaging = System.Drawing.Imaging;

    /// <summary>
    /// テクスチャの補完方法を指定します。
    /// </summary>
    public enum FilterType
    {
        Nearest,
        Linear,
    }

    /// <summary>
    /// OpenGL用のテクスチャを管理します。
    /// </summary>
    public class Texture : GLObject, ICachable
    {
        private int glTexture;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Texture(IGraphicsContext context)
            : base(context)
        {
        }

        /// <summary>
        /// テクスチャを削除します。
        /// </summary>
        public override void Destroy()
        {
            if (this.glTexture != 0)
            {
                var texture = this.glTexture;
                GLDisposer.AddTarget(
                    Context,
                    () => GL.DeleteTexture(texture));
                this.glTexture = 0;
            }

            Width = 0;
            Height = 0;
            OriginalWidth = 0;
            OriginalHeight = 0;
            IsPremultipliedAlpha = false;
        }

        /// <summary>
        /// テクスチャ名(ID)を取得します。
        /// </summary>
        public int TextureName
        {
            get { return this.glTexture; }
        }

        /// <summary>
        /// テクスチャが使用可能か調べます。
        /// </summary>
        public bool IsAvailable
        {
            get { return (TextureName != 0); }
        }

        /// <summary>
        /// テクスチャ読み込み後にmipmapを作成するかどうかを取得または設定します。
        /// </summary>
        public bool UseMipmap
        {
            get;
            set;
        } = true;

        /// <summary>
        /// テクスチャの補完方法を取得または設定します。
        /// </summary>
        public FilterType FilterType
        {
            get;
            set;
        } = FilterType.Linear;

        /// <summary>
        /// テクスチャ画像の幅をPixel数で取得します。
        /// </summary>
        /// <remarks>
        /// 読み込まれた画像サイズとは違う可能性があります。
        /// </remarks>
        public int Width
        {
            get;
            private set;
        }

        /// <summary>
        /// テクスチャ画像の高さをPixel数で取得します。
        /// </summary>
        /// <remarks>
        /// 読み込まれた画像サイズとは違う可能性があります。
        /// </remarks>
        public int Height
        {
            get;
            private set;
        }

        /// <summary>
        /// 読み込みに使われたオリジナル画像の幅をPixel数で取得します。
        /// </summary>
        public int OriginalWidth
        {
            get;
            private set;
        }

        /// <summary>
        /// 読み込みに使われたオリジナル画像の高さをPixel数で取得します。
        /// </summary>
        public int OriginalHeight
        {
            get;
            private set;
        }
        
        /// <summary>
        /// α乗算済みテクスチャかどうかを取得します。
        /// </summary>
        public bool IsPremultipliedAlpha
        {
            get;
            private set;
        }

        /// <summary>
        /// ビットマップサイズをbyte数で取得します。
        /// </summary>
        /// <remarks>
        /// ICachableインターフェースの実装に必要です。
        /// </remarks>
        public long ObjectSize
        {
            get { return (4 * Width * Height); }
        }

        /// <summary>
        /// テクスチャに関する属性を保存します。
        /// </summary>
        public virtual void Push()
        {
            if (TextureName == 0)
            {
                throw new InvalidOperationException(
                    "テクスチャの作成が完了していません。");
            }

            ValidateContext();
            GL.PushAttrib(AttribMask.TextureBit);
            Bind();
        }

        /// <summary>
        /// テクスチャに関する属性を呼び出します。
        /// </summary>
        public virtual void Pop()
        {
            if (TextureName == 0)
            {
                throw new InvalidOperationException(
                    "テクスチャの作成が完了していません。");
            }

            ValidateContext();
            GL.PopAttrib();
        }

        /// <summary>
        /// テクスチャをバインドします。
        /// </summary>
        public void Bind()
        {
            if (TextureName == 0)
            {
                throw new InvalidOperationException(
                    "テクスチャの作成が完了していません。");
            }

            ValidateContext();
            GL.BindTexture(TextureTarget.Texture2D, TextureName);
        }
        
        /// <summary>
        /// テクスチャのバインドを解除します。
        /// </summary>
        public static void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// ミップマップをサポートするメソッドのレベルを取得します。
        /// </summary>
        private static int GenerateMipmapSupportLevel
        {
            get
            {
                if (Misc.Version >= 30000 && Misc.HasExtension("GL_ARB_framebuffer_object"))
                {
                    return 2;
                }
                else if (Misc.Version >= 10400 && Misc.HasExtension("GL_SGIS_generate_mipmap"))
                {
                    return 1;
                }

                return 0;
            }
        }

        private static int GetGLFilter(FilterType filter, bool useMipmap = false)
        {
            return filter switch
            {
                FilterType.Nearest =>
                    (int)(useMipmap
                        ? TextureMinFilter.NearestMipmapNearest
                        : TextureMinFilter.Nearest),
                FilterType.Linear =>
                    (int)(useMipmap
                        ? TextureMinFilter.LinearMipmapLinear
                        : TextureMinFilter.Linear),
                _ => throw new InvalidOperationException(
                    $"{filter}: unknown enum value"),
            };
        }

        /// <summary>
        /// テクスチャデータの作成を行います。
        /// </summary>
        /// <remarks>
        /// イメージからテクスチャを作成するのみで
        /// サイズの変更などの余計なことは一切行いません。
        ///
        /// またこのメソッドには所有権を渡してもよいイメージオブジェクトを
        /// 与えてください。
        /// </remarks>
        private bool CreateInternal(Bitmap image, Size originalSize)
        {
            int texture = 0;

            GLWrap.Wrap(() => GL.GenTextures(1, out texture));

            //  Lock the image bits (so that we can pass them to OGL).
            var bitmapData = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly,
                Imaging.PixelFormat.Format32bppArgb);

            try
            {
                GLWrap.Wrap(() => GL.BindTexture(TextureTarget.Texture2D, texture));

                // TexParameterのGenerateMipmapを使う場合
                if (GenerateMipmapSupportLevel == 1)
                {
                    // glTexImage2Dの前にmipmapの使用設定を行う
                    //GLWrap.Wrap(() => GL.Hint(HintTarget.GenerateMipmapHint, HintMode.Nicest));
                    GLWrap.Wrap(() => GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1));
                }

                //  テクスチャデータをセットします。
#if true
                GLWrap.Wrap(() => GL.TexImage2D(
                    TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8,
                    image.Width, image.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    bitmapData.Scan0));
#else
                GLWrap.Wrap(() => GL.Build2DMipmaps(
                    TextureTarget.Texture2D, (int)OpenGL.GL_RGBA,
                    image.Width, image.Height,
                    OpenTK.Graphics.OpenGL.GL_BGRA, GL.GL_UNSIGNED_BYTE,
                    bitmapData.Scan0));
#endif
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }

            GLWrap.Wrap(() => GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp));
            GLWrap.Wrap(() => GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp));

            // glGenerateMipmapを使う場合
            if (UseMipmap && GenerateMipmapSupportLevel == 2)
            {
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
            
            if (UseMipmap && GenerateMipmapSupportLevel > 0)
            {
                // Mipmapを使う場合はフィルターを設定
                GLWrap.Wrap(() => GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, GetGLFilter(FilterType, UseMipmap)));
                GLWrap.Wrap(() => GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, GetGLFilter(FilterType)));
            }
            else
            {
                GLWrap.Wrap(() => GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, GetGLFilter(FilterType)));
                GLWrap.Wrap(() => GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, GetGLFilter(FilterType)));
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);
            
            // テクスチャの作成に成功したら、古いテクスチャを削除します。
            Destroy();

            this.glTexture = texture;
            Width = image.Width;
            Height = image.Height;
            OriginalWidth = originalSize.Width;
            OriginalHeight = originalSize.Height;
            return true;
        }

        /// <summary>
        /// イメージデータからテクスチャの作成を行います。
        /// </summary>
        public bool Create(Bitmap image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            ValidateContext();

            int[] textureMaxSizes = { 0 };
            GLWrap.Wrap(() => GL.GetInteger(GetPName.MaxTextureSize, textureMaxSizes));

            var textureMaxSize = textureMaxSizes[0];
            var targetWidth = image.Width;
            var targetHeight = image.Height;

            if (!Misc.HasExtension("GL_ARB_texture_non_power_of_two") ||
                image.Width >= textureMaxSize ||
                image.Height >= textureMaxSize)
            {
                // 2のn乗値の中から、元の画像サイズを超えるような
                // 一番小さな値を探します。
                targetWidth = textureMaxSize;
                for (int size = 1; size <= textureMaxSize; size *= 2)
                {
                    if (image.Width < size)
                    {
                        targetWidth = size;
                        break;
                    }
                }

                targetHeight = textureMaxSize;
                for (int size = 1; size <= textureMaxSize; size *= 2)
                {
                    if (image.Height < size)
                    {
                        targetHeight = size;
                        break;
                    }
                }
            }

            // 画像のリサイズが必要な場合
            var result = false;
            if (image.Width != targetWidth || image.Height != targetHeight)
            {
                using (var newImage = new Bitmap(targetWidth, targetHeight))
                {
                    DrawHighQuality(
                        newImage, image, image.Width, image.Height);
                    result = CreateInternal(newImage, image.Size);
                }
            }
            else
            {
                result = CreateInternal(image, image.Size);
            }

            return result;
        }

        /// <summary>
        /// 画像を綺麗に描画します。
        /// </summary>
        private static void DrawHighQuality(Bitmap dst, Bitmap src,
                                            int width, int height)
        {
            if (dst == null)
            {
                throw new ArgumentNullException(nameof(dst));
            }

            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            using (Graphics g = Graphics.FromImage(dst))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.DrawImage(src,
                    new Rectangle(0, 0, width, height), // dst
                    new Rectangle(0, 0, src.Width, src.Height), // src
                    GraphicsUnit.Pixel);
            }
        }

        /// <summary>
        /// ファイルからテクスチャを作成します。
        /// </summary>
        public bool Load(string filepath)
        {
            using (var image = new Bitmap(filepath))
            {
                if (image == null)
                {
                    return false;
                }

                return Create(image);
            }
        }
    }
}
