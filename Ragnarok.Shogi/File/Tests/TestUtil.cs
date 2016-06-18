#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

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

            return new HashSet<string>(System.IO.File.ReadAllLines(filepath, Encoding.UTF8));
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
            var dir = @"E:/Dropbox/NicoNico/shogi/test_kif";

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
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                        line = line + "failed";
                    }

                    Console.WriteLine(line);
                });

            SavePathList(filepath, pathList);
        }

        /// <summary>
        /// 棋譜の出力＋入力を行い、ファイルが正しく扱えているか調べます。
        /// </summary>
        public static void ReadWriteTest(KifuObject kifu0, KifuFormat format,
                                         int moveCount)
        {
            // 棋譜の書き出し
            var wrote = KifuWriter.WriteTo(kifu0, format);
            Assert.False(string.IsNullOrEmpty(wrote));

            // 棋譜の読み込み パート２
            var kifu1 = KifuReader.LoadFrom(wrote);
            Assert.NotNull(kifu1);

            // 読み込んだ棋譜の確認
            Assert.LessOrEqual(moveCount, kifu1.MoveList.Count());

            // 局面の比較を行います。
            var board0 = kifu0.StartBoard.Clone();
            kifu0.MoveList.ForEach(_ => board0.DoMove(_));

            var board1 = kifu1.StartBoard.Clone();
            kifu1.MoveList.ForEach(_ => board1.DoMove(_));

            Assert.True(Board.BoardEquals(kifu0.StartBoard, kifu1.StartBoard));
            Assert.True(kifu0.RootNode.NodeEquals(kifu1.RootNode, true));
            Assert.True(Board.BoardEquals(board0, board1));

            // ヘッダ要素を比較します。
            Assert.AreEqual(kifu0.Header.Count(), kifu1.Header.Count());
            foreach (var item0 in kifu0.Header)
            {
                Assert.True(kifu1.Header.Contains(item0.Key));
                Assert.AreEqual(item0.Value, kifu1.Header[item0.Key]);
            }
        }
    }
}
#endif
