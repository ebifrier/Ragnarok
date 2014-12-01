using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;

using SharpGL;

namespace Ragnarok.Forms.Shogi.GL
{
    using Ragnarok.Forms.Draw;

    /// <summary>
    /// 画像オブジェクトをキャッシュします。
    /// </summary>
    /// <remarks>
    /// エフェクトはアニメーションするため、画像が縦や横に連なって保存されています。
    /// スクリーンより大きい画像はビデオカードによっては表示できないことがあるため、
    /// このクラスでは各画像をクロッピングして管理します。
    /// </remarks>
    [CLSCompliant(false)]
    public class TextureCache
    {
        private readonly static object SyncRoot = new object();
        private readonly static Dictionary<OpenGL, TextureCache> CacheRoot =
            new Dictionary<OpenGL, TextureCache>();

        /// <summary>
        /// OpenGLオブジェクトに対応するキャッシュを取得します。
        /// </summary>
        public static TextureCache GetCache(OpenGL gl)
        {
            lock (SyncRoot)
            {
                TextureCache cache;
                if (CacheRoot.TryGetValue(gl, out cache))
                {
                    return cache;
                }

                cache = new TextureCache(gl);
                CacheRoot.Add(gl, cache);
                return cache;
            }
        }

        /// <summary>
        /// 画像をキャッシュから探し、もしなければ読み込みます。
        /// </summary>
        public static List<GL.Texture> GetTextureList(OpenGL gl, Uri imageUri, int count)
        {
            if (gl == null)
            {
                throw new ArgumentNullException("gl");
            }

            var cache = GetCache(gl);
            return cache.GetTextureList(imageUri, count);
        }

        private readonly object syncRoot = new object();
        private readonly Dictionary<Uri, List<GL.Texture>> cacheDic =
            new Dictionary<Uri, List<Texture>>();
        private readonly OpenGL gl;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TextureCache(OpenGL gl)
        {
            this.gl = gl;
        }

        /// <summary>
        /// ビットマップからテクスチャを読み込みます。
        /// </summary>
        private GL.Texture LoadTexture(Bitmap bitmap)
        {
            var tex = new GL.Texture(this.gl);
            if (!tex.Create(bitmap))
            {
                throw new RagnarokException(
                    "テクスチャの作成に失敗しました。");
            }

            return tex;
        }

        /// <summary>
        /// count個に分割されたテクスチャを返します。
        /// </summary>
        private List<GL.Texture> LoadTextureList(Uri imageUri, int count)
        {
            try
            {
                var imagePath = imageUri.LocalPath;
                var image = new Bitmap(imagePath);

                List<GL.Texture> list = null;
                if (count <= 1)
                {
                    // 画像一枚をテクスチャに直します。
                    list = new List<GL.Texture>
                    {
                        LoadTexture(image)
                    };
                }
                else
                {
                    // 各画像をクロッピングしテクスチャを作成します。
                    var w = image.Width / count;
                    var h = image.Height;

                    list = (from i in Enumerable.Range(0, count)
                            let bitmap = image.CropHighQuality(w * i, 0, w, h)
                            select LoadTexture(bitmap))
                           .ToList();
                }

                return list;
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "{0}: 画像の読み込みに失敗しました。",
                    imageUri);

                return null;
            }
        }

        /// <summary>
        /// 画像をキャッシュから探し、もしなければ読み込みます。
        /// </summary>
        public List<GL.Texture> GetTextureList(Uri imageUri, int count)
        {
            lock (this.syncRoot)
            {
                List<GL.Texture> imageList;
                if (this.cacheDic.TryGetValue(imageUri, out imageList))
                {
                    return imageList;
                }

                imageList = LoadTextureList(imageUri, count);
                this.cacheDic.Add(imageUri, imageList);
                return imageList;
            }
        }
    }
}
