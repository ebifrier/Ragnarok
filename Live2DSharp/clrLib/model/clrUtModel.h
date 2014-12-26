#pragma once

#include "CppSharp.h"
#include <model/UtModel.h>

namespace Live2DSharp
{
    ref class ALive2DModel;
    ref class UtModel;
}

namespace Live2DSharp
{
    public ref class UtModel : ICppInstance
    {
    public:

        property ::live2d::UtModel* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        UtModel(::live2d::UtModel* native);
        static UtModel^ __CreateInstance(::System::IntPtr native);
    };
}
