#pragma once

#include "CppSharp.h"
#include <memory/UtMemory.h>
#include "memory/clrLDAllocator.h"

namespace Live2DSharp
{
    ref class AMemoryHolder;
    ref class LDAllocator;
    ref class MemoryHolderSocket;
    ref class MemoryHolderTmp;
    ref class MemoryInfoSet;
    ref class MemoryParam;
    ref class UtMemory;
}

namespace Live2DSharp
{
    public ref class UtMemory : ICppInstance
    {
    public:

        property ::live2d::UtMemory* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        UtMemory(::live2d::UtMemory* native);
        static UtMemory^ __CreateInstance(::System::IntPtr native);
        static property Live2DSharp::MemoryParam^ StaticMemoryParam
        {
            Live2DSharp::MemoryParam^ get();
        }

        static property Live2DSharp::MemoryParam^ TmpMemoryParam
        {
            Live2DSharp::MemoryParam^ get();
        }

        static ::System::IntPtr allocator_malloc(unsigned int size, Live2DSharp::LDAllocator::Type allocType);

        static void allocator_free(::System::IntPtr ptr, Live2DSharp::LDAllocator::Type allocType);

        static ::System::IntPtr malloc_exe(Live2DSharp::MemoryParam^ memParam, unsigned int size);

        static ::System::IntPtr malloc_debug(Live2DSharp::MemoryParam^ memParam, unsigned int size, System::String^ filename, int lineno);

        static void free_exe(::System::IntPtr ptr);

        static void free_debug(::System::IntPtr ptr, System::String^ filename, int lineno);

        static ::System::IntPtr placementNew_debug(::System::IntPtr ptr, System::String^ filename, int lineno);

        static void setDebugInfo(System::String^ filename, int lineno);

        static System::String^ getDebugInfo(int* retLineno);

        static char* alignPtr(char* ptr, int align, int signatureSize);

        static void staticInit_notForClientCall(Live2DSharp::LDAllocator^ allocator);

        static void staticRelease_notForClientCall();
    };
}
