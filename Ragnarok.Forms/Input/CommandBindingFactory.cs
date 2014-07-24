using System;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Ragnarok.Forms.Input
{
    /// <summary>
    /// コマンドバインディングを作成します。
    /// </summary>
    internal interface ICommandBindingFactory
    {
        /// <summary>
        /// このコンポーネントに対応したファクトリかどうかを調べます。
        /// </summary>
        bool CanCreate(Component component);

        /// <summary>
        /// バインディングを作成します。
        /// </summary>
        CommandBindingBase Create(Component component, ICommand command,
                                  Func<object> commandParameterCallback);
    }
}
