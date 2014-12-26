#include "type/clrLDPoint.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::LDPoint::LDPoint(::live2d::LDPoint* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::LDPoint^ Live2DSharp::LDPoint::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::LDPoint((::live2d::LDPoint*) native.ToPointer());
}

Live2DSharp::LDPoint::LDPoint()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::LDPoint();
}

Live2DSharp::LDPoint::LDPoint(int x, int y)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::LDPoint(x, y);
}

int Live2DSharp::LDPoint::x::get()
{
    return ((::live2d::LDPoint*)NativePtr)->x;
}

void Live2DSharp::LDPoint::x::set(int value)
{
    ((::live2d::LDPoint*)NativePtr)->x = value;
}

int Live2DSharp::LDPoint::y::get()
{
    return ((::live2d::LDPoint*)NativePtr)->y;
}

void Live2DSharp::LDPoint::y::set(int value)
{
    ((::live2d::LDPoint*)NativePtr)->y = value;
}

