#include "param/clrParamDefFloat.h"
#include "id/clrParamID.h"
#include "io/clrBReader.h"
#include "memory/clrMemoryParam.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::ParamDefFloat::ParamDefFloat(::live2d::ParamDefFloat* native)
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)native)
{
}

Live2DSharp::ParamDefFloat^ Live2DSharp::ParamDefFloat::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::ParamDefFloat((::live2d::ParamDefFloat*) native.ToPointer());
}

Live2DSharp::ParamDefFloat::ParamDefFloat()
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)nullptr)
{
    NativePtr = new ::live2d::ParamDefFloat();
}

void Live2DSharp::ParamDefFloat::readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam)
{
    auto &arg0 = *(::live2d::BReader*)br->NativePtr;
    auto arg1 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::ParamDefFloat*)NativePtr)->readV2(arg0, arg1);
}

float Live2DSharp::ParamDefFloat::MinValue::get()
{
    auto __ret = ((::live2d::ParamDefFloat*)NativePtr)->getMinValue();
    return __ret;
}

float Live2DSharp::ParamDefFloat::MaxValue::get()
{
    auto __ret = ((::live2d::ParamDefFloat*)NativePtr)->getMaxValue();
    return __ret;
}

float Live2DSharp::ParamDefFloat::DefaultValue::get()
{
    auto __ret = ((::live2d::ParamDefFloat*)NativePtr)->getDefaultValue();
    return __ret;
}

Live2DSharp::ParamID^ Live2DSharp::ParamDefFloat::ParamID::get()
{
    auto __ret = ((::live2d::ParamDefFloat*)NativePtr)->getParamID();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::ParamID((::live2d::ParamID*)__ret);
}

