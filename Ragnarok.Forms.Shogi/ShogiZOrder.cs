using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Forms.Shogi
{
    public static class ShogiZOrder
    {
        /// <summary>
        /// 背景用のＺ座標です。
        /// </summary>
        public const double BackgroundZ = -1.0;
        /// <summary>
        /// 盤のＺ座標です。
        /// </summary>
        public const double BoardZ = 0.0;
        /// <summary>
        /// 盤上のオブジェクトのＺ座標です。
        /// </summary>
        public const double PostBoardZ = 10.0;
        /// <summary>
        /// 盤エフェクトのＺ座標です。
        /// </summary>
        public const double BoardEffectZ = 20.0;
        /// <summary>
        /// 駒のＺ座標です。
        /// </summary>
        public const double PieceZ = 30.0;
        /// <summary>
        /// 移動中の駒のＺ座標です。
        /// </summary>
        public const double MovingPieceZ = 40.0;
        /// <summary>
        /// 駒の上にくるエフェクト用のＺ座標です。
        /// </summary>
        public const double PostPieceZ = 50.0;
        /// <summary>
        /// エフェクト用のＺ座標です。
        /// </summary>
        public const double PreEffectZ = 60.0;
        /// <summary>
        /// エフェクト用のＺ座標です。
        /// </summary>
        public const double PreEffectZ2 = 70.0;
        /// <summary>
        /// エフェクト用のＺ座標です。
        /// </summary>
        public const double EffectZ = 80.0;
        /// <summary>
        /// エフェクト用のＺ座標です。
        /// </summary>
        public const double PostEffectZ = 80.0;
        /// <summary>
        /// エフェクト用のＺ座標です。
        /// </summary>
        public const double PostEffectZ2 = 90.0;
    }
}
