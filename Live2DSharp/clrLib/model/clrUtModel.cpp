#include "model/clrUtModel.h"
#include "./clrALive2DModel.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::UtModel::UtModel(::live2d::UtModel* native)
{
    NativePtr = native;
}

Live2DSharp::UtModel^ Live2DSharp::UtModel::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::UtModel((::live2d::UtModel*) native.ToPointer());
}

System::IntPtr Live2DSharp::UtModel::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::UtModel::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::UtModel*)object.ToPointer();
}
