using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace Ragnarok.Presentation.Extension
{
    /// <summary>
    /// Enumに付随するラベル名などを取得可能なクラスです。
    /// </summary>
    public class EnumWrapper
    {
        /// <summary>
        /// 列挙値を取得または設定します。
        /// </summary>
        public object Value
        {
            get;
            set;
        }

        /// <summary>
        /// 設定されたラベル名を取得します。
        /// </summary>
        public string Label
        {
            get { return EnumEx.GetEnumLabel(Value); }
        }

        /// <summary>
        /// 設定された説明を取得します。
        /// </summary>
        public string Description
        {
            get { return EnumEx.GetEnumDescription(Value); }
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public override bool Equals(object obj)
        {
            var wrapper = obj as EnumWrapper;
            if ((object)wrapper != null)
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
        public EnumWrapper(object value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// 指定のディレクトリにあるファイル一覧を取得する拡張構文です。
    /// </summary>
    [MarkupExtensionReturnType(typeof(List<EnumWrapper>))]
    public class EnumListExtension : MarkupExtension
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EnumListExtension()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EnumListExtension(Type enumType)
        {
            EnumType = enumType;
        }

        /// <summary>
        /// 基本となるディレクトリを取得または設定します。
        /// </summary>
        [DefaultValue((Type)null)]
        public Type EnumType
        {
            get;
            set;
        }

        /// <summary>
        /// ファイル一覧を取得します。
        /// </summary>
        public override object ProvideValue(IServiceProvider service)
        {
            if (EnumType == null)
            {
                throw new InvalidOperationException(
                    "列挙子の型が設定されていません。");
            }

            var result = new List<EnumWrapper>();
            foreach (var value in Enum.GetValues(EnumType))
            {
                result.Add(new EnumWrapper(value));
            }

            return result;
        }
    }
}
