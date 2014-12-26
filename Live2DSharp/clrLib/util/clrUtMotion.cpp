#include "util/clrUtMotion.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::UtMotion::UtMotion(::live2d::UtMotion* native)
{
    NativePtr = native;
}

Live2DSharp::UtMotion^ Live2DSharp::UtMotion::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::UtMotion((::live2d::UtMotion*) native.ToPointer());
}

float Live2DSharp::UtMotion::getEasingSine(float value)
{
    auto __ret = ::live2d::UtMotion::getEasingSine(value);
    return __ret;
}

System::IntPtr Live2DSharp::UtMotion::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::UtMotion::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::UtMotion*)object.ToPointer();
}
