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
    /// This class contains a few useful extenders for the DataGrid
    /// </summary>
    public static class DataGridUtil
    {
        /// <summary>
        /// リストを自動で終端までスクロールさせるための添付プロパティです。
        /// </summary>
        public static readonly DependencyProperty AutoScrollToEndProperty =
            DependencyProperty.RegisterAttached(
                "AutoScrollToEnd", typeof(bool),
                typeof(DataGridUtil),
                new UIPropertyMetadata(default(bool), OnAutoScrollToEndChanged));

        /// <summary>
        /// DataGridを自動で終端までスクロールさせるかどうかを取得します。
        /// </summary>
        public static bool GetAutoScrollToEnd(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToEndProperty);
        }

        /// <summary>
        /// DataGridを自動で終端までスクロールさせるかどうかを設定します。
        /// </summary>
        public static void SetAutoScrollToEnd(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToEndProperty, value);
        }

        /// <summary>
        /// DataGridを一番下までスクロールさせます。
        /// </summary>
        private static void OnAutoScrollToEndChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = s as DataGrid;
            var data = dataGrid.Items.SourceCollection as INotifyCollectionChanged;

            var peer = ItemsControlAutomationPeer.CreatePeerForElement(dataGrid);
            if (peer == null)
            {
                Log.Error(
                    "dataGridのItemsControlAutomationPeerが取得できません。");
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
