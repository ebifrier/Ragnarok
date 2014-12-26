#include "physics/clrPhysicsHair.h"
#include "./clrALive2DModel.h"
#include "memory/clrAMemoryHolder.h"
#include "memory/clrMemoryParam.h"
#include "physics/clrPhysicsParams.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::PhysicsPoint::PhysicsPoint(::live2d::PhysicsPoint* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::PhysicsPoint^ Live2DSharp::PhysicsPoint::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::PhysicsPoint((::live2d::PhysicsPoint*) native.ToPointer());
}

Live2DSharp::PhysicsPoint::PhysicsPoint()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::PhysicsPoint();
}

void Live2DSharp::PhysicsPoint::setupLast()
{
    ((::live2d::PhysicsPoint*)NativePtr)->setupLast();
}

float Live2DSharp::PhysicsPoint::mass::get()
{
    return ((::live2d::PhysicsPoint*)NativePtr)->mass;
}

void Live2DSharp::PhysicsPoint::mass::set(float value)
{
    ((::live2d::PhysicsPoint*)NativePtr)->mass = value;
}

float Live2DSharp::PhysicsPoint::x::get()
{
    return ((::live2d::PhysicsPoint*)NativePtr)->x;
}

void Live2DSharp::PhysicsPoint::x::set(float value)
{
    ((::live2d::PhysicsPoint*)NativePtr)->x = value;
}

float Live2DSharp::PhysicsPoint::y::get()
{
    return ((::live2d::PhysicsPoint*)NativePtr)->y;
}

void Live2DSharp::PhysicsPoint::y::set(float value)
{
    ((::live2d::PhysicsPoint*)NativePtr)->y = value;
}

float Live2DSharp::PhysicsPoint::vx::get()
{
    return ((::live2d::PhysicsPoint*)NativePtr)->vx;
}

void Live2DSharp::PhysicsPoint::vx::set(float value)
{
    ((::live2d::PhysicsPoint*)NativePtr)->vx = value;
}

float Live2DSharp::PhysicsPoint::vy::get()
{
    return ((::live2d::PhysicsPoint*)NativePtr)->vy;
}

void Live2DSharp::PhysicsPoint::vy::set(float value)
{
    ((::live2d::PhysicsPoint*)NativePtr)->vy = value;
}

float Live2DSharp::PhysicsPoint::ax::get()
{
    return ((::live2d::PhysicsPoint*)NativePtr)->ax;
}

void Live2DSharp::PhysicsPoint::ax::set(float value)
{
    ((::live2d::PhysicsPoint*)NativePtr)->ax = value;
}

float Live2DSharp::PhysicsPoint::ay::get()
{
    return ((::live2d::PhysicsPoint*)NativePtr)->ay;
}

void Live2DSharp::PhysicsPoint::ay::set(float value)
{
    ((::live2d::PhysicsPoint*)NativePtr)->ay = value;
}

float Live2DSharp::PhysicsPoint::fx::get()
{
    return ((::live2d::PhysicsPoint*)NativePtr)->fx;
}

void Live2DSharp::PhysicsPoint::fx::set(float value)
{
    ((::live2d::PhysicsPoint*)NativePtr)->fx = value;
}

float Live2DSharp::PhysicsPoint::fy::get()
{
    return ((::live2d::PhysicsPoint*)NativePtr)->fy;
}

void Live2DSharp::PhysicsPoint::fy::set(float value)
{
    ((::live2d::PhysicsPoint*)NativePtr)->fy = value;
}

float Live2DSharp::PhysicsPoint::last_x::get()
{
    return ((::live2d::PhysicsPoint*)NativePtr)->last_x;
}

void Live2DSharp::PhysicsPoint::last_x::set(float value)
{
    ((::live2d::PhysicsPoint*)NativePtr)->last_x = value;
}

float Live2DSharp::PhysicsPoint::last_y::get()
{
    return ((::live2d::PhysicsPoint*)NativePtr)->last_y;
}

void Live2DSharp::PhysicsPoint::last_y::set(float value)
{
    ((::live2d::PhysicsPoint*)NativePtr)->last_y = value;
}

float Live2DSharp::PhysicsPoint::last_vx::get()
{
    return ((::live2d::PhysicsPoint*)NativePtr)->last_vx;
}

void Live2DSharp::PhysicsPoint::last_vx::set(float value)
{
    ((::live2d::PhysicsPoint*)NativePtr)->last_vx = value;
}

float Live2DSharp::PhysicsPoint::last_vy::get()
{
    return ((::live2d::PhysicsPoint*)NativePtr)->last_vy;
}

void Live2DSharp::PhysicsPoint::last_vy::set(float value)
{
    ((::live2d::PhysicsPoint*)NativePtr)->last_vy = value;
}

Live2DSharp::PhysicsHair::PhysicsHair(::live2d::PhysicsHair* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::PhysicsHair^ Live2DSharp::PhysicsHair::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::PhysicsHair((::live2d::PhysicsHair*) native.ToPointer());
}

Live2DSharp::PhysicsHair::PhysicsHair()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::PhysicsHair();
}

Live2DSharp::PhysicsHair::PhysicsHair(float baseLengthM, float airRegistance, float mass)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::PhysicsHair(baseLengthM, airRegistance, mass);
}

void Live2DSharp::PhysicsHair::setup(float baseLengthM, float airRegistance, float mass)
{
    ((::live2d::PhysicsHair*)NativePtr)->setup(baseLengthM, airRegistance, mass);
}

void Live2DSharp::PhysicsHair::setup()
{
    ((::live2d::PhysicsHair*)NativePtr)->setup();
}

void Live2DSharp::PhysicsHair::addSrcParam(Live2DSharp::PhysicsHair::Src srcType, System::String^ paramID, float scale, float weight)
{
    auto arg0 = (::live2d::PhysicsHair::Src)srcType;
    auto _arg1 = clix::marshalString<clix::E_UTF8>(paramID);
    auto arg1 = _arg1.c_str();
    ((::live2d::PhysicsHair*)NativePtr)->addSrcParam(arg0, arg1, scale, weight);
}

void Live2DSharp::PhysicsHair::addTargetParam(Live2DSharp::PhysicsHair::Target targetType, System::String^ paramID, float scale, float weight)
{
    auto arg0 = (::live2d::PhysicsHair::Target)targetType;
    auto _arg1 = clix::marshalString<clix::E_UTF8>(paramID);
    auto arg1 = _arg1.c_str();
    ((::live2d::PhysicsHair*)NativePtr)->addTargetParam(arg0, arg1, scale, weight);
}

void Live2DSharp::PhysicsHair::update(Live2DSharp::ALive2DModel^ model, long long time)
{
    auto arg0 = (::live2d::ALive2DModel*)model->NativePtr;
    ((::live2d::PhysicsHair*)NativePtr)->update(arg0, time);
}

Live2DSharp::PhysicsPoint^ Live2DSharp::PhysicsHair::PhysicsPoint1::get()
{
    auto &__ret = ((::live2d::PhysicsHair*)NativePtr)->getPhysicsPoint1();
    return (Live2DSharp::PhysicsPoint^)((&__ret == nullptr) ? nullptr : gcnew Live2DSharp::PhysicsPoint((::live2d::PhysicsPoint*)&__ret));
}

Live2DSharp::PhysicsPoint^ Live2DSharp::PhysicsHair::PhysicsPoint2::get()
{
    auto &__ret = ((::live2d::PhysicsHair*)NativePtr)->getPhysicsPoint2();
    return (Live2DSharp::PhysicsPoint^)((&__ret == nullptr) ? nullptr : gcnew Live2DSharp::PhysicsPoint((::live2d::PhysicsPoint*)&__ret));
}

float Live2DSharp::PhysicsHair::GravityAngleDeg::get()
{
    auto __ret = ((::live2d::PhysicsHair*)NativePtr)->getGravityAngleDeg();
    return __ret;
}

void Live2DSharp::PhysicsHair::GravityAngleDeg::set(float angleDeg)
{
    ((::live2d::PhysicsHair*)NativePtr)->setGravityAngleDeg(angleDeg);
}

float Live2DSharp::PhysicsHair::AngleP1toP2Deg::get()
{
    auto __ret = ((::live2d::PhysicsHair*)NativePtr)->getAngleP1toP2Deg();
    return __ret;
}

float Live2DSharp::PhysicsHair::AngleP1toP2Deg_velocity::get()
{
    auto __ret = ((::live2d::PhysicsHair*)NativePtr)->getAngleP1toP2Deg_velocity();
    return __ret;
}

