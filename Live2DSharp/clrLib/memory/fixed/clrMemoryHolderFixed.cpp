#include "memory/fixed/clrMemoryHolderFixed.h"
#include "memory/clrAPageHeader.h"
#include "memory/fixed/clrMHPageHeaderFixed.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::MemoryHolderFixed::MemoryHolderFixed(::live2d::MemoryHolderFixed* native)
    : Live2DSharp::AMemoryHolder((::live2d::AMemoryHolder*)native)
{
}

Live2DSharp::MemoryHolderFixed^ Live2DSharp::MemoryHolderFixed::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::MemoryHolderFixed((::live2d::MemoryHolderFixed*) native.ToPointer());
}

Live2DSharp::MemoryHolderFixed::MemoryHolderFixed(Live2DSharp::LDAllocator::Type allocType, System::String^ holderName, unsigned int pageSize)
    : Live2DSharp::AMemoryHolder((::live2d::AMemoryHolder*)nullptr)
{
    auto arg0 = (::live2d::LDAllocator::Type)allocType;
    auto _arg1 = clix::marshalString<clix::E_UTF8>(holderName);
    auto arg1 = _arg1.c_str();
    auto arg2 = (::l2d_size_t)(::size_t)pageSize;
    NativePtr = new ::live2d::MemoryHolderFixed(arg0, arg1, arg2);
}

::System::IntPtr Live2DSharp::MemoryHolderFixed::malloc_exe(unsigned int size, int align)
{
    auto arg0 = (::l2d_size_t)(::size_t)size;
    auto __ret = ((::live2d::MemoryHolderFixed*)NativePtr)->malloc_exe(arg0, align);
    if (__ret == nullptr) return System::IntPtr();
    return ::System::IntPtr(__ret);
}

void Live2DSharp::MemoryHolderFixed::free_exe(Live2DSharp::APageHeader^ header, ::System::IntPtr ptr)
{
    auto arg0 = (::live2d::APageHeader*)header->NativePtr;
    auto arg1 = (void*)ptr;
    ((::live2d::MemoryHolderFixed*)NativePtr)->free_exe(arg0, arg1);
}

void Live2DSharp::MemoryHolderFixed::clear()
{
    ((::live2d::MemoryHolderFixed*)NativePtr)->clear();
}

void Live2DSharp::MemoryHolderFixed::healthCheck()
{
    ((::live2d::MemoryHolderFixed*)NativePtr)->healthCheck();
}

void Live2DSharp::MemoryHolderFixed::dumpPages(Live2DSharp::MHPageHeaderFixed^ c)
{
    auto arg0 = (::live2d::MHPageHeaderFixed*)c->NativePtr;
    ((::live2d::MemoryHolderFixed*)NativePtr)->dumpPages(arg0);
}

void Live2DSharp::MemoryHolderFixed::checkPages(Live2DSharp::MHPageHeaderFixed^ c)
{
    auto arg0 = (::live2d::MHPageHeaderFixed*)c->NativePtr;
    ((::live2d::MemoryHolderFixed*)NativePtr)->checkPages(arg0);
}

void Live2DSharp::MemoryHolderFixed::DefaultPageSize::set(unsigned int size)
{
    auto arg0 = (::l2d_size_t)(::size_t)size;
    ::live2d::MemoryHolderFixed::setDefaultPageSize(arg0);
}

