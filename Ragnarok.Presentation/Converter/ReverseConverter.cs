using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace Ragnarok.Presentation.Converter
{
    /// <summary>
    /// ObservableCollectionを逆順で表示します。
    /// </summary>
    public class ReverseConverter<TValue> : IValueConverter
    {
        /// <summary>
        /// コレクションを逆順に並び替えます。
        /// </summary>
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            var reversedList = new ObservableCollection<TValue>();

            var data = (IList<TValue>)value;
            for (var i = data.Count - 1; i >= 0; --i)
            {
                reversedList.Add(data[i]);
            }

            // 変更通知可能なリストならそれにも設定します。
            var observableCollection = data as INotifyCollectionChanged;
            if (observableCollection != null)
            {
                observableCollection.CollectionChanged += (sender, e) =>
                    DataCollectionChanged(reversedList, sender, e);
            }

            return reversedList;
        }

        /// <summary>
        /// リスト変更時に逆順のリストも更新します。
        /// </summary>
        void DataCollectionChanged(ObservableCollection<TValue> reversedList,
                                   object sender, NotifyCollectionChangedEventArgs e)
        {
            var data = (IList<TValue>)sender;

            reversedList.Clear();
            for (var i = data.Count - 1; i >= 0; --i)
            {
                reversedList.Add(data[i]);
            }
        }

        /// <summary>
        /// 実装されていません。
        /// </summary>
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// StringのObservableCollectionを逆順に並び替えます。
    /// </summary>
    public class StringReverseConverter : ReverseConverter<string>
    {
    }
}
