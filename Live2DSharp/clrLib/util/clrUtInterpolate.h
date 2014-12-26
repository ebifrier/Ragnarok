#pragma once

#include "CppSharp.h"
#include <util/UtInterpolate.h>

namespace Live2DSharp
{
    ref class ModelContext;
    ref class PivotManager;
    ref class UtInterpolate;
}

namespace Live2DSharp
{
    public ref class UtInterpolate : ICppInstance
    {
    public:

        property ::live2d::UtInterpolate* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        UtInterpolate(::live2d::UtInterpolate* native);
        static UtInterpolate^ __CreateInstance(::System::IntPtr native);
    };
}
