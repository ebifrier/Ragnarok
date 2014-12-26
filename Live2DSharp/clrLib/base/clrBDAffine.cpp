#include "base/clrBDAffine.h"
#include "./clrModelContext.h"
#include "base/private/clrAffineEnt.h"
#include "io/clrBReader.h"
#include "memory/clrMemoryParam.h"
#include "param/clrPivotManager.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::BDAffine::BDAffine(::live2d::BDAffine* native)
    : Live2DSharp::IBaseData((::live2d::IBaseData*)native)
{
}

Live2DSharp::BDAffine^ Live2DSharp::BDAffine::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::BDAffine((::live2d::BDAffine*) native.ToPointer());
}

Live2DSharp::BDAffine::BDAffine()
    : Live2DSharp::IBaseData((::live2d::IBaseData*)nullptr)
{
    NativePtr = new ::live2d::BDAffine();
}

void Live2DSharp::BDAffine::initDirect(Live2DSharp::MemoryParam^ memParam)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::BDAffine*)NativePtr)->initDirect(arg0);
}

void Live2DSharp::BDAffine::readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam)
{
    auto &arg0 = *(::live2d::BReader*)br->NativePtr;
    auto arg1 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::BDAffine*)NativePtr)->readV2(arg0, arg1);
}

Live2DSharp::IBaseContext^ Live2DSharp::BDAffine::init(Live2DSharp::ModelContext^ mdc)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto __ret = ((::live2d::BDAffine*)NativePtr)->init(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::IBaseContext((::live2d::IBaseContext*)__ret);
}

void Live2DSharp::BDAffine::setupInterpolate(Live2DSharp::ModelContext^ mdc, Live2DSharp::IBaseContext^ cdata)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg1 = (::live2d::IBaseContext*)cdata->NativePtr;
    ((::live2d::BDAffine*)NativePtr)->setupInterpolate(arg0, arg1);
}

void Live2DSharp::BDAffine::setupTransform(Live2DSharp::ModelContext^ mdc, Live2DSharp::IBaseContext^ cdata)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg1 = (::live2d::IBaseContext*)cdata->NativePtr;
    ((::live2d::BDAffine*)NativePtr)->setupTransform(arg0, arg1);
}

void Live2DSharp::BDAffine::transformPoints(Live2DSharp::ModelContext^ mdc, Live2DSharp::IBaseContext^ cdata, float* srcPoints, float* dstPoints, int numPoint, int pt_offset, int pt_step)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg1 = (::live2d::IBaseContext*)cdata->NativePtr;
    auto arg2 = (::l2d_pointf*)srcPoints;
    auto arg3 = (::l2d_pointf*)dstPoints;
    ((::live2d::BDAffine*)NativePtr)->transformPoints(arg0, arg1, arg2, arg3, numPoint, pt_offset, pt_step);
}

int Live2DSharp::BDAffine::Type::get()
{
    auto __ret = ((::live2d::BDAffine*)NativePtr)->getType();
    return __ret;
}

Live2DSharp::BDAffineContext::BDAffineContext(::live2d::BDAffineContext* native)
    : Live2DSharp::IBaseContext((::live2d::IBaseContext*)native)
{
}

Live2DSharp::BDAffineContext^ Live2DSharp::BDAffineContext::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::BDAffineContext((::live2d::BDAffineContext*) native.ToPointer());
}

Live2DSharp::BDAffineContext::BDAffineContext(Live2DSharp::BDAffine^ src)
    : Live2DSharp::IBaseContext((::live2d::IBaseContext*)nullptr)
{
    auto arg0 = (::live2d::BDAffine*)src->NativePtr;
    NativePtr = new ::live2d::BDAffineContext(arg0);
}

int Live2DSharp::BDAffineContext::tmpBaseDataIndex::get()
{
    return ((::live2d::BDAffineContext*)NativePtr)->tmpBaseDataIndex;
}

void Live2DSharp::BDAffineContext::tmpBaseDataIndex::set(int value)
{
    ((::live2d::BDAffineContext*)NativePtr)->tmpBaseDataIndex = value;
}

Live2DSharp::AffineEnt^ Live2DSharp::BDAffineContext::interpolatedAffine::get()
{
    return (((::live2d::BDAffineContext*)NativePtr)->interpolatedAffine == nullptr) ? nullptr : gcnew Live2DSharp::AffineEnt((::live2d::AffineEnt*)((::live2d::BDAffineContext*)NativePtr)->interpolatedAffine);
}

void Live2DSharp::BDAffineContext::interpolatedAffine::set(Live2DSharp::AffineEnt^ value)
{
    ((::live2d::BDAffineContext*)NativePtr)->interpolatedAffine = (::live2d::AffineEnt*)value->NativePtr;
}

Live2DSharp::AffineEnt^ Live2DSharp::BDAffineContext::transformedAffine::get()
{
    return (((::live2d::BDAffineContext*)NativePtr)->transformedAffine == nullptr) ? nullptr : gcnew Live2DSharp::AffineEnt((::live2d::AffineEnt*)((::live2d::BDAffineContext*)NativePtr)->transformedAffine);
}

void Live2DSharp::BDAffineContext::transformedAffine::set(Live2DSharp::AffineEnt^ value)
{
    ((::live2d::BDAffineContext*)NativePtr)->transformedAffine = (::live2d::AffineEnt*)value->NativePtr;
}

