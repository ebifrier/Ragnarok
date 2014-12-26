#pragma once

#include "CppSharp.h"
#include <memory/LDUnmanagedObject.h>

namespace Live2DSharp
{
    ref class LDUnmanagedObject;
    ref class MemoryParam;
}

namespace Live2DSharp
{
    public ref class LDUnmanagedObject : ICppInstance
    {
    public:

        property ::live2d::LDUnmanagedObject* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        LDUnmanagedObject(::live2d::LDUnmanagedObject* native);
        static LDUnmanagedObject^ __CreateInstance(::System::IntPtr native);
        LDUnmanagedObject();
    };
}
