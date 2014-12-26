#include "base/clrIBaseData.h"
#include "./clrModelContext.h"
#include "base/clrIBaseContext.h"
#include "id/clrBaseDataID.h"
#include "io/clrBReader.h"
#include "memory/clrMemoryParam.h"
#include "param/clrPivotManager.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::IBaseData::IBaseData(::live2d::IBaseData* native)
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)native)
{
}

Live2DSharp::IBaseData^ Live2DSharp::IBaseData::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::IBaseData((::live2d::IBaseData*) native.ToPointer());
}

Live2DSharp::IBaseData::IBaseData()
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)nullptr)
{
}

Live2DSharp::IBaseContext^ Live2DSharp::IBaseData::init(Live2DSharp::ModelContext^ mdc)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto __ret = ((::live2d::IBaseData*)NativePtr)->init(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::IBaseContext((::live2d::IBaseContext*)__ret);
}

void Live2DSharp::IBaseData::readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam)
{
    auto &arg0 = *(::live2d::BReader*)br->NativePtr;
    auto arg1 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::IBaseData*)NativePtr)->readV2(arg0, arg1);
}

void Live2DSharp::IBaseData::setupInterpolate(Live2DSharp::ModelContext^ mdc, Live2DSharp::IBaseContext^ cdata)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg1 = (::live2d::IBaseContext*)cdata->NativePtr;
    ((::live2d::IBaseData*)NativePtr)->setupInterpolate(arg0, arg1);
}

void Live2DSharp::IBaseData::setupTransform(Live2DSharp::ModelContext^ mdc, Live2DSharp::IBaseContext^ cdata)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg1 = (::live2d::IBaseContext*)cdata->NativePtr;
    ((::live2d::IBaseData*)NativePtr)->setupTransform(arg0, arg1);
}

void Live2DSharp::IBaseData::transformPoints(Live2DSharp::ModelContext^ mdc, Live2DSharp::IBaseContext^ cdata, float* srcPoints, float* dstPoints, int numPoint, int pt_offset, int pt_step)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg1 = (::live2d::IBaseContext*)cdata->NativePtr;
    auto arg2 = (::l2d_pointf*)srcPoints;
    auto arg3 = (::l2d_pointf*)dstPoints;
    ((::live2d::IBaseData*)NativePtr)->transformPoints(arg0, arg1, arg2, arg3, numPoint, pt_offset, pt_step);
}

Live2DSharp::BaseDataID^ Live2DSharp::IBaseData::TargetBaseDataID::get()
{
    auto __ret = ((::live2d::IBaseData*)NativePtr)->getTargetBaseDataID();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::BaseDataID((::live2d::BaseDataID*)__ret);
}

void Live2DSharp::IBaseData::TargetBaseDataID::set(Live2DSharp::BaseDataID^ id)
{
    auto arg0 = (::live2d::BaseDataID*)id->NativePtr;
    ((::live2d::IBaseData*)NativePtr)->setTargetBaseDataID(arg0);
}

Live2DSharp::BaseDataID^ Live2DSharp::IBaseData::BaseDataID::get()
{
    auto __ret = ((::live2d::IBaseData*)NativePtr)->getBaseDataID();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::BaseDataID((::live2d::BaseDataID*)__ret);
}

void Live2DSharp::IBaseData::BaseDataID::set(Live2DSharp::BaseDataID^ id)
{
    auto arg0 = (::live2d::BaseDataID*)id->NativePtr;
    ((::live2d::IBaseData*)NativePtr)->setBaseDataID(arg0);
}

int Live2DSharp::IBaseData::Type::get()
{
    auto __ret = ((::live2d::IBaseData*)NativePtr)->getType();
    return __ret;
}

bool Live2DSharp::IBaseData::needTransform::get()
{
    auto __ret = ((::live2d::IBaseData*)NativePtr)->needTransform();
    return __ret;
}

int Live2DSharp::IBaseData::BASE_INDEX_NOT_INIT::get()
{
    return ::live2d::IBaseData::BASE_INDEX_NOT_INIT;
}

int Live2DSharp::IBaseData::TYPE_BD_AFFINE::get()
{
    return ::live2d::IBaseData::TYPE_BD_AFFINE;
}

int Live2DSharp::IBaseData::TYPE_BD_BOX_GRID::get()
{
    return ::live2d::IBaseData::TYPE_BD_BOX_GRID;
}

