#include "memory/clrLDAllocator.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::LDAllocator::LDAllocator(::live2d::LDAllocator* native)
{
    NativePtr = native;
}

Live2DSharp::LDAllocator^ Live2DSharp::LDAllocator::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::LDAllocator((::live2d::LDAllocator*) native.ToPointer());
}

Live2DSharp::LDAllocator::LDAllocator()
{
}

::System::IntPtr Live2DSharp::LDAllocator::pageAlloc(unsigned int size, Live2DSharp::LDAllocator::Type allocType)
{
    auto arg1 = (::live2d::LDAllocator::Type)allocType;
    auto __ret = ((::live2d::LDAllocator*)NativePtr)->pageAlloc(size, arg1);
    if (__ret == nullptr) return System::IntPtr();
    return ::System::IntPtr(__ret);
}

void Live2DSharp::LDAllocator::pageFree(::System::IntPtr ptr, Live2DSharp::LDAllocator::Type allocType)
{
    auto arg0 = (void*)ptr;
    auto arg1 = (::live2d::LDAllocator::Type)allocType;
    ((::live2d::LDAllocator*)NativePtr)->pageFree(arg0, arg1);
}

void Live2DSharp::LDAllocator::init()
{
    ((::live2d::LDAllocator*)NativePtr)->init();
}

void Live2DSharp::LDAllocator::dispose()
{
    ((::live2d::LDAllocator*)NativePtr)->dispose();
}

System::IntPtr Live2DSharp::LDAllocator::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::LDAllocator::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::LDAllocator*)object.ToPointer();
}
