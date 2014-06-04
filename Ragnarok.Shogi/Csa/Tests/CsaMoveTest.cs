#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Shogi.Csa.Tests
{
    [TestFixture()]
    internal sealed class CsaMoveTest
    {
        [Test()]
        public void ParseTest()
        {
            Assert.AreEqual(
                new CsaMove
                {
                    Side = BWType.None,
                    SrcSquare = new Position(1, 2),
                    DstSquare = new Position(3, 4),
                    Piece = Piece.Hu,
                },
                CsaMove.Parse("1234FU"));
            Assert.AreEqual(
                new CsaMove
                {
                    Side = BWType.Black,
                    DstSquare = new Position(1, 9),
                    Piece = Piece.Ryu,
                },
                CsaMove.Parse("+0019RY"));
            Assert.AreEqual(
                new CsaMove
                {
                    Side = BWType.White,
                    SrcSquare = new Position(3, 3),
                    DstSquare = new Position(1, 9),
                    Piece = Piece.Gyoku,
                },
                CsaMove.Parse("-3319OU"));
            Assert.AreEqual(
                new CsaMove
                {
                    IsResigned = true,
                },
                CsaMove.Parse("%TORYO"));

            Assert.AreEqual(
                null,
                CsaMove.Parse("0019HU"));
            Assert.AreEqual(
                null,
                CsaMove.Parse("kj897dsflx"));
            Assert.AreEqual(
                new CsaMove
                {
                    Piece = Piece.Hu,
                },
                CsaMove.Parse("0000FU"));
        }

        [Test()]
        public void ToStringTest()
        {
            var move = new CsaMove
            {
                Side = BWType.None,
                SrcSquare = new Position(1, 2),
                DstSquare = new Position(3, 4),
                Piece = Piece.Hu,
            };
            Assert.AreEqual(
                "1234FU",
                move.ToCsaString());
            Assert.AreEqual(
                "34歩(12)",
                move.ToPersonalString());

            move = new CsaMove
            {
                Side = BWType.Black,
                DstSquare = new Position(1, 9),
                Piece = Piece.Ryu,
            };
            Assert.AreEqual(
                "+0019RY",
                move.ToCsaString());
            Assert.AreEqual(
                "▲19龍打",
                move.ToPersonalString());

            move = new CsaMove
            {
                Side = BWType.White,
                SrcSquare = new Position(3, 3),
                DstSquare = new Position(1, 9),
                Piece = Piece.Gyoku,
            };
            Assert.AreEqual(
                "-3319OU",
                move.ToCsaString());
            Assert.AreEqual(
                "△19玉(33)",
                move.ToPersonalString());

            move = new CsaMove
            {
                Piece = Piece.Hu,
            };
            Assert.AreEqual(
                "0000FU",
                move.ToCsaString());
            Assert.AreEqual(
                "○○○",
                move.ToPersonalString());

            move = new CsaMove
            {
                IsResigned = true,
            };
            Assert.AreEqual(
                "%TORYO",
                move.ToCsaString());
            Assert.AreEqual(
                "投了",
                move.ToPersonalString());
        }
    }
}
#endif
