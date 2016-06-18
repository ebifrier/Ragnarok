using System;
using System.Collections.Generic;
using System.Linq;

namespace Ragnarok.Forms.Shogi
{
    public static class ShogiZOrder
    {
        /// <summary>
        /// 盤のＺ座標です。
        /// </summary>
        public const double BoardZ = 0.0;
        /// <summary>
        /// 盤上のオブジェクトのＺ座標です。
        /// </summary>
        public const double PostBoardZ = 1.0;
        /// <summary>
        /// 盤エフェクトのＺ座標です。
        /// </summary>
        public const double BoardEffectZ = 2.0;
        /// <summary>
        /// 駒のＺ座標です。
        /// </summary>
        public const double PieceZ = 3.0;
        /// <summary>
        /// 移動中の駒のＺ座標です。
        /// </summary>
        public const double MovingPieceZ = 4.0;
        /// <summary>
        /// 駒の上にくるエフェクト用のＺ座標です。
        /// </summary>
        public const double PostPieceZ = 5.0;
        /// <summary>
        /// エフェクト用のＺ座標です。
        /// </summary>
        public const double PreEffectZ = 6.0;
        /// <summary>
        /// エフェクト用のＺ座標です。
        /// </summary>
        public const double PreEffectZ2 = 7.0;
        /// <summary>
        /// エフェクト用のＺ座標です。
        /// </summary>
        public const double EffectZ = 8.0;
        /// <summary>
        /// エフェクト用のＺ座標です。
        /// </summary>
        public const double PostEffectZ = 8.0;
        /// <summary>
        /// エフェクト用のＺ座標です。
        /// </summary>
        public const double PostEffectZ2 = 9.0;
    }
}
