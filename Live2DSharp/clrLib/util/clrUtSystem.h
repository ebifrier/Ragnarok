#pragma once

#include "CppSharp.h"
#include <util/UtSystem.h>

namespace Live2DSharp
{
    ref class UtSystem;
}

namespace Live2DSharp
{
    public ref class UtSystem : ICppInstance
    {
    public:

        property ::live2d::UtSystem* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        UtSystem(::live2d::UtSystem* native);
        static UtSystem^ __CreateInstance(::System::IntPtr native);
        static property long long TimeMSec
        {
            long long get();
        }

        static property long long UserTimeMSec
        {
            long long get();
            void set(long long);
        }

        static property bool isBigEndian
        {
            bool get();
        }

        static property long long updateUserTimeMSec
        {
            long long get();
        }

        static void exit(int code);
    };
}
