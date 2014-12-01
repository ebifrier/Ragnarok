using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Extra.Effect
{
    /// <summary>
    /// エフェクトに適用される引数の定義を行います。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class EffectArgumentAttribute : Attribute
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EffectArgumentAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// エフェクト引数のデフォルト値を取得します。
        /// </summary>
        /// <remarks>
        /// エフェクトファイルのPreLoadで使用されます。
        /// </remarks>
        public object DefaultValue
        {
            get;
            private set;
        }
    }
}
