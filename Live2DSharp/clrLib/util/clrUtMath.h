#pragma once

#include "CppSharp.h"
#include <util/UtMath.h>

namespace Live2DSharp
{
    ref class UtMath;
}

namespace Live2DSharp
{
    public ref class UtMath : ICppInstance
    {
    public:

        property ::live2d::UtMath* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        UtMath(::live2d::UtMath* native);
        static UtMath^ __CreateInstance(::System::IntPtr native);
        static float range(float v, float min, float max);

        static double getAngleNotAbs(float* v1, float* v2);

        static double getAngleDiff(double Q1, double Q2);

        static double fsin(double x);

        static double fcos(double x);

        static property double DEG_TO_RAD_D
        {
            double get();
        }

        static property float DEG_TO_RAD_F
        {
            float get();
        }

        static property double RAD_TO_DEG_D
        {
            double get();
        }

        static property float RAD_TO_DEG_F
        {
            float get();
        }
    };
}
