#pragma once

#include "CppSharp.h"
#include <memory/APageHeader.h>

namespace Live2DSharp
{
    ref class APageHeader;
}

namespace Live2DSharp
{
    public ref class APageHeader : ICppInstance
    {
    public:

        property ::live2d::APageHeader* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        APageHeader(::live2d::APageHeader* native);
        static APageHeader^ __CreateInstance(::System::IntPtr native);
        APageHeader();

        virtual void free_exe(::System::IntPtr ptr);

        static property int ENTRY_OFFSET
        {
            int get();
        }
    };
}
