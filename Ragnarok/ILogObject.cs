using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok
{
    /// <summary>
    /// ログ出力用のオブジェクトです。
    /// </summary>
    public interface ILogObject
    {
        /// <summary>
        /// ログ出力用の名前を取得します。
        /// </summary>
        string LogName { get; }
    }
}
