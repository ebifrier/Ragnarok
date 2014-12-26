#pragma once

#include "CppSharp.h"
#include <motion/MotionQueueEnt.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class AMotion;
    ref class Live2DMotion;
    ref class MotionQueueEnt;
}

namespace Live2DSharp
{
    /// <summary>
    /// MotionQueueManagerで再生している各モーションの管理クラス
    /// </summary>
    public ref class MotionQueueEnt : Live2DSharp::LDObject
    {
    public:

        MotionQueueEnt(::live2d::MotionQueueEnt* native);
        static MotionQueueEnt^ __CreateInstance(::System::IntPtr native);
        MotionQueueEnt();

        property long long StartTimeMSec
        {
            long long get();
            void set(long long);
        }

        property long long FadeInStartTimeMSec
        {
            long long get();
            void set(long long);
        }

        property long long EndTimeMSec
        {
            long long get();
            void set(long long);
        }

        property bool Finished
        {
            void set(bool);
        }

        property bool Available
        {
            void set(bool);
        }

        property long long State_time
        {
            long long get();
        }

        property float State_weight
        {
            float get();
        }

        property bool isFinished
        {
            bool get();
        }

        property bool isAvailable
        {
            bool get();
        }

        void startFadeout(long long fadeOutMsec);

        void setState(long long time, float weight);
    };
}
