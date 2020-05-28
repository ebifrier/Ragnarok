﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ragnarok.Utility;

namespace Ragnarok.Net.ProtoBuf
{
    /// <summary>
    /// protobuf用の送受信時のエラーコードです。
    /// </summary>
    /// <remarks>
    /// ０より大きい値はアプリケーション側が好きなように使うことが出来ます。
    /// </remarks>
    public static class PbErrorCode
    {
        /// <summary>
        /// エラーはありません。
        /// </summary>
        [Description("エラーはありません。")]
        public const int None = 0;

        /// <summary>
        /// 不明なエラーです。
        /// </summary>
        [Description("不明なエラーです。")]
        public const int Unknown = -1;

        /// <summary>
        /// データ処理中に例外が発生しました。
        /// </summary>
        [Description("データ処理中に例外が発生しました。")]
        public const int HandlerException = -2;

        /// <summary>
        /// 通信プロトコルのバージョンエラーです。
        /// </summary>
        [Description("通信プロトコルのバージョンエラーです。")]
        public const int VersionError = -3;

        /// <summary>
        /// レスポンスが返る前にタイムアウトしました。
        /// </summary>
        [Description("通信がタイムアウトしました。")]
        public const int Timeout = -4;
    }
}
