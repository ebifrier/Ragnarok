using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Utility
{
    /// <summary>
    /// Enumに付随するラベル名などを取得可能なクラスです。
    /// </summary>
    public class EnumWrapper<T>
    {
        /// <summary>
        /// 列挙値を取得または設定します。
        /// </summary>
        public T Value
        {
            get;
            set;
        }

        /// <summary>
        /// 設定されたラベル名を取得します。
        /// </summary>
        public string Label
        {
            get
            {
                var label = EnumUtil.GetLabel(Value);
                if (!string.IsNullOrEmpty(label))
                {
                    return label;
                }

                return Value.ToString();
            }
        }

        /// <summary>
        /// 設定された説明を取得します。
        /// </summary>
        public string Description
        {
            get { return EnumUtil.GetDescription(Value); }
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public override bool Equals(object obj)
        {
            var wrapper = obj as EnumWrapper<T>;
            if (wrapper != null)
            {
                return Value.Equals(wrapper.Value);
            }

            // 列挙値として比較
            return Value.Equals(obj);
        }

        /// <summary>
        /// ハッシュ値を取得します。
        /// </summary>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EnumWrapper(T value)
        {
            Value = value;
        }
    }
}
