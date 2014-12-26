#pragma once

#include "CppSharp.h"
#include <io/FileFormat2.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class ClassDef;
    ref class FileFormat2;
    ref class MemoryParam;
}

namespace Live2DSharp
{
    public ref class FileFormat2 : Live2DSharp::LDObject
    {
    public:

        FileFormat2(::live2d::FileFormat2* native);
        static FileFormat2^ __CreateInstance(::System::IntPtr native);
        FileFormat2();

        static ::System::IntPtr newInstance(Live2DSharp::MemoryParam^ memParam, int classNo);

        static bool isPrimitive(int classNo);

        static bool isPrimitiveDouble(int classNo);

        static bool isPrimitiveFloat(int classNo);

        static bool isPrimitiveInt(int classNo);

        static property int LIVE2D_FORMAT_VERSION_V2_6_INTIAL
        {
            int get();
        }

        static property int LIVE2D_FORMAT_VERSION_V2_7_OPACITY
        {
            int get();
        }

        static property int LIVE2D_FORMAT_VERSION_V2_8_TEX_OPTION
        {
            int get();
        }

        static property int LIVE2D_FORMAT_VERSION_V2_9_AVATAR_PARTS
        {
            int get();
        }

        static property int LIVE2D_FORMAT_VERSION_V2_10_SDK2
        {
            int get();
        }

        static property int LIVE2D_FORMAT_VERSION_AVAILABLE
        {
            int get();
        }

        static property int LIVE2D_FORMAT_EOF_VALUE
        {
            int get();
        }

        static property int NULL_NO
        {
            int get();
        }

        static property int ARRAY_NO
        {
            int get();
        }

        static property int OBJECT_REF
        {
            int get();
        }
    };
}
