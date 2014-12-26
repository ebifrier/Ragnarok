#pragma once

#include "CppSharp.h"
#include <util/UtDebug.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class DebugTimerObj;
    ref class UtDebug;
}

namespace Live2DSharp
{
    public ref class DebugTimerObj : Live2DSharp::LDObject
    {
    public:

        DebugTimerObj(::live2d::DebugTimerObj* native);
        static DebugTimerObj^ __CreateInstance(::System::IntPtr native);
        DebugTimerObj();

        property System::String^ key
        {
            System::String^ get();
            void set(System::String^);
        }

        property long long startTimeMs
        {
            long long get();
            void set(long long);
        }
    };

    public ref class UtDebug : ICppInstance
    {
    public:

        property ::live2d::UtDebug* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        UtDebug(::live2d::UtDebug* native);
        static UtDebug^ __CreateInstance(::System::IntPtr native);
        static void assertF(System::String^ file, int lineno, System::String^ format);

        static void error(System::String^ msg);

        static void dumpByte(System::String^ data, int length);

        static void print(System::String^ format);

        static void println(System::String^ format);

        static void debugBreak();

        static property unsigned int MEMORY_DEBUG_DUMP_ALLOCATOR
        {
            unsigned int get();
        }

        static property unsigned int MEMORY_DEBUG_DUMP_TMP
        {
            unsigned int get();
        }

        static property unsigned int MEMORY_DEBUG_DUMP_FIXED
        {
            unsigned int get();
        }

        static property unsigned int MEMORY_DEBUG_DUMP_UNMANAGED
        {
            unsigned int get();
        }

        static property unsigned int MEMORY_DEBUG_MEMORY_INFO_COUNT
        {
            unsigned int get();
        }

        static property unsigned int MEMORY_DEBUG_MEMORY_INFO_DUMP
        {
            unsigned int get();
        }

        static property unsigned int MEMORY_DEBUG_MEMORY_INFO_ALL
        {
            unsigned int get();
        }

        static property unsigned int MEMORY_DEBUG_MEMORY_INFO_KEEP_FREE
        {
            unsigned int get();
        }

        static property unsigned int MEMORY_DEBUG_MEMORY_DUMP_PLACEMENT_NEW
        {
            unsigned int get();
        }

        static property unsigned int MEMORY_DEBUG_DUMP_ALL
        {
            unsigned int get();
        }

        static property unsigned int READ_OBJECT_DEBUG_DUMP
        {
            unsigned int get();
        }
    };
}
