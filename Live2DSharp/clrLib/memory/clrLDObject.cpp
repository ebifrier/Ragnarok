#include "memory/clrLDObject.h"
#include "memory/clrAMemoryHolder.h"
#include "memory/clrMemoryParam.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::LDObject::LDObject(::live2d::LDObject* native)
{
    NativePtr = native;
}

Live2DSharp::LDObject^ Live2DSharp::LDObject::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::LDObject((::live2d::LDObject*) native.ToPointer());
}

Live2DSharp::LDObject::LDObject()
{
    NativePtr = new ::live2d::LDObject();
}

System::IntPtr Live2DSharp::LDObject::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::LDObject::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::LDObject*)object.ToPointer();
}
