using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
        public static void ForEach<T>(this IEnumerable<T> source, params Action<T>[] actions)
        {
            if (source == null)
            {
                return;
            }

            foreach (var item in source)
            {
                foreach (var action in actions)
                {
                    action(item);
                }
            }
        }

        /// <summary>
        /// each_with_index@ruby のlinq版。
        /// </summary>
        public static void ForEachWithIndex<T>(this IEnumerable<T> source,
                                               params Action<T, int>[] actions)
        {
            if (source == null)
            {
                return;
            }

            var index = 0;
            foreach (var item in source)
            {
                foreach (var action in actions)
                {
                    action(item, index);
                }

                index += 1;
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
        public static int FindIndex<T>(this IEnumerable<T> source, Predicate<T> pred)
        {
            if (source == null)
            {
                return -1;
            }

            var index = 0;
            foreach (var item in source)
            {
                if (pred(item))
                {
                    return index;
                }

                index += 1;
            }

            return -1;
        }

        /// <summary>
        /// 最初に指定の条件を満たす要素を削除します。
        /// </summary>
        public static bool RemoveIf<T>(this IList<T> source, Predicate<T> pred)
        {
            if (source == null)
            {
                return false;
            }

            var index = source.FindIndex(pred);
            if (index >= 0)
            {
                source.RemoveAt(index);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// <paramref name="source"/>に<paramref name="actions"/>を適用し、その要素列を返します。
        /// </summary>
        public static IEnumerable<T> Apply<T>(this IEnumerable<T> source,
                                              params Action<T>[] actions)
            where T : class
        {
            if (source == null)
            {
                yield break;
            }

            foreach (var item in source)
            {
                foreach (var action in actions)
                {
                    action(item);
                }

                yield return item;
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
        /// <paramref name="source"/>中の要素を<paramref name="count"/>
        /// 個ごとにまとめ直します。
        /// 最後の要素数が足りない場合は足りないまま返します。
        /// </summary>
        public static IEnumerable<IEnumerable<TSource>>
            TakeBy<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            while (source.Any())
            {
                yield return source.Take(count);

                source = source.Skip(count);
            }
        }

        /// <summary>
        /// <paramref name="source"/>中の要素を<paramref name="count"/>
        /// 個ごとにまとめ直します。
        /// 最後の要素数が足りない場合はdefault値で補います。
        /// </summary>
        public static IEnumerable<IEnumerable<TSource>> TakeOrDefaultBy<TSource>(
            this IEnumerable<TSource> source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            using (var enumerator = source.GetEnumerator())
            {
                var values = new TSource[count];
                while (enumerator.MoveNext())
                {
                    values[0] = enumerator.Current;

                    var index = 1;
                    for (; index < count; ++index)
                    {
                        if (!enumerator.MoveNext()) break;

                        values[index] = enumerator.Current;
                    }

                    for (; index < count; ++index)
                    {
                        values[index] = default;
                    }

                    yield return values;
                }
            }
        }

        /// <summary>
        /// Listクラスをリサイズします。
        /// </summary>
        public static void Resize<T>(this List<T> list, int size,
                                     T value = default)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            int curSize = list.Count;

            if (size < curSize)
            {
                list.RemoveRange(size, curSize - size);
            }
            else if (size > curSize)
            {
                if (size > list.Capacity)
                {
                    // 最適化
                    list.Capacity = size;
                }

                list.AddRange(Enumerable.Repeat(value, size - curSize));
            }
        }

        /// <summary>
        /// リストの要素をすべて削除し、新しいコレクションを代入します。
        /// </summary>
        public static void Assign<T>(this IList<T> list, IEnumerable<T> collection)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            list.Clear();
            collection.ForEach(_ => list.Add(_));
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
