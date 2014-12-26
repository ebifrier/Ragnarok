#include "param/clrParamDefSet.h"
#include "io/clrBReader.h"
#include "memory/clrMemoryParam.h"
#include "param/clrParamDefFloat.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::ParamDefSet::ParamDefSet(::live2d::ParamDefSet* native)
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)native)
{
}

Live2DSharp::ParamDefSet^ Live2DSharp::ParamDefSet::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::ParamDefSet((::live2d::ParamDefSet*) native.ToPointer());
}

Live2DSharp::ParamDefSet::ParamDefSet()
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)nullptr)
{
    NativePtr = new ::live2d::ParamDefSet();
}

void Live2DSharp::ParamDefSet::initDirect(Live2DSharp::MemoryParam^ memParam)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::ParamDefSet*)NativePtr)->initDirect(arg0);
}

void Live2DSharp::ParamDefSet::readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam)
{
    auto &arg0 = *(::live2d::BReader*)br->NativePtr;
    auto arg1 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::ParamDefSet*)NativePtr)->readV2(arg0, arg1);
}

