#include "avatar/clrAvatarPartsItem.h"
#include "base/clrIBaseData.h"
#include "draw/clrIDrawData.h"
#include "id/clrPartsDataID.h"
#include "io/clrBReader.h"
#include "memory/clrMemoryParam.h"
#include "model/clrPartsData.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::AvatarPartsItem::AvatarPartsItem(::live2d::AvatarPartsItem* native)
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)native)
{
}

Live2DSharp::AvatarPartsItem^ Live2DSharp::AvatarPartsItem::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::AvatarPartsItem((::live2d::AvatarPartsItem*) native.ToPointer());
}

Live2DSharp::AvatarPartsItem::AvatarPartsItem()
    : Live2DSharp::ISerializableV2((::live2d::ISerializableV2*)nullptr)
{
    NativePtr = new ::live2d::AvatarPartsItem();
}

void Live2DSharp::AvatarPartsItem::readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam)
{
    auto &arg0 = *(::live2d::BReader*)br->NativePtr;
    auto arg1 = (::live2d::MemoryParam*)memParam->NativePtr;
    ((::live2d::AvatarPartsItem*)NativePtr)->readV2(arg0, arg1);
}

void Live2DSharp::AvatarPartsItem::replacePartsData(Live2DSharp::PartsData^ parts)
{
    auto arg0 = (::live2d::PartsData*)parts->NativePtr;
    ((::live2d::AvatarPartsItem*)NativePtr)->replacePartsData(arg0);
}

int Live2DSharp::AvatarPartsItem::INSTANCE_COUNT::get()
{
    return ::live2d::AvatarPartsItem::INSTANCE_COUNT;
}

void Live2DSharp::AvatarPartsItem::INSTANCE_COUNT::set(int value)
{
    ::live2d::AvatarPartsItem::INSTANCE_COUNT = value;
}

