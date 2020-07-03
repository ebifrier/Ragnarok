#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using Ragnarok.Utility;

namespace Ragnarok.Shogi.Tests
{
    [TestFixture()]
    public sealed class BoardTest
    {
        /// <summary>
        /// 駒の各移動先に対して、その手が指せるのかどうかをチェックします。
        /// </summary>
        private static void CanMoveTo(Board board, Move move,
                                      List<Tuple<Square, bool>> availables)
        {
            foreach (var sq in Board.Squares())
            {
                var avail = availables.FirstOrDefault(_ => _.Item1 == sq);

                MethodUtil.SetPropertyValue(move, nameof(Move.DstSquare), sq);
                if (avail != null)
                {
                    if (avail.Item2)
                    {
                        // 成りが必須の場合
                        move.IsPromote = false;
                        Assert.False(board.CanMove(move));

                        move.IsPromote = true;
                        Assert.True(board.CanMove(move));
                    }
                    else
                    {
                        // 成りが必須でない場合
                        move.IsPromote = false;
                        Assert.True(board.CanMove(move));

                        move.IsPromote = true;
                        Assert.AreEqual(Board.CanPromote(move), board.CanMove(move));
                    }
                }
                else
                {
                    // そもそも移動できる場所ではない
                    move.IsPromote = false;
                    Assert.False(board.CanMove(move));

                    move.IsPromote = true;
                    Assert.False(board.CanMove(move));
                }
            }
        }

        private static Board MakeBoard1(Colour turn)
        {
            /*
            後手の持駒：飛　桂　香　歩三
              ９ ８ ７ ６ ５ ４ ３ ２ １
            +---------------------------+
            |v香 ・ 龍 ・ ・ ・ ・ ・ ・|一
            | ・ ・ ・ ・v玉 ・ ・ ・ ・|二
            | ・ 香v歩 ・v金 ・v桂 ・ ・|三
            | ・ 歩v銀v銀v金 ・ ・ 銀 ・|四
            | 歩 ・ ・ ・ ・ 歩 歩 ・ ・|五
            | 香 ・ 歩 ・ 馬 ・ ・ ・v歩|六
            |v歩 ・ 桂 金 ・ ・ ・ ・ ・|七
            | ・ 玉 銀 金 ・ ・ 馬 ・ ・|八
            | ・ ・ ・ ・ ・ ・ ・ ・ ・|九
            +---------------------------+
            先手の持駒：桂　歩七
            */

            var turnCh = (turn == Colour.Black ? 'b' : 'w');
            var sfen =
                "l1+R6/4k4/1Lp1g1n2/1Pssg2S1/P4PP2/L1P1+B3p/p1NG5/1KSG2+B2/9" +
                " " + turnCh + " N7Prnl3p 1";
            var board = Board.ParseSfen(sfen);

            Assert.NotNull(board);
            Assert.True(board.Validate());
            return board;
        }

        [Test()]
        public void MoveBlackTest()
        {
            var board = MakeBoard1(Colour.Black);

            var move = Move.CreateMove(
                Piece.BlackLance,
                Square.SQ83, Square.SQ82,
                true);
            Assert.True(board.CanMove(move));

            // 駒が設定されてないと動けません。
            MethodUtil.SetPropertyValue(move, "MovePiece", Piece.ProPawn);
            Assert.False(board.CanMove(move));
            MethodUtil.SetPropertyValue(move, "MovePiece", Piece.Lance);

            // 84の駒は移動できません。
            MethodUtil.SetPropertyValue(move, "SrcSquare", Square.SQ84);
            Assert.False(board.CanMove(move));
            MethodUtil.SetPropertyValue(move, "SrcSquare", Square.SQ83);

            CanMoveTo(board, move, new List<Tuple<Square, bool>>
            {
                Tuple.Create(Square.SQ82, false),
                Tuple.Create(Square.SQ81, true),
            });
        }

        [Test()]
        public void MoveWhiteTest()
        {
            var board = MakeBoard1(Colour.White);

            var move = Move.CreateMove(
                Piece.WhitePawn, Square.SQ97, Square.SQ98,
                true);
            Assert.True(board.CanMove(move));

            // 84の駒は移動できません。
            MethodUtil.SetPropertyValue(move, "SrcSquare", Square.SQ84);
            Assert.False(board.CanMove(move));
            MethodUtil.SetPropertyValue(move, "SrcSquare", Square.SQ97);

            CanMoveTo(board, move, new List<Tuple<Square, bool>>
            {
                Tuple.Create(Square.SQ98, false),
            });
        }

        [Test()]
        public void DoublePawnTest()
        {
            /*
            後手の持駒：飛　桂　香　歩三
              ９ ８ ７ ６ ５ ４ ３ ２ １
            +---------------------------+
            |v香 ・ 龍 ・ ・ ・ ・ ・ ・|一
            | ・ ・ ・ ・v玉 ・ ・ ・ ・|二
            | ・ 香v歩 ・v金 ・v桂 ・ ・|三
            | ・ 歩v銀v銀v金 ・ ・ 銀 ・|四
            | 歩 ・ ・ ・ ・ 歩 歩 ・ ・|五
            | 香 ・ 歩 ・ 馬 ・ ・ ・v歩|六
            |v歩 ・ 桂 金 ・ ・ ・ ・ ・|七
            | ・ 玉 銀 金 ・ ・ 馬 ・ ・|八
            | ・ ・ ・ ・ ・ ・ ・ ・ ・|九
            +---------------------------+
            先手の持駒：桂　歩七
            */

            var board = MakeBoard1(Colour.Black);
            var move = Move.CreateDrop(Piece.BlackPawn, Square.SQ34);
            Assert.False(board.CanMove(move));
            move = Move.CreateDrop(Piece.BlackPawn, Square.SQ93);
            Assert.False(board.CanMove(move));

            board = MakeBoard1(Colour.White);
            move = Move.CreateDrop(Piece.WhitePawn, Square.SQ75);
            Assert.False(board.CanMove(move));
            move = Move.CreateDrop(Piece.WhitePawn, Square.SQ13);
            Assert.False(board.CanMove(move));
        }

        [Test()]
        public void HashTest()
        {
            /*
            後手の持駒：飛　桂　香　歩三
              ９ ８ ７ ６ ５ ４ ３ ２ １
            +---------------------------+
            |v香 ・ 龍 ・ ・ ・ ・ ・ ・|一
            | ・ ・ ・ ・v玉 ・ ・ ・ ・|二
            | ・ 香v歩 ・v金 ・v桂 ・ ・|三
            | ・ 歩v銀v銀v金 ・ ・ 銀 ・|四
            | 歩 ・ ・ ・ ・ 歩 歩 ・ ・|五
            | 香 ・ 歩 ・ 馬 ・ ・ ・v歩|六
            |v歩 ・ 桂 金 ・ ・ ・ ・ ・|七
            | ・ 玉 銀 金 ・ ・ 馬 ・ ・|八
            | ・ ・ ・ ・ ・ ・ ・ ・ ・|九
            +---------------------------+
            先手の持駒：桂　歩七
            */

            var sfen =
                "l1+R6/4k4/1Lp1g1n2/1Pssg2S1/P4PP2/L1P1+B3p/p1NG5/1KSG2+B2/9" +
                " b N7Prnl3p 1";
            var board1 = Board.ParseSfen(sfen);
            var board2 = Board.ParseSfen(sfen);

            Assert.AreEqual(board1.HashValue, board2.HashValue);
            Assert.AreNotSame(board1.HashValue, (new Board()).HashValue);

            board2.SetHand(Piece.BlackPawn, 6);
            Assert.AreNotSame(board1.HashValue, board2.HashValue);

            board2 = board1.Clone();
            board2.SetHand(Piece.BlackKnight, 0);
            board2.SetHand(Piece.WhiteKnight, 2);
            Assert.AreNotSame(board1.HashValue, board2.HashValue);

            board2 = board1.Clone();
            board2[9, 1] = board2[9, 1].FlipColour();
            Assert.AreNotSame(board1.HashValue, board2.HashValue);

            board2 = board1.Clone();
            board2[7, 6] = board2[7, 6].FlipColour();
            Assert.AreNotSame(board1.HashValue, board2.HashValue);
        }

        [Test()]
        public void ListupBlackTest()
        {
            var board = MakeBoard1(Colour.Black);

            var list = board
                .ListupMoves(Piece.BlackDragon, Square.SQ41)
                .ToList();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(Square.SQ71, list[0].SrcSquare);

            list = board
                .ListupMoves(Piece.BlackDragon, Square.SQ32)
                .ToList();
            Assert.AreEqual(0, list.Count);

            list = board
                .ListupMoves(Piece.BlackHorse, Square.SQ47)
                .ToList();
            Assert.AreEqual(2, list.Count);

            list = board
                .ListupMoves(Piece.BlackGold, Square.SQ57)
                .ToList();
            Assert.AreEqual(2, list.Count);
        }

        [Test()]
        public void ListupWhiteTest()
        {
            var board = MakeBoard1(Colour.White);

            var list = board
                .ListupMoves(Piece.WhiteSilver, Square.SQ65)
                .ToList();
            Assert.AreEqual(2, list.Count);
        }

        [Test()]
        public void FlipPiecesTest()
        {
            var bod1 =
                "後手の持駒：飛　桂　香　歩三　\n" +
                "  ９ ８ ７ ６ ５ ４ ３ ２ １\n" +
                "+---------------------------+\n" +
                "|v香 ・ 龍 ・ ・ ・ ・ ・ ・|一\n" +
                "| ・ ・ ・ ・v玉 ・ ・ ・ ・|二\n" +
                "| ・ 香v歩 ・v金 ・v桂 ・ ・|三\n" +
                "| ・ 歩v銀v銀v金 ・ ・ 銀 ・|四\n" +
                "| 歩 ・ ・ ・ ・ 歩 歩 ・ ・|五\n" +
                "| 香 ・ 歩 ・ 馬 ・ ・ ・v歩|六\n" +
                "|v歩 ・ 桂 金 ・ ・ ・ ・ ・|七\n" +
                "| ・ 玉 銀 金 ・ ・ 馬 ・ ・|八\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|九\n" +
                "+---------------------------+\n" +
                "先手の持駒：桂　歩七　";
            var bod2 =
                "後手の持駒：桂　歩七　\n" +
                "  ９ ８ ７ ６ ５ ４ ３ ２ １\n" +
                "+---------------------------+\n" +
                "| ・ ・ ・ ・ ・ ・ ・ ・ ・|一\n" +
                "| ・ ・v馬 ・ ・v金v銀v玉 ・|二\n" +
                "| ・ ・ ・ ・ ・v金v桂 ・ 歩|三\n" +
                "| 歩 ・ ・ ・v馬 ・v歩 ・v香|四\n" +
                "| ・ ・v歩v歩 ・ ・ ・ ・v歩|五\n" +
                "| ・v銀 ・ ・ 金 銀 銀v歩 ・|六\n" +
                "| ・ ・ 桂 ・ 金 ・ 歩v香 ・|七\n" +
                "| ・ ・ ・ ・ 玉 ・ ・ ・ ・|八\n" +
                "| ・ ・ ・ ・ ・ ・v龍 ・ 香|九\n" +
                "+---------------------------+\n" +
                "先手の持駒：飛　桂　香　歩三　";

            var kifu1 = KifuReader.LoadFrom(bod1);
            Assert.NotNull(kifu1);

            var board1 = kifu1.CreateBoard();
            Assert.NotNull(board1);
            Assert.True(board1.Validate());

            var kifu2 = KifuReader.LoadFrom(bod2);
            Assert.NotNull(kifu2);

            var board2 = kifu2.CreateBoard();
            Assert.NotNull(board2);
            Assert.True(board2.Validate());

            var flippedBoard = board1.FlipPieces();
            Assert.True(Board.BoardEquals(flippedBoard, board2));

            Assert.True(Board.BoardEquals(flippedBoard.FlipPieces(), board1));
        }
    }
}
#endif
