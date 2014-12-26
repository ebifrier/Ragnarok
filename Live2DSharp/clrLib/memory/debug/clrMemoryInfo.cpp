#include "memory/debug/clrMemoryInfo.h"
#include "memory/clrAMemoryHolder.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::MemoryInfo::MemoryInfo(::live2d::MemoryInfo* native)
{
    NativePtr = native;
}

Live2DSharp::MemoryInfo^ Live2DSharp::MemoryInfo::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::MemoryInfo((::live2d::MemoryInfo*) native.ToPointer());
}

Live2DSharp::MemoryInfo::MemoryInfo()
{
    NativePtr = new ::live2d::MemoryInfo();
}

void Live2DSharp::MemoryInfo::dump(System::String^ message, unsigned int cur, unsigned int peak)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(message);
    auto arg0 = _arg0.c_str();
    auto arg1 = (::l2d_size_t)(::size_t)cur;
    auto arg2 = (::l2d_size_t)(::size_t)peak;
    ((::live2d::MemoryInfo*)NativePtr)->dump(arg0, arg1, arg2);
}

System::IntPtr Live2DSharp::MemoryInfo::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::MemoryInfo::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::MemoryInfo*)object.ToPointer();
}
