#include "type/clrLDColor.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::LDColor::LDColor(::live2d::LDColor* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::LDColor^ Live2DSharp::LDColor::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::LDColor((::live2d::LDColor*) native.ToPointer());
}

Live2DSharp::LDColor::LDColor()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::LDColor();
}

Live2DSharp::LDColor::LDColor(int color, bool useAlpha)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::LDColor(color, useAlpha);
}

