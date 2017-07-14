using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Utility
{
    /// <summary>
    /// プロパティなどに説明文をつけます。
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class LabelAttribute : Attribute
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LabelAttribute(string label)
        {
            Label = label;
        }

        /// <summary>
        /// その要素のラベル名を取得または設定します。
        /// </summary>
        public string Label
        {
            get;
            set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LabelAttribute()
        {
        }
    }
}
