#include "memory/clrAPageHeader.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::APageHeader::APageHeader(::live2d::APageHeader* native)
{
    NativePtr = native;
}

Live2DSharp::APageHeader^ Live2DSharp::APageHeader::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::APageHeader((::live2d::APageHeader*) native.ToPointer());
}

Live2DSharp::APageHeader::APageHeader()
{
}

void Live2DSharp::APageHeader::free_exe(::System::IntPtr ptr)
{
    auto arg0 = (void*)ptr;
    ((::live2d::APageHeader*)NativePtr)->free_exe(arg0);
}

System::IntPtr Live2DSharp::APageHeader::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::APageHeader::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::APageHeader*)object.ToPointer();
}
int Live2DSharp::APageHeader::ENTRY_OFFSET::get()
{
    return ::live2d::APageHeader::ENTRY_OFFSET;
}

