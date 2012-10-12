using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 先後の情報を示します。
    /// </summary>
    [DataContract()]
    public enum BWType
    {
        /// <summary>
        /// 駒箱の駒などです。
        /// </summary>
        [EnumMember()]
        None = 0,

        /// <summary>
        /// 先手です。
        /// </summary>
        [EnumMember()]
        Black = 1,

        /// <summary>
        /// 後手です。
        /// </summary>
        [EnumMember()]
        White = 2,
    }

    /// <summary>
    /// <see cref="BWType"/>の拡張メソッド用クラスです。
    /// </summary>
    public static class BWTypeExtension
    {
        /// <summary>
        /// 手番の先後を入れ替えます。
        /// </summary>
        public static BWType Toggle(this BWType self)
        {
            if (self != BWType.None)
            {
                return (
                    self == BWType.Black ?
                    BWType.White :
                    BWType.Black);
            }
            else
            {
                return BWType.None;
            }
        }
    }
}
