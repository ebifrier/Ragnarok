using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#if false
namespace Ragnarok.Presentation.Utility
{
    /// <summary>
    /// 様々なクラスへ便利なコンテキストメニューを追加します。
    /// </summary>
    public static class ContextMenuUtil
    {
        /// <summary>
        /// コピー可能なコンテキストメニューを追加するための添付プロパティです。
        /// </summary>
        public static readonly DependencyProperty HaveCopyContextMenuProperty =
            DependencyProperty.RegisterAttached(
                "HaveCopyContextMenu", typeof(bool),
                typeof(ContextMenuUtil),
                new UIPropertyMetadata(default(bool), OnHaveCopyContextMenu));

        /// <summary>
        /// コピー可能なコンテキストメニューを追加するかどうかを取得します。
        /// </summary>
        public static bool GetHaveCopyContextMenu(DependencyObject obj)
        {
            return (bool)obj.GetValue(HaveCopyContextMenuProperty);
        }

        /// <summary>
        /// コピー可能なコンテキストメニューを追加するかどうかを設定します。
        /// </summary>
        public static void SetHaveCopyContextMenu(DependencyObject obj, bool value)
        {
            obj.SetValue(HaveCopyContextMenuProperty, value);
        }

        static void ExecuteCopyItem(object sender, RoutedEventArgs e)
        {
            var source = e.OriginalSource as ContentControl;
            if (source == null)
            {
                return;
            }

            Clipboard.SetText(source.Content as string);
        }

        /// <summary>
        /// 
        /// </summary>
        private static void OnHaveCopyContextMenu(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var elem = s as FrameworkElement;

            if (!(bool)e.NewValue)
            {
                elem.ContextMenu = null;
                return;
            }

            var item = new MenuItem
            {
                Header = "コピー(_C)",
                //Command = ApplicationCommands.Copy,
            };
            item.Click += ExecuteCopyItem;

            var contextMenu = new ContextMenu();
            contextMenu.Items.Add(item);

            elem.ContextMenu = contextMenu;
        }
    }
}
#endif