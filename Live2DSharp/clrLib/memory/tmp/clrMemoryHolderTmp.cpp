#include "memory/tmp/clrMemoryHolderTmp.h"
#include "memory/clrAPageHeader.h"
#include "memory/tmp/clrMHBin.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::MemoryHolderTmp::MemoryHolderTmp(::live2d::MemoryHolderTmp* native)
    : Live2DSharp::LDUnmanagedObject((::live2d::LDUnmanagedObject*)native)
{
}

Live2DSharp::MemoryHolderTmp^ Live2DSharp::MemoryHolderTmp::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::MemoryHolderTmp((::live2d::MemoryHolderTmp*) native.ToPointer());
}

Live2DSharp::MemoryHolderTmp::MemoryHolderTmp(Live2DSharp::LDAllocator::Type allocType, System::String^ holderName)
    : Live2DSharp::LDUnmanagedObject((::live2d::LDUnmanagedObject*)nullptr)
{
    auto arg0 = (::live2d::LDAllocator::Type)allocType;
    auto _arg1 = clix::marshalString<clix::E_UTF8>(holderName);
    auto arg1 = _arg1.c_str();
    NativePtr = new ::live2d::MemoryHolderTmp(arg0, arg1);
}

::System::IntPtr Live2DSharp::MemoryHolderTmp::malloc_exe(unsigned int size, int align)
{
    auto arg0 = (::l2d_size_t)(::size_t)size;
    auto __ret = ((::live2d::MemoryHolderTmp*)NativePtr)->malloc_exe(arg0, align);
    if (__ret == nullptr) return System::IntPtr();
    return ::System::IntPtr(__ret);
}

void Live2DSharp::MemoryHolderTmp::free_exe(Live2DSharp::APageHeader^ header, ::System::IntPtr ptr)
{
    auto arg0 = (::live2d::APageHeader*)header->NativePtr;
    auto arg1 = (void*)ptr;
    ((::live2d::MemoryHolderTmp*)NativePtr)->free_exe(arg0, arg1);
}

void Live2DSharp::MemoryHolderTmp::clear()
{
    ((::live2d::MemoryHolderTmp*)NativePtr)->clear();
}

unsigned int Live2DSharp::MemoryHolderTmp::LARGE_0::get()
{
    return ::live2d::MemoryHolderTmp::LARGE_0;
}

unsigned int Live2DSharp::MemoryHolderTmp::PAGE_ALIGN::get()
{
    return ::live2d::MemoryHolderTmp::PAGE_ALIGN;
}

int Live2DSharp::MemoryHolderTmp::BIN_COUNT::get()
{
    return ::live2d::MemoryHolderTmp::BIN_COUNT;
}

cli::array<unsigned int>^ Live2DSharp::MemoryHolderTmp::CHUNK_SIZE::get()
{
    cli::array<unsigned int>^ __array = nullptr;
    if (::live2d::MemoryHolderTmp::CHUNK_SIZE != 0)
    {
        __array = gcnew cli::array<unsigned int>(6);
        for (int i = 0; i < 6; i++)
            __array[i] = ::live2d::MemoryHolderTmp::CHUNK_SIZE[i];
    }
    return __array;
}

Live2DSharp::MemoryHolderSocket::MemoryHolderSocket(::live2d::MemoryHolderSocket* native)
    : Live2DSharp::AMemoryHolder((::live2d::AMemoryHolder*)native)
{
}

Live2DSharp::MemoryHolderSocket^ Live2DSharp::MemoryHolderSocket::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::MemoryHolderSocket((::live2d::MemoryHolderSocket*) native.ToPointer());
}

Live2DSharp::MemoryHolderSocket::MemoryHolderSocket(Live2DSharp::MemoryHolderTmp^ impl)
    : Live2DSharp::AMemoryHolder((::live2d::AMemoryHolder*)nullptr)
{
    auto arg0 = (::live2d::MemoryHolderTmp*)impl->NativePtr;
    NativePtr = new ::live2d::MemoryHolderSocket(arg0);
}

::System::IntPtr Live2DSharp::MemoryHolderSocket::malloc_exe(unsigned int size, int align)
{
    auto arg0 = (::l2d_size_t)(::size_t)size;
    auto __ret = ((::live2d::MemoryHolderSocket*)NativePtr)->malloc_exe(arg0, align);
    if (__ret == nullptr) return System::IntPtr();
    return ::System::IntPtr(__ret);
}

void Live2DSharp::MemoryHolderSocket::free_exe(Live2DSharp::APageHeader^ header, ::System::IntPtr ptr)
{
    auto arg0 = (::live2d::APageHeader*)header->NativePtr;
    auto arg1 = (void*)ptr;
    ((::live2d::MemoryHolderSocket*)NativePtr)->free_exe(arg0, arg1);
}

void Live2DSharp::MemoryHolderSocket::clear()
{
    ((::live2d::MemoryHolderSocket*)NativePtr)->clear();
}

