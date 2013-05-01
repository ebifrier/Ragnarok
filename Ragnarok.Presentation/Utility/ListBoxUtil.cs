using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

namespace Ragnarok.Presentation.Utility
{
    /// <summary>
    /// This class contains a few useful extenders for the ListBox
    /// </summary>
    public static class ListBoxUtil
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
        /// ListBoxを一番下までスクロールさせます。
        /// </summary>
        private static void OnAutoScrollToEndChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var listBox = s as ListBox;
            var data = listBox.Items.SourceCollection as INotifyCollectionChanged;

            var peer = ItemsControlAutomationPeer.CreatePeerForElement(listBox);
            if (peer == null)
            {
                Log.Error(
                    "ListBoxのItemsControlAutomationPeerが取得できません。");
                return;
            }

            var handler = new NotifyCollectionChangedEventHandler(
                (s1, e1) =>
                {
                    var provider = peer.GetPattern(PatternInterface.Scroll)
                        as IScrollProvider;
                    if (provider == null || !provider.VerticallyScrollable)
                    {
                        // スクロールバーがない可能性。
                        return;
                    }

                    provider.SetScrollPercent(
                        ScrollPatternIdentifiers.NoScroll,
                        100);
                });

            if ((bool)e.NewValue)
            {
                data.CollectionChanged += handler;
            }
            else
            {
                data.CollectionChanged -= handler;
            }
        }
    }
}
