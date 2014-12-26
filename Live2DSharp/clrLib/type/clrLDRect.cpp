#include "type/clrLDRect.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::LDRect::LDRect(::live2d::LDRect* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::LDRect^ Live2DSharp::LDRect::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::LDRect((::live2d::LDRect*) native.ToPointer());
}

Live2DSharp::LDRect::LDRect()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::LDRect();
}

Live2DSharp::LDRect::LDRect(int x, int y, int w, int h)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::LDRect(x, y, w, h);
}

int Live2DSharp::LDRect::x::get()
{
    return ((::live2d::LDRect*)NativePtr)->x;
}

void Live2DSharp::LDRect::x::set(int value)
{
    ((::live2d::LDRect*)NativePtr)->x = value;
}

int Live2DSharp::LDRect::y::get()
{
    return ((::live2d::LDRect*)NativePtr)->y;
}

void Live2DSharp::LDRect::y::set(int value)
{
    ((::live2d::LDRect*)NativePtr)->y = value;
}

int Live2DSharp::LDRect::width::get()
{
    return ((::live2d::LDRect*)NativePtr)->width;
}

void Live2DSharp::LDRect::width::set(int value)
{
    ((::live2d::LDRect*)NativePtr)->width = value;
}

int Live2DSharp::LDRect::height::get()
{
    return ((::live2d::LDRect*)NativePtr)->height;
}

void Live2DSharp::LDRect::height::set(int value)
{
    ((::live2d::LDRect*)NativePtr)->height = value;
}

int Live2DSharp::LDRect::CenterX::get()
{
    auto __ret = ((::live2d::LDRect*)NativePtr)->getCenterX();
    return __ret;
}

int Live2DSharp::LDRect::CenterY::get()
{
    auto __ret = ((::live2d::LDRect*)NativePtr)->getCenterY();
    return __ret;
}

int Live2DSharp::LDRect::Right::get()
{
    auto __ret = ((::live2d::LDRect*)NativePtr)->getRight();
    return __ret;
}

int Live2DSharp::LDRect::geBottom::get()
{
    auto __ret = ((::live2d::LDRect*)NativePtr)->geBottom();
    return __ret;
}

