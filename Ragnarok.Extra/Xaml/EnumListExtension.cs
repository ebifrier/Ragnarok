using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;

namespace Ragnarok.Extra.Xaml
{
    /// <summary>
    /// Enumに付随するラベル名などを取得可能なクラスです。
    /// </summary>
    public class EnumWrapper : Utility.EnumWrapper<object>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EnumWrapper(object value)
            : base(value)
        {
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
            if ((object)EnumType == null)
            {
                throw new InvalidOperationException(
                    "列挙子の型が設定されていません。");
            }

            // OfTypeはLinqを使うために必要です。
            return Enum.GetValues(EnumType)
                .OfType<object>()
                .Select(_ => new EnumWrapper(_))
                .ToList();
        }
    }
}
