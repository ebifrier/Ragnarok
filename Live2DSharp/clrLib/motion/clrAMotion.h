#pragma once

#include "CppSharp.h"
#include <motion/AMotion.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class ALive2DModel;
    ref class AMotion;
    ref class MotionQueueEnt;
}

namespace Live2DSharp
{
    public ref class AMotion : Live2DSharp::LDObject
    {
    public:

        AMotion(::live2d::AMotion* native);
        static AMotion^ __CreateInstance(::System::IntPtr native);
        AMotion();

        property int FadeIn
        {
            int get();
            void set(int);
        }

        property int FadeOut
        {
            int get();
            void set(int);
        }

        property float Weight
        {
            float get();
            void set(float);
        }

        property int DurationMSec
        {
            int get();
        }

        property int LoopDurationMSec
        {
            int get();
        }

        property int OffsetMSec
        {
            void set(int);
        }

        void updateParam(Live2DSharp::ALive2DModel^ model, Live2DSharp::MotionQueueEnt^ motionQueueEnt);

        void reinit();
    };
}
