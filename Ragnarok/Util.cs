using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Ragnarok
{
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
        public static readonly Encoding SJisEncoding;
        public static CultureInfo DefaultCulture;

        private static PropertyChangedEventCaller propertyChangedCaller;
        private static CollectionChangedEventCaller collectionChangedCaller;
        private static Action<Action> eventCaller;

        static Util()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            SJisEncoding = Encoding.GetEncoding("shift_jis");
            DefaultCulture = new("ja-JP");
        }

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
        /// Actionをエラー処理つきで呼び出します。
        /// </summary>
        public static void SafeCall(Action caller, bool logging = true)
        {
            try
            {
                caller?.Invoke();
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

            return default;
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
            if (lhs is null || rhs is null)
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
                if (lhs is null || rhs is null)
                {
                    return false;
                }
            }

            return lhs.Equals(rhs);
        }

        /// <summary>
        /// default(type)を取得します。
        /// </summary>
        public static object GetDefaultValue(Type type)
        {
            if (type != null && type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        /// <summary>
        /// ストリームの内容を読み出します。
        /// </summary>
        public static byte[] ReadToEnd(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var result = new MemoryStream();
            var buffer = new byte[1024];
            var size = 0;

            while ((size = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                result.Write(buffer, 0, size);
#if MONO
                    // MONOだとReadがタイムアウトするため
                    if (size < buffer.Length) break;
#endif
            }

            result.Flush();
            return result.ToArray();
        }

        /// <summary>
        /// ストリームの内容を指定のエンコーディングで読み出します。
        /// </summary>
        public static string ReadToEnd(Stream stream, Encoding encoding)
        {
            using var reader = new StreamReader(stream, encoding);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// 埋め込みリソースからテキストを取得します。
        /// </summary>
        /// <remarks>
        /// <paramref name="resourcePath"/>には名前空間を含めたパスを指定してください。
        /// 例えば、"Ragnarok.Util.TestResource.txt" などになります。
        /// <paramref name="encoding"/>は指定がなければUTF8になります。
        /// </remarks>
        public static string GetResourceString(Assembly asm,
                                               string resourcePath,
                                               Encoding encoding = null)
        {
            using var stream = asm.GetManifestResourceStream(resourcePath);
            using var reader = new StreamReader(
                stream, encoding ?? Encoding.UTF8);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// ストリームの内容を行ごとに読み込みます。
        /// </summary>
        public static IEnumerable<string> ReadLines(Stream stream, Encoding encoding)
        {
            using var reader = new StreamReader(stream, encoding);
            while (!reader.EndOfStream)
            {
                yield return reader.ReadLine();
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
    }
}
