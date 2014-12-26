#include "motion/clrMotionQueueEnt.h"
#include "motion/clrAMotion.h"
#include "motion/clrLive2DMotion.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::MotionQueueEnt::MotionQueueEnt(::live2d::MotionQueueEnt* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::MotionQueueEnt^ Live2DSharp::MotionQueueEnt::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::MotionQueueEnt((::live2d::MotionQueueEnt*) native.ToPointer());
}

Live2DSharp::MotionQueueEnt::MotionQueueEnt()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::MotionQueueEnt();
}

void Live2DSharp::MotionQueueEnt::startFadeout(long long fadeOutMsec)
{
    ((::live2d::MotionQueueEnt*)NativePtr)->startFadeout(fadeOutMsec);
}

void Live2DSharp::MotionQueueEnt::setState(long long time, float weight)
{
    ((::live2d::MotionQueueEnt*)NativePtr)->setState(time, weight);
}

long long Live2DSharp::MotionQueueEnt::StartTimeMSec::get()
{
    auto __ret = ((::live2d::MotionQueueEnt*)NativePtr)->getStartTimeMSec();
    return __ret;
}

void Live2DSharp::MotionQueueEnt::StartTimeMSec::set(long long t)
{
    ((::live2d::MotionQueueEnt*)NativePtr)->setStartTimeMSec(t);
}

long long Live2DSharp::MotionQueueEnt::FadeInStartTimeMSec::get()
{
    auto __ret = ((::live2d::MotionQueueEnt*)NativePtr)->getFadeInStartTimeMSec();
    return __ret;
}

void Live2DSharp::MotionQueueEnt::FadeInStartTimeMSec::set(long long t)
{
    ((::live2d::MotionQueueEnt*)NativePtr)->setFadeInStartTimeMSec(t);
}

long long Live2DSharp::MotionQueueEnt::EndTimeMSec::get()
{
    auto __ret = ((::live2d::MotionQueueEnt*)NativePtr)->getEndTimeMSec();
    return __ret;
}

void Live2DSharp::MotionQueueEnt::EndTimeMSec::set(long long t)
{
    ((::live2d::MotionQueueEnt*)NativePtr)->setEndTimeMSec(t);
}

void Live2DSharp::MotionQueueEnt::Finished::set(bool f)
{
    ((::live2d::MotionQueueEnt*)NativePtr)->setFinished(f);
}

void Live2DSharp::MotionQueueEnt::Available::set(bool v)
{
    ((::live2d::MotionQueueEnt*)NativePtr)->setAvailable(v);
}

long long Live2DSharp::MotionQueueEnt::State_time::get()
{
    auto __ret = ((::live2d::MotionQueueEnt*)NativePtr)->getState_time();
    return __ret;
}

float Live2DSharp::MotionQueueEnt::State_weight::get()
{
    auto __ret = ((::live2d::MotionQueueEnt*)NativePtr)->getState_weight();
    return __ret;
}

bool Live2DSharp::MotionQueueEnt::isFinished::get()
{
    auto __ret = ((::live2d::MotionQueueEnt*)NativePtr)->isFinished();
    return __ret;
}

bool Live2DSharp::MotionQueueEnt::isAvailable::get()
{
    auto __ret = ((::live2d::MotionQueueEnt*)NativePtr)->isAvailable();
    return __ret;
}

