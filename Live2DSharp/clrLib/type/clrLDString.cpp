#include "type/clrLDString.h"
#include "io/clrRefString.h"
#include "memory/clrMemoryParam.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::LDString::LDString(::live2d::LDString* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::LDString^ Live2DSharp::LDString::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::LDString((::live2d::LDString*) native.ToPointer());
}

Live2DSharp::LDString::LDString()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::LDString();
}

Live2DSharp::LDString::LDString(System::String^ s, Live2DSharp::MemoryParam^ memParam)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(s);
    auto arg0 = _arg0.c_str();
    auto arg1 = (::live2d::MemoryParam*)memParam->NativePtr;
    NativePtr = new ::live2d::LDString(arg0, arg1);
}

Live2DSharp::LDString::LDString(System::String^ s, int length, Live2DSharp::MemoryParam^ memParam)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(s);
    auto arg0 = _arg0.c_str();
    auto arg2 = (::live2d::MemoryParam*)memParam->NativePtr;
    NativePtr = new ::live2d::LDString(arg0, length, arg2);
}

Live2DSharp::LDString::LDString(Live2DSharp::RefString^ refStr, Live2DSharp::MemoryParam^ memParam)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    auto &arg0 = *(::live2d::RefString*)refStr->NativePtr;
    auto arg1 = (::live2d::MemoryParam*)memParam->NativePtr;
    NativePtr = new ::live2d::LDString(arg0, arg1);
}

Live2DSharp::LDString::LDString(System::String^ s, int length, bool useptr, Live2DSharp::MemoryParam^ memParam)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(s);
    auto arg0 = _arg0.c_str();
    auto arg3 = (::live2d::MemoryParam*)memParam->NativePtr;
    NativePtr = new ::live2d::LDString(arg0, length, useptr, arg3);
}

bool Live2DSharp::LDString::operator<(Live2DSharp::LDString^ __op, Live2DSharp::LDString^ s)
{
    auto &arg0 = *(::live2d::LDString*)__op->NativePtr;
    auto &arg1 = *(::live2d::LDString*)s->NativePtr;
    auto __ret = arg0 < arg1;
    return __ret;
}

bool Live2DSharp::LDString::operator<(Live2DSharp::LDString^ __op, System::String^ c)
{
    auto &arg0 = *(::live2d::LDString*)__op->NativePtr;
    auto _arg1 = clix::marshalString<clix::E_UTF8>(c);
    auto arg1 = _arg1.c_str();
    auto __ret = arg0 < arg1;
    return __ret;
}

bool Live2DSharp::LDString::operator>(Live2DSharp::LDString^ __op, Live2DSharp::LDString^ s)
{
    auto &arg0 = *(::live2d::LDString*)__op->NativePtr;
    auto &arg1 = *(::live2d::LDString*)s->NativePtr;
    auto __ret = arg0 > arg1;
    return __ret;
}

bool Live2DSharp::LDString::operator>(Live2DSharp::LDString^ __op, System::String^ c)
{
    auto &arg0 = *(::live2d::LDString*)__op->NativePtr;
    auto _arg1 = clix::marshalString<clix::E_UTF8>(c);
    auto arg1 = _arg1.c_str();
    auto __ret = arg0 > arg1;
    return __ret;
}

bool Live2DSharp::LDString::equals(Live2DSharp::RefString^ refStr)
{
    auto &arg0 = *(::live2d::RefString*)refStr->NativePtr;
    auto __ret = ((::live2d::LDString*)NativePtr)->equals(arg0);
    return __ret;
}

Live2DSharp::LDString^ Live2DSharp::LDString::operator+(Live2DSharp::LDString^ __op, Live2DSharp::LDString^ s)
{
    auto &arg0 = *(::live2d::LDString*)__op->NativePtr;
    auto &arg1 = *(::live2d::LDString*)s->NativePtr;
    auto __ret = arg0 + arg1;
    auto ____ret = new ::live2d::LDString(__ret);
    return (____ret == nullptr) ? nullptr : gcnew Live2DSharp::LDString((::live2d::LDString*)____ret);
}

Live2DSharp::LDString^ Live2DSharp::LDString::operator+(Live2DSharp::LDString^ __op, System::String^ s)
{
    auto &arg0 = *(::live2d::LDString*)__op->NativePtr;
    auto _arg1 = clix::marshalString<clix::E_UTF8>(s);
    auto arg1 = _arg1.c_str();
    auto __ret = arg0 + arg1;
    auto ____ret = new ::live2d::LDString(__ret);
    return (____ret == nullptr) ? nullptr : gcnew Live2DSharp::LDString((::live2d::LDString*)____ret);
}

Live2DSharp::LDString^ Live2DSharp::LDString::append(System::String^ p, int length)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(p);
    auto arg0 = _arg0.c_str();
    auto &__ret = ((::live2d::LDString*)NativePtr)->append(arg0, length);
    return (Live2DSharp::LDString^)((&__ret == nullptr) ? nullptr : gcnew Live2DSharp::LDString((::live2d::LDString*)&__ret));
}

Live2DSharp::LDString^ Live2DSharp::LDString::append(int count, char p)
{
    auto &__ret = ((::live2d::LDString*)NativePtr)->append(count, p);
    return (Live2DSharp::LDString^)((&__ret == nullptr) ? nullptr : gcnew Live2DSharp::LDString((::live2d::LDString*)&__ret));
}

void Live2DSharp::LDString::clear()
{
    ((::live2d::LDString*)NativePtr)->clear();
}

int Live2DSharp::LDString::Hashcode::get()
{
    auto __ret = ((::live2d::LDString*)NativePtr)->getHashcode();
    return __ret;
}

unsigned int Live2DSharp::LDString::length::get()
{
    auto __ret = ((::live2d::LDString*)NativePtr)->length();
    return __ret;
}

int Live2DSharp::LDString::size::get()
{
    auto __ret = ((::live2d::LDString*)NativePtr)->size();
    return __ret;
}

System::String^ Live2DSharp::LDString::c_str::get()
{
    auto __ret = ((::live2d::LDString*)NativePtr)->c_str();
    if (__ret == nullptr) return nullptr;
    return clix::marshalString<clix::E_UTF8>(__ret);
}

