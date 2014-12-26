#include "model/clrPartsData.h"
#include "./clrModelContext.h"
#include "base/clrIBaseData.h"
#include "draw/clrIDrawData.h"
#include "id/clrPartsDataID.h"
#include "io/clrBReader.h"
#include "memory/clrMemoryParam.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::PartsData::PartsData(::live2d::PartsData* native)
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)native)
{
}

Live2DSharp::PartsData^ Live2DSharp::PartsData::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::PartsData((::live2d::PartsData*) native.ToPointer());
}

Live2DSharp::PartsData::PartsData()
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)nullptr)
{
    NativePtr = new ::live2d::PartsData();
}

void Live2DSharp::PartsData::initDirect(Live2DSharp::MemoryParam^ memParam)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::PartsData*)NativePtr)->initDirect(arg0);
}

Live2DSharp::PartsDataContext^ Live2DSharp::PartsData::init(Live2DSharp::ModelContext^ mdc)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto __ret = ((::live2d::PartsData*)NativePtr)->init(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::PartsDataContext((::live2d::PartsDataContext*)__ret);
}

void Live2DSharp::PartsData::addBaseData(Live2DSharp::IBaseData^ baseData)
{
    auto arg0 = (::live2d::IBaseData*)baseData->NativePtr;
    ((::live2d::PartsData*)NativePtr)->addBaseData(arg0);
}

void Live2DSharp::PartsData::addDrawData(Live2DSharp::IDrawData^ drawData)
{
    auto arg0 = (::live2d::IDrawData*)drawData->NativePtr;
    ((::live2d::PartsData*)NativePtr)->addDrawData(arg0);
}

void Live2DSharp::PartsData::readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam)
{
    auto &arg0 = *(::live2d::BReader*)br->NativePtr;
    auto arg1 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::PartsData*)NativePtr)->readV2(arg0, arg1);
}

void Live2DSharp::PartsData::Visible::set(bool v)
{
    ((::live2d::PartsData*)NativePtr)->setVisible(v);
}

void Live2DSharp::PartsData::Locked::set(bool v)
{
    ((::live2d::PartsData*)NativePtr)->setLocked(v);
}

Live2DSharp::PartsDataID^ Live2DSharp::PartsData::PartsDataID::get()
{
    auto __ret = ((::live2d::PartsData*)NativePtr)->getPartsDataID();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::PartsDataID((::live2d::PartsDataID*)__ret);
}

void Live2DSharp::PartsData::PartsDataID::set(Live2DSharp::PartsDataID^ id)
{
    auto arg0 = (::live2d::PartsDataID*)id->NativePtr;
    ((::live2d::PartsData*)NativePtr)->setPartsDataID(arg0);
}

Live2DSharp::PartsDataID^ Live2DSharp::PartsData::PartsID::get()
{
    auto __ret = ((::live2d::PartsData*)NativePtr)->getPartsID();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::PartsDataID((::live2d::PartsDataID*)__ret);
}

void Live2DSharp::PartsData::PartsID::set(Live2DSharp::PartsDataID^ id)
{
    auto arg0 = (::live2d::PartsDataID*)id->NativePtr;
    ((::live2d::PartsData*)NativePtr)->setPartsID(arg0);
}

bool Live2DSharp::PartsData::isVisible::get()
{
    auto __ret = ((::live2d::PartsData*)NativePtr)->isVisible();
    return __ret;
}

bool Live2DSharp::PartsData::isLocked::get()
{
    auto __ret = ((::live2d::PartsData*)NativePtr)->isLocked();
    return __ret;
}

int Live2DSharp::PartsData::INSTANCE_COUNT::get()
{
    return ::live2d::PartsData::INSTANCE_COUNT;
}

void Live2DSharp::PartsData::INSTANCE_COUNT::set(int value)
{
    ::live2d::PartsData::INSTANCE_COUNT = value;
}

Live2DSharp::PartsDataContext::PartsDataContext(::live2d::PartsDataContext* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::PartsDataContext^ Live2DSharp::PartsDataContext::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::PartsDataContext((::live2d::PartsDataContext*) native.ToPointer());
}

Live2DSharp::PartsDataContext::PartsDataContext(Live2DSharp::PartsData^ src)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    auto arg0 = (::live2d::PartsData*)src->NativePtr;
    NativePtr = new ::live2d::PartsDataContext(arg0);
}

float Live2DSharp::PartsDataContext::PartsOpacity::get()
{
    auto __ret = ((::live2d::PartsDataContext*)NativePtr)->getPartsOpacity();
    return __ret;
}

void Live2DSharp::PartsDataContext::PartsOpacity::set(float v)
{
    ((::live2d::PartsDataContext*)NativePtr)->setPartsOpacity(v);
}

