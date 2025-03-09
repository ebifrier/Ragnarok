using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Windowing.Common;

namespace Ragnarok.OpenGL
{
    /// <summary>
    /// OpenGL用のテクスチャを廃棄するためクラスです。
    /// </summary>
    /// <remarks>
    /// OpenGL用のテクスチャは作成されたスレッドと同じスレッドで
    /// Deleteされる必要がありますが、DisposeメソッドはGC用のスレッドで呼ばれるため
    /// そこで廃棄できません。
    /// </remarks>
    public sealed class GLDisposer
    {
        private readonly static object syncDisposer = new();
        private readonly static Dictionary<IGraphicsContext, GLDisposer>
            disposerDic = new();

        /// <summary>
        /// contextに紐づけられたシングルトンインスタンスを取得します。
        /// </summary>
        public static GLDisposer GetContextInstance(IGraphicsContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            lock (syncDisposer)
            {
                GLDisposer disposer;
                if (disposerDic.TryGetValue(context, out disposer))
                {
                    return disposer;
                }

                disposer = new GLDisposer(context);
                disposerDic.Add(context, disposer);
                return disposer;
            }
        }

        /// <summary>
        /// 削除するテクスチャを登録します。
        /// </summary>
        public static void AddTarget(IGraphicsContext context,
                                     Action callback)
        {
            var disposer = GetContextInstance(context);

            disposer.AddTarget(callback);
        }

        /// <summary>
        /// 登録されたテクスチャを削除します。
        /// </summary>
        public static void Update(IGraphicsContext context)
        {
            var disposer = GetContextInstance(context);

            disposer.Update();
        }

        private readonly object syncList = new();
        private readonly List<Action> deleteList = new();

        /// <summary>
        /// private コンストラクタ
        /// </summary>
        private GLDisposer(IGraphicsContext context)
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
        /// 削除するオブジェクトをリストに加えます。
        /// </summary>
        public void AddTarget(Action callback)
        {
            if (callback == null)
            {
                return;
            }

            lock (this.syncList)
            {
                this.deleteList.Add(callback);
            }
        }

        /// <summary>
        /// Disposerに登録されたオブジェクトをすべて破棄します。
        /// </summary>
        public void Update()
        {
            lock (this.syncList)
            {
                this.deleteList.ForEach(_ => GLw.C(() => _()));
                this.deleteList.Clear();
            }
        }
    }
}
