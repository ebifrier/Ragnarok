#include "motion/clrLive2DMotion.h"
#include "./clrALive2DModel.h"
#include "id/clrParamID.h"
#include "memory/clrAMemoryHolder.h"
#include "memory/clrMemoryParam.h"
#include "motion/clrMotionQueueEnt.h"
#include "type/clrLDString.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::Live2DMotion::Live2DMotion(::live2d::Live2DMotion* native)
    : Live2DSharp::AMotion((::live2d::AMotion*)native)
{
}

Live2DSharp::Live2DMotion^ Live2DSharp::Live2DMotion::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::Live2DMotion((::live2d::Live2DMotion*) native.ToPointer());
}

Live2DSharp::Live2DMotion::Live2DMotion()
    : Live2DSharp::AMotion((::live2d::AMotion*)nullptr)
{
    NativePtr = new ::live2d::Live2DMotion();
}

Live2DSharp::Live2DMotion^ Live2DSharp::Live2DMotion::loadMotion(Live2DSharp::LDString^ filepath)
{
    auto &arg0 = *(::live2d::LDString*)filepath->NativePtr;
    auto __ret = ::live2d::Live2DMotion::loadMotion(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::Live2DMotion((::live2d::Live2DMotion*)__ret);
}

Live2DSharp::Live2DMotion^ Live2DSharp::Live2DMotion::loadMotion(::System::IntPtr buf, int bufSize)
{
    auto arg0 = (const void*)buf;
    auto __ret = ::live2d::Live2DMotion::loadMotion(arg0, bufSize);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::Live2DMotion((::live2d::Live2DMotion*)__ret);
}

void Live2DSharp::Live2DMotion::dump()
{
    ((::live2d::Live2DMotion*)NativePtr)->dump();
}

void Live2DSharp::Live2DMotion::setParamFadeIn(System::String^ paramID, int value)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(paramID);
    auto arg0 = _arg0.c_str();
    ((::live2d::Live2DMotion*)NativePtr)->setParamFadeIn(arg0, value);
}

void Live2DSharp::Live2DMotion::setParamFadeOut(System::String^ paramID, int value)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(paramID);
    auto arg0 = _arg0.c_str();
    ((::live2d::Live2DMotion*)NativePtr)->setParamFadeOut(arg0, value);
}

int Live2DSharp::Live2DMotion::getParamFadeIn(System::String^ paramID)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(paramID);
    auto arg0 = _arg0.c_str();
    auto __ret = ((::live2d::Live2DMotion*)NativePtr)->getParamFadeIn(arg0);
    return __ret;
}

int Live2DSharp::Live2DMotion::getParamFadeOut(System::String^ paramID)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(paramID);
    auto arg0 = _arg0.c_str();
    auto __ret = ((::live2d::Live2DMotion*)NativePtr)->getParamFadeOut(arg0);
    return __ret;
}

void Live2DSharp::Live2DMotion::Loop::set(bool _loop)
{
    ((::live2d::Live2DMotion*)NativePtr)->setLoop(_loop);
}

void Live2DSharp::Live2DMotion::LoopFadeIn::set(bool _loopFadeIn)
{
    ((::live2d::Live2DMotion*)NativePtr)->setLoopFadeIn(_loopFadeIn);
}

int Live2DSharp::Live2DMotion::DurationMSec::get()
{
    auto __ret = ((::live2d::Live2DMotion*)NativePtr)->getDurationMSec();
    return __ret;
}

int Live2DSharp::Live2DMotion::LoopDurationMSec::get()
{
    auto __ret = ((::live2d::Live2DMotion*)NativePtr)->getLoopDurationMSec();
    return __ret;
}

bool Live2DSharp::Live2DMotion::isLoop::get()
{
    auto __ret = ((::live2d::Live2DMotion*)NativePtr)->isLoop();
    return __ret;
}

bool Live2DSharp::Live2DMotion::isLoopFadeIn::get()
{
    auto __ret = ((::live2d::Live2DMotion*)NativePtr)->isLoopFadeIn();
    return __ret;
}

Live2DSharp::Motion::Motion(::live2d::Motion* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::Motion^ Live2DSharp::Motion::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::Motion((::live2d::Motion*) native.ToPointer());
}

Live2DSharp::Motion::Motion(Live2DSharp::MemoryParam^ memParam)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    NativePtr = new ::live2d::Motion(arg0);
}

int Live2DSharp::Motion::getParamIndex(Live2DSharp::ALive2DModel^ model)
{
    auto arg0 = (::live2d::ALive2DModel*)model->NativePtr;
    auto __ret = ((::live2d::Motion*)NativePtr)->getParamIndex(arg0);
    return __ret;
}

Live2DSharp::LDString^ Live2DSharp::Motion::paramIDStr::get()
{
    return (((::live2d::Motion*)NativePtr)->paramIDStr == nullptr) ? nullptr : gcnew Live2DSharp::LDString((::live2d::LDString*)((::live2d::Motion*)NativePtr)->paramIDStr);
}

void Live2DSharp::Motion::paramIDStr::set(Live2DSharp::LDString^ value)
{
    ((::live2d::Motion*)NativePtr)->paramIDStr = (::live2d::LDString*)value->NativePtr;
}

Live2DSharp::ParamID^ Live2DSharp::Motion::cached_paramID::get()
{
    return (((::live2d::Motion*)NativePtr)->cached_paramID == nullptr) ? nullptr : gcnew Live2DSharp::ParamID((::live2d::ParamID*)((::live2d::Motion*)NativePtr)->cached_paramID);
}

void Live2DSharp::Motion::cached_paramID::set(Live2DSharp::ParamID^ value)
{
    ((::live2d::Motion*)NativePtr)->cached_paramID = (::live2d::ParamID*)value->NativePtr;
}

int Live2DSharp::Motion::cached_paramIndex::get()
{
    return ((::live2d::Motion*)NativePtr)->cached_paramIndex;
}

void Live2DSharp::Motion::cached_paramIndex::set(int value)
{
    ((::live2d::Motion*)NativePtr)->cached_paramIndex = value;
}

Live2DSharp::ALive2DModel^ Live2DSharp::Motion::cached_model_ofParamIndex::get()
{
    return (((::live2d::Motion*)NativePtr)->cached_model_ofParamIndex == nullptr) ? nullptr : gcnew Live2DSharp::ALive2DModel((::live2d::ALive2DModel*)((::live2d::Motion*)NativePtr)->cached_model_ofParamIndex);
}

void Live2DSharp::Motion::cached_model_ofParamIndex::set(Live2DSharp::ALive2DModel^ value)
{
    ((::live2d::Motion*)NativePtr)->cached_model_ofParamIndex = (::live2d::ALive2DModel*)value->NativePtr;
}

int Live2DSharp::Motion::motionType::get()
{
    return ((::live2d::Motion*)NativePtr)->motionType;
}

void Live2DSharp::Motion::motionType::set(int value)
{
    ((::live2d::Motion*)NativePtr)->motionType = value;
}

int Live2DSharp::Motion::fadeInMsec::get()
{
    return ((::live2d::Motion*)NativePtr)->fadeInMsec;
}

void Live2DSharp::Motion::fadeInMsec::set(int value)
{
    ((::live2d::Motion*)NativePtr)->fadeInMsec = value;
}

int Live2DSharp::Motion::fadeOutMsec::get()
{
    return ((::live2d::Motion*)NativePtr)->fadeOutMsec;
}

void Live2DSharp::Motion::fadeOutMsec::set(int value)
{
    ((::live2d::Motion*)NativePtr)->fadeOutMsec = value;
}

int Live2DSharp::Motion::MOTION_TYPE_PARAM::get()
{
    return ::live2d::Motion::MOTION_TYPE_PARAM;
}

int Live2DSharp::Motion::MOTION_TYPE_PARTS_VISIBLE::get()
{
    return ::live2d::Motion::MOTION_TYPE_PARTS_VISIBLE;
}

int Live2DSharp::Motion::MOTION_TYPE_PARAM_FADEIN::get()
{
    return ::live2d::Motion::MOTION_TYPE_PARAM_FADEIN;
}

int Live2DSharp::Motion::MOTION_TYPE_PARAM_FADEOUT::get()
{
    return ::live2d::Motion::MOTION_TYPE_PARAM_FADEOUT;
}

int Live2DSharp::Motion::MOTION_TYPE_LAYOUT_X::get()
{
    return ::live2d::Motion::MOTION_TYPE_LAYOUT_X;
}

int Live2DSharp::Motion::MOTION_TYPE_LAYOUT_Y::get()
{
    return ::live2d::Motion::MOTION_TYPE_LAYOUT_Y;
}

int Live2DSharp::Motion::MOTION_TYPE_LAYOUT_ANCHOR_X::get()
{
    return ::live2d::Motion::MOTION_TYPE_LAYOUT_ANCHOR_X;
}

int Live2DSharp::Motion::MOTION_TYPE_LAYOUT_ANCHOR_Y::get()
{
    return ::live2d::Motion::MOTION_TYPE_LAYOUT_ANCHOR_Y;
}

int Live2DSharp::Motion::MOTION_TYPE_LAYOUT_SCALE_X::get()
{
    return ::live2d::Motion::MOTION_TYPE_LAYOUT_SCALE_X;
}

int Live2DSharp::Motion::MOTION_TYPE_LAYOUT_SCALE_Y::get()
{
    return ::live2d::Motion::MOTION_TYPE_LAYOUT_SCALE_Y;
}

