using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Utility
{
    /// <summary>
    /// プロパティなどに説明文をつけます。
    /// </summary>
    /// <remarks>
    /// <see cref="System.ComponentModel.DescriptionAttribute"/>
    /// とほぼ同じです。将来、仕様が変わる可能性があるため
    /// 別クラスとして作りました。
    /// </remarks>
    [AttributeUsage(AttributeTargets.All)]
    public class LabelDescriptionAttribute : Attribute
    {
        /// <summary>
        /// その要素のラベル名を取得または設定します。
        /// </summary>
        public string Label
        {
            get;
            set;
        }

        /// <summary>
        /// 説明文を取得または設定します。
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LabelDescriptionAttribute()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LabelDescriptionAttribute(string description)
        {
            this.Description = description;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LabelDescriptionAttribute(string description, string label)
        {
            this.Label = label;
            this.Description = description;
        }
    }
}
