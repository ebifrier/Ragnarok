#pragma once

#include "CppSharp.h"
#include <DEF.h>

namespace Live2DSharp
{
    ref class DEF;
}

namespace Live2DSharp
{
    public ref class DEF : ICppInstance
    {
    public:

        property ::live2d::DEF* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        DEF(::live2d::DEF* native);
        static DEF^ __CreateInstance(::System::IntPtr native);
        static property int VERTEX_TYPE_OFFSET0_STEP2
        {
            int get();
        }

        static property int VERTEX_TYPE_OFFSET2_STEP5
        {
            int get();
        }

        static property int VERTEX_TYPE_OFFSET0_STEP5
        {
            int get();
        }

        static property int VERTEX_OFFSET
        {
            int get();
        }

        static property int VERTEX_STEP
        {
            int get();
        }

        static property int VERTEX_TYPE
        {
            int get();
        }

        static property int MAX_INTERPOLATION
        {
            int get();
        }

        static property int PIVOT_TABLE_SIZE
        {
            int get();
        }

        static property float GOSA
        {
            float get();
        }
    };
}
