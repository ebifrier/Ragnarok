using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.ObjectModel
{
    /// <summary>
    /// モデルが他モデルのプロパティに依存しているときに使います。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DependOnPropertyAttribute : Attribute
    {
        /// <summary>
        /// 依存先のプロパティを持つ型を取得します。
        /// </summary>
        public Type DependType
        {
            get;
            private set;
        }

        /// <summary>
        /// 依存先のプロパティ名を取得します。
        /// </summary>
        public string DependName
        {
            get;
            private set;
        }

        /// <summary>
        /// ハッシュコードを取得します。
        /// </summary>
        public override int GetHashCode()
        {
            return (
                this.DependType.GetHashCode() ^
                this.DependName.GetHashCode());
        }

        /// <summary>
        /// オブジェクトの内容が等しいかどうか調べます。
        /// </summary>
        public override bool Match(object obj)
        {
            var other = obj as DependOnPropertyAttribute;
            if (other == null)
            {
                return false;
            }

            if (this.DependType != other.DependType)
            {
                return false;
            }

            if (this.DependName != other.DependName)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DependOnPropertyAttribute(Type dependType, string dependName)
        {
            this.DependType = dependType;
            this.DependName = dependName;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DependOnPropertyAttribute(Type dependType)
        {
            this.DependType = dependType;
            this.DependName = null;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DependOnPropertyAttribute(string dependName)
        {
            this.DependName = dependName;
        }
    }
}
