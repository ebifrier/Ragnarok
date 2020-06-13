using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ragnarok.Shogi.Kif.Tests
{
    public static class SampleKif
    {
        /// <summary>
        /// 指定の名前を持つ棋譜をリソースから読み込みます。
        /// </summary>
        public static string Get(string resourceName, Encoding encoding = null)
        {
            var asm = Assembly.GetExecutingAssembly();
            var ns = typeof(SampleKif).Namespace + ".Sample.";
            encoding = encoding ?? KifuObject.DefaultEncoding;

            using (var stream = asm.GetManifestResourceStream(ns + resourceName))
            using (var reader = new StreamReader(stream, encoding))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// 棋譜をオブジェクトとして読み込みます。
        /// </summary>
        public static KifuObject LoadKif(string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            var ns = typeof(SampleKif).Namespace + ".Sample.";

            using (var stream = asm.GetManifestResourceStream(ns + resourceName))
            {
                return KifuReader.LoadFrom(stream);
            }
        }

        /// <summary>
        /// 指し手ごとの棋譜コメントを取得します。
        /// </summary>
        public static List<List<string>> GetCommentList(string kif)
        {
            var lines = kif
                .Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .SkipWhile(_ => !_.Contains("手数----指手---------消費時間"))
                .Skip(1)
                .Select(_ => _.FirstOrDefault() == '*' ? _.Substring(1) : null);

            var commentList = new List<List<string>>();
            var comments = new List<string>();
            foreach (var line in lines)
            {
                if (line != null)
                {
                    // コメント行
                    comments.Add(line);
                }
                else
                {
                    // コメントではなく、指し手行
                    commentList.Add(comments);
                    comments = new List<string>();
                }
            }

            return commentList;
        }
    }
}
