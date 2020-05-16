using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace Ragnarok.Utility
{
    /// <summary>
    /// ComObject関係の便利クラス。
    /// </summary>
    public static class ComUtil
    {
        /// <summary>
        /// 型名を指定せずにComObjectオブジェクトを作成します。
        /// </summary>
        public static ComObject<T> CreateCom<T>(T value)
            where T : class
        {
            return new ComObject<T>(value);
        }

        /// <summary>
        /// 型名を指定せずにComObjectオブジェクトを作成します。
        /// </summary>
        public static ComObject<T> CreateCom<T>(T value, Action<T> closer)
            where T : class
        {
            return new ComObject<T>(value, closer);
        }
    }

    /// <summary>
    /// ComObject参照カウンタの自動解放を行うクラスです。
    /// </summary>
    public sealed class ComObject<T> : IDisposable
        where T : class
    {
        private Action<T> closer;
        private bool disposed;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ComObject(T value, Action<T> closer)
        {
            this.closer = closer;
            Value = value;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ComObject(T value)
            : this(value, null)
        {
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~ComObject()
        {
            Dispose(false);
        }

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// オブジェクトを破棄します。
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                var closer = Interlocked.Exchange(ref this.closer, null);

                // disposingかどうかは関係ないはずだけど、
                // 自信が持てない。。。
                if (Value != null)
                {
                    closer?.Invoke(Value);
                    Marshal.ReleaseComObject(Value);
                    Value = null;
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// 対象となるオブジェクトの値を取得します。
        /// </summary>
        public T Value
        {
            get;
            private set;
        }
    }
}
