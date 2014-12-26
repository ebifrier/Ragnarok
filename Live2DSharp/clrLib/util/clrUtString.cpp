#include "util/clrUtString.h"
#include "type/clrLDString.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::UtString::UtString(::live2d::UtString* native)
{
    NativePtr = native;
}

Live2DSharp::UtString^ Live2DSharp::UtString::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::UtString((::live2d::UtString*) native.ToPointer());
}

Live2DSharp::LDString^ Live2DSharp::UtString::toString(System::String^ msg)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(msg);
    auto arg0 = _arg0.c_str();
    auto __ret = ::live2d::UtString::toString(arg0);
    auto ____ret = new ::live2d::LDString(__ret);
    return (____ret == nullptr) ? nullptr : gcnew Live2DSharp::LDString((::live2d::LDString*)____ret);
}

bool Live2DSharp::UtString::startsWith(System::String^ text, System::String^ startWord)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(text);
    auto arg0 = _arg0.c_str();
    auto _arg1 = clix::marshalString<clix::E_UTF8>(startWord);
    auto arg1 = _arg1.c_str();
    auto __ret = ::live2d::UtString::startsWith(arg0, arg1);
    return __ret;
}

float Live2DSharp::UtString::strToFloat(System::String^ str, int len, int pos, int* ret_endpos)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(str);
    auto arg0 = _arg0.c_str();
    auto arg3 = (int*)ret_endpos;
    auto __ret = ::live2d::UtString::strToFloat(arg0, len, pos, arg3);
    return __ret;
}

System::IntPtr Live2DSharp::UtString::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::UtString::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::UtString*)object.ToPointer();
}
