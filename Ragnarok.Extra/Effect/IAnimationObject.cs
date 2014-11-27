using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Extra.Effect
{
    /// <summary>
    /// アニメーションの基本インターフェースです。
    /// </summary>
    public interface IAnimationObject : IFrameObject
    {
        /// <summary>
        /// アニメーションを開始します。
        /// </summary>
        void Begin(object target);

        /// <summary>
        /// アニメーションを停止します。
        /// </summary>
        void Stop();
    }
}
