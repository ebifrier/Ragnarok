using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi.Kif.Tests
{
    internal static partial class SampleKif
    {
        /// <summary>
        /// 指し手ごとの棋譜コメントを取得します。
        /// </summary>
        public static List<string> GetCommentList(string kif)
        {
            var lines = kif
                .Split(new char[] { '\n' })
                .SkipWhile(_ => !_.Contains("手数----指手---------消費時間"))
                .Skip(1)
                .Select(_ => _.FirstOrDefault() == '*' ? _.Substring(1) : null);

            var commentList = new List<string>();
            var comment = string.Empty;
            foreach (var line in lines)
            {
                if (line != null)
                {
                    // コメント行
                    comment = (comment.Any() ? comment + "\n" + line : line);
                }
                else
                {
                    // コメントではなく、指し手行
                    commentList.Add(comment == string.Empty ? null : comment);
                    comment = string.Empty;
                }
            }

            return commentList;
        }
    }
}
