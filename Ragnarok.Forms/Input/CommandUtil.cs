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
        /// <remarks>
        /// コマンド名はTagで指定され、そのコマンドをアイテムに設定します。
        /// </remarks>
        public static void BindMenuCommand(Type type, IEnumerable<ToolStripItem> items,
                                           object parameter = null)
        {
            if ((object)type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (items == null)
            {
                return;
            }

            items.ForEach(_ => BindMenuCommand(type, _, parameter));
        }

        /// <remarks>
        /// コマンド名はTagで指定され、そのコマンドをアイテムに設定します。
        /// </remarks>
        public static void BindMenuCommand(Type type, IEnumerable<ToolStripItem> items,
                                           Func<object> parameterCallback)
        {
            if ((object)type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (items == null)
            {
                return;
            }

            items.ForEach(_ => BindMenuCommand(type, _, parameterCallback));
        }

        /// <summary>
        /// <paramref name="item"/>とその子メニューのコマンドバインディングを行います。
        /// </summary>
        public static void BindMenuCommand(Type type, ToolStripItem item,
                                           object parameter = null)
        {
            BindMenuCommandInternal(type, item, () => parameter);
        }

        /// <summary>
        /// <paramref name="item"/>とその子メニューのコマンドバインディングを行います。
        /// </summary>
        public static void BindMenuCommand(Type type, ToolStripItem item,
                                           Func<object> parameterCallback)
        {
            BindMenuCommandInternal(type, item, parameterCallback);
        }

        /// <summary>
        /// <paramref name="item"/>とその子メニューのコマンドバインディングを行います。
        /// </summary>
        /// <remarks>
        /// コマンド名はitem.Tagで指定され、そのコマンドをアイテムに設定します。
        /// </remarks>
        public static void BindMenuCommandInternal(Type type, ToolStripItem item,
                                                   Func<object> parameterCallback)
        {
            if ((object)type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (item == null)
            {
                return;
            }

            // まず自分のコマンドバインディングを行います。
            BindMenuCommandThisItem(type, item, parameterCallback);

            // DropDownItemの場合は、それが持つ子メニューも処理します。
            var dropDown = item as ToolStripDropDownItem;
            if (dropDown != null)
            {
                var list = dropDown.DropDownItems.OfType<ToolStripItem>();
                BindMenuCommand(type, list, parameterCallback);
                return;
            }
        }

        /// <summary>
        /// <paramref name="item"/>のコマンドバインディングを行います。
        /// </summary>
        private static void BindMenuCommandThisItem(Type type, ToolStripItem item,
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

            var command = GetCommand(type, tag);
            if (command == null)
            {
                return;
            }

            CommandManager.AddCommandBinding(item, command, parameterCallback);
        }

        /// <summary>
        /// コマンド名から指定のコマンドを取得します。
        /// </summary>
        private static ICommand GetCommand(Type type, string commandName)
        {
            var flags = BindingFlags.Public | BindingFlags.Static |
                        BindingFlags.GetField;

            var field = type.GetField(commandName, flags);
            if (field == null)
            {
                throw new InvalidOperationException(
                    commandName + ": 指定の名前のコマンドが存在しません。");
            }

            var command = field.GetValue(null) as ICommand;
            if (command == null)
            {
                throw new InvalidOperationException(
                    commandName + ": コマンドがICommandを継承していません。");
            }

            return command;
        }
    }
}
