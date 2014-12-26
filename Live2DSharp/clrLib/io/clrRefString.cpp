#include "io/clrRefString.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::RefString::RefString(::live2d::RefString* native)
{
    NativePtr = native;
}

Live2DSharp::RefString^ Live2DSharp::RefString::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::RefString((::live2d::RefString*) native.ToPointer());
}

Live2DSharp::RefString::RefString()
{
    NativePtr = new ::live2d::RefString();
}

Live2DSharp::RefString::RefString(System::String^ ptr, int length)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(ptr);
    auto arg0 = _arg0.c_str();
    NativePtr = new ::live2d::RefString(arg0, length);
}

void Live2DSharp::RefString::set(System::String^ ptr, int length)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(ptr);
    auto arg0 = _arg0.c_str();
    ((::live2d::RefString*)NativePtr)->set(arg0, length);
}

System::IntPtr Live2DSharp::RefString::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::RefString::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::RefString*)object.ToPointer();
}

System::String^ Live2DSharp::RefString::ptr_not_zero_end::get()
{
    return clix::marshalString<clix::E_UTF8>(((::live2d::RefString*)NativePtr)->ptr_not_zero_end);
}

void Live2DSharp::RefString::ptr_not_zero_end::set(System::String^ value)
{
    auto _value = clix::marshalString<clix::E_UTF8>(value);
    ((::live2d::RefString*)NativePtr)->ptr_not_zero_end = _value.c_str();
}

int Live2DSharp::RefString::length::get()
{
    return ((::live2d::RefString*)NativePtr)->length;
}

void Live2DSharp::RefString::length::set(int value)
{
    ((::live2d::RefString*)NativePtr)->length = value;
}

