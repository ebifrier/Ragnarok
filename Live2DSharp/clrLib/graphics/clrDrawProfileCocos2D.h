#pragma once

#include "CppSharp.h"
#include <graphics/DrawProfileCocos2D.h>

namespace Live2DSharp
{
    ref class DrawProfileCocos2D;
}

namespace Live2DSharp
{
    public ref class DrawProfileCocos2D : ICppInstance
    {
    public:

        property ::live2d::DrawProfileCocos2D* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        DrawProfileCocos2D(::live2d::DrawProfileCocos2D* native);
        static DrawProfileCocos2D^ __CreateInstance(::System::IntPtr native);
        DrawProfileCocos2D();

        static void preDraw();

        static void postDraw();
    };
}
