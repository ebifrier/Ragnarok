using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using OpenTK.Windowing.Common;

using Ragnarok.Utility;

namespace Ragnarok.OpenGL
{
    /// <summary>
    /// アニメーション用の分割されたテクスチャを管理します。
    /// </summary>
    public class AnimationTexture : ICachable
    {
        private readonly IGraphicsContext context;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AnimationTexture(IGraphicsContext context)
        {
            this.context = context ??
                throw new ArgumentNullException(nameof(context));
            TextureList = new List<Texture>();
        }

        /// <summary>
        /// テクスチャのリストを取得します。
        /// </summary>
        public List<Texture> TextureList
        {
            get;
            private set;
        }

        /// <summary>
        /// 含まれているテクスチャの数を取得します。
        /// </summary>
        public int Count
        {
            get { return TextureList.Count; }
        }

        /// <summary>
        /// キャッシュ用のオブジェクトサイズを取得します。
        /// </summary>
        public long ObjectSize
        {
            get { return TextureList.Sum(_ => _.ObjectSize); }
        }

        /// <summary>
        /// ファイルからテクスチャをアニメーション用に分割した状態で読み込みます。
        /// </summary>
        public bool Load(string filepath, int count)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new ArgumentNullException(nameof(filepath));
            }

            if (count <= 0)
            {
                throw new ArgumentException("countの値が小さすぎます。", nameof(count));
            }

            using (var image = new Bitmap(filepath))
            {
                if (image == null)
                {
                    return false;
                }

                return Create(image, count);
            }
        }

        /// <summary>
        /// テクスチャをアニメーション用に分割した状態で読み込みます。
        /// </summary>
        public bool Create(Bitmap image, int count)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (count <= 0)
            {
                throw new ArgumentException("countの値が小さすぎます。", nameof(count));
            }

            try
            {
                List<Texture> list = null;
                if (count == 1)
                {
                    // 画像一枚をテクスチャに直します。
                    list = new List<Texture>
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
                            let bitmap = CropHighQuality(image, w * i, 0, w, h)
                            select LoadTexture(bitmap))
                           .ToList();
                }

                TextureList = list;
                return true;
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "画像の読み込みに失敗しました。");

                return false;
            }
        }

        /// <summary>
        /// 画像の一部を綺麗に切り抜きます。
        /// </summary>
        private static Bitmap CropHighQuality(Bitmap bitmap, int x, int y,
                                              int width, int height)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            var target = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(target))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.DrawImage(bitmap,
                    new Rectangle(0, 0, width, height), // dst
                    new Rectangle(x, y, width, height), // src
                    GraphicsUnit.Pixel);
            }

            return target;
        }

        /// <summary>
        /// ビットマップからテクスチャを読み込みます。
        /// </summary>
        private Texture LoadTexture(Bitmap bitmap)
        {
            var tex = new Texture(this.context);
            if (!tex.Create(bitmap))
            {
                throw new RagnarokException(
                    "テクスチャの作成に失敗しました。");
            }

            return tex;
        }

        /// <summary>
        /// テクスチャをすべて削除します。
        /// </summary>
        public void Destroy()
        {
            foreach (var texture in TextureList)
            {
                texture.Destroy();
            }

            TextureList.Clear();
        }
    }
}
