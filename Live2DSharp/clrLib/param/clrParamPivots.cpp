#include "param/clrParamPivots.h"
#include "id/clrParamID.h"
#include "io/clrBReader.h"
#include "memory/clrMemoryParam.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::ParamPivots::ParamPivots(::live2d::ParamPivots* native)
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)native)
{
}

Live2DSharp::ParamPivots^ Live2DSharp::ParamPivots::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::ParamPivots((::live2d::ParamPivots*) native.ToPointer());
}

Live2DSharp::ParamPivots::ParamPivots()
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)nullptr)
{
    NativePtr = new ::live2d::ParamPivots();
}

void Live2DSharp::ParamPivots::readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam)
{
    auto &arg0 = *(::live2d::BReader*)br->NativePtr;
    auto arg1 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::ParamPivots*)NativePtr)->readV2(arg0, arg1);
}

int Live2DSharp::ParamPivots::getParamIndex(int initVersion)
{
    auto __ret = ((::live2d::ParamPivots*)NativePtr)->getParamIndex(initVersion);
    return __ret;
}

void Live2DSharp::ParamPivots::setParamIndex_(int index, int initVersion)
{
    ((::live2d::ParamPivots*)NativePtr)->setParamIndex_(index, initVersion);
}

void Live2DSharp::ParamPivots::setPivotValue(int _pivotCount, float* _values)
{
    auto arg1 = (::l2d_paramf*)_values;
    ((::live2d::ParamPivots*)NativePtr)->setPivotValue(_pivotCount, arg1);
}

Live2DSharp::ParamID^ Live2DSharp::ParamPivots::ParamID::get()
{
    auto __ret = ((::live2d::ParamPivots*)NativePtr)->getParamID();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::ParamID((::live2d::ParamID*)__ret);
}

void Live2DSharp::ParamPivots::ParamID::set(Live2DSharp::ParamID^ v)
{
    auto arg0 = (::live2d::ParamID*)v->NativePtr;
    ((::live2d::ParamPivots*)NativePtr)->setParamID(arg0);
}

int Live2DSharp::ParamPivots::PivotCount::get()
{
    auto __ret = ((::live2d::ParamPivots*)NativePtr)->getPivotCount();
    return __ret;
}

float* Live2DSharp::ParamPivots::PivotValue::get()
{
    auto __ret = ((::live2d::ParamPivots*)NativePtr)->getPivotValue();
    return reinterpret_cast<float*>(__ret);
}

int Live2DSharp::ParamPivots::TmpPivotIndex::get()
{
    auto __ret = ((::live2d::ParamPivots*)NativePtr)->getTmpPivotIndex();
    return __ret;
}

void Live2DSharp::ParamPivots::TmpPivotIndex::set(int v)
{
    ((::live2d::ParamPivots*)NativePtr)->setTmpPivotIndex(v);
}

float Live2DSharp::ParamPivots::TmpT::get()
{
    auto __ret = ((::live2d::ParamPivots*)NativePtr)->getTmpT();
    return __ret;
}

void Live2DSharp::ParamPivots::TmpT::set(float t)
{
    ((::live2d::ParamPivots*)NativePtr)->setTmpT(t);
}

int Live2DSharp::ParamPivots::PARAM_INDEX_NOT_INIT::get()
{
    return ::live2d::ParamPivots::PARAM_INDEX_NOT_INIT;
}

