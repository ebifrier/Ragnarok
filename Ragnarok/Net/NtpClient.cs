using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Ragnarok.Net
{
    using MathEx;

    /// <summary>
    /// NTPサーバーから時刻を取得します。
    /// 
    /// http://tools.ietf.org/html/rfc2030
    /// http://www.geocities.jp/heartland_cosmos_2211/rfc2030.html
    /// </summary>
    /// <remarks>
    /// 時刻同期には時間がかかることがあるため、別スレッドで同期します。
    /// </remarks>
    public static class NtpClient
    {
        /// <summary>
        /// 時刻同期の間隔です。
        /// </summary>
        private static readonly TimeSpan SyncInterval = TimeSpan.FromDays(1);
        /// <summary>
        /// 時刻を裏で同期するためのタイマーを開始します。
        /// </summary>
        /// <remarks>
        /// 時刻同期には時間がかかることがあるため、裏で同期します。
        /// </remarks>
        private static readonly Timer timer = new Timer(
            _ => SynchronizeTime(),
            null,
            TimeSpan.Zero,
            SyncInterval);
        private static TimeSpan offsetSpan = TimeSpan.Zero;

        /// <summary>
        /// NTPサーバーへのリクエスト結果を保持します。
        /// </summary>
        private class NtpClockInfo
        {
            /// <summary>
            /// クライアントがリクエストを送った時刻です。
            /// </summary>
            public DateTime ReferenceTime;
            /// <summary>
            /// クライアントがレスポンスを受け取った時刻です。
            /// </summary>
            public DateTime OriginateTime;
            /// <summary>
            /// NTPサーバーがリクエストを受け取った時刻です。
            /// </summary>
            public DateTime ReceiveTime;
            /// <summary>
            /// NTPサーバーがレスポンスを送った時刻です。
            /// </summary>
            public DateTime TransmitTime;

            /// <summary>
            /// クライアント時刻とサーバー時刻の差を計算します。
            /// </summary>
            /// <remarks>
            /// ((receive - reference) + (transmit - originate)) / 2
            /// </remarks>
            public TimeSpan GetOffset()
            {
                var dif1 = ReceiveTime - ReferenceTime;
                var dif2 = TransmitTime - OriginateTime;
                var msecs = (dif1 + dif2).TotalMilliseconds / 2;
                
                return TimeSpan.FromMilliseconds(msecs);
            }
        }

        /// <summary>
        /// ntpのサーバーリストです。
        /// </summary>
        private static readonly string[] NtpServerList =
        {
            "ntp.nict.jp",
            "jp.pool.ntp.org",
            "ntp1.jst.mfeed.ad.jp",
            "ntp3.jst.mfeed.ad.jp",
            "ntp4.jst.mfeed.ad.jp",
        };

        /// <summary>
        /// NTPサーバーから得られた日本標準時を利用して、
        /// 定期的に同期された現在時刻を取得します。
        /// </summary>
        public static DateTime GetTime()
        {
            var now = DateTime.Now;

            // ネットワークでの時刻同期には時間がかかることがあります。
            // 実用性を考えて、時刻が正しく同期されていなくても
            // 気にせずに時間を返します。
            return (now + offsetSpan);
        }

        /// <summary>
        /// NTPサーバーと時刻を強制同期します。
        /// </summary>
        private static void SynchronizeTime()
        {
            try
            {
                // NTPサーバーから時刻を取得します。
                var clockInfo = GetNetworkClockInfo();

                offsetSpan = clockInfo.GetOffset();
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "NTPサーバーから時刻の取得に失敗しました。");
            }
        }

        /// <summary>
        /// NTPサーバーから得られたバイナリデータから各種時刻を取得します。
        /// </summary>
        private static DateTime GetTimeFromBinary(byte[] ntpData, int offset)
        {
            ulong intpart = 0;
            ulong fracpart = 0;

            // 秒数を取得します。(big endian)
            for (var i = 0; i < 4; ++i)
            {
                intpart = 256 * intpart + ntpData[offset + i];
            }

            // 秒数の小数点以下を取得します。
            for (var i = 4; i < 8; ++i)
            {
                fracpart = 256 * fracpart + ntpData[offset + i];
            }

            var seconds = (double)intpart + ((double)fracpart / 0x100000000L);

            // 秒の最上位ビットが1のときは、1968～2036年を
            // 0のときは2036年以降を表します。
            var dateTime =
                ( (intpart & 0x80000000) != 0
                ? new DateTime(1900, 1, 1)
                : new DateTime(2036, 1, 1));

            dateTime += TimeSpan.FromSeconds(seconds);
            return dateTime.ToLocalTime();
        }

        /// <summary>
        /// ntpサーバーから日本標準時を取得します。
        /// </summary>
        private static NtpClockInfo GetNetworkClockInfo()
        {
            var count = NtpServerList.Length;
            var start = MathUtil.RandInt(0, count);

            // サーバー不可を分散するため、開始サーバーはランダムに選択します。
            for (var i = start; i < start + count; ++i)
            {
                try
                {
                    var host = NtpServerList[i % count];

                    // 存在したNTPサーバーから時刻を取得します。
                    return GetNetworkClockInfo(host, 123);
                }
                catch (Exception)
                {
                    // 例外は無視します。
                    /*Log.ErrorException(ex,
                        "{0}: 時刻の取得に失敗しました。", address);*/
                }
            }

            throw new InvalidOperationException(
                "ntpサーバーから時刻が取得できませんでした。");
        }

        /// <summary>
        /// <paramref name="host"/> から現在時刻を取得します。
        /// </summary>
        private static NtpClockInfo GetNetworkClockInfo(string host, int port)
        {
            using (var socket = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Dgram,
                ProtocolType.Udp)
            {
                SendTimeout = 2000,
                ReceiveTimeout = 2000,
            })
            using (new Utility.ActionOnDispose(socket.Close))
            {
                socket.Connect(host, port);

                var ntpData = new byte[48]; // RFC 2030
                ntpData[0] = 0x1B;

                // リクエスト送信時刻を保存しておきます。
                var referenceTime = DateTime.Now;

                // NTPサーバーからデータを取得します。
                var size = socket.Send(ntpData);
                if (size != ntpData.Length)
                {
                    throw new InvalidOperationException(
                        "データの送信に失敗しました。");
                }

                // 受信処理
                size = socket.Receive(ntpData);
                if (size != ntpData.Length)
                {
                    throw new InvalidOperationException(
                        "データの受信に失敗しました。");
                }

                return new NtpClockInfo()
                {
                    ReferenceTime = referenceTime,
                    OriginateTime = DateTime.Now,
                    ReceiveTime = GetTimeFromBinary(ntpData, 32),
                    TransmitTime = GetTimeFromBinary(ntpData, 40),
                };
            }
        }
    }
}
