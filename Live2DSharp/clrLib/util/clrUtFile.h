#pragma once

#include "CppSharp.h"
#include <util/UtFile.h>

namespace Live2DSharp
{
    ref class LDString;
    ref class UtFile;
}

namespace Live2DSharp
{
    public ref class UtFile : ICppInstance
    {
    public:

        property ::live2d::UtFile* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        UtFile(::live2d::UtFile* native);
        static UtFile^ __CreateInstance(::System::IntPtr native);
        static char* loadFile(Live2DSharp::LDString^ filepath, int* ret_bufsize);

        static char* loadFile(System::String^ filepath, int* ret_bufsize);

        static void releaseLoadBuffer(char* buf);
    };
}
