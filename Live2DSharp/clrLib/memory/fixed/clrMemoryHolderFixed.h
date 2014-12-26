#pragma once

#include "CppSharp.h"
#include <memory/fixed/MemoryHolderFixed.h>
#include "memory/clrAMemoryHolder.h"
#include "memory/clrLDAllocator.h"

namespace Live2DSharp
{
    ref class APageHeader;
    ref class MHPageHeaderFixed;
    ref class MemoryHolderFixed;
}

namespace Live2DSharp
{
    public ref class MemoryHolderFixed : Live2DSharp::AMemoryHolder
    {
    public:

        MemoryHolderFixed(::live2d::MemoryHolderFixed* native);
        static MemoryHolderFixed^ __CreateInstance(::System::IntPtr native);
        MemoryHolderFixed(Live2DSharp::LDAllocator::Type allocType, System::String^ holderName, unsigned int pageSize);

        static property unsigned int DefaultPageSize
        {
            void set(unsigned int);
        }

        virtual ::System::IntPtr malloc_exe(unsigned int size, int align) override;

        virtual void free_exe(Live2DSharp::APageHeader^ header, ::System::IntPtr ptr) override;

        virtual void clear() override;

        virtual void healthCheck() override;

        void dumpPages(Live2DSharp::MHPageHeaderFixed^ c);

        void checkPages(Live2DSharp::MHPageHeaderFixed^ c);
    };
}
