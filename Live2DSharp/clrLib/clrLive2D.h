#pragma once

#include "CppSharp.h"
#include <Live2D.h>

namespace Live2DSharp
{
    ref class LDAllocator;
    ref class Live2D;
}

namespace Live2DSharp
{
    public ref class Live2D : ICppInstance
    {
    public:

        property ::live2d::Live2D* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        Live2D(::live2d::Live2D* native);
        static Live2D^ __CreateInstance(::System::IntPtr native);
        static property System::String^ VersionStr
        {
            System::String^ get();
        }

        static property unsigned int VersionNo
        {
            unsigned int get();
        }

        static property bool BuildOption_RANGE_CHECK_POINT
        {
            bool get();
        }

        static property bool BuildOption_AVATAR_OPTION_A
        {
            bool get();
        }

        static property bool VertexDoubleBufferEnabled
        {
            void set(bool);
        }

        static property unsigned int Error
        {
            unsigned int get();
            void set(unsigned int);
        }

        static property bool isVertexDoubleBufferEnabled
        {
            bool get();
        }

        static void init(Live2DSharp::LDAllocator^ allocator);

        static void dispose();

        static property int L2D_NO_ERROR
        {
            int get();
        }

        static property int L2D_ERROR_LIVE2D_INIT_FAILED
        {
            int get();
        }

        static property int L2D_ERROR_FILE_LOAD_FAILED
        {
            int get();
        }

        static property int L2D_ERROR_MEMORY_ERROR
        {
            int get();
        }

        static property int L2D_ERROR_MODEL_DATA_VERSION_MISMATCH
        {
            int get();
        }

        static property int L2D_ERROR_MODEL_DATA_EOF_ERROR
        {
            int get();
        }

        static property int L2D_COLOR_BLEND_MODE_MULT
        {
            int get();
        }

        static property int L2D_COLOR_BLEND_MODE_ADD
        {
            int get();
        }

        static property int L2D_COLOR_BLEND_MODE_INTERPOLATE
        {
            int get();
        }
    };
}
