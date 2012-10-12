using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.ObjectModel
{
    /// <summary>
    /// 型とその型が持つプロパティ名をセットで持ちます。
    /// </summary>
    internal class TypeProperty
    {
        /// <summary>
        /// 対象となる型を取得します。
        /// </summary>
        public Type Type
        {
            get;
            private set;
        }

        /// <summary>
        /// プロパティ名を取得します。
        /// </summary>
        public string PropertyName
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TypeProperty(Type type, string propertyName)
        {
            this.Type = type;
            this.PropertyName = propertyName;
        }
    }
}
