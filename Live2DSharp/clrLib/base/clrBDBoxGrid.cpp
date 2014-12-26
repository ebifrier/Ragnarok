#include "base/clrBDBoxGrid.h"
#include "./clrModelContext.h"
#include "io/clrBReader.h"
#include "memory/clrMemoryParam.h"
#include "param/clrPivotManager.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::BDBoxGrid::BDBoxGrid(::live2d::BDBoxGrid* native)
    : Live2DSharp::IBaseData((::live2d::IBaseData*)native)
{
}

Live2DSharp::BDBoxGrid^ Live2DSharp::BDBoxGrid::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::BDBoxGrid((::live2d::BDBoxGrid*) native.ToPointer());
}

Live2DSharp::BDBoxGrid::BDBoxGrid()
    : Live2DSharp::IBaseData((::live2d::IBaseData*)nullptr)
{
    NativePtr = new ::live2d::BDBoxGrid();
}

void Live2DSharp::BDBoxGrid::initDirect(Live2DSharp::MemoryParam^ memParam)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::BDBoxGrid*)NativePtr)->initDirect(arg0);
}

void Live2DSharp::BDBoxGrid::readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam)
{
    auto &arg0 = *(::live2d::BReader*)br->NativePtr;
    auto arg1 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::BDBoxGrid*)NativePtr)->readV2(arg0, arg1);
}

Live2DSharp::IBaseContext^ Live2DSharp::BDBoxGrid::init(Live2DSharp::ModelContext^ mdc)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto __ret = ((::live2d::BDBoxGrid*)NativePtr)->init(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::IBaseContext((::live2d::IBaseContext*)__ret);
}

void Live2DSharp::BDBoxGrid::setupInterpolate(Live2DSharp::ModelContext^ mdc, Live2DSharp::IBaseContext^ cdata)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg1 = (::live2d::IBaseContext*)cdata->NativePtr;
    ((::live2d::BDBoxGrid*)NativePtr)->setupInterpolate(arg0, arg1);
}

void Live2DSharp::BDBoxGrid::setupTransform(Live2DSharp::ModelContext^ mdc, Live2DSharp::IBaseContext^ cdata)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg1 = (::live2d::IBaseContext*)cdata->NativePtr;
    ((::live2d::BDBoxGrid*)NativePtr)->setupTransform(arg0, arg1);
}

void Live2DSharp::BDBoxGrid::transformPoints(Live2DSharp::ModelContext^ mdc, Live2DSharp::IBaseContext^ cdata, float* srcPoints, float* dstPoints, int numPoint, int pt_offset, int pt_step)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg1 = (::live2d::IBaseContext*)cdata->NativePtr;
    auto arg2 = (::l2d_pointf*)srcPoints;
    auto arg3 = (::l2d_pointf*)dstPoints;
    ((::live2d::BDBoxGrid*)NativePtr)->transformPoints(arg0, arg1, arg2, arg3, numPoint, pt_offset, pt_step);
}

int Live2DSharp::BDBoxGrid::NumPts::get()
{
    auto __ret = ((::live2d::BDBoxGrid*)NativePtr)->getNumPts();
    return __ret;
}

int Live2DSharp::BDBoxGrid::Type::get()
{
    auto __ret = ((::live2d::BDBoxGrid*)NativePtr)->getType();
    return __ret;
}

Live2DSharp::BDBoxGridContext::BDBoxGridContext(::live2d::BDBoxGridContext* native)
    : Live2DSharp::IBaseContext((::live2d::IBaseContext*)native)
{
}

Live2DSharp::BDBoxGridContext^ Live2DSharp::BDBoxGridContext::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::BDBoxGridContext((::live2d::BDBoxGridContext*) native.ToPointer());
}

Live2DSharp::BDBoxGridContext::BDBoxGridContext(Live2DSharp::BDBoxGrid^ src)
    : Live2DSharp::IBaseContext((::live2d::IBaseContext*)nullptr)
{
    auto arg0 = (::live2d::BDBoxGrid*)src->NativePtr;
    NativePtr = new ::live2d::BDBoxGridContext(arg0);
}

int Live2DSharp::BDBoxGridContext::tmpBaseDataIndex::get()
{
    return ((::live2d::BDBoxGridContext*)NativePtr)->tmpBaseDataIndex;
}

void Live2DSharp::BDBoxGridContext::tmpBaseDataIndex::set(int value)
{
    ((::live2d::BDBoxGridContext*)NativePtr)->tmpBaseDataIndex = value;
}

float* Live2DSharp::BDBoxGridContext::interpolatedPoints::get()
{
    return reinterpret_cast<float*>(((::live2d::BDBoxGridContext*)NativePtr)->interpolatedPoints);
}

void Live2DSharp::BDBoxGridContext::interpolatedPoints::set(float* value)
{
    ((::live2d::BDBoxGridContext*)NativePtr)->interpolatedPoints = (::l2d_pointf*)value;
}

float* Live2DSharp::BDBoxGridContext::transformedPoints::get()
{
    return reinterpret_cast<float*>(((::live2d::BDBoxGridContext*)NativePtr)->transformedPoints);
}

void Live2DSharp::BDBoxGridContext::transformedPoints::set(float* value)
{
    ((::live2d::BDBoxGridContext*)NativePtr)->transformedPoints = (::l2d_pointf*)value;
}

