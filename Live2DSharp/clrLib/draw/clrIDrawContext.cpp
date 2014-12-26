#include "draw/clrIDrawContext.h"
#include "draw/clrIDrawData.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::IDrawContext::IDrawContext(::live2d::IDrawContext* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::IDrawContext^ Live2DSharp::IDrawContext::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::IDrawContext((::live2d::IDrawContext*) native.ToPointer());
}

Live2DSharp::IDrawContext::IDrawContext(Live2DSharp::IDrawData^ src)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    auto arg0 = (::live2d::IDrawData*)src->NativePtr;
    NativePtr = new ::live2d::IDrawContext(arg0);
}

int Live2DSharp::IDrawContext::interpolatedDrawOrder::get()
{
    return ((::live2d::IDrawContext*)NativePtr)->interpolatedDrawOrder;
}

void Live2DSharp::IDrawContext::interpolatedDrawOrder::set(int value)
{
    ((::live2d::IDrawContext*)NativePtr)->interpolatedDrawOrder = value;
}

float Live2DSharp::IDrawContext::interpolatedOpacity::get()
{
    return ((::live2d::IDrawContext*)NativePtr)->interpolatedOpacity;
}

void Live2DSharp::IDrawContext::interpolatedOpacity::set(float value)
{
    ((::live2d::IDrawContext*)NativePtr)->interpolatedOpacity = value;
}

bool Live2DSharp::IDrawContext::paramOutside::get()
{
    return ((::live2d::IDrawContext*)NativePtr)->paramOutside;
}

void Live2DSharp::IDrawContext::paramOutside::set(bool value)
{
    ((::live2d::IDrawContext*)NativePtr)->paramOutside = value;
}

float Live2DSharp::IDrawContext::partsOpacity::get()
{
    return ((::live2d::IDrawContext*)NativePtr)->partsOpacity;
}

void Live2DSharp::IDrawContext::partsOpacity::set(float value)
{
    ((::live2d::IDrawContext*)NativePtr)->partsOpacity = value;
}

bool Live2DSharp::IDrawContext::available::get()
{
    return ((::live2d::IDrawContext*)NativePtr)->available;
}

void Live2DSharp::IDrawContext::available::set(bool value)
{
    ((::live2d::IDrawContext*)NativePtr)->available = value;
}

float Live2DSharp::IDrawContext::baseOpacity::get()
{
    return ((::live2d::IDrawContext*)NativePtr)->baseOpacity;
}

void Live2DSharp::IDrawContext::baseOpacity::set(float value)
{
    ((::live2d::IDrawContext*)NativePtr)->baseOpacity = value;
}

::System::IntPtr Live2DSharp::IDrawContext::SrcPtr::get()
{
    auto __ret = ((::live2d::IDrawContext*)NativePtr)->getSrcPtr();
    if (__ret == nullptr) return System::IntPtr();
    return ::System::IntPtr(__ret);
}

int Live2DSharp::IDrawContext::PartsIndex::get()
{
    auto __ret = ((::live2d::IDrawContext*)NativePtr)->getPartsIndex();
    return __ret;
}

void Live2DSharp::IDrawContext::PartsIndex::set(int p)
{
    ((::live2d::IDrawContext*)NativePtr)->setPartsIndex(p);
}

bool Live2DSharp::IDrawContext::isAvailable::get()
{
    auto __ret = ((::live2d::IDrawContext*)NativePtr)->isAvailable();
    return __ret;
}

