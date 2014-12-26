#include "base/clrIBaseContext.h"
#include "base/clrIBaseData.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::IBaseContext::IBaseContext(::live2d::IBaseContext* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::IBaseContext^ Live2DSharp::IBaseContext::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::IBaseContext((::live2d::IBaseContext*) native.ToPointer());
}

Live2DSharp::IBaseContext::IBaseContext(Live2DSharp::IBaseData^ src)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    auto arg0 = (::live2d::IBaseData*)src->NativePtr;
    NativePtr = new ::live2d::IBaseContext(arg0);
}

::System::IntPtr Live2DSharp::IBaseContext::SrcPtr::get()
{
    auto __ret = ((::live2d::IBaseContext*)NativePtr)->getSrcPtr();
    if (__ret == nullptr) return System::IntPtr();
    return ::System::IntPtr(__ret);
}

int Live2DSharp::IBaseContext::PartsIndex::get()
{
    auto __ret = ((::live2d::IBaseContext*)NativePtr)->getPartsIndex();
    return __ret;
}

void Live2DSharp::IBaseContext::PartsIndex::set(int p)
{
    ((::live2d::IBaseContext*)NativePtr)->setPartsIndex(p);
}

void Live2DSharp::IBaseContext::OutsideParam::set(bool outsideParam)
{
    ((::live2d::IBaseContext*)NativePtr)->setOutsideParam(outsideParam);
}

void Live2DSharp::IBaseContext::Available::set(bool available)
{
    ((::live2d::IBaseContext*)NativePtr)->setAvailable(available);
}

float Live2DSharp::IBaseContext::TotalScale::get()
{
    auto __ret = ((::live2d::IBaseContext*)NativePtr)->getTotalScale();
    return __ret;
}

void Live2DSharp::IBaseContext::TotalScale_notForClient::set(float totalScale)
{
    ((::live2d::IBaseContext*)NativePtr)->setTotalScale_notForClient(totalScale);
}

float Live2DSharp::IBaseContext::InterpolatedOpacity::get()
{
    auto __ret = ((::live2d::IBaseContext*)NativePtr)->getInterpolatedOpacity();
    return __ret;
}

void Live2DSharp::IBaseContext::InterpolatedOpacity::set(float interpolatedOpacity)
{
    ((::live2d::IBaseContext*)NativePtr)->setInterpolatedOpacity(interpolatedOpacity);
}

float Live2DSharp::IBaseContext::TotalOpacity::get()
{
    auto __ret = ((::live2d::IBaseContext*)NativePtr)->getTotalOpacity();
    return __ret;
}

void Live2DSharp::IBaseContext::TotalOpacity::set(float totalOpacity)
{
    ((::live2d::IBaseContext*)NativePtr)->setTotalOpacity(totalOpacity);
}

bool Live2DSharp::IBaseContext::isOutsideParam::get()
{
    auto __ret = ((::live2d::IBaseContext*)NativePtr)->isOutsideParam();
    return __ret;
}

bool Live2DSharp::IBaseContext::isAvailable::get()
{
    auto __ret = ((::live2d::IBaseContext*)NativePtr)->isAvailable();
    return __ret;
}

