using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Forms.Bindings
{
    /// <summary>
    /// バインドされているプロパティ値の変更を通知するイベント引数です。
    /// </summary>
    public sealed class BindingPropertyChangedEventArgs : EventArgs
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BindingPropertyChangedEventArgs(string propertyName,
                                               object newValue,
                                               object oldValue)
        {
            PropertyName = propertyName;
            NewValue = newValue;
            OldValue = oldValue;
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
        /// 変更後の値を取得します。
        /// </summary>
        public object NewValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 変更前の値を取得します。
        /// </summary>
        public object OldValue
        {
            get;
            private set;
        }
    }
}
