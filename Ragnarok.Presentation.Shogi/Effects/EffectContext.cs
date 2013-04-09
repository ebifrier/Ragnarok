using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;

using Ragnarok.Shogi;

namespace Ragnarok.Presentation.Shogi.Effects
{
    /// <summary>
    /// 基本的なエフェクトのコンテキストデータです。
    /// </summary>
    public class EffectContext
    {
        /// <summary>
        /// 手番を取得または設定します。
        /// </summary>
        public BWType BWType
        {
            get;
            set;
        }

        /// <summary>
        /// 基準位置を取得または設定します。
        /// </summary>
        public Vector3D Coord
        {
            get;
            set;
        }

        /// <summary>
        /// スケールを取得または設定します。
        /// </summary>
        public Vector3D BaseScale
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 各マスに対するエフェクトのコンテキストデータです。
    /// </summary>
    /// <remarks>
    /// 位置の基準点は盤の左上となります。
    /// </remarks>
    public class CellEffectContext : EffectContext
    {
        /// <summary>
        /// 盤の基準位置を取得または設定します。
        /// </summary>
        public Vector3D BanCoord
        {
            get;
            set;
        }

        /// <summary>
        /// 盤のサイズを取得または設定します。
        /// </summary>
        public Vector3D BanScale
        {
            get;
            set;
        }

        /// <summary>
        /// 中心のマスの位置を取得または設定します。
        /// </summary>
        public Position CellPosition
        {
            get;
            set;
        }

        /// <summary>
        /// 各マスの位置を取得または設定します。
        /// </summary>
        public Position[] CellPositions
        {
            get;
            set;
        }
    }
}
