using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Ragnarok.Presentation.Utility
{
    /// <summary>
    /// WebBrowserクラス用のSourceプロパティへのBinding機能を提供します。
    /// </summary>
    public static class WebBrowserUtil
    {
        /// <summary>
        /// WebBrowser クラスの Source プロパティへの Binding 機能を提供する為の依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty BindableSourceProperty =
            DependencyProperty.RegisterAttached(
                "BindableSource", typeof(string),
                typeof(WebBrowserUtil),
                new FrameworkPropertyMetadata(null, BindableSourceChanged));

        /// <summary>
        /// BindableSourceProperty を取得します。
        /// </summary>
        public static string GetBindableSource(DependencyObject obj)
        {
            return (string)obj.GetValue(BindableSourceProperty);
        }

        /// <summary>
        /// BindableSourceProperty を設定します。
        /// </summary>
        public static void SetBindableSource(DependencyObject obj, string value)
        {
            obj.SetValue(BindableSourceProperty, value);
        }

        /// <summary>
        /// BindableSourceProperty が変更された時に発生します。
        /// </summary>
        public static void BindableSourceChanged(DependencyObject obj,
                                                 DependencyPropertyChangedEventArgs e)
        {
            var browser = obj as WebBrowser;
            if (browser != null)
            {
                var uri = e.NewValue as string;
                browser.Source = (uri != null ? new Uri(uri) : null);
            }
        }
    }
}
