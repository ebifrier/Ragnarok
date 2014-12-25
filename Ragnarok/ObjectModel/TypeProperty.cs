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
        /// Type.op_EqualityがMONOだと使えないため。
        /// </summary>
        public override bool Equals(object obj)
        {
            var status = this.PreEquals(obj);
            if (status != null)
            {
                return status.Value;
            }

            return Equals(obj as TypeProperty);
        }

        /// <summary>
        /// Type.op_EqualityがMONOだと使えないため。
        /// </summary>
        public bool Equals(TypeProperty other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return (
                Util.GenericEquals(Type, other.Type) &&
                PropertyName == other.PropertyName);
        }

        /// <summary>
        /// ハッシュ値を計算します。
        /// </summary>
        public override int GetHashCode()
        {
            return (Type.GetHashCode() ^ PropertyName.GetHashCode());
        }

        /// <summary>
        /// Type.op_EqualityがMONOだと使えないため。
        /// </summary>
        public static bool operator==(TypeProperty x, TypeProperty y)
        {
            return Util.GenericEquals(x, y);
        }

        /// <summary>
        /// Type.op_EqualityがMONOだと使えないため。
        /// </summary>
        public static bool operator!=(TypeProperty x, TypeProperty y)
        {
            return !(x == y);
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
