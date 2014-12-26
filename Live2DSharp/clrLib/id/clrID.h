#pragma once

#include "CppSharp.h"
#include <id/ID.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class AMemoryHolder;
    ref class ID;
    ref class MemoryParam;
}

namespace Live2DSharp
{
    public ref class ID : Live2DSharp::LDObject
    {
    public:

        ID(::live2d::ID* native);
        static ID^ __CreateInstance(::System::IntPtr native);
        ID();

        static void staticInit_notForClientCall();

        static void staticRelease_notForClientCall();
    };
}
