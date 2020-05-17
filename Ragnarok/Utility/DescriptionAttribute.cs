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
    public class DescriptionAttribute : Attribute
    {
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
        public DescriptionAttribute()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DescriptionAttribute(string description)
        {
            this.Description = description;
        }
    }
}
