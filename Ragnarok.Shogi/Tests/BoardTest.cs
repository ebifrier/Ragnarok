#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Ragnarok.Shogi.Tests
{
    using Sfen;

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

                    move.DstSquare = sq;
                    if (avail != null)
                    {
                        if (avail.Item2)
                        {
                            // 成りが必須の場合
                            move.ActionType = ActionType.None;
                            Assert.False(board.CanMove(move));

                            move.ActionType = ActionType.Unpromote;
                            Assert.False(board.CanMove(move));

                            move.ActionType = ActionType.Promote;
                            Assert.True(board.CanMove(move));
                        }
                        else
                        {
                            // 成りが必須でない場合
                            move.ActionType = ActionType.None;
                            Assert.True(board.CanMove(move));

                            move.ActionType = ActionType.Unpromote;
                            Assert.True(board.CanMove(move));

                            move.ActionType = ActionType.Promote;
                            Assert.AreEqual(Board.CanPromote(move), board.CanMove(move));
                        }
                    }
                    else
                    {
                        // そもそも移動できる場所ではない
                        move.ActionType = ActionType.None;
                        Assert.False(board.CanMove(move));

                        move.ActionType = ActionType.Unpromote;
                        Assert.False(board.CanMove(move));

                        move.ActionType = ActionType.Promote;
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
            var board = SfenBoard.Parse(sfen);

            Assert.NotNull(board);
            Assert.True(board.Validate());
            return board;
        }

        [Test()]
        public void MoveBlackTest1()
        {
            var board = MakeBoard1(BWType.Black);

            var move = new BoardMove
            {
                DstSquare = new Square(8, 2),
                SrcSquare = new Square(8, 3),
                MovePiece = new Piece(PieceType.Kyo, false),
                ActionType = ActionType.Promote,
                BWType = BWType.Black,
            };
            Assert.True(board.CanMove(move));

            // 駒が設定されてないと動けません。
            move.MovePiece = new Piece();
            Assert.False(board.CanMove(move));
            move.MovePiece = new Piece(PieceType.Kyo, false);

            // 84の駒は移動できません。
            move.SrcSquare = new Square(8, 4);
            Assert.False(board.CanMove(move));
            move.SrcSquare = new Square(8, 3);

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

            var move = new BoardMove
            {
                DstSquare = new Square(9, 8),
                SrcSquare = new Square(9, 7),
                MovePiece = new Piece(PieceType.Hu, false),
                ActionType = ActionType.Promote,
                BWType = BWType.White,
            };
            Assert.True(board.CanMove(move));

            // 84の駒は移動できません。
            move.SrcSquare = new Square(8, 4);
            Assert.False(board.CanMove(move));
            move.SrcSquare = new Square(9, 7);

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
    }
}
#endif
