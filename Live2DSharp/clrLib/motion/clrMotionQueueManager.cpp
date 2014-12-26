#include "motion/clrMotionQueueManager.h"
#include "./clrALive2DModel.h"
#include "memory/clrAMemoryHolder.h"
#include "memory/clrMemoryParam.h"
#include "motion/clrAMotion.h"
#include "motion/clrLive2DMotion.h"
#include "motion/clrMotionQueueEnt.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::MotionQueueManager::MotionQueueManager(::live2d::MotionQueueManager* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::MotionQueueManager^ Live2DSharp::MotionQueueManager::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::MotionQueueManager((::live2d::MotionQueueManager*) native.ToPointer());
}

Live2DSharp::MotionQueueManager::MotionQueueManager()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::MotionQueueManager();
}

int Live2DSharp::MotionQueueManager::startMotion(Live2DSharp::AMotion^ motion, bool autoDelete)
{
    auto arg0 = (::live2d::AMotion*)motion->NativePtr;
    auto __ret = ((::live2d::MotionQueueManager*)NativePtr)->startMotion(arg0, autoDelete);
    return __ret;
}

bool Live2DSharp::MotionQueueManager::updateParam(Live2DSharp::ALive2DModel^ model)
{
    auto arg0 = (::live2d::ALive2DModel*)model->NativePtr;
    auto __ret = ((::live2d::MotionQueueManager*)NativePtr)->updateParam(arg0);
    return __ret;
}

bool Live2DSharp::MotionQueueManager::isFinished()
{
    auto __ret = ((::live2d::MotionQueueManager*)NativePtr)->isFinished();
    return __ret;
}

bool Live2DSharp::MotionQueueManager::isFinished(int motionQueueEntNo)
{
    auto __ret = ((::live2d::MotionQueueManager*)NativePtr)->isFinished(motionQueueEntNo);
    return __ret;
}

void Live2DSharp::MotionQueueManager::stopAllMotions()
{
    ((::live2d::MotionQueueManager*)NativePtr)->stopAllMotions();
}

Live2DSharp::MotionQueueEnt^ Live2DSharp::MotionQueueManager::getMotionQueueEnt(int entNo)
{
    auto __ret = ((::live2d::MotionQueueManager*)NativePtr)->getMotionQueueEnt(entNo);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::MotionQueueEnt((::live2d::MotionQueueEnt*)__ret);
}

void Live2DSharp::MotionQueueManager::DUMP()
{
    ((::live2d::MotionQueueManager*)NativePtr)->DUMP();
}

void Live2DSharp::MotionQueueManager::MotionDebugMode::set(bool f)
{
    ((::live2d::MotionQueueManager*)NativePtr)->setMotionDebugMode(f);
}

