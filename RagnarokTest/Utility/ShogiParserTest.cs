using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Ragnarok;
using Ragnarok.Shogi;

namespace RagnarokTest.Utility
{
    class ShogiParserTest
    {
        private void AssertPlayer(string test, string nickname, SkillLevel skill)
        {
            var player = ShogiParser.ParsePlayer(test);

            Assert.AreEqual(nickname, player.Nickname);
            Assert.AreEqual(skill, player.SkillLevel);
        }

        public void ParsePlayerTest()
        {
            AssertPlayer("＠ てす", "てす", new SkillLevel());
            AssertPlayer("@てす", "てす", new SkillLevel());

            AssertPlayer("@ tesu@てす", "tesu@てす", new SkillLevel());

            AssertPlayer("5級", "",
                new SkillLevel(SkillKind.Kyu, 5));
            AssertPlayer("いちご級　@cat", "cat",
                new SkillLevel(SkillKind.Kyu, 15));
            AssertPlayer("⑨級　@　日本語123", "日本語123",
                new SkillLevel(SkillKind.Kyu, 9));
            AssertPlayer("くだん@123", "123",
                new SkillLevel(SkillKind.Dan, 9));

            AssertPlayer("微妙　@　ＴＴＴ", "ＴＴＴ",
                new SkillLevel());
        }
    }
}
