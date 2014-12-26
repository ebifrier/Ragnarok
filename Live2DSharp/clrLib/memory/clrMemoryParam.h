#pragma once

#include "CppSharp.h"
#include <memory/MemoryParam.h>
#include "memory/clrLDAllocator.h"
#include "memory/clrLDObject.h"
#include "memory/clrLDUnmanagedObject.h"

namespace Live2DSharp
{
    ref class AMemoryHolder;
    ref class MemoryPage;
    ref class MemoryParam;
    ref class MemoryStackEnt;
}

namespace Live2DSharp
{
    public ref class MemoryStackEnt : Live2DSharp::LDUnmanagedObject
    {
    public:

        MemoryStackEnt(::live2d::MemoryStackEnt* native);
        static MemoryStackEnt^ __CreateInstance(::System::IntPtr native);
        MemoryStackEnt();
    };

    public ref class MemoryParam : Live2DSharp::LDObject
    {
    public:

        MemoryParam(::live2d::MemoryParam* native);
        static MemoryParam^ __CreateInstance(::System::IntPtr native);
        MemoryParam(Live2DSharp::AMemoryHolder^ main, Live2DSharp::AMemoryHolder^ gpu);

        property Live2DSharp::LDAllocator::Type AllocType
        {
            Live2DSharp::LDAllocator::Type get();
        }

        property int AllocAlign
        {
            int get();
        }

        property Live2DSharp::AMemoryHolder^ CurMemoryHolder
        {
            Live2DSharp::AMemoryHolder^ get();
        }

        property Live2DSharp::AMemoryHolder^ MemoryHolderMain
        {
            void set(Live2DSharp::AMemoryHolder^);
        }

        property Live2DSharp::AMemoryHolder^ MemoryHolderGPU
        {
            void set(Live2DSharp::AMemoryHolder^);
        }

        Live2DSharp::LDAllocator::Type setAllocType(Live2DSharp::LDAllocator::Type allocType);

        int setAllocAlign(int align);

        ::System::IntPtr malloc_exe(unsigned int size);

        Live2DSharp::AMemoryHolder^ getMemoryHolder(Live2DSharp::LDAllocator::Type allocType);

        void clear();

        void checkMemory();
    };
}
