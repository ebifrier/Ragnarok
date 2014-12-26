#pragma once

#include "CppSharp.h"
#include <motion/EyeBlinkMotion.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class ALive2DModel;
    ref class EyeBlinkMotion;
    ref class LDString;
}

namespace Live2DSharp
{
    public ref class EyeBlinkMotion : Live2DSharp::LDObject
    {
    public:

        enum struct EYE_STATE
        {
            STATE_FIRST = 0,
            STATE_INTERVAL = 1,
            STATE_CLOSING = 2,
            STATE_CLOSED = 3,
            STATE_OPENING = 4
        };

        EyeBlinkMotion(::live2d::EyeBlinkMotion* native);
        static EyeBlinkMotion^ __CreateInstance(::System::IntPtr native);
        EyeBlinkMotion();

        property int Interval
        {
            void set(int);
        }

        property Live2DSharp::ALive2DModel^ Param
        {
            void set(Live2DSharp::ALive2DModel^);
        }

        property long long calcNextBlink
        {
            long long get();
        }

        void setEyeMotion(int closingMotionMsec, int closedMotionMsec, int openingMotionMsec);
    };
}
