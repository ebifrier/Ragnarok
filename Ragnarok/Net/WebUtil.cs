using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.ComponentModel;

namespace Ragnarok.Net
{
    /// <summary>
    /// Web関連のユーティリティーメソッドを持ちます。
    /// </summary>
    public static class WebUtil
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        static WebUtil()
        {
            DefaultTimeout = -1;
            IsConvertPostParamSpaceToPlus = false;
            
            // セキュリティ証明をパスします。
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }

        /// <summary>
        /// デフォルトのタイムアウト時間を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 単位はミリセカンドで、負数の場合はシステムのデフォルト値を使います。
        /// </remarks>
        public static int DefaultTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// POSTパラメータの半角空白を+記号に置き換えるかどうかを取得または設定します。
        /// </summary>
        public static bool IsConvertPostParamSpaceToPlus
        {
            get;
            set;
        }

        /// <summary>
        /// html文字列をエンコードします。
        /// </summary>
        public static string EncodeHtmlText(string text)
        {
            return WebUtility.HtmlEncode(text);
        }

        /// <summary>
        /// html文字列をデコードします。
        /// </summary>
        public static string DecodeHtmlText(string text)
        {
			return WebUtility.HtmlDecode(text);
        }

        /// <summary>
        /// htmlをエンコードします。
        /// </summary>
        public static string EncodeHtml(string text)
        {
            text = EncodeHtmlText(text);
            text = (IsConvertPostParamSpaceToPlus ?
                text.Replace(" ", "+") :
                text.Replace(" ", "&nbsp;")); // 空白文字・半角スペース
            return text;
        }

        /// <summary>
        /// htmlをデコードします。
        /// </summary>
        public static string DecodeHtml(string text)
        {
            text = DecodeHtmlText(text);
            text = (IsConvertPostParamSpaceToPlus ?
                text.Replace("+", " ") :
                text.Replace("&nbsp;", " ")); // 空白文字・半角スペース
            return text;
        }

        /// <summary>
        /// 与えられたパラメータをエンコードします。
        /// </summary>
        public static string EncodeParam(Dictionary<string, object> param)
        {
            if (param == null)
            {
                return null;
            }

            var enDataStrs = param.Select((pair) =>
            {
                string value = string.Empty;

                if (pair.Value != null)
                {
                    value = Uri.EscapeDataString(pair.Value.ToString());
                    if (!string.IsNullOrEmpty(value) && IsConvertPostParamSpaceToPlus)
                    {
                        value = value.Replace("%20", "+");
                    }
                }

                return $"{pair.Key}={value}";
            });

            return string.Join("&", enDataStrs.ToArray());
        }

        /// <summary>
        /// ポストするURLデータをエンコードします。
        /// </summary>
        public static byte[] EncodePostData(Dictionary<string, object> param)
        {
            if (param == null)
            {
                return null;
            }

            var encoded = EncodeParam(param);

            return Encoding.UTF8.GetBytes(encoded);
        }

        /// <summary>
        /// このアプリの標準的なHTTPリクエストを作成します。
        /// </summary>
        public static HttpWebRequest MakeNormalRequest(string url,
                                                       CookieContainer cc)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.UserAgent = "Ragnarok.NicoNico";
            request.AutomaticDecompression =
                DecompressionMethods.Deflate |
                DecompressionMethods.GZip;
            request.CookieContainer = cc;
            request.KeepAlive = false;

            // タイムアウトのデフォルト値が設定されていれば
            // それを使ってネットワークに接続します。
            if (DefaultTimeout > 0)
            {
                request.Timeout = DefaultTimeout;
            }

            return request;
        }

        /// <summary>
        /// POSTするデータを書き込みます。
        /// </summary>
        public static void WritePostData(HttpWebRequest request,
                                         Dictionary<string, object> param)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // パラメータがあれば、POSTを無ければGETを使います。
            var data = EncodePostData(param);
            if (data != null && data.Any())
            {
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var reqStream = request.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                }
            }
            else
            {
                request.Method = "GET";
            }
        }

        /// <summary>
        /// HTTPの取得要求を出します。
        /// </summary>
        public static byte[] RequestHttp(string url,
                                         Dictionary<string, object> param)
        {
            return RequestHttp(url, param, null);
        }

        /// <summary>
        /// HTTPの取得要求を出します。
        /// </summary>
        public static string RequestHttpText(string url,
                                             Dictionary<string, object> param,
                                             CookieContainer cc,
                                             Encoding encoding = null)
        {
            var buffer = RequestHttp(url, param, cc);
            if (buffer == null)
            {
                return null;
            }

            encoding = encoding ?? Encoding.UTF8;
            return encoding.GetString(buffer);
        }

        /// <summary>
        /// HTTPリクエストを出します。
        /// </summary>
        public static byte[] RequestHttp(string url,
                                         Dictionary<string, object> param,
                                         CookieContainer cc)
        {
            HttpWebRequest request = null;

            try
            {
                request = MakeNormalRequest(url, cc);
                WritePostData(request, param);

                return RequestHttp(request);
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        /// <summary>
        /// HTTPリクエストを出します。
        /// </summary>
        /// <exception cref="WebException" />
        public static byte[] RequestHttp(HttpWebRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // responseをCloseしないと、MONOでは次回のリクエストから
            // タイムアウトするようになります。
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response == null)
                {
                    return null;
                }

                // レスポンスをすべて読み出します。
                using (var stream = response.GetResponseStream())
                {
                    return Util.ReadToEnd(stream);
                }
            }
        }

        /// <summary>
        /// httpの非同期受信時に使われます。
        /// </summary>
        public delegate void RequestHttpAsyncCallback(IAsyncResult result,
                                                      byte[] data);

        /// <summary>
        /// httpの非同期受信時に使われます。
        /// </summary>
        public delegate void RequestHttpTextAsyncCallback(IAsyncResult result,
                                                          string text);

        /// <summary>
        /// httpデータ受信時のバッファリングサイズです。
        /// </summary>
        private const int BUFFERING_SIZE = 1024;

        /// <summary>
        /// 非同期でHTTPの取得要求を出します。
        /// </summary>
        public static IAsyncResult RequestHttpTextAsync(
            string url,
            Dictionary<string, object> param,
            CookieContainer cc, Encoding encoding,
            RequestHttpTextAsyncCallback callback)
        {
            return RequestHttpAsync(url, param, cc,
                (result, data) =>
                    callback(
                        result,
                        (data == null ? null : encoding.GetString(data))));
        }

        /// <summary>
        /// 非同期のHTTPリクエストを出します。
        /// </summary>
        public static IAsyncResult RequestHttpAsync(
            string url,
            Dictionary<string, object> param,
            CookieContainer cc,
            RequestHttpAsyncCallback callback)
        {
            var request = MakeNormalRequest(url, cc);
            WritePostData(request, param);

            return RequestHttpAsync(request, callback);
        }

        /// <summary>
        /// GetHttpResponseDoneが受け取るデータ型です。
        /// </summary>
        private class GetHttpResponseDoneData
        {
            public HttpWebRequest WebRequest;
            public RequestHttpAsyncCallback Callback;
        }

        /// <summary>
        /// ReadHttpStreamDoneが受け取るデータ型です。
        /// </summary>
        private class ReadHttpStreamDoneData
        {
            public Stream InputStream;
            public MemoryStream OutputStream;
            public byte[] Buffer;
            public RequestHttpAsyncCallback Callback;
        }

        /// <summary>
        /// 非同期のHTTPリクエストを出します。
        /// </summary>
        public static IAsyncResult RequestHttpAsync(
            HttpWebRequest request,
            RequestHttpAsyncCallback callback)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var data = new GetHttpResponseDoneData()
            {
                WebRequest = request,
                Callback = callback,
            };

            return request.BeginGetResponse(
                GetHttpResponseDone,
                data);
        }

        /// <summary>
        /// 非同期HTTPリクエストのレスポンスを取得したときに呼ばれます。
        /// </summary>
        private static void GetHttpResponseDone(IAsyncResult result)
        {
            var data = (GetHttpResponseDoneData)result.AsyncState;

            try
            {
                var response = data.WebRequest.EndGetResponse(result);
                if (response == null)
                {
                    data.Callback(result, null);
                    return;
                }

                // 入出力ストリームです。
                var inputStream = response.GetResponseStream();
                var outputStream = new MemoryStream();
                var buffer = new byte[BUFFERING_SIZE];

                var callData = new ReadHttpStreamDoneData()
                {
                    InputStream = inputStream,
                    OutputStream = outputStream,
                    Buffer = buffer,
                    Callback = data.Callback,
                };

                // レスポンスデータの読み込みを開始します。
                inputStream.BeginRead(
                    buffer, 0, buffer.Length,
                    ReadHttpStreamDone,
                    callData);
            }
            catch
            {
                data.Callback(result, null);
            }
        }

        /// <summary>
        /// 非同期HTTPレスポンスの読み込みが完了した時に呼ばれます。
        /// </summary>
        private static void ReadHttpStreamDone(IAsyncResult result)
        {
            var data = (ReadHttpStreamDoneData)result.AsyncState;

            try
            {
                var readSize = data.InputStream.EndRead(result);
                if (readSize == 0)
                {
                    // すべてのデータを受信した。
                    data.OutputStream.Capacity = (int)data.OutputStream.Length;
                    data.OutputStream.Flush();
                    data.Callback(result, data.OutputStream.GetBuffer());

                    data.InputStream.Dispose();
                    data.OutputStream.Dispose();
                    return;
                }

                // バッファに読み込んだデータを書き込みます。
                data.OutputStream.Write(data.Buffer, 0, readSize);

                // 再度データを読み込みます。
                data.InputStream.BeginRead(
                    data.Buffer, 0, data.Buffer.Length,
                    ReadHttpStreamDone,
                    data);
            }
            catch
            {
                data.Callback(result, null);
            }
        }
    }
}
