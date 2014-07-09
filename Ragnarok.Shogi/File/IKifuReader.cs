using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Ragnarok.Shogi.File
{
    /// <summary>
    /// 記譜ファイルの読み込みを行うためのインターフェースです。
    /// </summary>
    internal interface IKifuReader
    {
        /// <summary>
        /// このReaderで与えられたファイルを処理できるか調べます。
        /// </summary>
        bool CanHandle(TextReader reader);

        /// <summary>
        /// 棋譜ファイルを読み込みます。
        /// </summary>
        KifuObject Load(TextReader reader);
    }
}
