using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok
{
    /// <summary>
    /// Linq関係の拡張メソッドを定義します。
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// foreachのlinq版。
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> source,
                                      Action<T> action)
        {
            if (source == null)
            {
                return;
            }

            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// each_with_index@ruby のlinq版。
        /// </summary>
        public static void ForEachWithIndex<T>(this IEnumerable<T> source,
                                               Action<T, int> action)
        {
            var index = 0;

            if (source == null)
            {
                return;
            }

            foreach (var item in source)
            {
                action(item, index++);
            }
        }

        /// <summary>
        /// Whereにインデックスがついたバージョン。
        /// </summary>
        public static IEnumerable<T> WhereWithIndex<T>(this IEnumerable<T> source,
                                                       Func<T, int, bool> predicate)
        {
            if (source != null)
            {
                var index = 0;

                foreach (var elem in source)
                {
                    if (predicate(elem, index))
                    {
                        yield return elem;
                    }

                    index += 1;
                }
            }
        }

        /// <summary>
        /// Selectにインデックスがついたバージョン。
        /// </summary>
        public static IEnumerable<TResult> SelectWithIndex<TSource, TResult>(
            this IEnumerable<TSource> source, Func<TSource, int, TResult> func)
        {
            if (source != null)
            {
                var index = 0;

                foreach (var item in source)
                {
                    yield return func(item, index++);
                }
            }
        }

        /// <summary>
        /// 指定の条件を最初に満たす要素番号を取得します。
        /// </summary>
        /// <remarks>
        /// 条件を満たす要素がない場合は-1を返します。
        /// </remarks>
        public static int FindIndex<T>(this IList<T> self, Predicate<T> pred)
        {
            if (self == null)
            {
                return -1;
            }

            for (var i = 0; i < self.Count; ++i)
            {
                if (pred(self[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 最初に指定の条件を満たす要素を削除します。
        /// </summary>
        public static bool RemoveIf<T>(this IList<T> self, Predicate<T> pred)
        {
            if (self == null)
            {
                return false;
            }

            var index = self.FindIndex(pred);
            if (index >= 0)
            {
                self.RemoveAt(index);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// selfに指定の操作を適用します。
        /// </summary>
        public static T Apply<T>(this T self, params Action<T>[] actions)
            where T : class
        {
            ForEach(actions, _ => _(self));

            return self;
        }

        /// <summary>
        /// イベントハンドラを呼び出します。
        /// </summary>
        public static void RaiseEvent(this EventHandler handler, object sender,
                                      EventArgs e)
        {
            if (handler == null)
            {
                return;
            }

            handler(sender, e);
        }

        /// <summary>
        /// 例外が起こらないようにイベントハンドラを呼び出します。
        /// </summary>
        public static void SafeRaiseEvent(this EventHandler handler, object sender,
                                          EventArgs e)
        {
            if (handler == null)
            {
                return;
            }

            Util.SafeCall(() => handler(sender, e));
        }

        /// <summary>
        /// イベントハンドラを呼び出します。
        /// </summary>
        public static void RaiseEvent<TEventArgs>(this EventHandler<TEventArgs> handler,
                                                  object sender,
                                                  TEventArgs e)
            where TEventArgs : EventArgs
        {
            if (handler == null)
            {
                return;
            }

            handler(sender, e);
        }

        /// <summary>
        /// 例外が起こらないようにイベントハンドラを呼び出します。
        /// </summary>
        public static void SafeRaiseEvent<TEventArgs>(this EventHandler<TEventArgs> handler,
                                                      object sender,
                                                      TEventArgs e)
            where TEventArgs : EventArgs
        {
            if (handler == null)
            {
                return;
            }

            Util.SafeCall(() => handler(sender, e));
        }
    }
}
