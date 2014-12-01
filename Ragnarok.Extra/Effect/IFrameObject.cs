using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Extra.Effect
{
    /// <summary>
    /// フレーム毎に更新されるオブジェクトの基本インターフェースです。
    /// </summary>
    public interface IFrameObject
    {
        /// <summary>
        /// フレーム毎に呼ばれ、オブジェクトの更新処理を行います。
        /// </summary>
        void DoEnterFrame(TimeSpan elapsedTime, object state);
    }
}
