#include "type/clrLDRectF.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::LDRectF::LDRectF(::live2d::LDRectF* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::LDRectF^ Live2DSharp::LDRectF::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::LDRectF((::live2d::LDRectF*) native.ToPointer());
}

Live2DSharp::LDRectF::LDRectF()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::LDRectF();
}

Live2DSharp::LDRectF::LDRectF(float x, float y, float w, float h)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    auto arg0 = (::l2d_pointf)x;
    auto arg1 = (::l2d_pointf)y;
    auto arg2 = (::l2d_pointf)w;
    auto arg3 = (::l2d_pointf)h;
    NativePtr = new ::live2d::LDRectF(arg0, arg1, arg2, arg3);
}

float Live2DSharp::LDRectF::x::get()
{
    return ((::live2d::LDRectF*)NativePtr)->x;
}

void Live2DSharp::LDRectF::x::set(float value)
{
    ((::live2d::LDRectF*)NativePtr)->x = (::l2d_pointf)value;
}

float Live2DSharp::LDRectF::y::get()
{
    return ((::live2d::LDRectF*)NativePtr)->y;
}

void Live2DSharp::LDRectF::y::set(float value)
{
    ((::live2d::LDRectF*)NativePtr)->y = (::l2d_pointf)value;
}

float Live2DSharp::LDRectF::width::get()
{
    return ((::live2d::LDRectF*)NativePtr)->width;
}

void Live2DSharp::LDRectF::width::set(float value)
{
    ((::live2d::LDRectF*)NativePtr)->width = (::l2d_pointf)value;
}

float Live2DSharp::LDRectF::height::get()
{
    return ((::live2d::LDRectF*)NativePtr)->height;
}

void Live2DSharp::LDRectF::height::set(float value)
{
    ((::live2d::LDRectF*)NativePtr)->height = (::l2d_pointf)value;
}

float Live2DSharp::LDRectF::CenterX::get()
{
    auto __ret = ((::live2d::LDRectF*)NativePtr)->getCenterX();
    return __ret;
}

float Live2DSharp::LDRectF::CenterY::get()
{
    auto __ret = ((::live2d::LDRectF*)NativePtr)->getCenterY();
    return __ret;
}

float Live2DSharp::LDRectF::Right::get()
{
    auto __ret = ((::live2d::LDRectF*)NativePtr)->getRight();
    return __ret;
}

float Live2DSharp::LDRectF::geBottom::get()
{
    auto __ret = ((::live2d::LDRectF*)NativePtr)->geBottom();
    return __ret;
}

