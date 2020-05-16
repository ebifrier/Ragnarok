using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Ragnarok.Forms.Input
{
    /// <summary>
    /// コマンドに関するユーティリティクラスです。
    /// </summary>
    public static class CommandUtil
    {
        /// <summary>
        /// もし実行可能であればコマンドを実行します。
        /// </summary>
        public static void ExecuteCommand(this ICommand command, object parameter = null)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }

        /// <remarks>
        /// コマンド名はTagで指定され、そのコマンドをアイテムに設定します。
        /// </remarks>
        public static void BindMenuCommand(Type type, IEnumerable<ToolStripItem> items,
                                           object parameter = null)
        {
            BindMenuCommand(new[] { type }, items, parameter);
        }

        /// <remarks>
        /// コマンド名はTagで指定され、そのコマンドをアイテムに設定します。
        /// </remarks>
        public static void BindMenuCommand(Type type, IEnumerable<ToolStripItem> items,
                                           Func<object> parameterCallback)
        {
            BindMenuCommand(new[] { type }, items, parameterCallback);
        }

        /// <summary>
        /// <paramref name="item"/>とその子メニューのコマンドバインディングを行います。
        /// </summary>
        public static void BindMenuCommand(Type type, ToolStripItem item,
                                           object parameter = null)
        {
            BindMenuCommand(new[] { type }, item, parameter);
        }

        /// <summary>
        /// <paramref name="item"/>とその子メニューのコマンドバインディングを行います。
        /// </summary>
        public static void BindMenuCommand(Type type, ToolStripItem item,
                                           Func<object> parameterCallback)
        {
            BindMenuCommand(new[] { type }, item, parameterCallback);
        }

        /// <remarks>
        /// コマンド名はTagで指定され、そのコマンドをアイテムに設定します。
        /// </remarks>
        public static void BindMenuCommand(IEnumerable<Type> types,
                                           IEnumerable<ToolStripItem> items,
                                           object parameter = null)
        {
            if ((object)types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            if (items == null)
            {
                return;
            }

            items.ForEach(_ => BindMenuCommand(types, _, parameter));
        }

        /// <remarks>
        /// コマンド名はTagで指定され、そのコマンドをアイテムに設定します。
        /// </remarks>
        public static void BindMenuCommand(IEnumerable<Type> types,
                                           IEnumerable<ToolStripItem> items,
                                           Func<object> parameterCallback)
        {
            if ((object)types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            if (items == null)
            {
                return;
            }

            items.ForEach(_ => BindMenuCommand(types, _, parameterCallback));
        }

        /// <summary>
        /// <paramref name="item"/>とその子メニューのコマンドバインディングを行います。
        /// </summary>
        public static void BindMenuCommand(IEnumerable<Type> types, ToolStripItem item,
                                           object parameter = null)
        {
            BindMenuCommandInternal(types.ToArray(), item, () => parameter);
        }

        /// <summary>
        /// <paramref name="item"/>とその子メニューのコマンドバインディングを行います。
        /// </summary>
        public static void BindMenuCommand(IEnumerable<Type> types, ToolStripItem item,
                                           Func<object> parameterCallback)
        {
            BindMenuCommandInternal(types.ToArray(), item, parameterCallback);
        }

        /// <summary>
        /// <paramref name="item"/>とその子メニューのコマンドバインディングを行います。
        /// </summary>
        /// <remarks>
        /// コマンド名はitem.Tagで指定され、そのコマンドをアイテムに設定します。
        /// </remarks>
        public static void BindMenuCommandInternal(Type[] types, ToolStripItem item,
                                                   Func<object> parameterCallback)
        {
            if (types == null || !types.Any())
            {
                throw new ArgumentNullException(nameof(types));
            }

            if (item == null)
            {
                return;
            }

            // まず自分のコマンドバインディングを行います。
            BindMenuCommandThisItem(types, item, parameterCallback);

            // DropDownItemの場合は、それが持つ子メニューも処理します。
            var dropDown = item as ToolStripDropDownItem;
            if (dropDown != null)
            {
                var list = dropDown.DropDownItems.OfType<ToolStripItem>();
                BindMenuCommand(types, list, parameterCallback);
                return;
            }
        }

        /// <summary>
        /// <paramref name="item"/>のコマンドバインディングを行います。
        /// </summary>
        private static void BindMenuCommandThisItem(Type[] types, ToolStripItem item,
                                                    Func<object> parameterCallback)
        {
            if (item == null)
            {
                return;
            }

            var tag = item.Tag as string;
            if (string.IsNullOrEmpty(tag))
            {
                return;
            }

            var command = GetCommand(types, tag);
            if (command == null)
            {
                return;
            }

            CommandManager.AddCommandBinding(item, command, parameterCallback);
        }

        /// <summary>
        /// コマンド名から指定のコマンドを取得します。
        /// </summary>
        private static ICommand GetCommand(Type[] types, string commandName)
        {
            var flags = BindingFlags.Public | BindingFlags.Static |
                        BindingFlags.GetField;

            foreach (var type in types)
            {
                var field = type.GetField(commandName, flags);
                if (field == null)
                {
                    continue;
                }

                var command = field.GetValue(null) as ICommand;
                if (command == null)
                {
                    throw new InvalidOperationException(
                        commandName + ": コマンドがICommandを継承していません。");
                }

                return command;
            }

            throw new InvalidOperationException(
                commandName + ": 指定の名前のコマンドが存在しません。");
        }
    }
}
