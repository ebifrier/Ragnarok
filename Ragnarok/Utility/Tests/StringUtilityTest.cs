#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Utility.Tests
{
    [TestFixture()]
    public sealed class StringUtilityTest
    {
        [Test()]
        public void DictionaryTest()
        {
            // 引数１つ
            var format = "#{code}";
            var dic = new Dictionary<string, object>()
            {
                { "code", "C1" },
            };
            Assert.AreEqual("C1", StringUtility.NamedFormat(format, dic));

            // 引数２つ
            format = "_#{code} #{22}xx";
            dic = new Dictionary<string, object>()
            {
                { "code", "C1" },
                { "22", "xyz" },
            };
            Assert.AreEqual("_C1 xyzxx", StringUtility.NamedFormat(format, dic));

            // 引数３つ
            format =
                "SELECT #{code}, #{name}" +
                "FROM #{tableName}" +
                "WHERE #{createdAt} = @createdAt" +
                "ORDER BY #{code}";
            dic = new Dictionary<string, object>()
            {
                { "tableName", "TBL1" },
                { "code", "C1" },
                { "name", "C2" },
                { "createdAt", "C3" }
            };
            Assert.AreEqual(
                "SELECT C1, C2" +
                "FROM TBL1" +
                "WHERE C3 = @createdAt" +
                "ORDER BY C1",
                StringUtility.NamedFormat(format, dic));
        }

        /// <summary>
        /// オブジェクトを対象としたテストを行います。
        /// </summary>
        [Test()]
        public void ObjectTest()
        {
            // プロパティが一つ
            var format = "#{code}";
            var obj = new
            {
                code = "C1",
            };
            Assert.AreEqual("C1", StringUtility.NamedFormat(format, obj));

            // プロパティが２つ
            var format2 = "_#{Code} #{x33x}xx";
            var obj2 = new
            {
                code = "C1",
                Code = "C2",
                x33x = "xyz",
            };
            Assert.AreEqual("_C2 xyzxx", StringUtility.NamedFormat(format2, obj2));

            // プロパティが多数
            var format3 = "SELECT #{code}, #{name}" +
                "FROM #{tableName}" +
                "WHERE #{createdAt} = @createdAt" +
                "ORDER BY #{code}";
            var obj3 = new
            {
                tableName = "TBL1",
                code = "C1",
                name = "C2",
                createdAt = "C3"
            };
            Assert.AreEqual(
                "SELECT C1, C2" +
                "FROM TBL1" +
                "WHERE C3 = @createdAt" +
                "ORDER BY C1",
                StringUtility.NamedFormat(format3, obj3));
        }

        /// <summary>
        /// フォーマット指定子に関するテストを行います。
        /// </summary>
        [Test()]
        public void FormatTest()
        {
            var format = "#{Code:0000}";
            var obj = new { Code = 1 };
            Assert.AreEqual("0001", StringUtility.NamedFormat(format, obj));
        }
    }
}
#endif
