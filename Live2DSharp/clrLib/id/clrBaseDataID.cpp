#include "id/clrBaseDataID.h"
#include "io/clrRefString.h"
#include "type/clrLDString.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::BaseDataID::BaseDataID(::live2d::BaseDataID* native)
    : Live2DSharp::ID((::live2d::ID*)native)
{
}

Live2DSharp::BaseDataID^ Live2DSharp::BaseDataID::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::BaseDataID((::live2d::BaseDataID*) native.ToPointer());
}

Live2DSharp::BaseDataID^ Live2DSharp::BaseDataID::getID(Live2DSharp::LDString^ str)
{
    auto &arg0 = *(::live2d::LDString*)str->NativePtr;
    auto __ret = ::live2d::BaseDataID::getID(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::BaseDataID((::live2d::BaseDataID*)__ret);
}

Live2DSharp::BaseDataID^ Live2DSharp::BaseDataID::getID(Live2DSharp::RefString^ refStr)
{
    auto &arg0 = *(::live2d::RefString*)refStr->NativePtr;
    auto __ret = ::live2d::BaseDataID::getID(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::BaseDataID((::live2d::BaseDataID*)__ret);
}

void Live2DSharp::BaseDataID::releaseStored_notForClientCall()
{
    ::live2d::BaseDataID::releaseStored_notForClientCall();
}

System::String^ Live2DSharp::BaseDataID::toChar()
{
    auto __ret = ((::live2d::BaseDataID*)NativePtr)->toChar();
    if (__ret == nullptr) return nullptr;
    return clix::marshalString<clix::E_UTF8>(__ret);
}

Live2DSharp::BaseDataID^ Live2DSharp::BaseDataID::DST_BASE_ID::get()
{
    auto __ret = ::live2d::BaseDataID::DST_BASE_ID();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::BaseDataID((::live2d::BaseDataID*)__ret);
}

int Live2DSharp::BaseDataID::L2D_BASEDATA_ID_INITIAL_CAPACITY::get()
{
    return ::live2d::BaseDataID::L2D_BASEDATA_ID_INITIAL_CAPACITY;
}

