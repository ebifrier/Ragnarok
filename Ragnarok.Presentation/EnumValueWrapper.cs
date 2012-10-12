using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Wpf
{
    /// <summary>
    /// 列挙子の値からラベルや説明を取得可能なラッパークラスです。
    /// </summary>
    public class EnumValueWrapper : IEquatable<EnumValueWrapper>
    {
        /// <summary>
        /// 列挙子の値を取得または設定します。
        /// </summary>
        public object Value
        {
            get;
            set;
        }

        /// <summary>
        /// 列挙子に設定されたラベルを取得します。
        /// </summary>
        public string Label
        {
            get
            {
                return Util.GetEnumLabel(Value);
            }
        }

        /// <summary>
        /// 列挙子に設定された説明を取得します。
        /// </summary>
        public string Description
        {
            get
            {
                return Util.GetEnumDescription(Value);
            }
        }

        /// <summary>
        /// オブジェクトの等値性を比較します。
        /// </summary>
        public bool Equals(EnumValueWrapper wrapper)
        {
            if (wrapper == null)
            {
                return false;
            }

            return Equals(wrapper.Value);
        }

        /// <summary>
        /// オブジェクトの等値性を比較します。
        /// </summary>
        public override bool Equals(object obj)
        {
            var wrapper = obj as EnumValueWrapper;
            if (wrapper != null)
            {
                // 比較するオブジェクトはWrapperのValueです。
                obj = wrapper.Value;
            }

            if (obj == null)
            {
                return false;
            }

            return obj.Equals(Value);
        }

        /// <summary>
        /// ハッシュコードを取得します。
        /// </summary>
        public override int GetHashCode()
        {
            if (Value == null)
            {
                return 0;
            }

            return Value.GetHashCode();
        }

        /// <summary>
        /// 比較します。
        /// </summary>
        public static bool operator ==(EnumValueWrapper x, EnumValueWrapper y)
        {
            return Util.GenericClassEquals(x, y);
        }

        /// <summary>
        /// 比較します。
        /// </summary>
        public static bool operator !=(EnumValueWrapper x, EnumValueWrapper y)
        {
            return !(x == y);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EnumValueWrapper(object value)
        {
            Value = value;
        }
    }
}
