#include "memory/clrLDUnmanagedObject.h"
#include "memory/clrMemoryParam.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::LDUnmanagedObject::LDUnmanagedObject(::live2d::LDUnmanagedObject* native)
{
    NativePtr = native;
}

Live2DSharp::LDUnmanagedObject^ Live2DSharp::LDUnmanagedObject::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::LDUnmanagedObject((::live2d::LDUnmanagedObject*) native.ToPointer());
}

Live2DSharp::LDUnmanagedObject::LDUnmanagedObject()
{
    NativePtr = new ::live2d::LDUnmanagedObject();
}

System::IntPtr Live2DSharp::LDUnmanagedObject::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::LDUnmanagedObject::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::LDUnmanagedObject*)object.ToPointer();
}
