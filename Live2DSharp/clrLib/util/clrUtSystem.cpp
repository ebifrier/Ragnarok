#include "util/clrUtSystem.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::UtSystem::UtSystem(::live2d::UtSystem* native)
{
    NativePtr = native;
}

Live2DSharp::UtSystem^ Live2DSharp::UtSystem::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::UtSystem((::live2d::UtSystem*) native.ToPointer());
}

void Live2DSharp::UtSystem::exit(int code)
{
    ::live2d::UtSystem::exit(code);
}

System::IntPtr Live2DSharp::UtSystem::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::UtSystem::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::UtSystem*)object.ToPointer();
}

long long Live2DSharp::UtSystem::TimeMSec::get()
{
    auto __ret = ::live2d::UtSystem::getTimeMSec();
    return __ret;
}

long long Live2DSharp::UtSystem::UserTimeMSec::get()
{
    auto __ret = ::live2d::UtSystem::getUserTimeMSec();
    return __ret;
}

void Live2DSharp::UtSystem::UserTimeMSec::set(long long t)
{
    auto arg0 = (::l2d_int64)t;
    ::live2d::UtSystem::setUserTimeMSec(arg0);
}

bool Live2DSharp::UtSystem::isBigEndian::get()
{
    auto __ret = ::live2d::UtSystem::isBigEndian();
    return __ret;
}

long long Live2DSharp::UtSystem::updateUserTimeMSec::get()
{
    auto __ret = ::live2d::UtSystem::updateUserTimeMSec();
    return __ret;
}

