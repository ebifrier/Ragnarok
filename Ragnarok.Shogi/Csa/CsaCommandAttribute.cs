using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Ragnarok.Shogi.Csa
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CsaCommandAttribute : Attribute
    {
        /// <summary>
        /// 名称を取得します。
        /// </summary>
        public string Label
        {
            get;
            private set;
        }

        /// <summary>
        /// コマンドの解釈文字列を取得します。
        /// </summary>
        public string Command
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CsaCommandAttribute(string label, string command)
        {
            Label = label;
            Command = command;
        }
    }

    public static class CsaCommandAttributeUtil
    {
        /// <summary>
        /// 列挙値の説明文を取得します。
        /// </summary>
        public static string GetLabel(object value)
        {
            var attribute = EnumEx.GetAttribute<CsaCommandAttribute>(value);
            if (attribute == null)
            {
                return null;
            }

            return attribute.Label;
        }

        /// <summary>
        /// 列挙値のラベルを取得します。
        /// </summary>
        public static string GetCommand(object value)
        {
            var attribute = EnumEx.GetAttribute<CsaCommandAttribute>(value);
            if (attribute == null)
            {
                return null;
            }

            return attribute.Command;
        }
    }
}
