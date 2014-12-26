#include "./clrALive2DModel.h"
#include "./clrModelContext.h"
#include "draw/clrIDrawData.h"
#include "graphics/clrDrawParam.h"
#include "id/clrParamID.h"
#include "id/clrPartsDataID.h"
#include "model/clrModelImpl.h"
#include "type/clrLDString.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::ALive2DModel::ALive2DModel(::live2d::ALive2DModel* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::ALive2DModel^ Live2DSharp::ALive2DModel::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::ALive2DModel((::live2d::ALive2DModel*) native.ToPointer());
}

Live2DSharp::ALive2DModel::ALive2DModel()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::ALive2DModel();
}

float Live2DSharp::ALive2DModel::getParamFloat(System::String^ paramID)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(paramID);
    auto arg0 = _arg0.c_str();
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->getParamFloat(arg0);
    return __ret;
}

void Live2DSharp::ALive2DModel::setParamFloat(System::String^ paramID, float value, float weight)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(paramID);
    auto arg0 = _arg0.c_str();
    ((::live2d::ALive2DModel*)NativePtr)->setParamFloat(arg0, value, weight);
}

void Live2DSharp::ALive2DModel::addToParamFloat(System::String^ paramID, float value, float weight)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(paramID);
    auto arg0 = _arg0.c_str();
    ((::live2d::ALive2DModel*)NativePtr)->addToParamFloat(arg0, value, weight);
}

void Live2DSharp::ALive2DModel::multParamFloat(System::String^ paramID, float mult, float weight)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(paramID);
    auto arg0 = _arg0.c_str();
    ((::live2d::ALive2DModel*)NativePtr)->multParamFloat(arg0, mult, weight);
}

int Live2DSharp::ALive2DModel::getParamIndex(System::String^ paramID)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(paramID);
    auto arg0 = _arg0.c_str();
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->getParamIndex(arg0);
    return __ret;
}

int Live2DSharp::ALive2DModel::getParamIndex(Live2DSharp::ParamID^ paramID)
{
    auto arg0 = (::live2d::ParamID*)paramID->NativePtr;
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->getParamIndex(arg0);
    return __ret;
}

float Live2DSharp::ALive2DModel::getParamFloat(int paramIndex)
{
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->getParamFloat(paramIndex);
    return __ret;
}

void Live2DSharp::ALive2DModel::setParamFloat(int paramIndex, float value, float weight)
{
    ((::live2d::ALive2DModel*)NativePtr)->setParamFloat(paramIndex, value, weight);
}

void Live2DSharp::ALive2DModel::addToParamFloat(int paramIndex, float value, float weight)
{
    ((::live2d::ALive2DModel*)NativePtr)->addToParamFloat(paramIndex, value, weight);
}

void Live2DSharp::ALive2DModel::multParamFloat(int paramIndex, float mult, float weight)
{
    ((::live2d::ALive2DModel*)NativePtr)->multParamFloat(paramIndex, mult, weight);
}

void Live2DSharp::ALive2DModel::loadParam()
{
    ((::live2d::ALive2DModel*)NativePtr)->loadParam();
}

void Live2DSharp::ALive2DModel::saveParam()
{
    ((::live2d::ALive2DModel*)NativePtr)->saveParam();
}

void Live2DSharp::ALive2DModel::init()
{
    ((::live2d::ALive2DModel*)NativePtr)->init();
}

void Live2DSharp::ALive2DModel::update()
{
    ((::live2d::ALive2DModel*)NativePtr)->update();
}

void Live2DSharp::ALive2DModel::draw()
{
    ((::live2d::ALive2DModel*)NativePtr)->draw();
}

void Live2DSharp::ALive2DModel::setPartsOpacity(System::String^ partsID, float opacity)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(partsID);
    auto arg0 = _arg0.c_str();
    ((::live2d::ALive2DModel*)NativePtr)->setPartsOpacity(arg0, opacity);
}

void Live2DSharp::ALive2DModel::setPartsOpacity(int partsIndex, float opacity)
{
    ((::live2d::ALive2DModel*)NativePtr)->setPartsOpacity(partsIndex, opacity);
}

float Live2DSharp::ALive2DModel::getPartsOpacity(System::String^ partsID)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(partsID);
    auto arg0 = _arg0.c_str();
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->getPartsOpacity(arg0);
    return __ret;
}

float Live2DSharp::ALive2DModel::getPartsOpacity(int partsIndex)
{
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->getPartsOpacity(partsIndex);
    return __ret;
}

int Live2DSharp::ALive2DModel::generateModelTextureNo()
{
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->generateModelTextureNo();
    return __ret;
}

void Live2DSharp::ALive2DModel::releaseModelTextureNo(int no)
{
    ((::live2d::ALive2DModel*)NativePtr)->releaseModelTextureNo(no);
}

int Live2DSharp::ALive2DModel::getDrawDataIndex(System::String^ drawDataID)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(drawDataID);
    auto arg0 = _arg0.c_str();
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->getDrawDataIndex(arg0);
    return __ret;
}

Live2DSharp::IDrawData^ Live2DSharp::ALive2DModel::getDrawData(int drawIndex)
{
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->getDrawData(drawIndex);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::IDrawData((::live2d::IDrawData*)__ret);
}

float* Live2DSharp::ALive2DModel::getTransformedPoints(int drawIndex, int* pointCount)
{
    auto arg1 = (int*)pointCount;
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->getTransformedPoints(drawIndex, arg1);
    return reinterpret_cast<float*>(__ret);
}

unsigned short* Live2DSharp::ALive2DModel::getIndexArray(int drawIndex, int* polygonCount)
{
    auto arg1 = (int*)polygonCount;
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->getIndexArray(drawIndex, arg1);
    return reinterpret_cast<unsigned short*>(__ret);
}

void Live2DSharp::ALive2DModel::updateZBuffer_TestImpl(float startZ, float stepZ)
{
    ((::live2d::ALive2DModel*)NativePtr)->updateZBuffer_TestImpl(startZ, stepZ);
}

int Live2DSharp::ALive2DModel::getPartsDataIndex(System::String^ partsID)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(partsID);
    auto arg0 = _arg0.c_str();
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->getPartsDataIndex(arg0);
    return __ret;
}

int Live2DSharp::ALive2DModel::getPartsDataIndex(Live2DSharp::PartsDataID^ partsID)
{
    auto arg0 = (::live2d::PartsDataID*)partsID->NativePtr;
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->getPartsDataIndex(arg0);
    return __ret;
}

Live2DSharp::ModelImpl^ Live2DSharp::ALive2DModel::ModelImpl::get()
{
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->getModelImpl();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::ModelImpl((::live2d::ModelImpl*)__ret);
}

void Live2DSharp::ALive2DModel::ModelImpl::set(Live2DSharp::ModelImpl^ m)
{
    auto arg0 = (::live2d::ModelImpl*)m->NativePtr;
    ((::live2d::ALive2DModel*)NativePtr)->setModelImpl(arg0);
}

Live2DSharp::ModelContext^ Live2DSharp::ALive2DModel::ModelContext::get()
{
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->getModelContext();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::ModelContext((::live2d::ModelContext*)__ret);
}

int Live2DSharp::ALive2DModel::ErrorFlags::get()
{
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->getErrorFlags();
    return __ret;
}

float Live2DSharp::ALive2DModel::CanvasWidth::get()
{
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->getCanvasWidth();
    return __ret;
}

float Live2DSharp::ALive2DModel::CanvasHeight::get()
{
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->getCanvasHeight();
    return __ret;
}

Live2DSharp::DrawParam^ Live2DSharp::ALive2DModel::DrawParam::get()
{
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->getDrawParam();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::DrawParam((::live2d::DrawParam*)__ret);
}

void Live2DSharp::ALive2DModel::PremultipliedAlpha::set(bool b)
{
    ((::live2d::ALive2DModel*)NativePtr)->setPremultipliedAlpha(b);
}

int Live2DSharp::ALive2DModel::Anisotropy::get()
{
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->getAnisotropy();
    return __ret;
}

void Live2DSharp::ALive2DModel::Anisotropy::set(int n)
{
    ((::live2d::ALive2DModel*)NativePtr)->setAnisotropy(n);
}

bool Live2DSharp::ALive2DModel::isPremultipliedAlpha::get()
{
    auto __ret = ((::live2d::ALive2DModel*)NativePtr)->isPremultipliedAlpha();
    return __ret;
}

int Live2DSharp::ALive2DModel::FILE_LOAD_EOF_ERROR::get()
{
    return ::live2d::ALive2DModel::FILE_LOAD_EOF_ERROR;
}

int Live2DSharp::ALive2DModel::FILE_LOAD_VERSION_ERROR::get()
{
    return ::live2d::ALive2DModel::FILE_LOAD_VERSION_ERROR;
}

int Live2DSharp::ALive2DModel::INSTANCE_COUNT::get()
{
    return ::live2d::ALive2DModel::INSTANCE_COUNT;
}

void Live2DSharp::ALive2DModel::INSTANCE_COUNT::set(int value)
{
    ::live2d::ALive2DModel::INSTANCE_COUNT = value;
}

