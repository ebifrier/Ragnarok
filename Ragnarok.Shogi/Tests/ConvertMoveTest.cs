#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace Ragnarok.Shogi.Tests
{
    [TestFixture]
    internal sealed class ConvertMoveTest
    {
        /// <summary>
        /// 与えられた指し手が着手可能か調べ、もし着手可能な場合はそれを正式な表記に変換します。
        /// </summary>
        public static Move NormalizeMove(Board board, Move move)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }

            if (!board.Validate())
            {
                throw new ArgumentException("board");
            }

            if (move == null)
            {
                throw new ArgumentNullException("move");
            }

            if (!move.Validate())
            {
                throw new ArgumentException("move");
            }

            // 投了は常にさせることにします。
            if (move.IsResigned)
            {
                return move;
            }

            // 一度、指し手の正規化を行います（打を消したり、左を追加するなど）
            // あり得る指し手が複数ある場合は失敗とします。
            var bmove = board.ConvertMove(move,  true);
            if (bmove == null || !board.CanMove(bmove))
            {
                return null;
            }

            // 指し手を表記形式に再度変換します。
            // 移動元の情報は使いません。("65銀(55)"という表記にはしません)
            var newMove = board.ConvertMove(bmove, false);
            if (newMove == null)
            {
                return null;
            }

            // 最後に元の文字列を保存して返します。
            newMove.OriginalText = move.OriginalText;
            return newMove;
        }

        /// <summary>
        /// 指し手の正規化ができるか調べます。
        /// </summary>
        [Test]
        public void NormalizeTest1()
        {
            var board = new Board();

            foreach (var move in SampleMove.ChoKaigi.Select(_ => _.Move))
            {
                var newMove = NormalizeMove(board, move);
                Assert.NotNull(newMove);
                Assert.True(newMove.Validate());

                // 移動元の情報は使わない
                var bmove1 = board.ConvertMove(newMove, false);
                Assert.NotNull(bmove1);
                Assert.True(bmove1.Validate());
                Assert.True(board.CanMove(bmove1));

                // 移動元の情報を使う
                var bmove2 = board.ConvertMove(newMove, true);
                Assert.NotNull(bmove2);
                Assert.True(bmove2.Validate());
                
                // 実際に着手します。
                Assert.True(board.DoMove(bmove2));
            }
        }
    }
}
#endif
