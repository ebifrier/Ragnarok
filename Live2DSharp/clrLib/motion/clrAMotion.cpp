#include "motion/clrAMotion.h"
#include "./clrALive2DModel.h"
#include "motion/clrMotionQueueEnt.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::AMotion::AMotion(::live2d::AMotion* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::AMotion^ Live2DSharp::AMotion::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::AMotion((::live2d::AMotion*) native.ToPointer());
}

Live2DSharp::AMotion::AMotion()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
}

void Live2DSharp::AMotion::updateParam(Live2DSharp::ALive2DModel^ model, Live2DSharp::MotionQueueEnt^ motionQueueEnt)
{
    auto arg0 = (::live2d::ALive2DModel*)model->NativePtr;
    auto arg1 = (::live2d::MotionQueueEnt*)motionQueueEnt->NativePtr;
    ((::live2d::AMotion*)NativePtr)->updateParam(arg0, arg1);
}

void Live2DSharp::AMotion::reinit()
{
    ((::live2d::AMotion*)NativePtr)->reinit();
}

int Live2DSharp::AMotion::FadeIn::get()
{
    auto __ret = ((::live2d::AMotion*)NativePtr)->getFadeIn();
    return __ret;
}

void Live2DSharp::AMotion::FadeIn::set(int fadeInMsec)
{
    ((::live2d::AMotion*)NativePtr)->setFadeIn(fadeInMsec);
}

int Live2DSharp::AMotion::FadeOut::get()
{
    auto __ret = ((::live2d::AMotion*)NativePtr)->getFadeOut();
    return __ret;
}

void Live2DSharp::AMotion::FadeOut::set(int fadeOutMsec)
{
    ((::live2d::AMotion*)NativePtr)->setFadeOut(fadeOutMsec);
}

float Live2DSharp::AMotion::Weight::get()
{
    auto __ret = ((::live2d::AMotion*)NativePtr)->getWeight();
    return __ret;
}

void Live2DSharp::AMotion::Weight::set(float weight)
{
    ((::live2d::AMotion*)NativePtr)->setWeight(weight);
}

int Live2DSharp::AMotion::DurationMSec::get()
{
    auto __ret = ((::live2d::AMotion*)NativePtr)->getDurationMSec();
    return __ret;
}

int Live2DSharp::AMotion::LoopDurationMSec::get()
{
    auto __ret = ((::live2d::AMotion*)NativePtr)->getLoopDurationMSec();
    return __ret;
}

void Live2DSharp::AMotion::OffsetMSec::set(int offsetMsec)
{
    ((::live2d::AMotion*)NativePtr)->setOffsetMSec(offsetMsec);
}

