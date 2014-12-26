#include "util/clrUtMath.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::UtMath::UtMath(::live2d::UtMath* native)
{
    NativePtr = native;
}

Live2DSharp::UtMath^ Live2DSharp::UtMath::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::UtMath((::live2d::UtMath*) native.ToPointer());
}

float Live2DSharp::UtMath::range(float v, float min, float max)
{
    auto __ret = ::live2d::UtMath::range(v, min, max);
    return __ret;
}

double Live2DSharp::UtMath::getAngleNotAbs(float* v1, float* v2)
{
    auto arg0 = (::l2d_pointf*)v1;
    auto arg1 = (::l2d_pointf*)v2;
    auto __ret = ::live2d::UtMath::getAngleNotAbs(arg0, arg1);
    return __ret;
}

double Live2DSharp::UtMath::getAngleDiff(double Q1, double Q2)
{
    auto __ret = ::live2d::UtMath::getAngleDiff(Q1, Q2);
    return __ret;
}

double Live2DSharp::UtMath::fsin(double x)
{
    auto __ret = ::live2d::UtMath::fsin(x);
    return __ret;
}

double Live2DSharp::UtMath::fcos(double x)
{
    auto __ret = ::live2d::UtMath::fcos(x);
    return __ret;
}

System::IntPtr Live2DSharp::UtMath::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::UtMath::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::UtMath*)object.ToPointer();
}
double Live2DSharp::UtMath::DEG_TO_RAD_D::get()
{
    return ::live2d::UtMath::DEG_TO_RAD_D;
}

float Live2DSharp::UtMath::DEG_TO_RAD_F::get()
{
    return ::live2d::UtMath::DEG_TO_RAD_F;
}

double Live2DSharp::UtMath::RAD_TO_DEG_D::get()
{
    return ::live2d::UtMath::RAD_TO_DEG_D;
}

float Live2DSharp::UtMath::RAD_TO_DEG_F::get()
{
    return ::live2d::UtMath::RAD_TO_DEG_F;
}

