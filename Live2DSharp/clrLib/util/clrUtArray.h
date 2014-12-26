#pragma once

#include "CppSharp.h"
#include <util/UtArray.h>

namespace Live2DSharp
{
    ref class MemoryParam;
    ref class UtArray;
}

namespace Live2DSharp
{
    public ref class UtArray : ICppInstance
    {
    public:

        property ::live2d::UtArray* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        UtArray(::live2d::UtArray* native);
        static UtArray^ __CreateInstance(::System::IntPtr native);
        static float* floatArray(Live2DSharp::MemoryParam^ memParam, int num);

        static void dumpPoints(float* array, int w, int h);
    };
}
