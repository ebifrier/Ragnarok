#include "memory/debug/clrMemoryInfoSet.h"
#include "memory/clrAMemoryHolder.h"
#include "memory/clrMemoryParam.h"
#include "memory/debug/clrMemoryInfo.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::MemoryInfoSet::MemoryInfoSet(::live2d::MemoryInfoSet* native)
{
    NativePtr = native;
}

Live2DSharp::MemoryInfoSet^ Live2DSharp::MemoryInfoSet::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::MemoryInfoSet((::live2d::MemoryInfoSet*) native.ToPointer());
}

Live2DSharp::MemoryInfoSet::MemoryInfoSet(System::String^ setName)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(setName);
    auto arg0 = _arg0.c_str();
    NativePtr = new ::live2d::MemoryInfoSet(arg0);
}

void Live2DSharp::MemoryInfoSet::dumpList(Live2DSharp::MemoryInfo^ info)
{
    auto arg0 = (::live2d::MemoryInfo*)info->NativePtr;
    ((::live2d::MemoryInfoSet*)NativePtr)->dumpList(arg0);
}

void Live2DSharp::MemoryInfoSet::addMemoryInfo(::System::IntPtr ptr, Live2DSharp::MemoryParam^ owner, unsigned int size, System::String^ filename, int lineno)
{
    auto arg0 = (void*)ptr;
    auto arg1 = (::live2d::MemoryParam*)owner->NativePtr;
    auto arg2 = (::l2d_size_t)(::size_t)size;
    auto _arg3 = clix::marshalString<clix::E_UTF8>(filename);
    auto arg3 = _arg3.c_str();
    ((::live2d::MemoryInfoSet*)NativePtr)->addMemoryInfo(arg0, arg1, arg2, arg3, lineno);
}

void Live2DSharp::MemoryInfoSet::removeMemoryInfo(::System::IntPtr ptr)
{
    auto arg0 = (void*)ptr;
    ((::live2d::MemoryInfoSet*)NativePtr)->removeMemoryInfo(arg0);
}

System::IntPtr Live2DSharp::MemoryInfoSet::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::MemoryInfoSet::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::MemoryInfoSet*)object.ToPointer();
}

int Live2DSharp::MemoryInfoSet::MallocTotal::get()
{
    auto __ret = ((::live2d::MemoryInfoSet*)NativePtr)->getMallocTotal();
    return __ret;
}

int Live2DSharp::MemoryInfoSet::RestCount::get()
{
    auto __ret = ((::live2d::MemoryInfoSet*)NativePtr)->getRestCount();
    return __ret;
}

int Live2DSharp::MemoryInfoSet::TotalMemory::get()
{
    auto __ret = ((::live2d::MemoryInfoSet*)NativePtr)->getTotalMemory();
    return __ret;
}

int Live2DSharp::MemoryInfoSet::CurMemory::get()
{
    auto __ret = ((::live2d::MemoryInfoSet*)NativePtr)->getCurMemory();
    return __ret;
}

int Live2DSharp::MemoryInfoSet::PeakMemory::get()
{
    auto __ret = ((::live2d::MemoryInfoSet*)NativePtr)->getPeakMemory();
    return __ret;
}

