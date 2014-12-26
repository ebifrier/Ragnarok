using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using SharpGL;

namespace Live2DSharp.Framework
{
    public static class L2DTextureLoader
    {
        /// <summary>
        /// テクスチャを読み込みます。
        /// </summary>
        public static uint LoadTexture(OpenGL gl, string filepath)
        {
            var textures = new uint[1];

            //  We need to load the texture from file.
            using (var textureImage = new Bitmap(filepath))
            {
                //  A bit of extra initialisation here, we have to enable textures.
                gl.Enable(OpenGL.GL_TEXTURE_2D);

                //  Get one texture id, and stick it into the textures array.
                gl.GenTextures(1, textures);

                //  Bind the texture.
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, textures[0]);

                gl.PixelStore(OpenGL.GL_UNPACK_ALIGNMENT, 8);

                var data = textureImage.LockBits(
                    new Rectangle(0, 0, textureImage.Width, textureImage.Height),
                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                //  Tell OpenGL where the texture data is.
                gl.Build2DMipmaps(
                    OpenGL.GL_TEXTURE_2D, OpenGL.GL_RGBA,
                    textureImage.Width, textureImage.Height,
                    OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE,
                    data.Scan0);

                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);

                textureImage.UnlockBits(data);
            }

            return textures[0];
        }

        /// <summary>
        /// α乗算済み画像データに変換します。
        /// </summary>
        private static void MakePremutipliedAlpha(BitmapData data)
        {
            if (data.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new InvalidOperationException(
                    "対応していないピクセルフォーマットです。");
            }

            unsafe
            {
                for (var y = 0; y < data.Height; ++y)
                {
                    byte* p = (byte*)data.Scan0 + (data.Stride * y);

                    for (var x = 0; x < data.Width; ++x)
                    {
                        var a = p[x * 4 + 0];

                        p[x * 4 + 1] = (byte)((p[x * 4 + 1] * a + 255) >> 8);
                        p[x * 4 + 2] = (byte)((p[x * 4 + 2] * a + 255) >> 8);
                        p[x * 4 + 3] = (byte)((p[x * 4 + 3] * a + 255) >> 8);
                    }
                }
            }
        }

        /// <summary>
        /// テクスチャの削除を行います。
        /// </summary>
        public static void DestroyTexture(OpenGL gl, uint textureNo)
        {
            if (textureNo == 0)
            {
                return;
            }

            var textures = new uint[] { textureNo };
            gl.DeleteTextures(1, textures);
        }
    }
}
