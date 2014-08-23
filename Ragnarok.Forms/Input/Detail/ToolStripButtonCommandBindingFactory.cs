using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ragnarok.Forms.Input.Detail
{
    /// <summary>
    /// ToolStripButtonクラスに対応したバインディングを作成します。
    /// </summary>
    internal sealed class ToolStripButtonCommandBindingFactory : ICommandBindingFactory
    {
        /// <summary>
        /// このコンポーネントに対応したファクトリかどうかを調べます。
        /// </summary>
        public bool CanCreate(Component component)
        {
            return (component is ToolStripButton);
        }

        /// <summary>
        /// バインディングを作成します。
        /// </summary>
        public CommandBindingBase Create(Component component, ICommand command,
                                         Func<object> commandParameterCallback)
        {
            var target = component as ToolStripButton;
            if (target == null)
            {
                throw new ArgumentException(
                    "This factory cannot create a CommandBindingBase for the passed component.");
            }

            return new ToolStripButtonCommandBinding(target, command, commandParameterCallback);
        }
    }
}
