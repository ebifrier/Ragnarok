#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Ragnarok.Shogi
{
    using static Piece;

    [TestFixture]
    public sealed class PieceTest
    {
        [Test]
        public void BaseTest()
        {
            Assert.AreEqual(BlackPawn, Pawn);
            Assert.AreEqual(BlackLance, Lance);
            Assert.AreEqual(BlackKnight, Knight);
            Assert.AreEqual(BlackSilver, Silver);
            Assert.AreEqual(BlackBishop, Bishop);
            Assert.AreEqual(BlackRook, Rook);
            Assert.AreEqual(BlackGold, Gold);
            Assert.AreEqual(BlackKing, King);
            Assert.AreEqual(BlackProPawn, ProPawn);
            Assert.AreEqual(BlackProLance, ProLance);
            Assert.AreEqual(BlackProKnight, ProKnight);
            Assert.AreEqual(BlackProSilver, ProSilver);
            Assert.AreEqual(BlackHorse, Horse);
            Assert.AreEqual(BlackDragon, Dragon);
            Assert.AreEqual(BlackQueen, Queen);
        }

        [Test]
        public void RawTypesTest()
        {
            var pieces = PieceUtil.RawTypes().ToList();
            Assert.AreEqual(8, pieces.Count);
            Assert.AreEqual(BlackPawn, pieces[0]);
            Assert.AreEqual(BlackLance, pieces[1]);
            Assert.AreEqual(BlackKnight, pieces[2]);
            Assert.AreEqual(BlackSilver, pieces[3]);
            Assert.AreEqual(BlackBishop, pieces[4]);
            Assert.AreEqual(BlackRook, pieces[5]);
            Assert.AreEqual(BlackGold, pieces[6]);
            Assert.AreEqual(BlackKing, pieces[7]);

            pieces = PieceUtil.RawTypes(Colour.White).ToList();
            Assert.AreEqual(8, pieces.Count);
            Assert.AreEqual(WhitePawn, pieces[0]);
            Assert.AreEqual(WhiteLance, pieces[1]);
            Assert.AreEqual(WhiteKnight, pieces[2]);
            Assert.AreEqual(WhiteSilver, pieces[3]);
            Assert.AreEqual(WhiteBishop, pieces[4]);
            Assert.AreEqual(WhiteRook, pieces[5]);
            Assert.AreEqual(WhiteGold, pieces[6]);
            Assert.AreEqual(WhiteKing, pieces[7]);
        }

        [Test]
        public void PieceTypesTest()
        {
            var pieces = PieceUtil.PieceTypes().ToList();
            Assert.AreEqual(14, pieces.Count);
            Assert.AreEqual(BlackPawn, pieces[0]);
            Assert.AreEqual(BlackLance, pieces[1]);
            Assert.AreEqual(BlackKnight, pieces[2]);
            Assert.AreEqual(BlackSilver, pieces[3]);
            Assert.AreEqual(BlackBishop, pieces[4]);
            Assert.AreEqual(BlackRook, pieces[5]);
            Assert.AreEqual(BlackGold, pieces[6]);
            Assert.AreEqual(BlackKing, pieces[7]);
            Assert.AreEqual(BlackProPawn, pieces[8]);
            Assert.AreEqual(BlackProLance, pieces[9]);
            Assert.AreEqual(BlackProKnight, pieces[10]);
            Assert.AreEqual(BlackProSilver, pieces[11]);
            Assert.AreEqual(BlackHorse, pieces[12]);
            Assert.AreEqual(BlackDragon, pieces[13]);

            pieces = PieceUtil.PieceTypes(Colour.White).ToList();
            Assert.AreEqual(14, pieces.Count);
            Assert.AreEqual(WhitePawn, pieces[0]);
            Assert.AreEqual(WhiteLance, pieces[1]);
            Assert.AreEqual(WhiteKnight, pieces[2]);
            Assert.AreEqual(WhiteSilver, pieces[3]);
            Assert.AreEqual(WhiteBishop, pieces[4]);
            Assert.AreEqual(WhiteRook, pieces[5]);
            Assert.AreEqual(WhiteGold, pieces[6]);
            Assert.AreEqual(WhiteKing, pieces[7]);
            Assert.AreEqual(WhiteProPawn, pieces[8]);
            Assert.AreEqual(WhiteProLance, pieces[9]);
            Assert.AreEqual(WhiteProKnight, pieces[10]);
            Assert.AreEqual(WhiteProSilver, pieces[11]);
            Assert.AreEqual(WhiteHorse, pieces[12]);
            Assert.AreEqual(WhiteDragon, pieces[13]);
        }

        [Test]
        public void BlackWhiteTest()
        {
            var pieces = PieceUtil.BlackWhitePieces().ToList();
            Assert.AreEqual(28, pieces.Count);
            Assert.AreEqual(BlackPawn, pieces[0]);
            Assert.AreEqual(BlackLance, pieces[1]);
            Assert.AreEqual(BlackKnight, pieces[2]);
            Assert.AreEqual(BlackSilver, pieces[3]);
            Assert.AreEqual(BlackBishop, pieces[4]);
            Assert.AreEqual(BlackRook, pieces[5]);
            Assert.AreEqual(BlackGold, pieces[6]);
            Assert.AreEqual(BlackKing, pieces[7]);
            Assert.AreEqual(BlackProPawn, pieces[8]);
            Assert.AreEqual(BlackProLance, pieces[9]);
            Assert.AreEqual(BlackProKnight, pieces[10]);
            Assert.AreEqual(BlackProSilver, pieces[11]);
            Assert.AreEqual(BlackHorse, pieces[12]);
            Assert.AreEqual(BlackDragon, pieces[13]);
            Assert.AreEqual(WhitePawn, pieces[14]);
            Assert.AreEqual(WhiteLance, pieces[15]);
            Assert.AreEqual(WhiteKnight, pieces[16]);
            Assert.AreEqual(WhiteSilver, pieces[17]);
            Assert.AreEqual(WhiteBishop, pieces[18]);
            Assert.AreEqual(WhiteRook, pieces[19]);
            Assert.AreEqual(WhiteGold, pieces[20]);
            Assert.AreEqual(WhiteKing, pieces[21]);
            Assert.AreEqual(WhiteProPawn, pieces[22]);
            Assert.AreEqual(WhiteProLance, pieces[23]);
            Assert.AreEqual(WhiteProKnight, pieces[24]);
            Assert.AreEqual(WhiteProSilver, pieces[25]);
            Assert.AreEqual(WhiteHorse, pieces[26]);
            Assert.AreEqual(WhiteDragon, pieces[27]);
        }

        [Test]
        public void PromoteTest()
        {
            Assert.AreEqual(BlackProPawn, BlackPawn.Promote());
            Assert.AreEqual(BlackProLance, BlackLance.Promote());
            Assert.AreEqual(BlackProKnight, BlackKnight.Promote());
            Assert.AreEqual(BlackProSilver, BlackSilver.Promote());
            Assert.AreEqual(BlackHorse, BlackBishop.Promote());
            Assert.AreEqual(BlackDragon, BlackRook.Promote());
            Assert.AreEqual(BlackGold, BlackGold.Promote());
            Assert.AreEqual(BlackKing, BlackKing.Promote());
            Assert.AreEqual(BlackProPawn, BlackProPawn.Promote());
            Assert.AreEqual(BlackProLance, BlackProLance.Promote());
            Assert.AreEqual(BlackProKnight, BlackProKnight.Promote());
            Assert.AreEqual(BlackProSilver, BlackProSilver.Promote());
            Assert.AreEqual(BlackHorse, BlackHorse.Promote());
            Assert.AreEqual(BlackDragon, BlackDragon.Promote());
            Assert.AreEqual(WhiteProPawn, WhitePawn.Promote());
            Assert.AreEqual(WhiteProLance, WhiteLance.Promote());
            Assert.AreEqual(WhiteProKnight, WhiteKnight.Promote());
            Assert.AreEqual(WhiteProSilver, WhiteSilver.Promote());
            Assert.AreEqual(WhiteHorse, WhiteBishop.Promote());
            Assert.AreEqual(WhiteDragon, WhiteRook.Promote());
            Assert.AreEqual(WhiteGold, WhiteGold.Promote());
            Assert.AreEqual(WhiteKing, WhiteKing.Promote());
            Assert.AreEqual(WhiteProPawn, WhiteProPawn.Promote());
            Assert.AreEqual(WhiteProLance, WhiteProLance.Promote());
            Assert.AreEqual(WhiteProKnight, WhiteProKnight.Promote());
            Assert.AreEqual(WhiteProSilver, WhiteProSilver.Promote());
            Assert.AreEqual(WhiteHorse, WhiteHorse.Promote());
            Assert.AreEqual(WhiteDragon, WhiteDragon.Promote());
        }

        [Test]
        public void UnpromoteTest()
        {
            Assert.AreEqual(BlackPawn, BlackPawn.Unpromote());
            Assert.AreEqual(BlackLance, BlackLance.Unpromote());
            Assert.AreEqual(BlackKnight, BlackKnight.Unpromote());
            Assert.AreEqual(BlackSilver, BlackSilver.Unpromote());
            Assert.AreEqual(BlackBishop, BlackBishop.Unpromote());
            Assert.AreEqual(BlackRook, BlackRook.Unpromote());
            Assert.AreEqual(BlackGold, BlackGold.Unpromote());
            Assert.AreEqual(BlackKing, BlackKing.Unpromote());
            Assert.AreEqual(BlackPawn, BlackProPawn.Unpromote());
            Assert.AreEqual(BlackLance, BlackProLance.Unpromote());
            Assert.AreEqual(BlackKnight, BlackProKnight.Unpromote());
            Assert.AreEqual(BlackSilver, BlackProSilver.Unpromote());
            Assert.AreEqual(BlackBishop, BlackHorse.Unpromote());
            Assert.AreEqual(BlackRook, BlackDragon.Unpromote());
            Assert.AreEqual(WhitePawn, WhitePawn.Unpromote());
            Assert.AreEqual(WhiteLance, WhiteLance.Unpromote());
            Assert.AreEqual(WhiteKnight, WhiteKnight.Unpromote());
            Assert.AreEqual(WhiteSilver, WhiteSilver.Unpromote());
            Assert.AreEqual(WhiteBishop, WhiteBishop.Unpromote());
            Assert.AreEqual(WhiteRook, WhiteRook.Unpromote());
            Assert.AreEqual(WhiteGold, WhiteGold.Unpromote());
            Assert.AreEqual(WhiteKing, WhiteKing.Unpromote());
            Assert.AreEqual(WhitePawn, WhiteProPawn.Unpromote());
            Assert.AreEqual(WhiteLance, WhiteProLance.Unpromote());
            Assert.AreEqual(WhiteKnight, WhiteProKnight.Unpromote());
            Assert.AreEqual(WhiteSilver, WhiteProSilver.Unpromote());
            Assert.AreEqual(WhiteBishop, WhiteHorse.Unpromote());
            Assert.AreEqual(WhiteRook, WhiteDragon.Unpromote());
        }

        [Test]
        public void IsPromotedTest()
        {
            Assert.AreEqual(false, BlackPawn.IsPromoted());
            Assert.AreEqual(false, BlackLance.IsPromoted());
            Assert.AreEqual(false, BlackKnight.IsPromoted());
            Assert.AreEqual(false, BlackSilver.IsPromoted());
            Assert.AreEqual(false, BlackBishop.IsPromoted());
            Assert.AreEqual(false, BlackRook.IsPromoted());
            Assert.AreEqual(false, BlackGold.IsPromoted());
            Assert.AreEqual(false, BlackKing.IsPromoted());
            Assert.AreEqual(true, BlackProPawn.IsPromoted());
            Assert.AreEqual(true, BlackProLance.IsPromoted());
            Assert.AreEqual(true, BlackProKnight.IsPromoted());
            Assert.AreEqual(true, BlackProSilver.IsPromoted());
            Assert.AreEqual(true, BlackHorse.IsPromoted());
            Assert.AreEqual(true, BlackDragon.IsPromoted());
            Assert.AreEqual(false, WhitePawn.IsPromoted());
            Assert.AreEqual(false, WhiteLance.IsPromoted());
            Assert.AreEqual(false, WhiteKnight.IsPromoted());
            Assert.AreEqual(false, WhiteSilver.IsPromoted());
            Assert.AreEqual(false, WhiteBishop.IsPromoted());
            Assert.AreEqual(false, WhiteRook.IsPromoted());
            Assert.AreEqual(false, WhiteGold.IsPromoted());
            Assert.AreEqual(false, WhiteKing.IsPromoted());
            Assert.AreEqual(true, WhiteProPawn.IsPromoted());
            Assert.AreEqual(true, WhiteProLance.IsPromoted());
            Assert.AreEqual(true, WhiteProKnight.IsPromoted());
            Assert.AreEqual(true, WhiteProSilver.IsPromoted());
            Assert.AreEqual(true, WhiteHorse.IsPromoted());
            Assert.AreEqual(true, WhiteDragon.IsPromoted());
        }

        [Test]
        public void GetColourTest()
        {
            Assert.AreEqual(Colour.Black, BlackPawn.GetColour());
            Assert.AreEqual(Colour.Black, BlackLance.GetColour());
            Assert.AreEqual(Colour.Black, BlackKnight.GetColour());
            Assert.AreEqual(Colour.Black, BlackSilver.GetColour());
            Assert.AreEqual(Colour.Black, BlackBishop.GetColour());
            Assert.AreEqual(Colour.Black, BlackRook.GetColour());
            Assert.AreEqual(Colour.Black, BlackGold.GetColour());
            Assert.AreEqual(Colour.Black, BlackKing.GetColour());
            Assert.AreEqual(Colour.Black, BlackProPawn.GetColour());
            Assert.AreEqual(Colour.Black, BlackProLance.GetColour());
            Assert.AreEqual(Colour.Black, BlackProKnight.GetColour());
            Assert.AreEqual(Colour.Black, BlackProSilver.GetColour());
            Assert.AreEqual(Colour.Black, BlackHorse.GetColour());
            Assert.AreEqual(Colour.Black, BlackDragon.GetColour());
            Assert.AreEqual(Colour.White, WhitePawn.GetColour());
            Assert.AreEqual(Colour.White, WhiteLance.GetColour());
            Assert.AreEqual(Colour.White, WhiteKnight.GetColour());
            Assert.AreEqual(Colour.White, WhiteSilver.GetColour());
            Assert.AreEqual(Colour.White, WhiteBishop.GetColour());
            Assert.AreEqual(Colour.White, WhiteRook.GetColour());
            Assert.AreEqual(Colour.White, WhiteGold.GetColour());
            Assert.AreEqual(Colour.White, WhiteKing.GetColour());
            Assert.AreEqual(Colour.White, WhiteProPawn.GetColour());
            Assert.AreEqual(Colour.White, WhiteProLance.GetColour());
            Assert.AreEqual(Colour.White, WhiteProKnight.GetColour());
            Assert.AreEqual(Colour.White, WhiteProSilver.GetColour());
            Assert.AreEqual(Colour.White, WhiteHorse.GetColour());
            Assert.AreEqual(Colour.White, WhiteDragon.GetColour());
        }

        [Test]
        public void FlipColourTest()
        {
            Assert.AreEqual(WhitePawn, BlackPawn.FlipColour());
            Assert.AreEqual(WhiteLance, BlackLance.FlipColour());
            Assert.AreEqual(WhiteKnight, BlackKnight.FlipColour());
            Assert.AreEqual(WhiteSilver, BlackSilver.FlipColour());
            Assert.AreEqual(WhiteBishop, BlackBishop.FlipColour());
            Assert.AreEqual(WhiteRook, BlackRook.FlipColour());
            Assert.AreEqual(WhiteGold, BlackGold.FlipColour());
            Assert.AreEqual(WhiteKing, BlackKing.FlipColour());
            Assert.AreEqual(WhiteProPawn, BlackProPawn.FlipColour());
            Assert.AreEqual(WhiteProLance, BlackProLance.FlipColour());
            Assert.AreEqual(WhiteProKnight, BlackProKnight.FlipColour());
            Assert.AreEqual(WhiteProSilver, BlackProSilver.FlipColour());
            Assert.AreEqual(WhiteHorse, BlackHorse.FlipColour());
            Assert.AreEqual(WhiteDragon, BlackDragon.FlipColour());
            Assert.AreEqual(BlackPawn, WhitePawn.FlipColour());
            Assert.AreEqual(BlackLance, WhiteLance.FlipColour());
            Assert.AreEqual(BlackKnight, WhiteKnight.FlipColour());
            Assert.AreEqual(BlackSilver, WhiteSilver.FlipColour());
            Assert.AreEqual(BlackBishop, WhiteBishop.FlipColour());
            Assert.AreEqual(BlackRook, WhiteRook.FlipColour());
            Assert.AreEqual(BlackGold, WhiteGold.FlipColour());
            Assert.AreEqual(BlackKing, WhiteKing.FlipColour());
            Assert.AreEqual(BlackProPawn, WhiteProPawn.FlipColour());
            Assert.AreEqual(BlackProLance, WhiteProLance.FlipColour());
            Assert.AreEqual(BlackProKnight, WhiteProKnight.FlipColour());
            Assert.AreEqual(BlackProSilver, WhiteProSilver.FlipColour());
            Assert.AreEqual(BlackHorse, WhiteHorse.FlipColour());
            Assert.AreEqual(BlackDragon, WhiteDragon.FlipColour());
        }

        [Test]
        public void WithColourTest()
        {
            Assert.AreEqual(BlackPawn, BlackPawn.With(Colour.Black));
            Assert.AreEqual(BlackLance, BlackLance.With(Colour.Black));
            Assert.AreEqual(BlackKnight, BlackKnight.With(Colour.Black));
            Assert.AreEqual(BlackSilver, BlackSilver.With(Colour.Black));
            Assert.AreEqual(BlackBishop, BlackBishop.With(Colour.Black));
            Assert.AreEqual(BlackRook, BlackRook.With(Colour.Black));
            Assert.AreEqual(BlackGold, BlackGold.With(Colour.Black));
            Assert.AreEqual(BlackKing, BlackKing.With(Colour.Black));
            Assert.AreEqual(BlackProPawn, BlackProPawn.With(Colour.Black));
            Assert.AreEqual(BlackProLance, BlackProLance.With(Colour.Black));
            Assert.AreEqual(BlackProKnight, BlackProKnight.With(Colour.Black));
            Assert.AreEqual(BlackProSilver, BlackProSilver.With(Colour.Black));
            Assert.AreEqual(BlackHorse, BlackHorse.With(Colour.Black));
            Assert.AreEqual(BlackDragon, BlackDragon.With(Colour.Black));

            Assert.AreEqual(WhitePawn, BlackPawn.With(Colour.White));
            Assert.AreEqual(WhiteLance, BlackLance.With(Colour.White));
            Assert.AreEqual(WhiteKnight, BlackKnight.With(Colour.White));
            Assert.AreEqual(WhiteSilver, BlackSilver.With(Colour.White));
            Assert.AreEqual(WhiteBishop, BlackBishop.With(Colour.White));
            Assert.AreEqual(WhiteRook, BlackRook.With(Colour.White));
            Assert.AreEqual(WhiteGold, BlackGold.With(Colour.White));
            Assert.AreEqual(WhiteKing, BlackKing.With(Colour.White));
            Assert.AreEqual(WhiteProPawn, BlackProPawn.With(Colour.White));
            Assert.AreEqual(WhiteProLance, BlackProLance.With(Colour.White));
            Assert.AreEqual(WhiteProKnight, BlackProKnight.With(Colour.White));
            Assert.AreEqual(WhiteProSilver, BlackProSilver.With(Colour.White));
            Assert.AreEqual(WhiteHorse, BlackHorse.With(Colour.White));
            Assert.AreEqual(WhiteDragon, BlackDragon.With(Colour.White));

            Assert.AreEqual(BlackPawn, WhitePawn.With(Colour.Black));
            Assert.AreEqual(BlackLance, WhiteLance.With(Colour.Black));
            Assert.AreEqual(BlackKnight, WhiteKnight.With(Colour.Black));
            Assert.AreEqual(BlackSilver, WhiteSilver.With(Colour.Black));
            Assert.AreEqual(BlackBishop, WhiteBishop.With(Colour.Black));
            Assert.AreEqual(BlackRook, WhiteRook.With(Colour.Black));
            Assert.AreEqual(BlackGold, WhiteGold.With(Colour.Black));
            Assert.AreEqual(BlackKing, WhiteKing.With(Colour.Black));
            Assert.AreEqual(BlackProPawn, WhiteProPawn.With(Colour.Black));
            Assert.AreEqual(BlackProLance, WhiteProLance.With(Colour.Black));
            Assert.AreEqual(BlackProKnight, WhiteProKnight.With(Colour.Black));
            Assert.AreEqual(BlackProSilver, WhiteProSilver.With(Colour.Black));
            Assert.AreEqual(BlackHorse, WhiteHorse.With(Colour.Black));
            Assert.AreEqual(BlackDragon, WhiteDragon.With(Colour.Black));

            Assert.AreEqual(WhitePawn, WhitePawn.With(Colour.White));
            Assert.AreEqual(WhiteLance, WhiteLance.With(Colour.White));
            Assert.AreEqual(WhiteKnight, WhiteKnight.With(Colour.White));
            Assert.AreEqual(WhiteSilver, WhiteSilver.With(Colour.White));
            Assert.AreEqual(WhiteBishop, WhiteBishop.With(Colour.White));
            Assert.AreEqual(WhiteRook, WhiteRook.With(Colour.White));
            Assert.AreEqual(WhiteGold, WhiteGold.With(Colour.White));
            Assert.AreEqual(WhiteKing, WhiteKing.With(Colour.White));
            Assert.AreEqual(WhiteProPawn, WhiteProPawn.With(Colour.White));
            Assert.AreEqual(WhiteProLance, WhiteProLance.With(Colour.White));
            Assert.AreEqual(WhiteProKnight, WhiteProKnight.With(Colour.White));
            Assert.AreEqual(WhiteProSilver, WhiteProSilver.With(Colour.White));
            Assert.AreEqual(WhiteHorse, WhiteHorse.With(Colour.White));
            Assert.AreEqual(WhiteDragon, WhiteDragon.With(Colour.White));
        }

        [Test]
        public void GetRawTypeTest()
        {
            Assert.AreEqual(Pawn, BlackPawn.GetRawType());
            Assert.AreEqual(Lance, BlackLance.GetRawType());
            Assert.AreEqual(Knight, BlackKnight.GetRawType());
            Assert.AreEqual(Silver, BlackSilver.GetRawType());
            Assert.AreEqual(Bishop, BlackBishop.GetRawType());
            Assert.AreEqual(Rook, BlackRook.GetRawType());
            Assert.AreEqual(Gold, BlackGold.GetRawType());
            Assert.AreEqual(King, BlackKing.GetRawType());
            Assert.AreEqual(Pawn, BlackProPawn.GetRawType());
            Assert.AreEqual(Lance, BlackProLance.GetRawType());
            Assert.AreEqual(Knight, BlackProKnight.GetRawType());
            Assert.AreEqual(Silver, BlackProSilver.GetRawType());
            Assert.AreEqual(Bishop, BlackHorse.GetRawType());
            Assert.AreEqual(Rook, BlackDragon.GetRawType());
            Assert.AreEqual(Pawn, WhitePawn.GetRawType());
            Assert.AreEqual(Lance, WhiteLance.GetRawType());
            Assert.AreEqual(Knight, WhiteKnight.GetRawType());
            Assert.AreEqual(Silver, WhiteSilver.GetRawType());
            Assert.AreEqual(Bishop, WhiteBishop.GetRawType());
            Assert.AreEqual(Rook, WhiteRook.GetRawType());
            Assert.AreEqual(Gold, WhiteGold.GetRawType());
            Assert.AreEqual(King, WhiteKing.GetRawType());
            Assert.AreEqual(Pawn, WhiteProPawn.GetRawType());
            Assert.AreEqual(Lance, WhiteProLance.GetRawType());
            Assert.AreEqual(Knight, WhiteProKnight.GetRawType());
            Assert.AreEqual(Silver, WhiteProSilver.GetRawType());
            Assert.AreEqual(Bishop, WhiteHorse.GetRawType());
            Assert.AreEqual(Rook, WhiteDragon.GetRawType());
        }

        [Test]
        public void GetPieceTypeTest()
        {
            Assert.AreEqual(Pawn, BlackPawn.GetPieceType());
            Assert.AreEqual(Lance, BlackLance.GetPieceType());
            Assert.AreEqual(Knight, BlackKnight.GetPieceType());
            Assert.AreEqual(Silver, BlackSilver.GetPieceType());
            Assert.AreEqual(Bishop, BlackBishop.GetPieceType());
            Assert.AreEqual(Rook, BlackRook.GetPieceType());
            Assert.AreEqual(Gold, BlackGold.GetPieceType());
            Assert.AreEqual(King, BlackKing.GetPieceType());
            Assert.AreEqual(ProPawn, BlackProPawn.GetPieceType());
            Assert.AreEqual(ProLance, BlackProLance.GetPieceType());
            Assert.AreEqual(ProKnight, BlackProKnight.GetPieceType());
            Assert.AreEqual(ProSilver, BlackProSilver.GetPieceType());
            Assert.AreEqual(Horse, BlackHorse.GetPieceType());
            Assert.AreEqual(Dragon, BlackDragon.GetPieceType());
            Assert.AreEqual(Pawn, WhitePawn.GetPieceType());
            Assert.AreEqual(Lance, WhiteLance.GetPieceType());
            Assert.AreEqual(Knight, WhiteKnight.GetPieceType());
            Assert.AreEqual(Silver, WhiteSilver.GetPieceType());
            Assert.AreEqual(Bishop, WhiteBishop.GetPieceType());
            Assert.AreEqual(Rook, WhiteRook.GetPieceType());
            Assert.AreEqual(Gold, WhiteGold.GetPieceType());
            Assert.AreEqual(King, WhiteKing.GetPieceType());
            Assert.AreEqual(ProPawn, WhiteProPawn.GetPieceType());
            Assert.AreEqual(ProLance, WhiteProLance.GetPieceType());
            Assert.AreEqual(ProKnight, WhiteProKnight.GetPieceType());
            Assert.AreEqual(ProSilver, WhiteProSilver.GetPieceType());
            Assert.AreEqual(Horse, WhiteHorse.GetPieceType());
            Assert.AreEqual(Dragon, WhiteDragon.GetPieceType());
        }
    }
}
#endif
