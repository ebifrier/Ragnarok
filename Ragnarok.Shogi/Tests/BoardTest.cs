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
    internal sealed class BoardTest
    {
        /// <summary>
        /// 駒の各移動先に対して、その手が指せるのかどうかをチェックします。
        /// </summary>
        private void CanMoveTo(Board board, BoardMove move,
                               List<Tuple<Square, bool>> availables)
        {
            for (var file = 1; file <= Board.BoardSize; ++file)
            {
                for (var rank = 1; rank <= Board.BoardSize; ++rank)
                {
                    var sq = new Square(file, rank);
                    var avail = availables.FirstOrDefault(_ => _.Item1 == sq);

                    MethodUtil.SetPropertyValue(move, "DstSquare", sq);
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
        }

        private Board MakeBoard1(BWType turn)
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

            var turnCh = (turn == BWType.Black ? 'b' : 'w');
            var sfen =
                "l1+R6/4k4/1Lp1g1n2/1Pssg2S1/P4PP2/L1P1+B3p/p1NG5/1KSG2+B2/9" +
                " " + turnCh + " N7Prnl3p 1";
            var board = Board.ParseSfen(sfen);

            Assert.NotNull(board);
            Assert.True(board.Validate());
            return board;
        }

        [Test()]
        public void MoveBlackTest1()
        {
            var board = MakeBoard1(BWType.Black);

            var move = BoardMove.CreateMove(
                BWType.Black, new Square(8, 3), new Square(8, 2),
                new Piece(PieceType.Kyo, false), true);
            Assert.True(board.CanMove(move));

            // 駒が設定されてないと動けません。
            MethodUtil.SetPropertyValue(move, "MovePiece", new Piece());
            Assert.False(board.CanMove(move));
            MethodUtil.SetPropertyValue(move, "MovePiece", new Piece(PieceType.Kyo, false));

            // 84の駒は移動できません。
            MethodUtil.SetPropertyValue(move, "SrcSquare", new Square(8, 4));
            Assert.False(board.CanMove(move));
            MethodUtil.SetPropertyValue(move, "SrcSquare", new Square(8, 3));

            CanMoveTo(board, move, new List<Tuple<Square, bool>>
            {
                Tuple.Create(new Square(8, 2), false),
                Tuple.Create(new Square(8, 1), true),
            });
        }

        [Test()]
        public void MoveWhiteTest1()
        {
            var board = MakeBoard1(BWType.White);

            var move = BoardMove.CreateMove(
                BWType.White, new Square(9, 7), new Square(9, 8),
                new Piece(PieceType.Hu, false), true);
            Assert.True(board.CanMove(move));

            // 84の駒は移動できません。
            MethodUtil.SetPropertyValue(move, "SrcSquare", new Square(8, 4));
            Assert.False(board.CanMove(move));
            MethodUtil.SetPropertyValue(move, "SrcSquare", new Square(9, 7));

            CanMoveTo(board, move, new List<Tuple<Square, bool>>
            {
                Tuple.Create(new Square(9, 8), false),
            });
        }

        [Test()]
        public void ListupBlackTest()
        {
            var board = MakeBoard1(BWType.Black);

            var list = board
                .ListupMoves(Piece.Ryu, BWType.Black, new Square(4, 1))
                .ToList();
            Assert.AreEqual(1, list.Count());
            Assert.AreEqual(new Square(7, 1), list[0].SrcSquare);

            list = board
                .ListupMoves(Piece.Ryu, BWType.Black, new Square(3, 2))
                .ToList();
            Assert.AreEqual(0, list.Count());

            list = board
                .ListupMoves(Piece.Uma, BWType.Black, new Square(4, 7))
                .ToList();
            Assert.AreEqual(2, list.Count());

            list = board
                .ListupMoves(Piece.Kin, BWType.Black, new Square(5, 7))
                .ToList();
            Assert.AreEqual(2, list.Count());
        }

        [Test()]
        public void ListupWhiteTest()
        {
            var board = MakeBoard1(BWType.White);

            var list = board
                .ListupMoves(Piece.Gin, BWType.White, new Square(6, 5))
                .ToList();
            Assert.AreEqual(2, list.Count());
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

            board1.FlipPieces();
            Assert.True(board1.BoardEquals(board2));
        }
    }
}
#endif
