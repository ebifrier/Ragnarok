#pragma once

#include "CppSharp.h"
#include <io/RefString.h>

namespace Live2DSharp
{
    ref class RefString;
}

namespace Live2DSharp
{
    public ref class RefString : ICppInstance
    {
    public:

        property ::live2d::RefString* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        RefString(::live2d::RefString* native);
        static RefString^ __CreateInstance(::System::IntPtr native);
        RefString();

        RefString(System::String^ ptr, int length);

        property System::String^ ptr_not_zero_end
        {
            System::String^ get();
            void set(System::String^);
        }

        property int length
        {
            int get();
            void set(int);
        }

        void set(System::String^ ptr, int length);
    };
}
