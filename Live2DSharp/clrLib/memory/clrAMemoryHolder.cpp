#include "memory/clrAMemoryHolder.h"
#include "memory/clrAPageHeader.h"
#include "memory/clrMemoryParam.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::AllocHeader::AllocHeader(::live2d::AllocHeader* native)
{
    NativePtr = native;
}

Live2DSharp::AllocHeader^ Live2DSharp::AllocHeader::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::AllocHeader((::live2d::AllocHeader*) native.ToPointer());
}

void Live2DSharp::AllocHeader::free_exe(::System::IntPtr ptr)
{
    auto arg0 = (void*)ptr;
    ((::live2d::AllocHeader*)NativePtr)->free_exe(arg0);
}

System::IntPtr Live2DSharp::AllocHeader::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::AllocHeader::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::AllocHeader*)object.ToPointer();
}

Live2DSharp::APageHeader^ Live2DSharp::AllocHeader::ptrToPageHeader::get()
{
    return (((::live2d::AllocHeader*)NativePtr)->ptrToPageHeader == nullptr) ? nullptr : gcnew Live2DSharp::APageHeader((::live2d::APageHeader*)((::live2d::AllocHeader*)NativePtr)->ptrToPageHeader);
}

void Live2DSharp::AllocHeader::ptrToPageHeader::set(Live2DSharp::APageHeader^ value)
{
    ((::live2d::AllocHeader*)NativePtr)->ptrToPageHeader = (::live2d::APageHeader*)value->NativePtr;
}

Live2DSharp::AMemoryHolder::AMemoryHolder(::live2d::AMemoryHolder* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::AMemoryHolder^ Live2DSharp::AMemoryHolder::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::AMemoryHolder((::live2d::AMemoryHolder*) native.ToPointer());
}

Live2DSharp::AMemoryHolder::AMemoryHolder()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
}

::System::IntPtr Live2DSharp::AMemoryHolder::malloc_exe(unsigned int size, int align)
{
    auto arg0 = (::l2d_size_t)(::size_t)size;
    auto __ret = ((::live2d::AMemoryHolder*)NativePtr)->malloc_exe(arg0, align);
    if (__ret == nullptr) return System::IntPtr();
    return ::System::IntPtr(__ret);
}

void Live2DSharp::AMemoryHolder::free_exe(Live2DSharp::APageHeader^ header, ::System::IntPtr ptr)
{
    auto arg0 = (::live2d::APageHeader*)header->NativePtr;
    auto arg1 = (void*)ptr;
    ((::live2d::AMemoryHolder*)NativePtr)->free_exe(arg0, arg1);
}

void Live2DSharp::AMemoryHolder::clear()
{
    ((::live2d::AMemoryHolder*)NativePtr)->clear();
}

void Live2DSharp::AMemoryHolder::healthCheck()
{
    ((::live2d::AMemoryHolder*)NativePtr)->healthCheck();
}

void Live2DSharp::AMemoryHolder::MemoryParam::set(Live2DSharp::MemoryParam^ group)
{
    auto arg0 = (::live2d::MemoryParam*)group->NativePtr;
    ((::live2d::AMemoryHolder*)NativePtr)->setMemoryParam(arg0);
}

int Live2DSharp::AMemoryHolder::CheckValue::get()
{
    auto __ret = ((::live2d::AMemoryHolder*)NativePtr)->getCheckValue();
    return __ret;
}

int Live2DSharp::AMemoryHolder::CHECK_VALUE::get()
{
    return ::live2d::AMemoryHolder::CHECK_VALUE;
}

int Live2DSharp::AMemoryHolder::MIN_CHUNK_REST::get()
{
    return ::live2d::AMemoryHolder::MIN_CHUNK_REST;
}

int Live2DSharp::AMemoryHolder::MIN_ALIGN::get()
{
    return ::live2d::AMemoryHolder::MIN_ALIGN;
}

