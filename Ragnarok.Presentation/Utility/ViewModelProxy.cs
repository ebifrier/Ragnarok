using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;

using Ragnarok.Utility;
using Ragnarok.ObjectModel;

namespace Ragnarok.Presentation.Utility
{
    /// <summary>
    /// あるViewModelの状態を保存し、必要であれば元に戻せるような
    /// モデルオブジェクトを作成します。
    /// </summary>
    /// <remarks>
    /// あるコントロールを設定ダイアログで変更するとき、
    /// ダイアログのDataContextに直接コントロールを設定することで
    /// その変更がすぐに確認できるようになります。
    /// 
    /// ただし、キャンセルボタンの押下時はコントロールの状態を元に
    /// 戻す必要があるため、一工夫しています。
    /// </remarks>
    public class ViewModelProxy : DynamicViewModel
    {
        private Dictionary<string, object> oldPropertyValueDic =
            new Dictionary<string, object>();

        /// <summary>
        /// 対象となるViewModelを取得します。
        /// </summary>
        public object ViewModel
        {
            get;
            private set;
        }

        /// <summary>
        /// ViewModelの状態を元に戻します。
        /// </summary>
        public void RollbackViewModel()
        {
            lock (this.oldPropertyValueDic)
            {
                this.oldPropertyValueDic.ForEach(_ =>
                    MethodUtil.SetPropertyValue(ViewModel, _.Key, _.Value));
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ViewModelProxy(object viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }

            PropertyChanging += MyPropertyChanging;
            ViewModel = viewModel;

            AddDependModel(viewModel);
        }

        private void MyPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            lock (this.oldPropertyValueDic)
            {
                // ViewModelの状態を元すため、プロパティの元の値を保存します。
                if (!this.oldPropertyValueDic.ContainsKey(e.PropertyName))
                {
                    this.oldPropertyValueDic[e.PropertyName] =
                        MethodUtil.GetPropertyValue(sender, e.PropertyName);
                }
            }

            base.OnPropertyChanging(e);
        }
    }
}
