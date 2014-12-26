#pragma once

#include "CppSharp.h"
#include <type/LDPointF.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class LDPointF;
}

namespace Live2DSharp
{
    public ref class LDPointF : Live2DSharp::LDObject
    {
    public:

        LDPointF(::live2d::LDPointF* native);
        static LDPointF^ __CreateInstance(::System::IntPtr native);
        LDPointF();

        LDPointF(float x, float y);

        property float x
        {
            float get();
            void set(float);
        }

        property float y
        {
            float get();
            void set(float);
        }
    };
}
