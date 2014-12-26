#include "motion/clrLive2DMotionBin.h"
#include "./clrALive2DModel.h"
#include "id/clrParamID.h"
#include "memory/clrAMemoryHolder.h"
#include "memory/clrMemoryParam.h"
#include "motion/clrMotionQueueEnt.h"
#include "type/clrLDString.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::Live2DMotionBin::Live2DMotionBin(::live2d::Live2DMotionBin* native)
    : Live2DSharp::AMotion((::live2d::AMotion*)native)
{
}

Live2DSharp::Live2DMotionBin^ Live2DSharp::Live2DMotionBin::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::Live2DMotionBin((::live2d::Live2DMotionBin*) native.ToPointer());
}

Live2DSharp::Live2DMotionBin::Live2DMotionBin()
    : Live2DSharp::AMotion((::live2d::AMotion*)nullptr)
{
    NativePtr = new ::live2d::Live2DMotionBin();
}

Live2DSharp::Live2DMotionBin^ Live2DSharp::Live2DMotionBin::loadMotion(Live2DSharp::LDString^ filepath)
{
    auto &arg0 = *(::live2d::LDString*)filepath->NativePtr;
    auto __ret = ::live2d::Live2DMotionBin::loadMotion(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::Live2DMotionBin((::live2d::Live2DMotionBin*)__ret);
}

Live2DSharp::Live2DMotionBin^ Live2DSharp::Live2DMotionBin::loadMotion(::System::IntPtr buf, int bufSize, Live2DSharp::Live2DMotionBin::BufType bufType)
{
    auto arg0 = (const void*)buf;
    auto arg2 = (::live2d::Live2DMotionBin::BufType)bufType;
    auto __ret = ::live2d::Live2DMotionBin::loadMotion(arg0, bufSize, arg2);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::Live2DMotionBin((::live2d::Live2DMotionBin*)__ret);
}

void Live2DSharp::Live2DMotionBin::dump()
{
    ((::live2d::Live2DMotionBin*)NativePtr)->dump();
}

void Live2DSharp::Live2DMotionBin::Loop::set(bool _loop)
{
    ((::live2d::Live2DMotionBin*)NativePtr)->setLoop(_loop);
}

int Live2DSharp::Live2DMotionBin::DurationMSec::get()
{
    auto __ret = ((::live2d::Live2DMotionBin*)NativePtr)->getDurationMSec();
    return __ret;
}

int Live2DSharp::Live2DMotionBin::LoopDurationMSec::get()
{
    auto __ret = ((::live2d::Live2DMotionBin*)NativePtr)->getLoopDurationMSec();
    return __ret;
}

bool Live2DSharp::Live2DMotionBin::isLoop::get()
{
    auto __ret = ((::live2d::Live2DMotionBin*)NativePtr)->isLoop();
    return __ret;
}

Live2DSharp::MotionBin::MotionBin(::live2d::MotionBin* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::MotionBin^ Live2DSharp::MotionBin::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::MotionBin((::live2d::MotionBin*) native.ToPointer());
}

Live2DSharp::MotionBin::MotionBin()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::MotionBin();
}

int Live2DSharp::MotionBin::getParamIndex(Live2DSharp::ALive2DModel^ model)
{
    auto arg0 = (::live2d::ALive2DModel*)model->NativePtr;
    auto __ret = ((::live2d::MotionBin*)NativePtr)->getParamIndex(arg0);
    return __ret;
}

Live2DSharp::LDString^ Live2DSharp::MotionBin::paramIDStr::get()
{
    return (((::live2d::MotionBin*)NativePtr)->paramIDStr == nullptr) ? nullptr : gcnew Live2DSharp::LDString((::live2d::LDString*)((::live2d::MotionBin*)NativePtr)->paramIDStr);
}

void Live2DSharp::MotionBin::paramIDStr::set(Live2DSharp::LDString^ value)
{
    ((::live2d::MotionBin*)NativePtr)->paramIDStr = (::live2d::LDString*)value->NativePtr;
}

Live2DSharp::ParamID^ Live2DSharp::MotionBin::cached_paramID::get()
{
    return (((::live2d::MotionBin*)NativePtr)->cached_paramID == nullptr) ? nullptr : gcnew Live2DSharp::ParamID((::live2d::ParamID*)((::live2d::MotionBin*)NativePtr)->cached_paramID);
}

void Live2DSharp::MotionBin::cached_paramID::set(Live2DSharp::ParamID^ value)
{
    ((::live2d::MotionBin*)NativePtr)->cached_paramID = (::live2d::ParamID*)value->NativePtr;
}

int Live2DSharp::MotionBin::cached_paramIndex::get()
{
    return ((::live2d::MotionBin*)NativePtr)->cached_paramIndex;
}

void Live2DSharp::MotionBin::cached_paramIndex::set(int value)
{
    ((::live2d::MotionBin*)NativePtr)->cached_paramIndex = value;
}

Live2DSharp::ALive2DModel^ Live2DSharp::MotionBin::cached_model_ofParamIndex::get()
{
    return (((::live2d::MotionBin*)NativePtr)->cached_model_ofParamIndex == nullptr) ? nullptr : gcnew Live2DSharp::ALive2DModel((::live2d::ALive2DModel*)((::live2d::MotionBin*)NativePtr)->cached_model_ofParamIndex);
}

void Live2DSharp::MotionBin::cached_model_ofParamIndex::set(Live2DSharp::ALive2DModel^ value)
{
    ((::live2d::MotionBin*)NativePtr)->cached_model_ofParamIndex = (::live2d::ALive2DModel*)value->NativePtr;
}

bool Live2DSharp::MotionBin::isShortArray::get()
{
    return ((::live2d::MotionBin*)NativePtr)->isShortArray;
}

void Live2DSharp::MotionBin::isShortArray::set(bool value)
{
    ((::live2d::MotionBin*)NativePtr)->isShortArray = value;
}

::System::IntPtr Live2DSharp::MotionBin::valuePtr::get()
{
    return ::System::IntPtr(((::live2d::MotionBin*)NativePtr)->valuePtr);
}

void Live2DSharp::MotionBin::valuePtr::set(::System::IntPtr value)
{
    ((::live2d::MotionBin*)NativePtr)->valuePtr = (void*)value;
}

float Live2DSharp::MotionBin::minValue::get()
{
    return ((::live2d::MotionBin*)NativePtr)->minValue;
}

void Live2DSharp::MotionBin::minValue::set(float value)
{
    ((::live2d::MotionBin*)NativePtr)->minValue = value;
}

float Live2DSharp::MotionBin::maxValue::get()
{
    return ((::live2d::MotionBin*)NativePtr)->maxValue;
}

void Live2DSharp::MotionBin::maxValue::set(float value)
{
    ((::live2d::MotionBin*)NativePtr)->maxValue = value;
}

int Live2DSharp::MotionBin::valueCount::get()
{
    return ((::live2d::MotionBin*)NativePtr)->valueCount;
}

void Live2DSharp::MotionBin::valueCount::set(int value)
{
    ((::live2d::MotionBin*)NativePtr)->valueCount = value;
}

int Live2DSharp::MotionBin::motionType::get()
{
    return ((::live2d::MotionBin*)NativePtr)->motionType;
}

void Live2DSharp::MotionBin::motionType::set(int value)
{
    ((::live2d::MotionBin*)NativePtr)->motionType = value;
}

int Live2DSharp::MotionBin::MOTION_TYPE_PARAM::get()
{
    return ::live2d::MotionBin::MOTION_TYPE_PARAM;
}

int Live2DSharp::MotionBin::MOTION_TYPE_PARTS_VISIBLE::get()
{
    return ::live2d::MotionBin::MOTION_TYPE_PARTS_VISIBLE;
}

int Live2DSharp::MotionBin::MOTION_TYPE_LAYOUT_X::get()
{
    return ::live2d::MotionBin::MOTION_TYPE_LAYOUT_X;
}

int Live2DSharp::MotionBin::MOTION_TYPE_LAYOUT_Y::get()
{
    return ::live2d::MotionBin::MOTION_TYPE_LAYOUT_Y;
}

int Live2DSharp::MotionBin::MOTION_TYPE_LAYOUT_ANCHOR_X::get()
{
    return ::live2d::MotionBin::MOTION_TYPE_LAYOUT_ANCHOR_X;
}

int Live2DSharp::MotionBin::MOTION_TYPE_LAYOUT_ANCHOR_Y::get()
{
    return ::live2d::MotionBin::MOTION_TYPE_LAYOUT_ANCHOR_Y;
}

int Live2DSharp::MotionBin::MOTION_TYPE_LAYOUT_SCALE_X::get()
{
    return ::live2d::MotionBin::MOTION_TYPE_LAYOUT_SCALE_X;
}

int Live2DSharp::MotionBin::MOTION_TYPE_LAYOUT_SCALE_Y::get()
{
    return ::live2d::MotionBin::MOTION_TYPE_LAYOUT_SCALE_Y;
}

