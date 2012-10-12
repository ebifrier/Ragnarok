using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.NicoNico.Live
{
    /// <summary>
    /// コメントサーバーからくるchat_resultメッセージのstatusを判別します。
    /// </summary>
    internal enum ChatResultStatus
    {
        /// <summary>
        /// エラーはありません。
        /// </summary>
        None = 0,

        /// <summary>
        /// 連続アクセスエラーや同じ内容のコメントを連続投稿した場合などです。
        /// </summary>
        NormalError = 1,

        /// <summary>
        /// 
        /// </summary>
        Error2 = 2,

        /// <summary>
        /// 
        /// </summary>
        Error3 = 3,

        /// <summary>
        /// PostKeyが不正の値。
        /// </summary>
        PostKey = 4,

        /// <summary>
        /// 不明なエラー。
        /// </summary>
        Unknown = 100,
    }

    /// <summary>
    /// chat_resultメッセージを処理します。
    /// </summary>
    internal static class ChatResultUtil
    {
        /// <summary>
        /// 文字列から<see cref="ChatResultStatus"/>の値を取得します。
        /// </summary>
        public static ChatResultStatus GetStatus(string text)
        {
            var value = StrUtil.ToInt(text, -1);

            foreach (var enumValue in EnumEx.GetValues<ChatResultStatus>())
            {
                if ((int)enumValue == value)
                {
                    return enumValue;
                }
            }

            return ChatResultStatus.Unknown;
        }
    }
}
