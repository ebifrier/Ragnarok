using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Ragnarok.Presentation.Utility
{
    /// <summary>
    /// This class contains a few useful extenders for the ListBox
    /// </summary>
    public sealed class ListBoxUtil : DependencyObject
    {
        /// <summary>
        /// リストを自動で終端までスクロールさせるための添付プロパティです。
        /// </summary>
        public static readonly DependencyProperty AutoScrollToEndProperty =
            DependencyProperty.RegisterAttached(
                "AutoScrollToEnd", typeof(bool),
                typeof(ListBoxUtil),
                new UIPropertyMetadata(default(bool), OnAutoScrollToEndChanged));

        /// <summary>
        /// リストを自動で終端までスクロールさせるかどうかを取得します。
        /// </summary>
        public static bool GetAutoScrollToEnd(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToEndProperty);
        }

        /// <summary>
        /// リストを自動で終端までスクロールさせるかどうかを設定します。
        /// </summary>
        public static void SetAutoScrollToEnd(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToEndProperty, value);
        }

        /// <summary>
        /// This method will be called when the AutoScrollToEnd
        /// property was changed
        /// </summary>
        public static void OnAutoScrollToEndChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var listBox = s as ListBox;
            var listBoxItems = listBox.Items;
            var data = listBoxItems.SourceCollection as INotifyCollectionChanged;

            var scrollToEndHandler = new NotifyCollectionChangedEventHandler(
                (s1, e1) =>
                {
                    if (listBox.Items.Count > 0)
                    {
                        var lastItem = listBox.Items[listBox.Items.Count - 1];
                        listBox.ScrollIntoView(lastItem);
                    }
                });

            if ((bool)e.NewValue)
            {
                data.CollectionChanged += scrollToEndHandler;
            }
            else
            {
                data.CollectionChanged -= scrollToEndHandler;
            }
        }
    }
}
