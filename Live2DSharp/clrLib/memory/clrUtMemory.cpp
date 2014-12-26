#include "memory/clrUtMemory.h"
#include "memory/clrAMemoryHolder.h"
#include "memory/clrLDAllocator.h"
#include "memory/clrMemoryParam.h"
#include "memory/debug/clrMemoryInfoSet.h"
#include "memory/tmp/clrMemoryHolderTmp.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::UtMemory::UtMemory(::live2d::UtMemory* native)
{
    NativePtr = native;
}

Live2DSharp::UtMemory^ Live2DSharp::UtMemory::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::UtMemory((::live2d::UtMemory*) native.ToPointer());
}

::System::IntPtr Live2DSharp::UtMemory::allocator_malloc(unsigned int size, Live2DSharp::LDAllocator::Type allocType)
{
    auto arg0 = (::l2d_size_t)(::size_t)size;
    auto arg1 = (::live2d::LDAllocator::Type)allocType;
    auto __ret = ::live2d::UtMemory::allocator_malloc(arg0, arg1);
    if (__ret == nullptr) return System::IntPtr();
    return ::System::IntPtr(__ret);
}

void Live2DSharp::UtMemory::allocator_free(::System::IntPtr ptr, Live2DSharp::LDAllocator::Type allocType)
{
    auto arg0 = (void*)ptr;
    auto arg1 = (::live2d::LDAllocator::Type)allocType;
    ::live2d::UtMemory::allocator_free(arg0, arg1);
}

::System::IntPtr Live2DSharp::UtMemory::malloc_exe(Live2DSharp::MemoryParam^ memParam, unsigned int size)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    auto arg1 = (::l2d_size_t)(::size_t)size;
    auto __ret = ::live2d::UtMemory::malloc_exe(arg0, arg1);
    if (__ret == nullptr) return System::IntPtr();
    return ::System::IntPtr(__ret);
}

::System::IntPtr Live2DSharp::UtMemory::malloc_debug(Live2DSharp::MemoryParam^ memParam, unsigned int size, System::String^ filename, int lineno)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    auto arg1 = (::l2d_size_t)(::size_t)size;
    auto _arg2 = clix::marshalString<clix::E_UTF8>(filename);
    auto arg2 = _arg2.c_str();
    auto __ret = ::live2d::UtMemory::malloc_debug(arg0, arg1, arg2, lineno);
    if (__ret == nullptr) return System::IntPtr();
    return ::System::IntPtr(__ret);
}

void Live2DSharp::UtMemory::free_exe(::System::IntPtr ptr)
{
    auto arg0 = (void*)ptr;
    ::live2d::UtMemory::free_exe(arg0);
}

void Live2DSharp::UtMemory::free_debug(::System::IntPtr ptr, System::String^ filename, int lineno)
{
    auto arg0 = (void*)ptr;
    auto _arg1 = clix::marshalString<clix::E_UTF8>(filename);
    auto arg1 = _arg1.c_str();
    ::live2d::UtMemory::free_debug(arg0, arg1, lineno);
}

::System::IntPtr Live2DSharp::UtMemory::placementNew_debug(::System::IntPtr ptr, System::String^ filename, int lineno)
{
    auto arg0 = (void*)ptr;
    auto _arg1 = clix::marshalString<clix::E_UTF8>(filename);
    auto arg1 = _arg1.c_str();
    auto __ret = ::live2d::UtMemory::placementNew_debug(arg0, arg1, lineno);
    if (__ret == nullptr) return System::IntPtr();
    return ::System::IntPtr(__ret);
}

void Live2DSharp::UtMemory::setDebugInfo(System::String^ filename, int lineno)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(filename);
    auto arg0 = _arg0.c_str();
    ::live2d::UtMemory::setDebugInfo(arg0, lineno);
}

System::String^ Live2DSharp::UtMemory::getDebugInfo(int* retLineno)
{
    auto arg0 = (int&)retLineno;
    auto __ret = ::live2d::UtMemory::getDebugInfo(arg0);
    if (__ret == nullptr) return nullptr;
    return clix::marshalString<clix::E_UTF8>(__ret);
}

char* Live2DSharp::UtMemory::alignPtr(char* ptr, int align, int signatureSize)
{
    auto arg0 = (char*)ptr;
    auto __ret = ::live2d::UtMemory::alignPtr(arg0, align, signatureSize);
    return __ret;
}

void Live2DSharp::UtMemory::staticInit_notForClientCall(Live2DSharp::LDAllocator^ allocator)
{
    auto arg0 = (::live2d::LDAllocator*)allocator->NativePtr;
    ::live2d::UtMemory::staticInit_notForClientCall(arg0);
}

void Live2DSharp::UtMemory::staticRelease_notForClientCall()
{
    ::live2d::UtMemory::staticRelease_notForClientCall();
}

System::IntPtr Live2DSharp::UtMemory::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::UtMemory::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::UtMemory*)object.ToPointer();
}

Live2DSharp::MemoryParam^ Live2DSharp::UtMemory::StaticMemoryParam::get()
{
    auto __ret = ::live2d::UtMemory::getStaticMemoryParam();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::MemoryParam((::live2d::MemoryParam*)__ret);
}

Live2DSharp::MemoryParam^ Live2DSharp::UtMemory::TmpMemoryParam::get()
{
    auto __ret = ::live2d::UtMemory::getTmpMemoryParam();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::MemoryParam((::live2d::MemoryParam*)__ret);
}

