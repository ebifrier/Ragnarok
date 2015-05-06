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
    /// OpenGL用のテクスチャを廃棄するためクラスです。
    /// </summary>
    /// <remarks>
    /// OpenGL用のテクスチャは作成されたスレッドと同じスレッドで
    /// Deleteされる必要がありますが、DisposeメソッドはGC用のスレッドで呼ばれるため
    /// そこで廃棄できません。
    /// </remarks>
    [CLSCompliant(false)]
    public sealed class TextureDisposer
    {
        private readonly static object syncInstance = new object();
        private readonly static Dictionary<IGraphicsContext, TextureDisposer> instanceDic =
            new Dictionary<IGraphicsContext, TextureDisposer>();

        /// <summary>
        /// シングルトンインスタンスを取得します。
        /// </summary>
        public static TextureDisposer GetInstance(IGraphicsContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            lock (syncInstance)
            {
                TextureDisposer instance;
                if (instanceDic.TryGetValue(context, out instance))
                {
                    return instance;
                }

                instance = new TextureDisposer(context);
                instanceDic.Add(context, instance);
                return instance;
            }
        }

        /// <summary>
        /// 削除するテクスチャを登録します。
        /// </summary>
        public static void AddDeleteTexture(IGraphicsContext context, uint textureName)
        {
            var instance = GetInstance(context);

            instance.AddDeleteTexture(textureName);
        }

        /// <summary>
        /// 登録されたテクスチャを削除します。
        /// </summary>
        public static void Update(IGraphicsContext context)
        {
            var instance = GetInstance(context);

            instance.Update();
        }

        private readonly object syncRoot = new object();
        private List<uint> deleteTextureList = new List<uint>();

        /// <summary>
        /// private コンストラクタ
        /// </summary>
        private TextureDisposer(IGraphicsContext context)
        {
            Context = context;
        }

        /// <summary>
        /// OpenGL用のオブジェクトを取得します。
        /// </summary>
        public IGraphicsContext Context
        {
            get;
            private set;
        }

        /// <summary>
        /// 削除するテクスチャのリストに加えます。
        /// </summary>
        public void AddDeleteTexture(uint textureName)
        {
            if (textureName == 0)
            {
                return;
            }

            lock (this.syncRoot)
            {
                this.deleteTextureList.Add(textureName);
            }
        }

        /// <summary>
        /// Disposerに登録されたオブジェクトをすべて破棄します。
        /// </summary>
        public void Update()
        {
            uint[] textureNames = null;

            lock (this.syncRoot)
            {
                textureNames = this.deleteTextureList.ToArray();
                this.deleteTextureList = new List<uint>();
            }

            // テクスチャをまとめて削除します。
            GL.DeleteTextures(textureNames.Count(), textureNames);
        }
    }
}
