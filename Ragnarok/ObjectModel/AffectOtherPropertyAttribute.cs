using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.ObjectModel
{
    /// <summary>
    /// 他プロパティに影響を与える場合に使います。
    /// </summary>
    [Obsolete("DependOnPropertyAttributeを使ってください。")]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class AffectOtherPropertyAttribute : Attribute
    {
        /// <summary>
        /// 影響を与えるプロパティの名前を取得します。
        /// </summary>
        public string AffectProperty
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AffectOtherPropertyAttribute(string propertyName)
        {
            this.AffectProperty = propertyName;
        }
    }
}
