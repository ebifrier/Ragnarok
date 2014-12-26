#pragma once

#include "CppSharp.h"
#include <type/LDRectF.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class LDRectF;
}

namespace Live2DSharp
{
    public ref class LDRectF : Live2DSharp::LDObject
    {
    public:

        LDRectF(::live2d::LDRectF* native);
        static LDRectF^ __CreateInstance(::System::IntPtr native);
        LDRectF();

        LDRectF(float x, float y, float w, float h);

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

        property float width
        {
            float get();
            void set(float);
        }

        property float height
        {
            float get();
            void set(float);
        }

        property float CenterX
        {
            float get();
        }

        property float CenterY
        {
            float get();
        }

        property float Right
        {
            float get();
        }

        property float geBottom
        {
            float get();
        }
    };
}
