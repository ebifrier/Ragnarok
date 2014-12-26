#include "motion/clrEyeBlinkMotion.h"
#include "./clrALive2DModel.h"
#include "type/clrLDString.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::EyeBlinkMotion::EyeBlinkMotion(::live2d::EyeBlinkMotion* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::EyeBlinkMotion^ Live2DSharp::EyeBlinkMotion::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::EyeBlinkMotion((::live2d::EyeBlinkMotion*) native.ToPointer());
}

Live2DSharp::EyeBlinkMotion::EyeBlinkMotion()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::EyeBlinkMotion();
}

void Live2DSharp::EyeBlinkMotion::setEyeMotion(int closingMotionMsec, int closedMotionMsec, int openingMotionMsec)
{
    ((::live2d::EyeBlinkMotion*)NativePtr)->setEyeMotion(closingMotionMsec, closedMotionMsec, openingMotionMsec);
}

void Live2DSharp::EyeBlinkMotion::Interval::set(int blinkIntervalMsec)
{
    ((::live2d::EyeBlinkMotion*)NativePtr)->setInterval(blinkIntervalMsec);
}

void Live2DSharp::EyeBlinkMotion::Param::set(Live2DSharp::ALive2DModel^ model)
{
    auto arg0 = (::live2d::ALive2DModel*)model->NativePtr;
    ((::live2d::EyeBlinkMotion*)NativePtr)->setParam(arg0);
}

long long Live2DSharp::EyeBlinkMotion::calcNextBlink::get()
{
    auto __ret = ((::live2d::EyeBlinkMotion*)NativePtr)->calcNextBlink();
    return __ret;
}

