using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Ragnarok.Presentation.Utility
{
    /// <summary>
    /// 何らかの事情でDataContextを使えない場合に
    /// モデルデータを迂回して参照するためのクラスです。
    /// </summary>
    /// <example>
    /// &lt;r:BindingProxy x:Key="proxy" /&gt;
    /// 
    /// &lt;Grid x:Key="grid"
    ///       Width="{Binding Data.Width,
    ///                       Source={StaticResource proxy}}"
    ///       Height="{Binding Data.Height,
    ///                       Source={StaticResource proxy}}" /&gt;
    /// &lt;/Grid&gt;
    /// 
    /// ソースコードのどこかでproxy.Dataを設定する。
    /// </example>
    public class BindingProxy : Freezable
    {
        /// <summary>
        /// 実際のモデルデータを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(
                "Data", typeof(object), typeof(BindingProxy),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 実際のモデルデータを取得または設定します。
        /// </summary>
        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        /// <summary>
        /// Freezableを継承するために必要です。
        /// </summary>
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
    }
}
