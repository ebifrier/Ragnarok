#include "io/clrISerializableV2.h"
#include "io/clrBReader.h"
#include "memory/clrMemoryParam.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::ISerializableV2::ISerializableV2(::live2d::ISerializableV2* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::ISerializableV2^ Live2DSharp::ISerializableV2::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::ISerializableV2((::live2d::ISerializableV2*) native.ToPointer());
}

Live2DSharp::ISerializableV2::ISerializableV2()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
}

void Live2DSharp::ISerializableV2::readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam)
{
    auto &arg0 = *(::live2d::BReader*)br->NativePtr;
    auto arg1 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::ISerializableV2*)NativePtr)->readV2(arg0, arg1);
}

