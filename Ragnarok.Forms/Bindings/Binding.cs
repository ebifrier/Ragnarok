using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ragnarok.Forms.Bindings
{
    using Converter;

    /// <summary>
    /// バインディングされたプロパティ値の変更を扱います。
    /// </summary>
    public delegate void BindingPropertyChangedCallback(Control c, BindingPropertyChangedEventArgs e);

    /// <summary>
    /// 値の正規化を行います。
    /// </summary>
    public delegate object CoerceBindingValueCallback(Control c, object baseValue);

    /// <summary>
    /// バインディングを扱うクラスです。
    /// </summary>
    public class Binding
    {
        /// <summary>
        /// バインディングコレクションを保持する親クラスを取得します。
        /// </summary>
        public Control Control
        {
            get;
            internal set;
        }

        /// <summary>
        /// バインドされるターゲットコントロールを取得します。
        /// </summary>
        public object BindableTarget
        {
            get;
            set;
        }

        /// <summary>
        /// バインドされるコントロールのプロパティ名を取得します。
        /// </summary>
        public string BindingPropertyName
        {
            get;
            set;
        }

        /// <summary>
        /// バインドするデータソースを取得します。
        /// </summary>
        public object DataSource
        {
            get;
            set;
        }

        /// <summary>
        /// バインドするデータソースのプロパティ名を取得します。
        /// </summary>
        public string DataSourcePropertyName
        {
            get;
            set;
        }

        /// <summary>
        /// モードを取得または設定します。
        /// </summary>
        public BindingMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// 値の変換を行うコンバーターを取得または設定します。
        /// </summary>
        public IValueConverter Converter
        {
            get;
            set;
        }

        /// <summary>
        /// フォーマット文字列を取得または設定します。
        /// </summary>
        public string StringFormat
        {
            get;
            set;
        }

        /// <summary>
        /// プロパティ値変更時に呼ばれるコールバックを取得または設定します。
        /// </summary>
        public BindingPropertyChangedCallback PropertyChanged
        {
            get;
            set;
        }

        /// <summary>
        /// 値の調整を行うコールバックを取得または設定します。
        /// </summary>
        public CoerceBindingValueCallback CoerceValue
        {
            get;
            set;
        }

        /// <summary>
        /// 今バインド中かどうかを取得します。
        /// </summary>
        public bool IsBinding
        {
            get;
            internal set;
        }
    }
}
