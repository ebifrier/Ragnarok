#pragma once

#include "CppSharp.h"
#include <util/UtMotion.h>

namespace Live2DSharp
{
    ref class UtMotion;
}

namespace Live2DSharp
{
    public ref class UtMotion : ICppInstance
    {
    public:

        property ::live2d::UtMotion* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        UtMotion(::live2d::UtMotion* native);
        static UtMotion^ __CreateInstance(::System::IntPtr native);
        static float getEasingSine(float value);
    };
}
