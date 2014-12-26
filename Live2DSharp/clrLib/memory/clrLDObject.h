#pragma once

#include "CppSharp.h"
#include <memory/LDObject.h>

namespace Live2DSharp
{
    ref class AMemoryHolder;
    ref class LDObject;
    ref class MemoryParam;
}

namespace Live2DSharp
{
    public ref class LDObject : ICppInstance
    {
    public:

        property ::live2d::LDObject* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        LDObject(::live2d::LDObject* native);
        static LDObject^ __CreateInstance(::System::IntPtr native);
        LDObject();
    };
}
