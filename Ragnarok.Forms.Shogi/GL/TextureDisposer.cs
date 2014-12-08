using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGL;

namespace Ragnarok.Forms.Shogi.GL
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
        private readonly static Dictionary<OpenGL, TextureDisposer> instanceDic =
            new Dictionary<OpenGL, TextureDisposer>();

        /// <summary>
        /// シングルトンインスタンスを取得します。
        /// </summary>
        public static TextureDisposer GetInstance(OpenGL gl)
        {
            if (gl == null)
            {
                throw new ArgumentNullException("gl");
            }

            lock(syncInstance)
            {
                TextureDisposer instance;
                if (instanceDic.TryGetValue(gl, out instance))
                {
                    return instance;
                }

                instance = new TextureDisposer(gl);
                instanceDic.Add(gl, instance);
                return instance;
            }
        }

        /// <summary>
        /// 削除するテクスチャを登録します。
        /// </summary>
        public static void AddDeleteTexture(OpenGL gl, uint textureName)
        {
            var instance = GetInstance(gl);

            instance.AddDeleteTexture(textureName);
        }

        /// <summary>
        /// 登録されたテクスチャを削除します。
        /// </summary>
        public static void Update(OpenGL gl)
        {
            var instance = GetInstance(gl);

            instance.Update();
        }

        private readonly object syncRoot = new object();
        private List<uint> deleteTextureList = new List<uint>();

        /// <summary>
        /// private コンストラクタ
        /// </summary>
        private TextureDisposer(OpenGL gl)
        {
            OpenGL = gl;
        }

        /// <summary>
        /// OpenGL用のオブジェクトを取得します。
        /// </summary>
        public OpenGL OpenGL
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
            OpenGL.DeleteTextures(textureNames.Count(), textureNames);
        }
    }
}
