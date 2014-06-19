#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Shogi.Tests
{
    using Sfen;

    /// <summary>
    /// 詰みや王手に関するテストを行います。
    /// </summary>
    [TestFixture()]
    internal sealed class BoardCheckMateTest
    {
        /// <summary>
        /// 通常の詰み
        /// </summary>
        [Test()]
        public void BlackTest1()
        {
            /*
            後手の持駒：飛　角　金二　歩二
              ９ ８ ７ ６ ５ ４ ３ ２ １
            +---------------------------+
            |v香v桂v銀v金v玉v金v銀v桂v香|一
            | ・ ・ ・ ・ ・ ・ ・ ・ ・|二
            |v歩 ・v歩v歩v歩v歩 ・v歩v歩|三
            | ・v歩 ・ ・ ・ ・v歩 ・ ・|四
            | ・ ・ ・ ・ ・ ・ ・ ・ ・|五
            | ・ ・ 歩 ・ ・ ・ ・ ・ ・|六
            | 歩 歩 ・ 歩 ・v角 歩 歩 歩|七
            | ・ ・ ・ ・v龍 ・ ・ ・ ・|八
            | 香 桂 銀 ・ 玉 ・ 銀 桂 香|九
            +---------------------------+
            先手の持駒：なし
            */
            var board = SfenBoard.Parse(
                "lnsgkgsnl/9/p1pppp1pp/1p4p2/9/2P6/PP1P1bPPP/4+r4/LNS1K1SNL" +
                " b rb2g2p 1");

            Assert.True(board.IsChecked(BWType.Black));
            Assert.False(board.IsChecked(BWType.White));
            Assert.True(board.IsCheckMated());
        }

        /// <summary>
        /// 合い駒利かずの詰み
        /// </summary>
        [Test()]
        public void BlackTest2()
        {
            /*
            後手の持駒：飛　角　金二　銀　歩二
              ９ ８ ７ ６ ５ ４ ３ ２ １
            +---------------------------+
            |v香v桂 ・v金v玉v金v銀v桂v香|一
            | ・ ・ ・ ・ ・ ・ ・ ・ ・|二
            |v歩 ・v歩v歩v歩v歩 ・v歩v歩|三
            | ・v歩 ・ ・ ・ ・v歩 ・ ・|四
            | ・ ・ ・ ・ ・ ・ ・ ・ ・|五
            | ・ ・ 歩 ・ ・ ・ ・ ・ ・|六
            | 歩 歩 ・ 歩v龍v馬 歩 歩 歩|七
            | ・ ・ ・ ・ ・ ・v銀 ・ ・|八
            | 香 桂 銀 ・ 玉 ・ ・ 桂 香|九
            +---------------------------+
            先手の持駒：なし
            */
            var board = SfenBoard.Parse(
                "ln1gkgsnl/9/p1pppp1pp/1p4p2/9/2P6/PP1P+r+bPPP/6s2/LNS1K2NL" +
                " b rb2gs2p 1");

            Assert.True(board.IsChecked(BWType.Black));
            Assert.False(board.IsChecked(BWType.White));
            Assert.True(board.IsCheckMated());
        }

        /// <summary>
        /// 合い駒が利くため詰まない
        /// </summary>
        [Test()]
        public void BlackTest3()
        {
            /*
            後手の持駒：飛　金二　銀　歩二
              ９ ８ ７ ６ ５ ４ ３ ２ １
            +---------------------------+
            |v香v桂 ・v金v玉v金v銀v桂v香|一
            | ・ ・ ・ ・ ・ ・ ・ ・ ・|二
            |v歩 ・v歩v歩v歩v歩 ・v歩v歩|三
            | ・v歩 ・ ・ ・ ・v歩 ・ ・|四
            | ・ ・ ・ ・ ・ ・ ・ ・ ・|五
            | ・ ・ 歩 ・ ・ ・ ・ ・ ・|六
            | 歩 歩 ・ 歩v龍v馬 歩 歩 歩|七
            | ・ ・ ・ ・ ・ ・v銀 ・ ・|八
            | 香 桂 銀 ・ 玉 ・ ・ 桂 香|九
            +---------------------------+
            先手の持駒：角
            */
            var board = SfenBoard.Parse(
                "ln1gkgsnl/9/p1pppp1pp/1p4p2/9/2P6/PP1P+r+bPPP/6s2/LNS1K2NL" +
                " b r2gs2pB 1");

            Assert.True(board.IsChecked(BWType.Black));
            Assert.False(board.IsChecked(BWType.White));
            Assert.False(board.IsCheckMated());
        }

        /// <summary>
        /// 後手番　通常の詰み
        /// </summary>
        [Test()]
        public void WhiteTest1()
        {
            /*
            後手の持駒：角　金二　銀　歩二
              ９ ８ ７ ６ ５ ４ ３ ２ １
            +---------------------------+
            |v香v桂 ・ ・v玉 ・v銀v桂v香|一
            | ・ ・v金 ・ 龍 ・v金 ・ ・|二
            |v歩 ・v歩v歩 龍v歩 ・v歩v歩|三
            | ・v歩 ・ ・ ・ ・v歩 ・ ・|四
            | ・ ・ ・ ・ ・ ・ ・ ・ ・|五
            | ・ ・ 歩 ・ ・ ・ ・ ・ ・|六
            | 歩 歩 ・ 歩 ・v馬 歩 歩 歩|七
            | ・ ・ ・ ・ ・ ・v銀 ・ ・|八
            | 香 桂 銀 ・ 玉 ・ ・ 桂 香|九
            +---------------------------+
            先手の持駒：歩
            */
            var board = SfenBoard.Parse(
                "ln2k1snl/2g1+R1g2/p1pp+Rp1pp/1p4p2/9/2P6/PP1P1+bPPP/6s2/LNS1K2NL" +
                " w Pb2gs2p 1");

            Assert.False(board.IsChecked(BWType.Black));
            Assert.True(board.IsChecked(BWType.White));
            Assert.True(board.IsCheckMated());
        }

        /// <summary>
        /// 後手番　合い駒が利くため詰まない
        /// </summary>
        [Test()]
        public void WhiteTest3()
        {
            /*
            後手の持駒：銀　歩
              ９ ８ ７ ６ ５ ４ ３ ２ １
            +---------------------------+
            |v香v桂 ・ ・v玉 ・v銀v桂v香|一
            | ・ ・ 金 ・ ・ ・v金 ・ ・|二
            |v歩 ・v歩v歩 龍 龍 ・v歩v歩|三
            | ・v歩 ・ ・v歩 ・v歩 ・ ・|四
            | ・ ・ ・ ・ ・ ・ ・ ・ ・|五
            | ・ ・ 歩 ・ ・ ・ ・ ・ ・|六
            | 歩 歩 ・ 歩 ・v馬 歩 歩 歩|七
            | ・ ・ ・ ・ ・ ・v銀 ・ ・|八
            | 香 桂 銀 ・ 玉 ・ ・ 桂 香|九
            +---------------------------+
            先手の持駒：角　金二　歩二
            */
            var board = SfenBoard.Parse(
                "ln2k1snl/2G3g2/p1pp+R+R1pp/1p2p1p2/9/2P6/PP1P1+bPPP/6s2/LNS1K2NL" +
                " w B2G2Psp 1");

            Assert.False(board.IsChecked(BWType.Black));
            Assert.True(board.IsChecked(BWType.White));
            Assert.False(board.IsCheckMated());
        }
    }
}
#endif
