#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Ragnarok.Shogi.Sfen.Tests
{
    [TestFixture()]
    internal sealed class SfenTest
    {
        [Test()]
        public void ParseBoardTest()
        {
            // 初期局面
            Assert.True(Board.BoardEquals(
                new Board(),
                Board.ParseSfen(
                    "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL" +
                    " b - 1")));

            // 最後の数字はなくてもよい
            Assert.True(Board.BoardEquals(
                new Board(),
                Board.ParseSfen(
                    "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL" +
                    " b -")));

            // 持ち駒はないとダメ
            Assert.Catch<SfenException>(() =>
                Board.ParseSfen(
                    "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL b"));

            // 筋が少ない
            Assert.Catch<SfenException>(() =>
                Board.ParseSfen(
                    "9/9/p/9/9/9/9/9/9 b -"));

            // 筋が多い
            Assert.Catch<SfenException>(() =>
                Board.ParseSfen(
                    "9/9/p9/9/9/9/9/9/9 b -"));

            // 段が多い
            Assert.Catch<SfenException>(() =>
                Board.ParseSfen(
                    "9/9/9/9/9/9/9/9/9/p b -"));

            // 段が少ないのは問題にしない
            Assert.True(Board.BoardEquals(
                new Board(false),
                Board.ParseSfen("9/9/9/9/9/9/9/9 b -")));

            // 成れない駒を成る
            Assert.True(Board.BoardEquals(
                Board.ParseSfen("9/9/9/9/9/9/9/G8/9 b -"),
                Board.ParseSfen("9/9/9/9/9/9/9/+G8/9 b -")));
        }

        [Test()]
        public void ParseBoardTest2()
        {
            var board = new Board(false);

            board[9, 1] = new BoardPiece(Piece.Kyo, BWType.White);
            board[6, 1] = new BoardPiece(Piece.Kei, BWType.White);
            board[2, 1] = new BoardPiece(Piece.Kei, BWType.White);
            board[1, 1] = new BoardPiece(Piece.Kyo, BWType.White);
            board[3, 2] = new BoardPiece(Piece.Kyo, BWType.White);
            board[2, 2] = new BoardPiece(Piece.Hisya, BWType.White);
            board[2, 3] = new BoardPiece(Piece.Gyoku, BWType.White);
            board[9, 4] = new BoardPiece(Piece.Hu, BWType.White);
            board[8, 4] = new BoardPiece(Piece.Hu, BWType.White);
            board[5, 4] = new BoardPiece(Piece.Ryu, BWType.Black);
            board[2, 5] = new BoardPiece(Piece.Kin, BWType.Black);
            board[1, 5] = new BoardPiece(Piece.Hu, BWType.White);
            board[9, 6] = new BoardPiece(Piece.Hu, BWType.Black);
            board[8, 6] = new BoardPiece(Piece.Hu, BWType.Black);
            board[7, 6] = new BoardPiece(Piece.Gyoku, BWType.Black);
            board[6, 6] = new BoardPiece(Piece.Kin, BWType.Black);
            board[5, 6] = new BoardPiece(Piece.Kei, BWType.Black);
            board[3, 6] = new BoardPiece(Piece.Hu, BWType.Black);
            board[1, 6] = new BoardPiece(Piece.Hu, BWType.Black);
            board[4, 7] = new BoardPiece(Piece.Hu, BWType.Black);
            board[5, 8] = new BoardPiece(Piece.Gin, BWType.Black);
            board[9, 9] = new BoardPiece(Piece.Kyo, BWType.Black);
            board[4, 9] = new BoardPiece(Piece.Kei, BWType.Black);
            board[1, 9] = new BoardPiece(Piece.Uma, BWType.White);

            // 駒台の設定
            board.SetHand(PieceType.Kaku, BWType.Black, 1);
            board.SetHand(PieceType.Kin, BWType.Black, 2);
            board.SetHand(PieceType.Gin, BWType.Black, 2);
            board.SetHand(PieceType.Hu, BWType.Black, 10);
            board.SetHand(PieceType.Gin, BWType.White, 1);

            // 手番
            board.Turn = BWType.White;

            Assert.True(Board.BoardEquals(
                Board.ParseSfen("l2n3nl/6lr1/7k1/pp2+R4/7Gp/PPKGN1P1P/5P3/4S4/L4N2+b w B2G2S10Ps 1"),
                board));
        }

        private List<Move> MakeMoveList(string sfen)
        {
            var board = new Board();

            var sfenList = sfen.Split(
                new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            return board.SfenToMoveList(sfenList).ToList();
        }

        [Test()]
        public void SfenToBoardMoveTest()
        {
            // すべて正しい指し手
            var moveList = MakeMoveList("1g1f 4a3b 6i7h");
            Assert.AreEqual(3, moveList.Count());

            // 2つめの指し手が正しくない
            moveList = MakeMoveList("1g1f 4a8b 6i7h");
            Assert.AreEqual(1, moveList.Count());
        }

        private void BoardAndMoveTest(string boardSfen, string moveListSfen)
        {
            // 指し手から局面を作ります。
            var board1 = new Board();
            var sfenMoveList =
                moveListSfen.Split(
                    new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            // 指し手を読み込みます。
            var moveList = board1.SfenToMoveList(sfenMoveList)
                .ToList();
            Assert.AreEqual(sfenMoveList.Count(), moveList.Count());

            moveList.ForEach(_ => Assert.True(board1.DoMove(_)));

            // 出力したSFEN形式の指し手が同じかどうかの確認もします。
            var result = string.Join(" ",
                moveList.Select(_ => _.ToSfen()).ToArray());
            Assert.AreEqual(moveListSfen, result);


            // 局面を直接読み込みます。
            var board2 = Board.ParseSfen(boardSfen);            

            // 出力したSFEN形式の局面が同じかどうかの確認もします。
            // (最後の数字には意味がないため、意図的に削っています)
            result = board2.ToSfen();
            Assert.AreEqual(
                boardSfen.Substring(0, boardSfen.Length - 1),
                result.Substring(0, result.Length - 1));


            // ２つの局面を比較します。
            Assert.True(Board.BoardEquals(board1, board2));
        }

        [Test()]
        public void ComplexTest()
        {
            BoardAndMoveTest(
                "lnsgkgsnl/1r5b1/ppppppppp/9/9/9/PPPPPPPPP/1B5R1/LNSGKGSNL" +
                " b - 1",
                "");

            BoardAndMoveTest(
                "l8/4+R4/1L2pgn2/1Ps1k2S1/P2p1SP2/LlP2b2p/ppNGP4/2S6/2KG5" +
                " w RBG6P2n2p 1",

                "7g7f 3c3d 2g2f 2b3c 8h3c+ 2a3c 5i6h 8b4b 6h7h 5a6b " +
                "7h8h 6b7b 3i4h 2c2d 7i7h 4b2b 4i5h 3a3b 6g6f 7b8b " +
                "9g9f 9c9d 5h6g 7a7b 3g3f 2d2e 2f2e 2b2e P*2f 2e2a " +
                "4g4f 4a5b 4h4g 4c4d 2i3g B*1e 4g3h 1e2f 2h2i 6c6d " +
                "8i7g 1c1d 1g1f 3b4c 8g8f P*2e 2i2g 2a2d 3h4g 4c5d " +
                "5g5f 5d6c 5f5e 8c8d 6f6e 6d6e B*3b 6c7d 3b4a+ 6a5a " +
                "4a3a 3d3e 5e5d 5c5d 3a3b 5a4b 3b5d 5b5c 5d5e 4b4c " +
                "3f3e 9d9e 9f9e P*9f 9i9f 5c5d 5e5f P*9g 4g3f 8d8e " +
                "8f8e 4d4e 4f4e 1d1e 1f1e 1a1e P*1f 1e1f 1i1f P*1e " +
                "P*6c 7b6c 8e8d 1e1f L*8c 8b7b 3f2e 2f3g+ 2e2d 3g2g " +
                "R*8b 7b6a 8b8a+ 6a5b N*5e R*9h 8h7i 5d5e 5f5e 2g4e " +
                "5e7c 9h9i+ G*8i 9i8i 7i8i 5b5c 8a5a G*5b 7c6b 5c5d " +
                "6b5b 6c5b 5a5b P*5c S*5f L*8f 8i7i B*4f P*5g P*8g 5f4e");
        }

        [Test()]
        public void SpecialMoveTest()
        {
            var move = Move.CreateSpecialMove(BWType.Black, SpecialMoveType.Interrupt);

            Assert.True(move.IsSpecialMove);
            Assert.AreEqual(SpecialMoveType.Interrupt, move.SpecialMoveType);
            Assert.AreEqual("", move.ToSfen());
        }
    }
}
#endif
