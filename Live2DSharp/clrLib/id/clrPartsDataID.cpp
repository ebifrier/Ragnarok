#include "id/clrPartsDataID.h"
#include "io/clrRefString.h"
#include "type/clrLDString.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::PartsDataID::PartsDataID(::live2d::PartsDataID* native)
    : Live2DSharp::ID((::live2d::ID*)native)
{
}

Live2DSharp::PartsDataID^ Live2DSharp::PartsDataID::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::PartsDataID((::live2d::PartsDataID*) native.ToPointer());
}

Live2DSharp::PartsDataID^ Live2DSharp::PartsDataID::getID(Live2DSharp::LDString^ str)
{
    auto &arg0 = *(::live2d::LDString*)str->NativePtr;
    auto __ret = ::live2d::PartsDataID::getID(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::PartsDataID((::live2d::PartsDataID*)__ret);
}

Live2DSharp::PartsDataID^ Live2DSharp::PartsDataID::getID(Live2DSharp::RefString^ refStr)
{
    auto &arg0 = *(::live2d::RefString*)refStr->NativePtr;
    auto __ret = ::live2d::PartsDataID::getID(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::PartsDataID((::live2d::PartsDataID*)__ret);
}

void Live2DSharp::PartsDataID::releaseStored_notForClientCall()
{
    ::live2d::PartsDataID::releaseStored_notForClientCall();
}

System::String^ Live2DSharp::PartsDataID::toChar()
{
    auto __ret = ((::live2d::PartsDataID*)NativePtr)->toChar();
    if (__ret == nullptr) return nullptr;
    return clix::marshalString<clix::E_UTF8>(__ret);
}

int Live2DSharp::PartsDataID::L2D_PARTS_ID_INITIAL_CAPACITY::get()
{
    return ::live2d::PartsDataID::L2D_PARTS_ID_INITIAL_CAPACITY;
}

