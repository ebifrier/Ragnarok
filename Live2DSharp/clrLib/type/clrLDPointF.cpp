#include "type/clrLDPointF.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::LDPointF::LDPointF(::live2d::LDPointF* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::LDPointF^ Live2DSharp::LDPointF::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::LDPointF((::live2d::LDPointF*) native.ToPointer());
}

Live2DSharp::LDPointF::LDPointF()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::LDPointF();
}

Live2DSharp::LDPointF::LDPointF(float x, float y)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    auto arg0 = (::l2d_pointf)x;
    auto arg1 = (::l2d_pointf)y;
    NativePtr = new ::live2d::LDPointF(arg0, arg1);
}

float Live2DSharp::LDPointF::x::get()
{
    return ((::live2d::LDPointF*)NativePtr)->x;
}

void Live2DSharp::LDPointF::x::set(float value)
{
    ((::live2d::LDPointF*)NativePtr)->x = (::l2d_pointf)value;
}

float Live2DSharp::LDPointF::y::get()
{
    return ((::live2d::LDPointF*)NativePtr)->y;
}

void Live2DSharp::LDPointF::y::set(float value)
{
    ((::live2d::LDPointF*)NativePtr)->y = (::l2d_pointf)value;
}

