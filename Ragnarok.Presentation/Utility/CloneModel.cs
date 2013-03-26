using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;

using Ragnarok.Utility;
using Ragnarok.ObjectModel;

namespace Ragnarok.Presentation.Utility
{
    /// <summary>
    /// あるオブジェクトが持つプロパティとその値をすべて
    /// コピーするオブジェクトクラスです。
    /// </summary>
    public class CloneModel : DynamicDictionary
    {
        /// <summary>
        /// <paramref ref="target"/>のプロパティ一覧を作成します。
        /// </summary>
        public static IEnumerable<DynamicPropertyInfo> CreatePropertyList(
            DependencyObject target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            return MethodUtil
                .GetPropertyDic(target.GetType())
                .Where(_ => _.Value.CanRead)
                .Where(_ => _.Value.CanWrite)
                .Select(_ =>
                    new DynamicPropertyInfo(
                        _.Key,
                        _.Value.PropertyType,
                        _.Value.GetValue(target)));
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CloneModel(DependencyObject target)
            : base(CreatePropertyList(target), false)
        {
            SourceType = target.GetType();
        }

        /// <summary>
        /// コピー元のオブジェクト型を取得します。
        /// </summary>
        public Type SourceType
        {
            get;
            private set;
        }

        /// <summary>
        /// <paramref ref="target"/>に対して、各プロパティ値を設定します。
        /// </summary>
        public void SetValuesToTarget(DependencyObject target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            /*if (!target.GetType().IsAssignableFrom(SourceType))
            {
                throw new InvalidOperationException(
                    "型に整合性がありません。");
            }*/

            ChangedPropertyNames.ForEach(_ =>
                MethodUtil.SetPropertyValue(target, _, this[_]));
        }
    }
}
