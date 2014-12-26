#pragma once

#include "CppSharp.h"
#include <util/UtString.h>

namespace Live2DSharp
{
    ref class LDString;
    ref class UtString;
}

namespace Live2DSharp
{
    public ref class UtString : ICppInstance
    {
    public:

        property ::live2d::UtString* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        UtString(::live2d::UtString* native);
        static UtString^ __CreateInstance(::System::IntPtr native);
        static Live2DSharp::LDString^ toString(System::String^ msg);

        static bool startsWith(System::String^ text, System::String^ startWord);

        static float strToFloat(System::String^ str, int len, int pos, int* ret_endpos);
    };
}
