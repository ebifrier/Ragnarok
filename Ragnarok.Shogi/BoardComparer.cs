using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 盤面が同じであるか比較します。
    /// </summary>
    public class BoardComparer : IEqualityComparer<Board>
    {
        /// <summary>
        /// 局面の値的な比較を行います。
        /// </summary>
        public bool Equals(Board lhs, Board rhs)
        {
            // どちらもnullか、同じオブジェクトなら真を返します。
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            // objectとして比較します。比較の仕方を間違えると
            // 無限ループになるので注意が必要です。
            if ((object)lhs == null || (object)rhs == null)
            {
                return false;
            }

            return lhs.BoardEquals(rhs);
        }

        /// <summary>
        /// 局面のハッシュ値を計算します。
        /// </summary>
        public int GetHashCode(Board x)
        {
            return x.GetBoardHash();
        }
    }
}
