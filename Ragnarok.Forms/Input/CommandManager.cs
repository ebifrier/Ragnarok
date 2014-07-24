using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace Ragnarok.Forms.Input
{
    public static class CommandManager
    {
        private static readonly List<ICommandBindingFactory> factories =
            new List<ICommandBindingFactory>()
            {
                new Detail.ButtonCommandBindingFactory(),
                /*new MenuItemCommandBindingFactory(),
                new ToolStripButtonCommandBindingFactory()*/
            };

        private static readonly List<CommandBindingBase> bindings =
            new List<CommandBindingBase>();

        /// <summary>
        /// コマンドの実行可否状態が変更される変わった可能性が
        /// あるときに呼ばれるイベントです。
        /// </summary>
        public static event EventHandler RequerySuggested;

        /// <summary>
        /// コマンドの実行可否状態をすべて再チェックします。
        /// </summary>
        public static void InvalidateRequerySuggested()
        {
            var handler = RequerySuggested;

            handler.SafeRaiseEvent(null, EventArgs.Empty);
        }

        /// <summary>
        /// コマンドのバインディングを追加します。
        /// </summary>
        public static CommandBindingBase AddCommandBinding(Component component,
                                                           ICommand command)
        {
            return AddCommandBinding(component, command, () => null);
        }

        /// <summary>
        /// コマンドのバインディングを追加します。
        /// </summary>
        public static CommandBindingBase AddCommandBinding(Component component,
                                                           ICommand command,
                                                           object commandParameter)
        {
            return AddCommandBinding(component, command, () => commandParameter);
        }

        /// <summary>
        /// コマンドのバインディングを追加します。
        /// </summary>
        public static CommandBindingBase AddCommandBinding(Component component,
                                                           ICommand command,
                                                           Func<object> commandParameterCallback)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }

            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (commandParameterCallback == null)
            {
                throw new ArgumentNullException("commandParameterCallback");
            }

            foreach (var factory in factories)
            {
                if (factory.CanCreate(component))
                {
                    var binding = factory.Create(component, command,
                                                 commandParameterCallback);

                    bindings.Add(binding);
                    return binding;
                }
            }

            throw new NotSupportedException(
                string.Format(
                    "コンポ―ネント'{0}'はコマンドバインディングに対応していません。",
                    component.GetType().Name));
        }

        /// <summary>
        /// バインディングを削除します。
        /// </summary>
        public static void RemoveCommandBinding(CommandBindingBase binding)
        {
            if (binding == null)
            {
                throw new ArgumentNullException("binding");
            }

            bindings.Remove(binding);
        }
    }
}
