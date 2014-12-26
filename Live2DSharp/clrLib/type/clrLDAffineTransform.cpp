#include "type/clrLDAffineTransform.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::LDAffineTransform::LDAffineTransform(::live2d::LDAffineTransform* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::LDAffineTransform^ Live2DSharp::LDAffineTransform::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::LDAffineTransform((::live2d::LDAffineTransform*) native.ToPointer());
}

Live2DSharp::LDAffineTransform::LDAffineTransform()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::LDAffineTransform();
}

Live2DSharp::LDAffineTransform::LDAffineTransform(float m1, float m2, float m3, float m4, float m5, float m6)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::LDAffineTransform(m1, m2, m3, m4, m5, m6);
}

void Live2DSharp::LDAffineTransform::factorize(float* ret)
{
    auto arg0 = (float*)ret;
    ((::live2d::LDAffineTransform*)NativePtr)->factorize(arg0);
}

void Live2DSharp::LDAffineTransform::getMatrix(float* ret)
{
    auto arg0 = (float*)ret;
    ((::live2d::LDAffineTransform*)NativePtr)->getMatrix(arg0);
}

void Live2DSharp::LDAffineTransform::interpolate(Live2DSharp::LDAffineTransform^ aa1, Live2DSharp::LDAffineTransform^ aa2, float t, Live2DSharp::LDAffineTransform^ ret)
{
    auto &arg0 = *(::live2d::LDAffineTransform*)aa1->NativePtr;
    auto &arg1 = *(::live2d::LDAffineTransform*)aa2->NativePtr;
    auto &arg3 = *(::live2d::LDAffineTransform*)ret->NativePtr;
    ::live2d::LDAffineTransform::interpolate(arg0, arg1, t, arg3);
}

void Live2DSharp::LDAffineTransform::transform(float* src, float* dst, int numPoint)
{
    auto arg0 = (float*)src;
    auto arg1 = (float*)dst;
    ((::live2d::LDAffineTransform*)NativePtr)->transform(arg0, arg1, numPoint);
}

void Live2DSharp::LDAffineTransform::Factor::set(float* fmat)
{
    auto arg0 = (float*)fmat;
    ((::live2d::LDAffineTransform*)NativePtr)->setFactor(arg0);
}

