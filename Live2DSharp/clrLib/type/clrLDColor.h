#pragma once

#include "CppSharp.h"
#include <type/LDColor.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class LDColor;
}

namespace Live2DSharp
{
    public ref class LDColor : Live2DSharp::LDObject
    {
    public:

        LDColor(::live2d::LDColor* native);
        static LDColor^ __CreateInstance(::System::IntPtr native);
        LDColor();

        LDColor(int color, bool useAlpha);
    };
}
