#include "param/clrPivotManager.h"
#include "./clrModelContext.h"
#include "id/clrParamID.h"
#include "io/clrBReader.h"
#include "memory/clrMemoryParam.h"
#include "param/clrParamPivots.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::PivotManager::PivotManager(::live2d::PivotManager* native)
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)native)
{
}

Live2DSharp::PivotManager^ Live2DSharp::PivotManager::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::PivotManager((::live2d::PivotManager*) native.ToPointer());
}

Live2DSharp::PivotManager::PivotManager()
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)nullptr)
{
    NativePtr = new ::live2d::PivotManager();
}

void Live2DSharp::PivotManager::readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam)
{
    auto &arg0 = *(::live2d::BReader*)br->NativePtr;
    auto arg1 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::PivotManager*)NativePtr)->readV2(arg0, arg1);
}

void Live2DSharp::PivotManager::initDirect(Live2DSharp::MemoryParam^ memParam)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::PivotManager*)NativePtr)->initDirect(arg0);
}

int Live2DSharp::PivotManager::calcPivotValue(Live2DSharp::ModelContext^ mdc, bool* ret_paramOutside)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto arg1 = (bool*)ret_paramOutside;
    auto __ret = ((::live2d::PivotManager*)NativePtr)->calcPivotValue(arg0, arg1);
    return __ret;
}

void Live2DSharp::PivotManager::calcPivotIndexies(unsigned short* array64, float* tmpT_array, int interpolateCount)
{
    auto arg0 = (unsigned short*)array64;
    auto arg1 = (float*)tmpT_array;
    ((::live2d::PivotManager*)NativePtr)->calcPivotIndexies(arg0, arg1, interpolateCount);
}

bool Live2DSharp::PivotManager::checkParamUpdated(Live2DSharp::ModelContext^ mdc)
{
    auto &arg0 = *(::live2d::ModelContext*)mdc->NativePtr;
    auto __ret = ((::live2d::PivotManager*)NativePtr)->checkParamUpdated(arg0);
    return __ret;
}

int Live2DSharp::PivotManager::ParamCount::get()
{
    auto __ret = ((::live2d::PivotManager*)NativePtr)->getParamCount();
    return __ret;
}

