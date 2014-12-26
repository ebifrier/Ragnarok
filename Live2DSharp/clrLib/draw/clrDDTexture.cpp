#include "draw/clrDDTexture.h"
#include "./clrModelContext.h"
#include "graphics/clrDrawParam.h"
#include "io/clrBReader.h"
#include "memory/clrMemoryParam.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::DDTexture::DDTexture(::live2d::DDTexture* native)
    : Live2DSharp::IDrawData((::live2d::IDrawData*)native)
{
}

Live2DSharp::DDTexture^ Live2DSharp::DDTexture::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::DDTexture((::live2d::DDTexture*) native.ToPointer());
}

Live2DSharp::DDTexture::DDTexture()
    : Live2DSharp::IDrawData((::live2d::IDrawData*)nullptr)
{
    NativePtr = new ::live2d::DDTexture();
}

void Live2DSharp::DDTexture::readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam)
{
    auto &arg0 = *(::live2d::BReader*)br->NativePtr;
    auto arg1 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::DDTexture*)NativePtr)->readV2(arg0, arg1);
}

void Live2DSharp::DDTexture::initDirect(Live2DSharp::MemoryParam^ memParam)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::DDTexture*)NativePtr)->initDirect(arg0);
}

Live2DSharp::IDrawContext^ Live2DSharp::DDTexture::init(Live2DSharp::ModelContext^ mdc)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto __ret = ((::live2d::DDTexture*)NativePtr)->init(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::IDrawContext((::live2d::IDrawContext*)__ret);
}

void Live2DSharp::DDTexture::setupInterpolate(Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ cdata)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg1 = (::live2d::IDrawContext*)cdata->NativePtr;
    ((::live2d::DDTexture*)NativePtr)->setupInterpolate(arg0, arg1);
}

void Live2DSharp::DDTexture::setupTransform(Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ cdata)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg1 = (::live2d::IDrawContext*)cdata->NativePtr;
    ((::live2d::DDTexture*)NativePtr)->setupTransform(arg0, arg1);
}

void Live2DSharp::DDTexture::draw(Live2DSharp::DrawParam^ dp, Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ cdata)
{
    auto &arg0 = *(::live2d::DrawParam*)dp->NativePtr;
    auto &arg1 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg2 = (::live2d::IDrawContext*)cdata->NativePtr;
    ((::live2d::DDTexture*)NativePtr)->draw(arg0, arg1, arg2);
}

void Live2DSharp::DDTexture::setZ_TestImpl(Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ _cdata, float z)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg1 = (::live2d::IDrawContext*)_cdata->NativePtr;
    ((::live2d::DDTexture*)NativePtr)->setZ_TestImpl(arg0, arg1, z);
}

unsigned short* Live2DSharp::DDTexture::getIndexArray(int* polygonCount)
{
    auto arg0 = (int*)polygonCount;
    auto __ret = ((::live2d::DDTexture*)NativePtr)->getIndexArray(arg0);
    return reinterpret_cast<unsigned short*>(__ret);
}

int Live2DSharp::DDTexture::TextureNo::get()
{
    auto __ret = ((::live2d::DDTexture*)NativePtr)->getTextureNo();
    return __ret;
}

void Live2DSharp::DDTexture::TextureNo::set(int no)
{
    ((::live2d::DDTexture*)NativePtr)->setTextureNo(no);
}

float* Live2DSharp::DDTexture::UvMap::get()
{
    auto __ret = ((::live2d::DDTexture*)NativePtr)->getUvMap();
    return reinterpret_cast<float*>(__ret);
}

int Live2DSharp::DDTexture::NumPoints::get()
{
    auto __ret = ((::live2d::DDTexture*)NativePtr)->getNumPoints();
    return __ret;
}

int Live2DSharp::DDTexture::NumPolygons::get()
{
    auto __ret = ((::live2d::DDTexture*)NativePtr)->getNumPolygons();
    return __ret;
}

int Live2DSharp::DDTexture::Type::get()
{
    auto __ret = ((::live2d::DDTexture*)NativePtr)->getType();
    return __ret;
}

int Live2DSharp::DDTexture::OptionFlag::get()
{
    auto __ret = ((::live2d::DDTexture*)NativePtr)->getOptionFlag();
    return __ret;
}

int Live2DSharp::DDTexture::Option_KanojoColor::get()
{
    auto __ret = ((::live2d::DDTexture*)NativePtr)->getOption_KanojoColor();
    return __ret;
}

int Live2DSharp::DDTexture::OPTION_FLAG_BARCODE_KANOJO_COLOR_CONVERT::get()
{
    return ::live2d::DDTexture::OPTION_FLAG_BARCODE_KANOJO_COLOR_CONVERT;
}

int Live2DSharp::DDTexture::MASK_COLOR_COMPOSITION::get()
{
    return ::live2d::DDTexture::MASK_COLOR_COMPOSITION;
}

int Live2DSharp::DDTexture::COLOR_COMPOSITION_NORMAL::get()
{
    return ::live2d::DDTexture::COLOR_COMPOSITION_NORMAL;
}

int Live2DSharp::DDTexture::COLOR_COMPOSITION_SCREEN::get()
{
    return ::live2d::DDTexture::COLOR_COMPOSITION_SCREEN;
}

int Live2DSharp::DDTexture::COLOR_COMPOSITION_MULTIPLY::get()
{
    return ::live2d::DDTexture::COLOR_COMPOSITION_MULTIPLY;
}

int Live2DSharp::DDTexture::INSTANCE_COUNT::get()
{
    return ::live2d::DDTexture::INSTANCE_COUNT;
}

void Live2DSharp::DDTexture::INSTANCE_COUNT::set(int value)
{
    ::live2d::DDTexture::INSTANCE_COUNT = value;
}

Live2DSharp::DDTextureContext::DDTextureContext(::live2d::DDTextureContext* native)
    : Live2DSharp::IDrawContext((::live2d::IDrawContext*)native)
{
}

Live2DSharp::DDTextureContext^ Live2DSharp::DDTextureContext::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::DDTextureContext((::live2d::DDTextureContext*) native.ToPointer());
}

Live2DSharp::DDTextureContext::DDTextureContext(Live2DSharp::IDrawData^ src)
    : Live2DSharp::IDrawContext((::live2d::IDrawContext*)nullptr)
{
    auto arg0 = (::live2d::IDrawData*)src->NativePtr;
    NativePtr = new ::live2d::DDTextureContext(arg0);
}

float* Live2DSharp::DDTextureContext::getTransformedPoints(int* pointCount)
{
    auto arg0 = (int*)pointCount;
    auto __ret = ((::live2d::DDTextureContext*)NativePtr)->getTransformedPoints(arg0);
    return reinterpret_cast<float*>(__ret);
}

int Live2DSharp::DDTextureContext::tmpBaseDataIndex::get()
{
    return ((::live2d::DDTextureContext*)NativePtr)->tmpBaseDataIndex;
}

void Live2DSharp::DDTextureContext::tmpBaseDataIndex::set(int value)
{
    ((::live2d::DDTextureContext*)NativePtr)->tmpBaseDataIndex = value;
}

float* Live2DSharp::DDTextureContext::interpolatedPoints::get()
{
    return reinterpret_cast<float*>(((::live2d::DDTextureContext*)NativePtr)->interpolatedPoints);
}

void Live2DSharp::DDTextureContext::interpolatedPoints::set(float* value)
{
    ((::live2d::DDTextureContext*)NativePtr)->interpolatedPoints = (::l2d_pointf*)value;
}

float* Live2DSharp::DDTextureContext::transformedPoints::get()
{
    return reinterpret_cast<float*>(((::live2d::DDTextureContext*)NativePtr)->transformedPoints);
}

void Live2DSharp::DDTextureContext::transformedPoints::set(float* value)
{
    ((::live2d::DDTextureContext*)NativePtr)->transformedPoints = (::l2d_pointf*)value;
}

float* Live2DSharp::DDTextureContext::drawPoints::get()
{
    return reinterpret_cast<float*>(((::live2d::DDTextureContext*)NativePtr)->drawPoints);
}

void Live2DSharp::DDTextureContext::drawPoints::set(float* value)
{
    ((::live2d::DDTextureContext*)NativePtr)->drawPoints = (::l2d_pointf*)value;
}

unsigned char Live2DSharp::DDTextureContext::not_updated_count::get()
{
    return ((::live2d::DDTextureContext*)NativePtr)->not_updated_count;
}

void Live2DSharp::DDTextureContext::not_updated_count::set(unsigned char value)
{
    ((::live2d::DDTextureContext*)NativePtr)->not_updated_count = value;
}

