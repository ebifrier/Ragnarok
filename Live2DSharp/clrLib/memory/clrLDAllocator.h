#pragma once

#include "CppSharp.h"
#include <memory/LDAllocator.h>

namespace Live2DSharp
{
    ref class LDAllocator;
}

namespace Live2DSharp
{
    public ref class LDAllocator : ICppInstance
    {
    public:

        enum struct Type
        {
            MAIN = 0,
            GPU = 1
        };

        property ::live2d::LDAllocator* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        LDAllocator(::live2d::LDAllocator* native);
        static LDAllocator^ __CreateInstance(::System::IntPtr native);
        LDAllocator();

        virtual ::System::IntPtr pageAlloc(unsigned int size, Live2DSharp::LDAllocator::Type allocType);

        virtual void pageFree(::System::IntPtr ptr, Live2DSharp::LDAllocator::Type allocType);

        virtual void init();

        virtual void dispose();
    };
}
