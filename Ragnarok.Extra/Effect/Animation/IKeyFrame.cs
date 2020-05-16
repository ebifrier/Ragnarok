using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ragnarok.Extra.Effect.Animation
{
    /// <summary>
    /// キーフレームオブジェクトの基本インターフェースです。
    /// </summary>
    public interface IKeyFrame
    {
        /// <summary>
        /// キーフレーム時間を取得します。
        /// </summary>
        TimeSpan KeyTime { get; }
    }

    /// <summary>
    /// キーフレームオブジェクト用の便利クラスです。
    /// </summary>
    public static class KeyFrameUtil
    {
        /// <summary>
        /// キーフレーム配列が時刻順に並んでいるか調べます。
        /// </summary>
        /// <remarks>
        /// <paramref name="keyFrames"/>の各要素がKeyTime順に並んでいるか調べ、
        /// もし並んでいなかったら例外を返します。
        /// </remarks>
        public static void ValidateKeyFrames(IEnumerable<IKeyFrame> keyFrames)
        {
            if (keyFrames == null)
            {
                throw new ArgumentNullException(nameof(keyFrames));
            }

            IKeyFrame prev = null;
            foreach (var curr in keyFrames)
            {
                if (prev != null)
                {
                    if (prev.KeyTime > curr.KeyTime)
                    {
                        throw new EffectException(
                            "KeyFrame配列がKeyTimeの順序通りに並んでいません。");
                    }
                }

                prev = curr;
            }

            // ここまで来たということは問題ないということ。
        }

        /// <summary>
        /// 与えられた時刻に対応するKeyFrameとその一つ前にKeyFrameを探します。
        /// </summary>
        public static bool FindKeyFrame<T>(IEnumerable<T> keyFrames,
                                           TimeSpan frameTime,
                                           out T prevKeyFrame,
                                           out T currKeyFrame)
            where T: class, IKeyFrame
        {
            if (keyFrames == null)
            {
                throw new ArgumentNullException(nameof(keyFrames));
            }

            // 各キーフレームを検索し、frameTimeを含むキーフレームを探します。
            T prev = null;
            
            foreach (var curr in keyFrames)
            {
                if (prev == null)
                {
                    // 一番最初の要素は特別扱いする
                    if (frameTime <= curr.KeyTime)
                    {
                        prevKeyFrame = null;
                        currKeyFrame = curr;
                        return true;
                    }
                }
                else
                {
                    // ２つ目以降の要素の場合は、prevがありまぁす。
                    if (prev.KeyTime <= frameTime && frameTime <= curr.KeyTime)
                    {
                        prevKeyFrame = prev;
                        currKeyFrame = curr;
                        return true;
                    }
                }

                prev = curr;
            }

            // 結局見つからず
            prevKeyFrame = null;
            currKeyFrame = null;
            return false;
        }
    }
}
