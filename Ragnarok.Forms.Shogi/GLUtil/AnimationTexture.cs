using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Ragnarok.Forms.Draw;
using Ragnarok.Utility;

namespace Ragnarok.Forms.Shogi.GLUtil
{
    /// <summary>
    /// アニメーション用の分割されたテクスチャを管理します。
    /// </summary>
    public class AnimationTexture : ICachable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AnimationTexture()
        {
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
            get { return TextureList.Count(); }
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
        public bool Load(string filepath, int count,
                         bool toPremultipliedAlpha = false)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new ArgumentNullException("filepath");
            }

            if (count <= 0)
            {
                throw new ArgumentException("count");
            }

            using (var image = new Bitmap(filepath))
            {
                if (image == null)
                {
                    return false;
                }

                return Create(image, count, toPremultipliedAlpha);
            }
        }

        /// <summary>
        /// テクスチャをアニメーション用に分割した状態で読み込みます。
        /// </summary>
        public bool Create(Bitmap image, int count,
                           bool toPremultipliedAlpha = false)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            if (count <= 0)
            {
                throw new ArgumentException("count");
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
                            let bitmap = image.CropHighQuality(w * i, 0, w, h)
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
        /// ビットマップからテクスチャを読み込みます。
        /// </summary>
        private Texture LoadTexture(Bitmap bitmap)
        {
            var tex = new Texture();
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
