using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok
{
    /// <summary>
    /// 開始時に呼んでほしい処理があります。
    /// </summary>
    public static class Initializer
    {
        /// <summary>
        /// 初期化処理を行います。
        /// </summary>
        public static void Initialize()
        {
            // 静的コンストラクタで時刻同期を行います。
            Net.NtpClient.GetTime();

            // Util.SJisEncodingを外部ライブラリから初期化すると
            // 依存関係が足りないというエラーが出ます。
            // static変数なので、一度初期化すれば問題ありません。
            Util.SJisEncoding.GetEncoder();
        }
    }
}
