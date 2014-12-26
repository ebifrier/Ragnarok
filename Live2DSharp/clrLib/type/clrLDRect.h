#pragma once

#include "CppSharp.h"
#include <type/LDRect.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class LDRect;
}

namespace Live2DSharp
{
    public ref class LDRect : Live2DSharp::LDObject
    {
    public:

        LDRect(::live2d::LDRect* native);
        static LDRect^ __CreateInstance(::System::IntPtr native);
        LDRect();

        LDRect(int x, int y, int w, int h);

        property int x
        {
            int get();
            void set(int);
        }

        property int y
        {
            int get();
            void set(int);
        }

        property int width
        {
            int get();
            void set(int);
        }

        property int height
        {
            int get();
            void set(int);
        }

        property int CenterX
        {
            int get();
        }

        property int CenterY
        {
            int get();
        }

        property int Right
        {
            int get();
        }

        property int geBottom
        {
            int get();
        }
    };
}
