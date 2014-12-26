#pragma once

#include "CppSharp.h"
#include <memory/debug/MemoryInfo.h>

namespace Live2DSharp
{
    ref class AMemoryHolder;
    ref class MemoryInfo;
}

namespace Live2DSharp
{
    public ref class MemoryInfo : ICppInstance
    {
    public:

        property ::live2d::MemoryInfo* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        MemoryInfo(::live2d::MemoryInfo* native);
        static MemoryInfo^ __CreateInstance(::System::IntPtr native);
        MemoryInfo();

        void dump(System::String^ message, unsigned int cur, unsigned int peak);
    };
}
