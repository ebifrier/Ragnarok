#pragma once

#include "CppSharp.h"
#include <io/ISerializableV2.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class BReader;
    ref class ISerializableV2;
    ref class MemoryParam;
}

namespace Live2DSharp
{
    public ref class ISerializableV2 : Live2DSharp::LDObject
    {
    public:

        ISerializableV2(::live2d::ISerializableV2* native);
        static ISerializableV2^ __CreateInstance(::System::IntPtr native);
        ISerializableV2();

        virtual void readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam);
    };
}
