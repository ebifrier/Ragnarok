using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Forms.Bindings
{
    /// <summary>
    /// バインディングモードです。
    /// </summary>
    public enum BindingMode
    {
        /// <summary>
        /// デフォルト
        /// </summary>
        Default,
        /// <summary>
        /// ソースからコントロール側へのデータ変更のみを扱います。
        /// </summary>
        OneWay,
        /// <summary>
        /// コントロールからソース側へのデータ変更のみを扱います。
        /// </summary>
        OneWayToSource,
        /// <summary>
        /// コントロールとソース、両方のデータ変更を通知します。
        /// </summary>
        TwoWay,
    }
}
