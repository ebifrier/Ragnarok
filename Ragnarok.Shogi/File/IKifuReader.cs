using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Shogi.File
{
    /// <summary>
    /// 記譜ファイルの読み込みを行うためのインターフェースです。
    /// </summary>
    internal interface IKifuReader
    {
        KifuObject LoadFrom(string text);
    }
}
