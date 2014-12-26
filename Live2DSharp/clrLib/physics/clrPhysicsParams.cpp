#include "physics/clrPhysicsParams.h"
#include "./clrALive2DModel.h"
#include "physics/clrPhysicsHair.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::IPhysicsSrc::IPhysicsSrc(::live2d::IPhysicsSrc* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::IPhysicsSrc^ Live2DSharp::IPhysicsSrc::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::IPhysicsSrc((::live2d::IPhysicsSrc*) native.ToPointer());
}

Live2DSharp::IPhysicsSrc::IPhysicsSrc(System::String^ _paramID, float _scale, float _weight)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(_paramID);
    auto arg0 = _arg0.c_str();
    NativePtr = new ::live2d::IPhysicsSrc(arg0, _scale, _weight);
}

void Live2DSharp::IPhysicsSrc::updateSrc(Live2DSharp::ALive2DModel^ model, Live2DSharp::PhysicsHair^ hair)
{
    auto arg0 = (::live2d::ALive2DModel*)model->NativePtr;
    auto &arg1 = *(::live2d::PhysicsHair*)hair->NativePtr;
    ((::live2d::IPhysicsSrc*)NativePtr)->updateSrc(arg0, arg1);
}

Live2DSharp::PhysicsSrc::PhysicsSrc(::live2d::PhysicsSrc* native)
    : Live2DSharp::IPhysicsSrc((::live2d::IPhysicsSrc*)native)
{
}

Live2DSharp::PhysicsSrc^ Live2DSharp::PhysicsSrc::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::PhysicsSrc((::live2d::PhysicsSrc*) native.ToPointer());
}

Live2DSharp::PhysicsSrc::PhysicsSrc(Live2DSharp::PhysicsHair::Src srcType, System::String^ _paramID, float _scale, float _weight)
    : Live2DSharp::IPhysicsSrc((::live2d::IPhysicsSrc*)nullptr)
{
    auto arg0 = (::live2d::PhysicsHair::Src)srcType;
    auto _arg1 = clix::marshalString<clix::E_UTF8>(_paramID);
    auto arg1 = _arg1.c_str();
    NativePtr = new ::live2d::PhysicsSrc(arg0, arg1, _scale, _weight);
}

void Live2DSharp::PhysicsSrc::updateSrc(Live2DSharp::ALive2DModel^ model, Live2DSharp::PhysicsHair^ hair)
{
    auto arg0 = (::live2d::ALive2DModel*)model->NativePtr;
    auto &arg1 = *(::live2d::PhysicsHair*)hair->NativePtr;
    ((::live2d::PhysicsSrc*)NativePtr)->updateSrc(arg0, arg1);
}

Live2DSharp::IPhysicsTarget::IPhysicsTarget(::live2d::IPhysicsTarget* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::IPhysicsTarget^ Live2DSharp::IPhysicsTarget::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::IPhysicsTarget((::live2d::IPhysicsTarget*) native.ToPointer());
}

Live2DSharp::IPhysicsTarget::IPhysicsTarget(System::String^ _paramID, float _scale, float _weight)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(_paramID);
    auto arg0 = _arg0.c_str();
    NativePtr = new ::live2d::IPhysicsTarget(arg0, _scale, _weight);
}

void Live2DSharp::IPhysicsTarget::updateTarget(Live2DSharp::ALive2DModel^ model, Live2DSharp::PhysicsHair^ hair)
{
    auto arg0 = (::live2d::ALive2DModel*)model->NativePtr;
    auto &arg1 = *(::live2d::PhysicsHair*)hair->NativePtr;
    ((::live2d::IPhysicsTarget*)NativePtr)->updateTarget(arg0, arg1);
}

Live2DSharp::PhysicsTarget::PhysicsTarget(::live2d::PhysicsTarget* native)
    : Live2DSharp::IPhysicsTarget((::live2d::IPhysicsTarget*)native)
{
}

Live2DSharp::PhysicsTarget^ Live2DSharp::PhysicsTarget::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::PhysicsTarget((::live2d::PhysicsTarget*) native.ToPointer());
}

Live2DSharp::PhysicsTarget::PhysicsTarget(Live2DSharp::PhysicsHair::Target targetType, System::String^ _paramID, float _scale, float _weight)
    : Live2DSharp::IPhysicsTarget((::live2d::IPhysicsTarget*)nullptr)
{
    auto arg0 = (::live2d::PhysicsHair::Target)targetType;
    auto _arg1 = clix::marshalString<clix::E_UTF8>(_paramID);
    auto arg1 = _arg1.c_str();
    NativePtr = new ::live2d::PhysicsTarget(arg0, arg1, _scale, _weight);
}

void Live2DSharp::PhysicsTarget::updateTarget(Live2DSharp::ALive2DModel^ model, Live2DSharp::PhysicsHair^ hair)
{
    auto arg0 = (::live2d::ALive2DModel*)model->NativePtr;
    auto &arg1 = *(::live2d::PhysicsHair*)hair->NativePtr;
    ((::live2d::PhysicsTarget*)NativePtr)->updateTarget(arg0, arg1);
}

