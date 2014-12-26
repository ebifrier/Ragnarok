#include "util/clrUtFile.h"
#include "type/clrLDString.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::UtFile::UtFile(::live2d::UtFile* native)
{
    NativePtr = native;
}

Live2DSharp::UtFile^ Live2DSharp::UtFile::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::UtFile((::live2d::UtFile*) native.ToPointer());
}

char* Live2DSharp::UtFile::loadFile(Live2DSharp::LDString^ filepath, int* ret_bufsize)
{
    auto &arg0 = *(::live2d::LDString*)filepath->NativePtr;
    auto arg1 = (int*)ret_bufsize;
    auto __ret = ::live2d::UtFile::loadFile(arg0, arg1);
    return __ret;
}

char* Live2DSharp::UtFile::loadFile(System::String^ filepath, int* ret_bufsize)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(filepath);
    auto arg0 = _arg0.c_str();
    auto arg1 = (int*)ret_bufsize;
    auto __ret = ::live2d::UtFile::loadFile(arg0, arg1);
    return __ret;
}

void Live2DSharp::UtFile::releaseLoadBuffer(char* buf)
{
    auto arg0 = (char*)buf;
    ::live2d::UtFile::releaseLoadBuffer(arg0);
}

System::IntPtr Live2DSharp::UtFile::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::UtFile::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::UtFile*)object.ToPointer();
}
