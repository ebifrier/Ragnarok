using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ragnarok.Utility;

namespace Ragnarok.ObjectModel
{
    /// <summary>
    /// あるオブジェクトが持つプロパティとその値をすべて
    /// コピーするオブジェクトクラスです。
    /// </summary>
    public class CloneObject : DynamicDictionary
    {
        /// <summary>
        /// コピー元のオブジェクト型を取得します。
        /// </summary>
        public Type SourceType
        {
            get;
            private set;
        }

        /// <summary>
        /// <paramref ref="target"/>のプロパティ一覧を作成します。
        /// </summary>
        public static IEnumerable<DynamicPropertyInfo> CreatePropertyList(object target)
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
        /// <paramref ref="target"/>に対して、各プロパティ値を設定します。
        /// </summary>
        public void SetValuesToTarget(object target)
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

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CloneObject(object target)
            : base(CreatePropertyList(target), false)
        {
            SourceType = target.GetType();
        }
    }
}
