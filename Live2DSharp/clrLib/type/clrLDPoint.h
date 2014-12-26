#pragma once

#include "CppSharp.h"
#include <type/LDPoint.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class LDPoint;
}

namespace Live2DSharp
{
    public ref class LDPoint : Live2DSharp::LDObject
    {
    public:

        LDPoint(::live2d::LDPoint* native);
        static LDPoint^ __CreateInstance(::System::IntPtr native);
        LDPoint();

        LDPoint(int x, int y);

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
    };
}
