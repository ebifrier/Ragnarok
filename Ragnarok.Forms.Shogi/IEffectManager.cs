using System;
using System.Collections.Generic;

using Ragnarok.Shogi;

namespace Ragnarok.Forms.Shogi
{
    /// <summary>
    /// 将棋盤のエフェクト管理オブジェクトです。
    /// </summary>
    [CLSCompliant(false)]
    public interface IEffectManager
    {
        /// <summary>
        /// エフェクトを表示するオブジェクトを取得または設定します。
        /// </summary>
        View.GLShogiElement Container
        {
            get;
            set;
        }

        /// <summary>
        /// 初期化時・オブジェクトの破棄時などに呼ばれます。
        /// </summary>
        void Clear();

        /// <summary>
        /// 局面を初期化した時に呼ばれ、エフェクトを初期化します。
        /// </summary>
        void InitEffect(BWType bwType);

        /// <summary>
        /// エフェクトを追加します。
        /// </summary>
        void Moved(BoardMove move, bool isUndo);

        /// <summary>
        /// 駒の移動を開始したときに呼ばれます。
        /// </summary>
        void BeginMove(Square square, BoardPiece piece);

        /// <summary>
        /// 駒の移動が終わったときに呼ばれます。
        /// </summary>
        void EndMove();
    }
}
