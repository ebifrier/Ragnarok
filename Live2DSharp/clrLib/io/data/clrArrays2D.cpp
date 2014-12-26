#include "io/data/clrArrays2D.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::Arrays2D::Arrays2D(::live2d::Arrays2D* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::Arrays2D^ Live2DSharp::Arrays2D::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::Arrays2D((::live2d::Arrays2D*) native.ToPointer());
}

Live2DSharp::Arrays2D::Arrays2D(void** ptr, int size1, int size2)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    auto arg0 = (void**)ptr;
    NativePtr = new ::live2d::Arrays2D(arg0, size1, size2);
}

