using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ragnarok.Forms.Input.Detail
{
    /// <summary>
    /// ButtonBaseクラスに対応したバインディングを作成します。
    /// </summary>
    internal sealed class ButtonCommandBindingFactory : ICommandBindingFactory
    {
        /// <summary>
        /// このコンポーネントに対応したファクトリかどうかを調べます。
        /// </summary>
        public bool CanCreate(Component component)
        {
            return (component is ButtonBase);
        }

        /// <summary>
        /// バインディングを作成します。
        /// </summary>
        public CommandBindingBase Create(Component component, ICommand command,
                                         Func<object> commandParameterCallback)
        {
            var button = component as ButtonBase;
            if (button == null)
            {
                throw new ArgumentException(
                    "This factory cannot create a CommandBindingBase for the passed component.");
            }

            return new ButtonCommandBinding(button, command, commandParameterCallback);
        }
    }
}
