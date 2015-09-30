using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Ragnarok
{
    using Ragnarok.Utility;

    /// <summary>
    /// PropertyChangedイベントを呼ぶメソッド型です。
    /// </summary>
    public delegate void PropertyChangedEventCaller(
        PropertyChangedEventHandler handler,
        object sender,
        PropertyChangedEventArgs e);

    /// <summary>
    /// CollectionChangedイベントを呼ぶメソッド型です。
    /// </summary>
    public delegate void CollectionChangedEventCaller(
        NotifyCollectionChangedEventHandler handler,
        object sender,
        NotifyCollectionChangedEventArgs e);

    /// <summary>
    /// ユーティリティクラスです。
    /// </summary>
    public static class Util
    {
        private static readonly Encoding SJisEncoding =
            Encoding.GetEncoding("Shift_JIS");

        private static PropertyChangedEventCaller propertyChangedCaller;
        private static CollectionChangedEventCaller collectionChangedCaller;
        private static Action<Action> eventCaller;

        /// <summary>
        /// PropertyChangedイベントを呼びだすデリゲートを設定します。
        /// </summary>
        /// <remarks>
        /// WPFを使うプログラムではUIスレッドから呼び出し場合があります。
        /// </remarks>
        public static void SetPropertyChangedCaller(PropertyChangedEventCaller caller)
        {
            propertyChangedCaller = caller;
        }

        /// <summary>
        /// PropertyChangedイベントを呼び出します。
        /// </summary>
        public static void CallPropertyChanged(this PropertyChangedEventHandler handler,
                                               object sender,
                                               PropertyChangedEventArgs e)
        {
            if (handler == null)
            {
                return;
            }

            var caller = propertyChangedCaller;
            if (caller != null)
            {
                caller(handler, sender, e);
            }
            else
            {
                handler(sender, e);
            }
        }

        /// <summary>
        /// CollectionChangedイベントを呼びだすデリゲートを設定します。
        /// </summary>
        /// <remarks>
        /// WPFを使うプログラムではUIスレッドから呼び出し場合があります。
        /// </remarks>
        public static void SetColletionChangedCaller(CollectionChangedEventCaller caller)
        {
            collectionChangedCaller = caller;
        }

        /// <summary>
        /// PropertyChangedイベントを呼び出します。
        /// </summary>
        public static void CallCollectionChanged(this NotifyCollectionChangedEventHandler handler,
                                                 object sender,
                                                 NotifyCollectionChangedEventArgs e)
        {
            if (handler == null)
            {
                return;
            }

            var caller = collectionChangedCaller;
            if (caller != null)
            {
                caller(handler, sender, e);
            }
            else
            {
                handler(sender, e);
            }
        }

        /// <summary>
        /// イベントを呼びだすデリゲートを設定します。
        /// </summary>
        /// <remarks>
        /// WPFを使うプログラムではUIスレッドから呼び出し場合があります。
        /// </remarks>
        public static void SetEventCaller(Action<Action> caller)
        {
            eventCaller = caller;
        }

        /// <summary>
        /// イベントを呼び出します。
        /// </summary>
        public static void CallEvent(this Action handler)
        {
            if (handler == null)
            {
                return;
            }

            var caller = eventCaller;
            if (caller != null)
            {
                caller(handler);
            }
            else
            {
                handler();
            }
        }

        /// <summary>
        /// オブジェクトの中身を交換します。
        /// </summary>
        public static void Swap<T>(ref T x, ref T y)
        {
            T tmp = x;
            x = y;
            y = tmp;
        }

        /// <summary>
        /// 例外が致命的なものならその例外をthrowします。
        /// </summary>
        public static void ThrowIfFatal(Exception ex)
        {
            if (ex is StackOverflowException ||
                ex is OutOfMemoryException ||
                ex is System.Threading.ThreadAbortException)
            {
                throw ex;
            }
        }

        /// <summary>
        /// default(type)を取得します。
        /// </summary>
        public static object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        /// <summary>
        /// <paramref name="value"/>がnullであれば<paramref name="defaultValue"/>
        /// を、そうでなければ値をそのまま返します。
        /// </summary>
        public static T GetValue<T>(T value, T defaultValue)
            where T: class
        {
            if (value == null)
            {
                return defaultValue;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// <paramref name="value"/>がnullであれば<paramref name="defaultValue"/>
        /// を、そうでなければ値をそのまま返します。
        /// </summary>
        public static T GetNullableValue<T>(T? value, T defaultValue)
            where T: struct
        {
            if (value == null)
            {
                return defaultValue;
            }
            else
            {
                return value.Value;
            }
        }

        /// <summary>
        /// Actionをエラー処理つきで呼び出します。
        /// </summary>
        public static void SafeCall(Action caller, bool logging = true)
        {
            try
            {
                if (caller != null)
                {
                    caller();
                }
            }
            catch (Exception ex)
            {
                if (logging)
                {
                    Log.ErrorException(ex,
                        "Actionの呼び出しに失敗しました。");
                }
            }
        }

        /// <summary>
        /// Funcをエラー処理つきで呼び出します。
        /// </summary>
        public static TResult SafeCall<TResult>(Func<TResult> caller, bool logging = true)
        {
            try
            {
                if (caller != null)
                {
                    return caller();
                }
            }
            catch (Exception ex)
            {
                if (logging)
                {
                    Log.ErrorException(ex,
                        "Funcの呼び出しに失敗しました。");
                }
            }

            return default(TResult);
        }

        /// <summary>
        /// オブジェクトをobject型の状態で比較します。
        /// </summary>
        /// <remarks>
        /// 主にEquals(object obj)のオーバーライドで使われます。
        /// </remarks>
        public static bool? PreEquals(this object lhs, object rhs)
        {
            // どちらもnullか、同じオブジェクトなら真を返します。
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            // objectとして比較します。比較の仕方を間違えると
            // 無限ループになるので注意が必要です。
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
            {
                return false;
            }

            // Equals(object obj)の中では普通、型を合わせて比較します。
            //   lhs.Equals(rhs as LhsType)
            // と
            //   rhs.Equals(lhs as RhsType)
            // の結果を合わせるためには、as演算子の結果も合わせる必要があり、
            // 継承があった場合には型の比較をしないと上手くいかなくなります。
            if (!lhs.GetType().Equals(rhs.GetType()))
            {
                return false;
            }

            return null;
        }

        /// <summary>
        /// オブジェクトをnull値などを考慮しながら比較します。
        /// </summary>
        /// <remarks>
        /// 主にoperator==の実装で使われます。
        /// </remarks>
        public static bool GenericEquals<T>(T lhs, T rhs)
        {
            if (!typeof(T).IsValueType)
            {
                // どちらもnullか、同じオブジェクトなら真を返します。
                if (ReferenceEquals(lhs, rhs))
                {
                    return true;
                }

                // objectとして比較します。比較の仕方を間違えると
                // 無限ループになるので注意が必要です。
                if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
                {
                    return false;
                }
            }

            return lhs.Equals(rhs);
        }

        /// <summary>
        /// TimeSpanのミリ秒部分を<paramref name="millis"/> / 1000 にします。
        /// </summary>
        public static TimeSpan MillisecondsTo(this TimeSpan self, int millis)
        {
            if (self == TimeSpan.MinValue || self == TimeSpan.MaxValue)
            {
                return self;
            }

            return TimeSpan.FromSeconds(
                Math.Floor(self.TotalSeconds) + ((double)millis / 1000.0));
        }

        /// <summary>
        /// TimeSpanのミリ秒部分を０にします。
        /// </summary>
        public static TimeSpan MillisecondsToZero(this TimeSpan self)
        {
            return MillisecondsTo(self, 0);
        }

        /// <summary>
        /// IDictionaryから指定のキーを持つ要素を探し、
        /// もしなければデフォルト値を返します。
        /// </summary>
        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dic,
                                                    TKey key)
        {
            TValue value;
            if (dic.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                return default(TValue);
            }
        }

        /// <summary>
        /// 全角文字を２文字分として文字数をカウントします。
        /// </summary>
        public static int HankakuLength(this string self)
        {
            if (string.IsNullOrEmpty(self))
            {
                return 0;
            }

            try
            {
                // sjisの2バイト文字はすべて全角なのでそれを利用して調べます。
                return SJisEncoding.GetByteCount(self);
            }
            catch (EncoderFallbackException)
            {
                return -1;
            }
        }

        /// <summary>
        /// 全角文字を２文字分として、必要な文字数以下になるように文字列を切り詰めます。
        /// </summary>
        /// <example>
        /// てすと => (3) => て
        /// てすと => (4) => てす
        /// てtt => (3) => てt
        /// oかeri => (3) => oか
        /// </example>
        public static string HankakuSubstring(this string self, int hankakuLength)
        {
            if (string.IsNullOrEmpty(self))
            {
                return self;
            }

            for (var length = Math.Min(hankakuLength, self.Length);
                 length > 0; --length)
            {
                var substr = self.Substring(0, length);
                var hanLen = substr.HankakuLength();

                // utf8 => sjisの変換に失敗した場合はすべて全角文字であると仮定します。
                hanLen = (hanLen >= 0 ? hanLen : substr.Length * 2);
                if (hanLen <= hankakuLength)
                {
                    return substr;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 空白文字か調べます。
        /// </summary>
        public static bool IsWhiteSpaceEx(this char self)
        {
            return (
                char.IsWhiteSpace(self) ||
                self == '\u200c' ||
                self == '\u200e');
        }

        /// <summary>
        /// 空白文字のみで構成されているか調べます。
        /// </summary>
        public static bool IsWhiteSpaceOnly(this string self)
        {
            if (string.IsNullOrEmpty(self))
            {
                return true;
            }

            return self.All(IsWhiteSpaceEx);
        }

        /// <summary>
        /// 文字列中の空白があるインデックスを取得します。
        /// </summary>
        public static int IndexOfWhitespace(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return -1;
            }

            for (var i = 0; i < text.Length; ++i)
            {
                if (char.IsWhiteSpace(text[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// <paramref name="c"/>が全角文字か調べます。
        /// </summary>
        public static bool IsZenkaku(this char c)
        {
            try
            {
                // sjisの2バイト文字はすべて全角なのでそれを利用して調べます。
                var count = SJisEncoding.GetByteCount(new char[] { c });

                return (count == 2);
            }
            catch (EncoderFallbackException)
            {
                // sjisに変換できない文字だった場合は
                // 全角文字であると仮定します。
                return false;
            }
        }

        /// <summary>
        /// 特定の文字で前後をくくります。
        /// </summary>
        public static string Quote(this string text, string c)
        {
            if (string.IsNullOrEmpty(c))
            {
                return text;
            }

            return (c + text + c);
        }

        /// <summary>
        /// 前後にある特定の文字列を削除します。
        /// </summary>
        public static string RemoveQuote(this string text, params char[] quotes)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (!quotes.Any())
            {
                quotes = new char[] {'\'', '\"'};
            }

            var trimmedText = text.TrimStart(quotes);
            return trimmedText.TrimEnd(quotes);
        }

        /// <summary>
        /// 指定のパスのファイルをバイト列として読み込みます。
        /// </summary>
        public static byte[] ReadFile(string filepath)
        {
            using (var stream = new FileStream(filepath, FileMode.Open))
            {
                return ReadToEnd(stream);
            }
        }

        /// <summary>
        /// ストリームの内容を読み出します。
        /// </summary>
        public static byte[] ReadToEnd(Stream stream)
        {
            using (var result = new MemoryStream())
            {
                var buffer = new byte[1024];
                var size = 0;

                while ((size = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    result.Write(buffer, 0, size);
                }

                result.Flush();
                return result.ToArray();
            }
        }

        /// <summary>
        /// 指定のパスのファイルを文字列にして読み込みます。
        /// </summary>
        public static string ReadFile(string filepath, Encoding encoding)
        {
            using (var stream = new FileStream(filepath, FileMode.Open))
            {
                return ReadToEnd(stream, encoding);
            }
        }

        /// <summary>
        /// ストリームの内容を指定のエンコーディングで読み出します。
        /// </summary>
        public static string ReadToEnd(Stream stream, Encoding encoding)
        {
            using (var reader = new StreamReader(stream, encoding))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// ストリームの内容を行ごとに読み込みます。
        /// </summary>
        public static IEnumerable<string> ReadLines(string filepath, Encoding encoding)
        {
            using (var stream = new FileStream(filepath, FileMode.Open))
            {
                foreach (var line in ReadLines(stream, encoding))
                {
                    yield return line;
                }
            }
        }

        /// <summary>
        /// ストリームの内容を行ごとに読み込みます。
        /// </summary>
        public static IEnumerable<string> ReadLines(Stream stream, Encoding encoding)
        {
            using (var reader = new StreamReader(stream, encoding))
            {
                while (!reader.EndOfStream)
                {
                    yield return reader.ReadLine();
                }
            }
        }

        /// <summary>
        /// パスセパレータなどを標準化します。
        /// </summary>
        public static string NormalizePath(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            // Normalize separator
            path = path.Replace('/', Path.DirectorySeparatorChar);
            path = path.Replace('\\', Path.DirectorySeparatorChar);
            return path;
        }

        /// <summary>
        /// 空の一時ファイルを作成し、そのファイル名を返します。
        /// </summary>
        public static string GetTempFileName()
        {
            return GetTempFileName(".tmp");
        }

        /// <summary>
        /// 空の一時ファイルを作成し、そのファイル名を返します。
        /// </summary>
        /// <remarks>
        /// Path.GetTempFileNameは一時ファイルの数が65535個を超えると
        /// 例外を発生させます。
        /// また、指定の拡張子でファイルを作ることもできないため、
        /// 新たにメソッドを作成しました。
        /// </remarks>
        public static string GetTempFileName(string extension)
        {
            Exception exception = null;

            for (var attempt = 0; attempt < 10; ++attempt)
            {
                var filename = Path.GetRandomFileName();
                filename = Path.ChangeExtension(filename, extension);
                filename = Path.Combine(Path.GetTempPath(), filename);

                try
                {
                    using (new FileStream(filename, FileMode.CreateNew))
                    {
                    }

                    return filename;
                }
                catch (IOException ex)
                {
                    exception = ex;
                }
            }

            throw new IOException(
                "一時ファイルの作成に失敗しました。",
                exception);
        }

        /// <summary>
        /// 安全にDisposeを行います。
        /// </summary>
        public static void SafeDispose<T>(ref T resource)
            where T : class
        {
            if (resource == null)
            {
                return;
            }

            var disposer = resource as IDisposable;
            if (disposer != null)
            {
                try
                {
                    disposer.Dispose();
                }
                catch
                {
                }
            }

            resource = null;
        }

        /// <summary>
        /// クラスフィールドの<see ref="LabelDescriptionAttribute"/>
        /// 属性を取得します。
        /// </summary>
        public static LabelDescriptionAttribute GetFieldLabelAttribute<T>(
            Type type, T value)
        {
            var fields = type.GetFields(
                BindingFlags.Static |
                BindingFlags.Public);

            // LabelDescriptionAttributeを持ち、フィールドの値が
            // valueと同じフィールドを探します。
            return fields
                .Where(_ => value.Equals(_.GetValue(null)))
                .Select(_ => _.GetCustomAttributes(
                    typeof(LabelDescriptionAttribute), true))
                .Where(_ => _.Any())
                .OfType<LabelDescriptionAttribute>()
                .FirstOrDefault();
        }

        /// <summary>
        /// クラスフィールドの説明を取得します。
        /// </summary>
        public static string GetFieldDescription<T>(Type type, T value)
        {
            var attribute = GetFieldLabelAttribute(type, value);
            if (attribute == null)
            {
                return null;
            }

            return attribute.Description;
        }

        /// <summary>
        /// クラスフィールドのラベルを取得します。
        /// </summary>
        public static string GetFieldLabel<T>(Type type, T value)
        {
            var attribute = GetFieldLabelAttribute(type, value);
            if (attribute == null)
            {
                return null;
            }

            return attribute.Label;
        }

        /// <summary>
        /// 必要なら親をさかのぼってディレクトリを作成します。
        /// </summary>
        public static void CreateDirectoryRecursive(string dirName)
        {
            if (string.IsNullOrEmpty(dirName))
            {
                return;
            }

            if (Directory.Exists(dirName))
            {
                // すでにあるし。
                return;
            }

            // 親ディレクトリを作成。
            var parentDirName = Path.GetDirectoryName(dirName);
            CreateDirectoryRecursive(parentDirName);

            // 自ディレクトリを作成。
            Directory.CreateDirectory(dirName);
        }

        /// <summary>
        /// CurrentDomainのアセンブリ一覧から特定の名前を持つ型を探します。
        /// </summary>
        /// <remarks>
        /// 型の取得において、自ら読み込んだアセンブリではType.GetType(string)
        /// に失敗することがあります。
        /// </remarks>
        public static Type FindTypeFromCurrentDomain(string name)
        {
            var type = Type.GetType(name);
            if ((object)type != null)
            {
                return type;
            }

            return GetCurrentDomainAssembliesType()
                .Select(_ => _.GetType(name))
                .FirstOrDefault(_ => _ != null);
        }

        /// <summary>
        /// CurrentDomainのアセンブリ一覧を取得します。
        /// </summary>
        public static IEnumerable<Assembly> GetCurrentDomainAssembliesType()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        /// <summary>
        /// 対象がGC可能な弱い参照を持つイベントハンドラを作成します。
        /// </summary>
        public static EventHandler MakeWeak(
            this EventHandler eventHandler,
            UnregisterCallback unregister)
        {
            if (eventHandler == null)
            {
                throw new ArgumentNullException("eventHandler");
            }

            if (eventHandler.Method.IsStatic || eventHandler.Target == null)
            {
                // staticメソッドの場合はそのまま返します。
                return eventHandler;
            }

            var wehType = typeof(WeakEventHandler<>).MakeGenericType(
                eventHandler.Method.DeclaringType);

            var weh = (IWeakEventHandler)Activator.CreateInstance(
                wehType,
                new object[]
                {
                    eventHandler,
                    unregister,
                });

            return weh.Handler;
        }

        /// <summary>
        /// 対象がGC可能な弱い参照を持つイベントハンドラを作成します。
        /// </summary>
        public static EventHandler<TEventArgs> MakeWeak<TEventArgs>(
            this EventHandler<TEventArgs> eventHandler,
            UnregisterCallback<TEventArgs> unregister)
            where TEventArgs : EventArgs
        {
            if (eventHandler == null)
            {
                throw new ArgumentNullException("eventHandler");
            }

            if (eventHandler.Method.IsStatic || eventHandler.Target == null)
            {
                // staticメソッドの場合はそのまま返します。
                return eventHandler;
            }

            var wehType = typeof(WeakEventHandler<,>).MakeGenericType(
                eventHandler.Method.DeclaringType, typeof(TEventArgs));

            var weh = (IWeakEventHandler<TEventArgs>)Activator.CreateInstance(
                wehType,
                new object[]
                {
                    eventHandler,
                    unregister,
                });

            return weh.Handler;
        }
    }
}
