#include "id/clrID.h"
#include "memory/clrAMemoryHolder.h"
#include "memory/clrMemoryParam.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::ID::ID(::live2d::ID* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::ID^ Live2DSharp::ID::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::ID((::live2d::ID*) native.ToPointer());
}

Live2DSharp::ID::ID()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::ID();
}

void Live2DSharp::ID::staticInit_notForClientCall()
{
    ::live2d::ID::staticInit_notForClientCall();
}

void Live2DSharp::ID::staticRelease_notForClientCall()
{
    ::live2d::ID::staticRelease_notForClientCall();
}

