#include "util/clrUtInterpolate.h"
#include "./clrModelContext.h"
#include "param/clrPivotManager.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::UtInterpolate::UtInterpolate(::live2d::UtInterpolate* native)
{
    NativePtr = native;
}

Live2DSharp::UtInterpolate^ Live2DSharp::UtInterpolate::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::UtInterpolate((::live2d::UtInterpolate*) native.ToPointer());
}

System::IntPtr Live2DSharp::UtInterpolate::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::UtInterpolate::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::UtInterpolate*)object.ToPointer();
}
