using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ragnarok.Shogi.File.Tests
{
    internal static class TestUtil
    {
        /// <summary>
        /// 正しく読み込めた棋譜のリストを読み込みます。
        /// </summary>
        /// <remarks>
        /// 大量の棋譜をテストするため、一度正しく読み込めた棋譜は
        /// テストしないようにします。
        /// </remarks>
        public static HashSet<string> LoadPathList(string filepath)
        {
            if (!System.IO.File.Exists(filepath))
            {
                return new HashSet<string>();
            }

            return new HashSet<string>(Util.ReadLines(filepath, Encoding.UTF8));
        }

        /// <summary>
        /// 正しく読み込めた棋譜のリストを保存します。
        /// </summary>
        public static void SavePathList(string filepath, HashSet<string> pathList)
        {
            using (var writer = new StreamWriter(filepath, false, Encoding.UTF8))
            {
                pathList.ForEach(_ => writer.WriteLine(_));
            }
        }

        /// <summary>
        /// ki2ファイルの一覧を取得します。
        /// </summary>
        public static IEnumerable<string> FileList(string extenstion,
                                                   HashSet<string> pathList = null)
        {
            var dir = @"E:/Dropbox/NicoNico/bin/kifuexpl/database";

            pathList = pathList ?? new HashSet<string>();
            return Directory
                .EnumerateFiles(dir, extenstion, SearchOption.AllDirectories)
                .Where(_ => !pathList.Contains(_));
        }

        /// <summary>
        /// すべての棋譜のテストを行います。
        /// </summary>
        public static void KifTest(string filepath, string extension,
                                   Action<string> test)
        {
            var pathList = new HashSet<string>();
            var fileList = FileList(extension).ToList();

            Parallel.ForEach(fileList,
            //fileList.ForEach(
                path =>
                {
                    var line = path + "... ";

                    try
                    {
                        test(path);
                        lock (pathList)
                        {
                            pathList.Add(path);
                        }

                        line = line + "ok";
                    }
                    catch (Exception)
                    {
                        line = line + "failed";
                    }

                    Console.WriteLine(line);
                });

            SavePathList(filepath, pathList);
        }
    }
}
