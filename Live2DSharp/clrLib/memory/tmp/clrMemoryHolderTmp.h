#pragma once

#include "CppSharp.h"
#include <memory/tmp/MemoryHolderTmp.h>
#include "memory/clrAMemoryHolder.h"
#include "memory/clrLDAllocator.h"
#include "memory/clrLDUnmanagedObject.h"

namespace Live2DSharp
{
    ref class APageHeader;
    ref class MHBin;
    ref class MHPageHeaderTmp;
    ref class MemoryHolderSocket;
    ref class MemoryHolderTmp;
}

namespace Live2DSharp
{
    public ref class MemoryHolderTmp : Live2DSharp::LDUnmanagedObject
    {
    public:

        MemoryHolderTmp(::live2d::MemoryHolderTmp* native);
        static MemoryHolderTmp^ __CreateInstance(::System::IntPtr native);
        MemoryHolderTmp(Live2DSharp::LDAllocator::Type allocType, System::String^ holderName);

        virtual ::System::IntPtr malloc_exe(unsigned int size, int align);

        virtual void free_exe(Live2DSharp::APageHeader^ header, ::System::IntPtr ptr);

        virtual void clear();

        static property unsigned int LARGE_0
        {
            unsigned int get();
        }

        static property unsigned int PAGE_ALIGN
        {
            unsigned int get();
        }

        static property int BIN_COUNT
        {
            int get();
        }

        static property cli::array<unsigned int>^ CHUNK_SIZE
        {
            cli::array<unsigned int>^ get();
        }
    };

    public ref class MemoryHolderSocket : Live2DSharp::AMemoryHolder
    {
    public:

        MemoryHolderSocket(::live2d::MemoryHolderSocket* native);
        static MemoryHolderSocket^ __CreateInstance(::System::IntPtr native);
        MemoryHolderSocket(Live2DSharp::MemoryHolderTmp^ impl);

        virtual ::System::IntPtr malloc_exe(unsigned int size, int align) override;

        virtual void free_exe(Live2DSharp::APageHeader^ header, ::System::IntPtr ptr) override;

        virtual void clear() override;
    };
}
