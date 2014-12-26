#include "draw/clrIDrawData.h"
#include "./clrModelContext.h"
#include "draw/clrIDrawContext.h"
#include "graphics/clrDrawParam.h"
#include "id/clrBaseDataID.h"
#include "id/clrDrawDataID.h"
#include "io/clrBReader.h"
#include "memory/clrMemoryParam.h"
#include "param/clrPivotManager.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::IDrawData::IDrawData(::live2d::IDrawData* native)
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)native)
{
}

Live2DSharp::IDrawData^ Live2DSharp::IDrawData::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::IDrawData((::live2d::IDrawData*) native.ToPointer());
}

Live2DSharp::IDrawData::IDrawData()
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)nullptr)
{
}

void Live2DSharp::IDrawData::readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam)
{
    auto &arg0 = *(::live2d::BReader*)br->NativePtr;
    auto arg1 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::IDrawData*)NativePtr)->readV2(arg0, arg1);
}

float Live2DSharp::IDrawData::getOpacity(Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ cdata)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg1 = (::live2d::IDrawContext*)cdata->NativePtr;
    auto __ret = ((::live2d::IDrawData*)NativePtr)->getOpacity(arg0, arg1);
    return __ret;
}

int Live2DSharp::IDrawData::getDrawOrder(Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ cdata)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg1 = (::live2d::IDrawContext*)cdata->NativePtr;
    auto __ret = ((::live2d::IDrawData*)NativePtr)->getDrawOrder(arg0, arg1);
    return __ret;
}

Live2DSharp::IDrawContext^ Live2DSharp::IDrawData::init(Live2DSharp::ModelContext^ mdc)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto __ret = ((::live2d::IDrawData*)NativePtr)->init(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::IDrawContext((::live2d::IDrawContext*)__ret);
}

void Live2DSharp::IDrawData::setupInterpolate(Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ cdata)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg1 = (::live2d::IDrawContext*)cdata->NativePtr;
    ((::live2d::IDrawData*)NativePtr)->setupInterpolate(arg0, arg1);
}

void Live2DSharp::IDrawData::setupTransform(Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ cdata)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg1 = (::live2d::IDrawContext*)cdata->NativePtr;
    ((::live2d::IDrawData*)NativePtr)->setupTransform(arg0, arg1);
}

void Live2DSharp::IDrawData::draw(Live2DSharp::DrawParam^ dp, Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ cdata)
{
    auto &arg0 = *(::live2d::DrawParam*)dp->NativePtr;
    auto &arg1 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg2 = (::live2d::IDrawContext*)cdata->NativePtr;
    ((::live2d::IDrawData*)NativePtr)->draw(arg0, arg1, arg2);
}

void Live2DSharp::IDrawData::deviceLost(Live2DSharp::IDrawContext^ drawContext)
{
    auto arg0 = (::live2d::IDrawContext*)drawContext->NativePtr;
    ((::live2d::IDrawData*)NativePtr)->deviceLost(arg0);
}

void Live2DSharp::IDrawData::setZ_TestImpl(Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ _cdata, float z)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg1 = (::live2d::IDrawContext*)_cdata->NativePtr;
    ((::live2d::IDrawData*)NativePtr)->setZ_TestImpl(arg0, arg1, z);
}

Live2DSharp::BaseDataID^ Live2DSharp::IDrawData::TargetBaseDataID::get()
{
    auto __ret = ((::live2d::IDrawData*)NativePtr)->getTargetBaseDataID();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::BaseDataID((::live2d::BaseDataID*)__ret);
}

void Live2DSharp::IDrawData::TargetBaseDataID::set(Live2DSharp::BaseDataID^ id)
{
    auto arg0 = (::live2d::BaseDataID*)id->NativePtr;
    ((::live2d::IDrawData*)NativePtr)->setTargetBaseDataID(arg0);
}

Live2DSharp::DrawDataID^ Live2DSharp::IDrawData::DrawDataID::get()
{
    auto __ret = ((::live2d::IDrawData*)NativePtr)->getDrawDataID();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::DrawDataID((::live2d::DrawDataID*)__ret);
}

void Live2DSharp::IDrawData::DrawDataID::set(Live2DSharp::DrawDataID^ id)
{
    auto arg0 = (::live2d::DrawDataID*)id->NativePtr;
    ((::live2d::IDrawData*)NativePtr)->setDrawDataID(arg0);
}

int Live2DSharp::IDrawData::TotalMinOrder::get()
{
    auto __ret = ::live2d::IDrawData::getTotalMinOrder();
    return __ret;
}

int Live2DSharp::IDrawData::TotalMaxOrder::get()
{
    auto __ret = ::live2d::IDrawData::getTotalMaxOrder();
    return __ret;
}

int Live2DSharp::IDrawData::Type::get()
{
    auto __ret = ((::live2d::IDrawData*)NativePtr)->getType();
    return __ret;
}

bool Live2DSharp::IDrawData::needTransform::get()
{
    auto __ret = ((::live2d::IDrawData*)NativePtr)->needTransform();
    return __ret;
}

int Live2DSharp::IDrawData::BASE_INDEX_NOT_INIT::get()
{
    return ::live2d::IDrawData::BASE_INDEX_NOT_INIT;
}

int Live2DSharp::IDrawData::DEFAULT_ORDER::get()
{
    return ::live2d::IDrawData::DEFAULT_ORDER;
}

int Live2DSharp::IDrawData::TYPE_DD_TEXTURE::get()
{
    return ::live2d::IDrawData::TYPE_DD_TEXTURE;
}

