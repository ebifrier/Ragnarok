using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi.File
{
    /// <summary>
    /// kifファイルの各ヘッダアイテムを保持します。
    /// </summary>
    public sealed class HeaderItem
    {
        /// <summary>
        /// キーを取得します。
        /// </summary>
        public string Key
        {
            get;
            private set;
        }

        /// <summary>
        /// 値を取得します。
        /// </summary>
        public string Value
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HeaderItem(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
