#include "./clrPriorityMotionQueueManager.h"
#include "./clrALive2DModel.h"
#include "motion/clrAMotion.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::Framework::PriorityMotionQueueManager::PriorityMotionQueueManager(::live2d::framework::PriorityMotionQueueManager* native)
    : Live2DSharp::MotionQueueManager((::live2d::MotionQueueManager*)native)
{
}

Live2DSharp::Framework::PriorityMotionQueueManager^ Live2DSharp::Framework::PriorityMotionQueueManager::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::Framework::PriorityMotionQueueManager((::live2d::framework::PriorityMotionQueueManager*) native.ToPointer());
}

Live2DSharp::Framework::PriorityMotionQueueManager::PriorityMotionQueueManager()
    : Live2DSharp::MotionQueueManager((::live2d::MotionQueueManager*)nullptr)
{
    NativePtr = new ::live2d::framework::PriorityMotionQueueManager();
}

int Live2DSharp::Framework::PriorityMotionQueueManager::startMotionPriority(Live2DSharp::AMotion^ motion, bool isDelete, int priority)
{
    auto arg0 = (::live2d::AMotion*)motion->NativePtr;
    auto __ret = ((::live2d::framework::PriorityMotionQueueManager*)NativePtr)->startMotionPriority(arg0, isDelete, priority);
    return __ret;
}

bool Live2DSharp::Framework::PriorityMotionQueueManager::updateParam(Live2DSharp::ALive2DModel^ model)
{
    auto arg0 = (::live2d::ALive2DModel*)model->NativePtr;
    auto __ret = ((::live2d::framework::PriorityMotionQueueManager*)NativePtr)->updateParam(arg0);
    return __ret;
}

bool Live2DSharp::Framework::PriorityMotionQueueManager::reserveMotion(int priority)
{
    auto __ret = ((::live2d::framework::PriorityMotionQueueManager*)NativePtr)->reserveMotion(priority);
    return __ret;
}

int Live2DSharp::Framework::PriorityMotionQueueManager::CurrentPriority::get()
{
    auto __ret = ((::live2d::framework::PriorityMotionQueueManager*)NativePtr)->getCurrentPriority();
    return __ret;
}

int Live2DSharp::Framework::PriorityMotionQueueManager::ReservePriority::get()
{
    auto __ret = ((::live2d::framework::PriorityMotionQueueManager*)NativePtr)->getReservePriority();
    return __ret;
}

void Live2DSharp::Framework::PriorityMotionQueueManager::ReservePriority::set(int val)
{
    ((::live2d::framework::PriorityMotionQueueManager*)NativePtr)->setReservePriority(val);
}

