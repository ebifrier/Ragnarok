#include "base/private/clrAffineEnt.h"
#include "io/clrBReader.h"
#include "memory/clrMemoryParam.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::AffineEnt::AffineEnt(::live2d::AffineEnt* native)
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)native)
{
}

Live2DSharp::AffineEnt^ Live2DSharp::AffineEnt::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::AffineEnt((::live2d::AffineEnt*) native.ToPointer());
}

Live2DSharp::AffineEnt::AffineEnt()
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)nullptr)
{
    NativePtr = new ::live2d::AffineEnt();
}

void Live2DSharp::AffineEnt::init(Live2DSharp::AffineEnt^ ent)
{
    auto &arg0 = *(::live2d::AffineEnt*)ent->NativePtr;
    ((::live2d::AffineEnt*)NativePtr)->init(arg0);
}

void Live2DSharp::AffineEnt::readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam)
{
    auto &arg0 = *(::live2d::BReader*)br->NativePtr;
    auto arg1 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::AffineEnt*)NativePtr)->readV2(arg0, arg1);
}

void Live2DSharp::AffineEnt::DUMP()
{
    ((::live2d::AffineEnt*)NativePtr)->DUMP();
}

float Live2DSharp::AffineEnt::originX::get()
{
    return ((::live2d::AffineEnt*)NativePtr)->originX;
}

void Live2DSharp::AffineEnt::originX::set(float value)
{
    ((::live2d::AffineEnt*)NativePtr)->originX = value;
}

float Live2DSharp::AffineEnt::originY::get()
{
    return ((::live2d::AffineEnt*)NativePtr)->originY;
}

void Live2DSharp::AffineEnt::originY::set(float value)
{
    ((::live2d::AffineEnt*)NativePtr)->originY = value;
}

float Live2DSharp::AffineEnt::scaleX::get()
{
    return ((::live2d::AffineEnt*)NativePtr)->scaleX;
}

void Live2DSharp::AffineEnt::scaleX::set(float value)
{
    ((::live2d::AffineEnt*)NativePtr)->scaleX = value;
}

float Live2DSharp::AffineEnt::scaleY::get()
{
    return ((::live2d::AffineEnt*)NativePtr)->scaleY;
}

void Live2DSharp::AffineEnt::scaleY::set(float value)
{
    ((::live2d::AffineEnt*)NativePtr)->scaleY = value;
}

float Live2DSharp::AffineEnt::rotateDeg::get()
{
    return ((::live2d::AffineEnt*)NativePtr)->rotateDeg;
}

void Live2DSharp::AffineEnt::rotateDeg::set(float value)
{
    ((::live2d::AffineEnt*)NativePtr)->rotateDeg = value;
}

bool Live2DSharp::AffineEnt::reflectX::get()
{
    return ((::live2d::AffineEnt*)NativePtr)->reflectX;
}

void Live2DSharp::AffineEnt::reflectX::set(bool value)
{
    ((::live2d::AffineEnt*)NativePtr)->reflectX = value;
}

bool Live2DSharp::AffineEnt::reflectY::get()
{
    return ((::live2d::AffineEnt*)NativePtr)->reflectY;
}

void Live2DSharp::AffineEnt::reflectY::set(bool value)
{
    ((::live2d::AffineEnt*)NativePtr)->reflectY = value;
}

