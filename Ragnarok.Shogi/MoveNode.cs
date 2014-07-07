using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 変化を木構造で表すためのクラス
    /// </summary>
    public sealed class MoveNode
    {
        /// <summary>
        /// 手数を取得または設定します。
        /// </summary>
        public int MoveCount
        {
            get;
            set;
        }

        /// <summary>
        /// 指し手を取得または設定します。
        /// </summary>
        public BoardMove Move
        {
            get;
            set;
        }

        /// <summary>
        /// 次の指し手を取得または設定します。
        /// </summary>
        /// <remarks>
        /// index=0が本譜、それ以外は違う変化の手となります。
        /// </remarks>
        public List<MoveNode> NextNodes
        {
            get;
            set;
        }

        /// <summary>
        /// 本譜における次の指し手を取得します。
        /// </summary>
        public MoveNode NextNode
        {
            get { return NextNodes.FirstOrDefault(); }
        }

        /// <summary>
        /// 次の指し手に本譜以外の変化があるか調べます。
        /// </summary>
        public bool HasVariationNext
        {
            get { return (NextNodes.Count() > 1); }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MoveNode()
        {
            NextNodes = new List<MoveNode>();
        }
    }
}
