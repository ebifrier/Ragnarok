using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Ragnarok.Shogi.File
{
    /// <summary>
    /// 記譜ファイルの書き込みを行うためのインターフェースです。
    /// </summary>
    internal interface IKifuWriter
    {
        /// <summary>
        /// 棋譜ファイルの保存を行います。
        /// </summary>
        void Save(TextWriter writer, KifuObject kifu);
    }
}
