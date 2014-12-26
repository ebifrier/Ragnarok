#include "memory/fixed/clrMHPageHeaderFixed.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::MHPageHeaderFixed::MHPageHeaderFixed(::live2d::MHPageHeaderFixed* native)
    : Live2DSharp::APageHeader((::live2d::APageHeader*)native)
{
}

Live2DSharp::MHPageHeaderFixed^ Live2DSharp::MHPageHeaderFixed::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::MHPageHeaderFixed((::live2d::MHPageHeaderFixed*) native.ToPointer());
}

Live2DSharp::MHPageHeaderFixed::MHPageHeaderFixed()
    : Live2DSharp::APageHeader((::live2d::APageHeader*)nullptr)
{
    NativePtr = new ::live2d::MHPageHeaderFixed();
}

void Live2DSharp::MHPageHeaderFixed::free_exe(::System::IntPtr ptr)
{
    auto arg0 = (void*)ptr;
    ((::live2d::MHPageHeaderFixed*)NativePtr)->free_exe(arg0);
}

Live2DSharp::MHPageHeaderFixed^ Live2DSharp::MHPageHeaderFixed::nextPage::get()
{
    return (((::live2d::MHPageHeaderFixed*)NativePtr)->nextPage == nullptr) ? nullptr : gcnew Live2DSharp::MHPageHeaderFixed((::live2d::MHPageHeaderFixed*)((::live2d::MHPageHeaderFixed*)NativePtr)->nextPage);
}

void Live2DSharp::MHPageHeaderFixed::nextPage::set(Live2DSharp::MHPageHeaderFixed^ value)
{
    ((::live2d::MHPageHeaderFixed*)NativePtr)->nextPage = (::live2d::MHPageHeaderFixed*)value->NativePtr;
}

char* Live2DSharp::MHPageHeaderFixed::endPtr::get()
{
    return ((::live2d::MHPageHeaderFixed*)NativePtr)->endPtr;
}

void Live2DSharp::MHPageHeaderFixed::endPtr::set(char* value)
{
    ((::live2d::MHPageHeaderFixed*)NativePtr)->endPtr = (char*)value;
}

unsigned int Live2DSharp::MHPageHeaderFixed::size::get()
{
    return ((::live2d::MHPageHeaderFixed*)NativePtr)->size;
}

void Live2DSharp::MHPageHeaderFixed::size::set(unsigned int value)
{
    ((::live2d::MHPageHeaderFixed*)NativePtr)->size = (::l2d_uint32)value;
}

char* Live2DSharp::MHPageHeaderFixed::curPtr::get()
{
    return ((::live2d::MHPageHeaderFixed*)NativePtr)->curPtr;
}

void Live2DSharp::MHPageHeaderFixed::curPtr::set(char* value)
{
    ((::live2d::MHPageHeaderFixed*)NativePtr)->curPtr = (char*)value;
}

unsigned int Live2DSharp::MHPageHeaderFixed::rest::get()
{
    return ((::live2d::MHPageHeaderFixed*)NativePtr)->rest;
}

void Live2DSharp::MHPageHeaderFixed::rest::set(unsigned int value)
{
    ((::live2d::MHPageHeaderFixed*)NativePtr)->rest = (::l2d_uint32)value;
}

unsigned int Live2DSharp::MHPageHeaderFixed::pageNo::get()
{
    return ((::live2d::MHPageHeaderFixed*)NativePtr)->pageNo;
}

void Live2DSharp::MHPageHeaderFixed::pageNo::set(unsigned int value)
{
    ((::live2d::MHPageHeaderFixed*)NativePtr)->pageNo = (::l2d_uint32)value;
}

int Live2DSharp::MHPageHeaderFixed::PageNo::get()
{
    auto __ret = ((::live2d::MHPageHeaderFixed*)NativePtr)->getPageNo();
    return __ret;
}

int Live2DSharp::MHPageHeaderFixed::PageAmount::get()
{
    auto __ret = ((::live2d::MHPageHeaderFixed*)NativePtr)->getPageAmount();
    return __ret;
}

::System::IntPtr Live2DSharp::MHPageHeaderFixed::StartPtr::get()
{
    auto __ret = ((::live2d::MHPageHeaderFixed*)NativePtr)->getStartPtr();
    if (__ret == nullptr) return System::IntPtr();
    return ::System::IntPtr(__ret);
}

int Live2DSharp::MHPageHeaderFixed::pageAmount::get()
{
    return ::live2d::MHPageHeaderFixed::pageAmount;
}

void Live2DSharp::MHPageHeaderFixed::pageAmount::set(int value)
{
    ::live2d::MHPageHeaderFixed::pageAmount = value;
}

