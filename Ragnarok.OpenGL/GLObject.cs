using System;
using System.Collections.Generic;
using OpenTK.Windowing.Common;

namespace Ragnarok.OpenGL
{
    /// <summary>
    /// OpenGL用のテクスチャを管理します。
    /// </summary>
    public abstract class GLObject : IDisposable
    {
        private static readonly object s_lock = new();
        private static readonly List<WeakReference<GLObject>> s_objects = new();

        private static void AddToList(GLObject obj)
        {
            lock (s_lock)
            {
                s_objects.Add(new WeakReference<GLObject>(obj));
            }
        }

        private static void DeleteFromList(GLObject target)
        {
            lock (s_lock)
            {
                s_objects.RemoveIf(_ =>
                    !_.TryGetTarget(out GLObject obj) ||
                    Equals(target, obj));
            }
        }

        /// <summary>
        /// 現在のコンテキストが持つOpenGLオブジェクトを削除します。
        /// </summary>
        /// <remarks>
        /// このメソッドは登録されているGLObjectのDispose()を呼び出しますが
        /// OpenGLの資源は削除していないことに注意してください。
        /// このメソッドはOpenGL資源の削除登録を行っているだけです。
        /// 実際の削除には、GLDispose.Update() の呼び出しが必要です。
        /// </remarks>
        public static void DisposeAll(IGraphicsContext context)
        {
            lock (s_lock)
            {
                for (var index = 0; index < s_objects.Count;)
                {
                    if (!s_objects[index].TryGetTarget(out GLObject obj))
                    {
                        // 要素を削除したため、indexの更新は行いません。
                        s_objects.RemoveAt(index);
                        continue;
                    }

                    if (obj.Context == context)
                    {
                        var oldValue = s_objects[index];
                        obj.Dispose();

                        // Dispose()呼び出し中に、要素が削除されることがあるため
                        // その確認を行います。
                        if (index < s_objects.Count &&
                            ReferenceEquals(s_objects[index], oldValue))
                        {
                            s_objects.RemoveAt(index);
                        }
                        continue;
                    }

                    index += 1;
                }
            }
        }

        private bool disposed;

        /// <summary>
        /// オブジェクトの破棄処理を行います。
        /// </summary>
        public abstract void Destroy();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GLObject(IGraphicsContext context)
        {
            Context = context ??
                throw new ArgumentNullException(nameof(context));

            AddToList(this);
        }

        /// <summary>
        /// ファイナライザ
        /// </summary>
        ~GLObject()
        {
            Dispose(false);
        }

        /// <summary>
        /// オブジェクトの削除を行います。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// オブジェクトの削除を行います。
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                DeleteFromList(this);
                Destroy();

                this.disposed = true;
            }
        }

        /// <summary>
        /// オブジェクト名を取得します。
        /// </summary>
        public int ObjectName
        {
            get;
            private set;
        }

        /// <summary>
        /// コンテキストを取得します。
        /// </summary>
        public IGraphicsContext Context
        {
            get;
            private set;
        }

        /// <summary>
        /// コンテキストの確認を行います。
        /// </summary>
        protected void ValidateContext()
        {
            if (!Context.IsCurrent)
            {
                throw new GLException(
                    "OpenGLコンテキストが正しく設定されていません。");
            }
        }
    }
}
