#include "util/clrUtArray.h"
#include "memory/clrMemoryParam.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::UtArray::UtArray(::live2d::UtArray* native)
{
    NativePtr = native;
}

Live2DSharp::UtArray^ Live2DSharp::UtArray::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::UtArray((::live2d::UtArray*) native.ToPointer());
}

float* Live2DSharp::UtArray::floatArray(Live2DSharp::MemoryParam^ memParam, int num)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    auto __ret = ::live2d::UtArray::floatArray(arg0, num);
    return __ret;
}

void Live2DSharp::UtArray::dumpPoints(float* array, int w, int h)
{
    auto arg0 = (float*)array;
    ::live2d::UtArray::dumpPoints(arg0, w, h);
}

System::IntPtr Live2DSharp::UtArray::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::UtArray::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::UtArray*)object.ToPointer();
}
