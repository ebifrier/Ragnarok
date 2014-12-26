#include "memory/tmp/clrMHBin.h"
#include "memory/tmp/clrMemoryHolderTmp.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::MHBin::MHBin(::live2d::MHBin* native)
{
    NativePtr = native;
}

Live2DSharp::MHBin^ Live2DSharp::MHBin::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::MHBin((::live2d::MHBin*) native.ToPointer());
}

Live2DSharp::MHBin::MHBin()
{
    NativePtr = new ::live2d::MHBin();
}

void Live2DSharp::MHBin::init(unsigned short binNo, unsigned int _chunkSize, unsigned int _pageSize)
{
    auto arg0 = (::l2d_uint16)binNo;
    auto arg1 = (::l2d_size_t)(::size_t)_chunkSize;
    auto arg2 = (::l2d_size_t)(::size_t)_pageSize;
    ((::live2d::MHBin*)NativePtr)->init(arg0, arg1, arg2);
}

unsigned int Live2DSharp::MHBin::getChunkSize(unsigned int malloc_size)
{
    auto arg0 = (::l2d_size_t)(::size_t)malloc_size;
    auto __ret = ((::live2d::MHBin*)NativePtr)->getChunkSize(arg0);
    return __ret;
}

System::IntPtr Live2DSharp::MHBin::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::MHBin::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::MHBin*)object.ToPointer();
}

unsigned int Live2DSharp::MHBin::chunkSize::get()
{
    return ((::live2d::MHBin*)NativePtr)->chunkSize;
}

void Live2DSharp::MHBin::chunkSize::set(unsigned int value)
{
    ((::live2d::MHBin*)NativePtr)->chunkSize = (::l2d_size_t)(::size_t)value;
}

unsigned int Live2DSharp::MHBin::pageSize::get()
{
    return ((::live2d::MHBin*)NativePtr)->pageSize;
}

void Live2DSharp::MHBin::pageSize::set(unsigned int value)
{
    ((::live2d::MHBin*)NativePtr)->pageSize = (::l2d_size_t)(::size_t)value;
}

unsigned short Live2DSharp::MHBin::pageChunkCount::get()
{
    return ((::live2d::MHBin*)NativePtr)->pageChunkCount;
}

void Live2DSharp::MHBin::pageChunkCount::set(unsigned short value)
{
    ((::live2d::MHBin*)NativePtr)->pageChunkCount = (::l2d_uint16)value;
}

unsigned short Live2DSharp::MHBin::binNo::get()
{
    return ((::live2d::MHBin*)NativePtr)->binNo;
}

void Live2DSharp::MHBin::binNo::set(unsigned short value)
{
    ((::live2d::MHBin*)NativePtr)->binNo = (::l2d_uint16)value;
}

cli::array<unsigned int>^ Live2DSharp::MHBin::bitmask::get()
{
    cli::array<unsigned int>^ __array = nullptr;
    if (((::live2d::MHBin*)NativePtr)->bitmask != 0)
    {
        __array = gcnew cli::array<unsigned int>(3);
        for (int i = 0; i < 3; i++)
            __array[i] = ((::live2d::MHBin*)NativePtr)->bitmask[i];
    }
    return __array;
}

void Live2DSharp::MHBin::bitmask::set(cli::array<unsigned int>^ value)
{
    if (value != nullptr)
    {
        for (int i = 0; i < 3; i++)
            ((::live2d::MHBin*)NativePtr)->bitmask[i] = value[i];
    }
}

