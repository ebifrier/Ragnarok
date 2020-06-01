using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Ragnarok.Utility;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 先後の情報を示します。
    /// </summary>
    /// <remarks>
    /// ColorではGUI用のColorクラスと名前が同じになってしまうため
    /// イギリス式のColourにしています。
    /// </remarks>
    [DataContract()]
    public enum Colour
    {
        /// <summary>
        /// 駒箱の駒などです。
        /// </summary>
        [EnumMember()]
        [Label(Label = "手番なし")]
        None = 0,

        /// <summary>
        /// 先手です。
        /// </summary>
        [EnumMember()]
        [Label(Label = "先手")]
        Black = 1,

        /// <summary>
        /// 後手です。
        /// </summary>
        [EnumMember()]
        [Label(Label = "後手")]
        White = 2,
    }

    /// <summary>
    /// <see cref="Colour"/>の拡張メソッド用クラスです。
    /// </summary>
    public static class ColourUtil
    {
        /// <summary>
        /// None, Black, Whiteを返します。
        /// </summary>
        public static IEnumerable<Colour> Values()
        {
            yield return Colour.None;
            yield return Colour.Black;
            yield return Colour.White;
        }

        /// <summary>
        /// Black, Whiteを返します。
        /// </summary>
        public static IEnumerable<Colour> BlackWhite()
        {
            yield return Colour.Black;
            yield return Colour.White;
        }

        /// <summary>
        /// 手番の先後を入れ替えます。
        /// </summary>
        public static Colour Flip(this Colour self)
        {
            if (self != Colour.None)
            {
                return (
                    self == Colour.Black ?
                    Colour.White :
                    Colour.Black);
            }
            else
            {
                return Colour.None;
            }
        }

        /// <summary>
        /// 手番を0 or 1のインデックスに変換します。
        /// </summary>
        public static int GetIndex(this Colour self)
        {
            return (self != Colour.White ? 0 : 1);
        }

        /// <summary>
        /// 先手なら+1、後手なら-1を返します。
        /// </summary>
        public static int Sign(this Colour self)
        {
            return (self == Colour.White ? -1 : +1);
        }
    }
}
