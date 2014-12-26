#pragma once

#include "CppSharp.h"
#include <memory/debug/MemoryInfoSet.h>

namespace Live2DSharp
{
    ref class AMemoryHolder;
    ref class MemoryInfo;
    ref class MemoryInfoSet;
    ref class MemoryParam;
}

namespace Live2DSharp
{
    public ref class MemoryInfoSet : ICppInstance
    {
    public:

        property ::live2d::MemoryInfoSet* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        MemoryInfoSet(::live2d::MemoryInfoSet* native);
        static MemoryInfoSet^ __CreateInstance(::System::IntPtr native);
        MemoryInfoSet(System::String^ setName);

        property int MallocTotal
        {
            int get();
        }

        property int RestCount
        {
            int get();
        }

        property int TotalMemory
        {
            int get();
        }

        property int CurMemory
        {
            int get();
        }

        property int PeakMemory
        {
            int get();
        }

        void dumpList(Live2DSharp::MemoryInfo^ info);

        void addMemoryInfo(::System::IntPtr ptr, Live2DSharp::MemoryParam^ owner, unsigned int size, System::String^ filename, int lineno);

        void removeMemoryInfo(::System::IntPtr ptr);
    };
}
