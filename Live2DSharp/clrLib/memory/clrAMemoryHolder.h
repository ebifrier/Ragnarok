#pragma once

#include "CppSharp.h"
#include <memory/AMemoryHolder.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class AMemoryHolder;
    ref class APageHeader;
    ref class AllocHeader;
    ref class MemoryParam;
}

namespace Live2DSharp
{
    public ref class AllocHeader : ICppInstance
    {
    public:

        property ::live2d::AllocHeader* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        AllocHeader(::live2d::AllocHeader* native);
        static AllocHeader^ __CreateInstance(::System::IntPtr native);
        property Live2DSharp::APageHeader^ ptrToPageHeader
        {
            Live2DSharp::APageHeader^ get();
            void set(Live2DSharp::APageHeader^);
        }

        void free_exe(::System::IntPtr ptr);
    };

    public ref class AMemoryHolder : Live2DSharp::LDObject
    {
    public:

        AMemoryHolder(::live2d::AMemoryHolder* native);
        static AMemoryHolder^ __CreateInstance(::System::IntPtr native);
        AMemoryHolder();

        property Live2DSharp::MemoryParam^ MemoryParam
        {
            void set(Live2DSharp::MemoryParam^);
        }

        property int CheckValue
        {
            int get();
        }

        virtual ::System::IntPtr malloc_exe(unsigned int size, int align);

        virtual void free_exe(Live2DSharp::APageHeader^ header, ::System::IntPtr ptr);

        virtual void clear();

        virtual void healthCheck();

        static property int CHECK_VALUE
        {
            int get();
        }

        static property int MIN_CHUNK_REST
        {
            int get();
        }

        static property int MIN_ALIGN
        {
            int get();
        }
    };
}
