#include "model/clrModelImpl.h"
#include "base/clrIBaseData.h"
#include "io/clrBReader.h"
#include "memory/clrAMemoryHolder.h"
#include "memory/clrMemoryParam.h"
#include "model/clrPartsData.h"
#include "param/clrParamDefSet.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::ModelImpl::ModelImpl(::live2d::ModelImpl* native)
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)native)
{
}

Live2DSharp::ModelImpl^ Live2DSharp::ModelImpl::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::ModelImpl((::live2d::ModelImpl*) native.ToPointer());
}

Live2DSharp::ModelImpl::ModelImpl()
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)nullptr)
{
    NativePtr = new ::live2d::ModelImpl();
}

void Live2DSharp::ModelImpl::initDirect()
{
    ((::live2d::ModelImpl*)NativePtr)->initDirect();
}

void Live2DSharp::ModelImpl::readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam)
{
    auto &arg0 = *(::live2d::BReader*)br->NativePtr;
    auto arg1 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::ModelImpl*)NativePtr)->readV2(arg0, arg1);
}

void Live2DSharp::ModelImpl::addPartsData(Live2DSharp::PartsData^ parts)
{
    auto arg0 = (::live2d::PartsData*)parts->NativePtr;
    ((::live2d::ModelImpl*)NativePtr)->addPartsData(arg0);
}

Live2DSharp::ParamDefSet^ Live2DSharp::ModelImpl::ParamDefSet::get()
{
    auto __ret = ((::live2d::ModelImpl*)NativePtr)->getParamDefSet();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::ParamDefSet((::live2d::ParamDefSet*)__ret);
}

float Live2DSharp::ModelImpl::CanvasWidth::get()
{
    auto __ret = ((::live2d::ModelImpl*)NativePtr)->getCanvasWidth();
    return __ret;
}

float Live2DSharp::ModelImpl::CanvasHeight::get()
{
    auto __ret = ((::live2d::ModelImpl*)NativePtr)->getCanvasHeight();
    return __ret;
}

int Live2DSharp::ModelImpl::INSTANCE_COUNT::get()
{
    return ::live2d::ModelImpl::INSTANCE_COUNT;
}

void Live2DSharp::ModelImpl::INSTANCE_COUNT::set(int value)
{
    ::live2d::ModelImpl::INSTANCE_COUNT = value;
}

