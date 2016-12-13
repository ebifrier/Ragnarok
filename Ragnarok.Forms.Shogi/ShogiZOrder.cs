using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Forms.Shogi
{
    public static class ShogiZOrder
    {
        /// <summary>
        /// 基本となるＺ座標です。
        /// </summary>
        public const double BaseZ = 0.0;
        /// <summary>
        /// 盤の前のＺ座標です。
        /// </summary>
        public const double PreBoardZ = BoardZ - 1.0;
        /// <summary>
        /// 盤のＺ座標です。
        /// </summary>
        public const double BoardZ = 2.0;
        /// <summary>
        /// 盤上のオブジェクトのＺ座標です。
        /// </summary>
        public const double PostBoardZ = BoardZ + 1.0;
        /// <summary>
        /// 盤エフェクトのＺ座標です。
        /// </summary>
        public const double BoardEffectZ = BoardZ + 2.0;
        /// <summary>
        /// 駒のＺ座標です。
        /// </summary>
        public const double PieceZ = 5.0;
        /// <summary>
        /// 移動中の駒のＺ座標です。
        /// </summary>
        public const double MovingPieceZ = PieceZ + 1.0;
        /// <summary>
        /// 駒の上にくるエフェクト用のＺ座標です。
        /// </summary>
        public const double PostPieceZ = PieceZ + 2.0;
        /// <summary>
        /// エフェクト用のＺ座標です。
        /// </summary>
        public const double PreEffectZ = EffectZ - 2.0;
        /// <summary>
        /// エフェクト用のＺ座標です。
        /// </summary>
        public const double PreEffectZ2 = EffectZ - 1.0;
        /// <summary>
        /// エフェクト用のＺ座標です。
        /// </summary>
        public const double EffectZ = 10.0;
        /// <summary>
        /// エフェクト用のＺ座標です。
        /// </summary>
        public const double PostEffectZ = EffectZ + 1.0;
        /// <summary>
        /// エフェクト用のＺ座標です。
        /// </summary>
        public const double PostEffectZ2 = EffectZ + 2.0;
    }
}
