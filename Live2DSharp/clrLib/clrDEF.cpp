#include "./clrDEF.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::DEF::DEF(::live2d::DEF* native)
{
    NativePtr = native;
}

Live2DSharp::DEF^ Live2DSharp::DEF::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::DEF((::live2d::DEF*) native.ToPointer());
}

System::IntPtr Live2DSharp::DEF::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::DEF::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::DEF*)object.ToPointer();
}
int Live2DSharp::DEF::VERTEX_TYPE_OFFSET0_STEP2::get()
{
    return ::live2d::DEF::VERTEX_TYPE_OFFSET0_STEP2;
}

int Live2DSharp::DEF::VERTEX_TYPE_OFFSET2_STEP5::get()
{
    return ::live2d::DEF::VERTEX_TYPE_OFFSET2_STEP5;
}

int Live2DSharp::DEF::VERTEX_TYPE_OFFSET0_STEP5::get()
{
    return ::live2d::DEF::VERTEX_TYPE_OFFSET0_STEP5;
}

int Live2DSharp::DEF::VERTEX_OFFSET::get()
{
    return ::live2d::DEF::VERTEX_OFFSET;
}

int Live2DSharp::DEF::VERTEX_STEP::get()
{
    return ::live2d::DEF::VERTEX_STEP;
}

int Live2DSharp::DEF::VERTEX_TYPE::get()
{
    return ::live2d::DEF::VERTEX_TYPE;
}

int Live2DSharp::DEF::MAX_INTERPOLATION::get()
{
    return ::live2d::DEF::MAX_INTERPOLATION;
}

int Live2DSharp::DEF::PIVOT_TABLE_SIZE::get()
{
    return ::live2d::DEF::PIVOT_TABLE_SIZE;
}

float Live2DSharp::DEF::GOSA::get()
{
    return ::live2d::DEF::GOSA;
}

