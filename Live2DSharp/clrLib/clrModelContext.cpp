#include "./clrModelContext.h"
#include "./clrALive2DModel.h"
#include "base/clrIBaseContext.h"
#include "base/clrIBaseData.h"
#include "draw/clrIDrawContext.h"
#include "draw/clrIDrawData.h"
#include "graphics/clrDrawParam.h"
#include "id/clrBaseDataID.h"
#include "id/clrDrawDataID.h"
#include "id/clrParamID.h"
#include "id/clrPartsDataID.h"
#include "memory/clrAMemoryHolder.h"
#include "memory/clrMemoryParam.h"
#include "model/clrPartsData.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::ModelContext::ModelContext(::live2d::ModelContext* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::ModelContext^ Live2DSharp::ModelContext::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::ModelContext((::live2d::ModelContext*) native.ToPointer());
}

Live2DSharp::ModelContext::ModelContext(Live2DSharp::ALive2DModel^ model)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    auto arg0 = (::live2d::ALive2DModel*)model->NativePtr;
    NativePtr = new ::live2d::ModelContext(arg0);
}

void Live2DSharp::ModelContext::release()
{
    ((::live2d::ModelContext*)NativePtr)->release();
}

void Live2DSharp::ModelContext::init()
{
    ((::live2d::ModelContext*)NativePtr)->init();
}

bool Live2DSharp::ModelContext::requireSetup()
{
    auto __ret = ((::live2d::ModelContext*)NativePtr)->requireSetup();
    return __ret;
}

void Live2DSharp::ModelContext::draw(Live2DSharp::DrawParam^ dp)
{
    auto &arg0 = *(::live2d::DrawParam*)dp->NativePtr;
    ((::live2d::ModelContext*)NativePtr)->draw(arg0);
}

bool Live2DSharp::ModelContext::isParamUpdated(int paramIndex)
{
    auto __ret = ((::live2d::ModelContext*)NativePtr)->isParamUpdated(paramIndex);
    return __ret;
}

int Live2DSharp::ModelContext::getParamIndex(Live2DSharp::ParamID^ paramID)
{
    auto arg0 = (::live2d::ParamID*)paramID->NativePtr;
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getParamIndex(arg0);
    return __ret;
}

int Live2DSharp::ModelContext::getBaseDataIndex(Live2DSharp::BaseDataID^ baseID)
{
    auto arg0 = (::live2d::BaseDataID*)baseID->NativePtr;
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getBaseDataIndex(arg0);
    return __ret;
}

int Live2DSharp::ModelContext::getPartsDataIndex(Live2DSharp::PartsDataID^ partsID)
{
    auto arg0 = (::live2d::PartsDataID*)partsID->NativePtr;
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getPartsDataIndex(arg0);
    return __ret;
}

int Live2DSharp::ModelContext::getDrawDataIndex(Live2DSharp::DrawDataID^ drawDataID)
{
    auto arg0 = (::live2d::DrawDataID*)drawDataID->NativePtr;
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getDrawDataIndex(arg0);
    return __ret;
}

int Live2DSharp::ModelContext::addFloatParam(Live2DSharp::ParamID^ id, float value, float min, float max)
{
    auto arg0 = (::live2d::ParamID*)id->NativePtr;
    auto arg1 = (::l2d_paramf)value;
    auto arg2 = (::l2d_paramf)min;
    auto arg3 = (::l2d_paramf)max;
    auto __ret = ((::live2d::ModelContext*)NativePtr)->addFloatParam(arg0, arg1, arg2, arg3);
    return __ret;
}

void Live2DSharp::ModelContext::setBaseData(unsigned int baseDataIndex, Live2DSharp::IBaseData^ baseData)
{
    auto arg1 = (::live2d::IBaseData*)baseData->NativePtr;
    ((::live2d::ModelContext*)NativePtr)->setBaseData(baseDataIndex, arg1);
}

void Live2DSharp::ModelContext::setParamFloat(unsigned int paramIndex, float value)
{
    auto arg1 = (::l2d_paramf)value;
    ((::live2d::ModelContext*)NativePtr)->setParamFloat(paramIndex, arg1);
}

float Live2DSharp::ModelContext::getParamMax(unsigned int paramIndex)
{
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getParamMax(paramIndex);
    return __ret;
}

float Live2DSharp::ModelContext::getParamMin(unsigned int paramIndex)
{
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getParamMin(paramIndex);
    return __ret;
}

void Live2DSharp::ModelContext::loadParam()
{
    ((::live2d::ModelContext*)NativePtr)->loadParam();
}

void Live2DSharp::ModelContext::saveParam()
{
    ((::live2d::ModelContext*)NativePtr)->saveParam();
}

void Live2DSharp::ModelContext::setPartsOpacity(int partIndex, float opacity)
{
    ((::live2d::ModelContext*)NativePtr)->setPartsOpacity(partIndex, opacity);
}

float Live2DSharp::ModelContext::getPartsOpacity(int partIndex)
{
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getPartsOpacity(partIndex);
    return __ret;
}

Live2DSharp::IBaseData^ Live2DSharp::ModelContext::getBaseData(unsigned int baseDataIndex)
{
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getBaseData(baseDataIndex);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::IBaseData((::live2d::IBaseData*)__ret);
}

Live2DSharp::IDrawData^ Live2DSharp::ModelContext::getDrawData(unsigned int drawDataIndex)
{
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getDrawData(drawDataIndex);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::IDrawData((::live2d::IDrawData*)__ret);
}

Live2DSharp::IBaseContext^ Live2DSharp::ModelContext::getBaseContext(unsigned int baseDataIndex)
{
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getBaseContext(baseDataIndex);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::IBaseContext((::live2d::IBaseContext*)__ret);
}

Live2DSharp::IDrawContext^ Live2DSharp::ModelContext::getDrawContext(unsigned int drawDataIndex)
{
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getDrawContext(drawDataIndex);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::IDrawContext((::live2d::IDrawContext*)__ret);
}

Live2DSharp::PartsDataContext^ Live2DSharp::ModelContext::getPartsContext(unsigned int partsDataIndex)
{
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getPartsContext(partsDataIndex);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::PartsDataContext((::live2d::PartsDataContext*)__ret);
}

float Live2DSharp::ModelContext::getParamFloat(unsigned int paramIndex)
{
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getParamFloat(paramIndex);
    return __ret;
}

void Live2DSharp::ModelContext::deviceLost()
{
    ((::live2d::ModelContext*)NativePtr)->deviceLost();
}

void Live2DSharp::ModelContext::updateZBuffer_TestImpl(float startZ, float stepZ)
{
    ((::live2d::ModelContext*)NativePtr)->updateZBuffer_TestImpl(startZ, stepZ);
}

Live2DSharp::MemoryParam^ Live2DSharp::ModelContext::MemoryParam::get()
{
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getMemoryParam();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::MemoryParam((::live2d::MemoryParam*)__ret);
}

int Live2DSharp::ModelContext::InitVersion::get()
{
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getInitVersion();
    return __ret;
}

unsigned short* Live2DSharp::ModelContext::TmpPivotTableIndicesRef::get()
{
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getTmpPivotTableIndicesRef();
    return __ret;
}

float* Live2DSharp::ModelContext::TmpT_ArrayRef::get()
{
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getTmpT_ArrayRef();
    return __ret;
}

int Live2DSharp::ModelContext::BaseDataCount::get()
{
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getBaseDataCount();
    return __ret;
}

int Live2DSharp::ModelContext::DrawDataCount::get()
{
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getDrawDataCount();
    return __ret;
}

int Live2DSharp::ModelContext::PartsDataCount::get()
{
    auto __ret = ((::live2d::ModelContext*)NativePtr)->getPartsDataCount();
    return __ret;
}

bool Live2DSharp::ModelContext::update::get()
{
    auto __ret = ((::live2d::ModelContext*)NativePtr)->update();
    return __ret;
}

unsigned short Live2DSharp::ModelContext::NOT_USED_ORDER::get()
{
    return ::live2d::ModelContext::NOT_USED_ORDER;
}

unsigned short Live2DSharp::ModelContext::NO_NEXT::get()
{
    return ::live2d::ModelContext::NO_NEXT;
}

