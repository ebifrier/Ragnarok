#include "id/clrDrawDataID.h"
#include "io/clrRefString.h"
#include "type/clrLDString.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::DrawDataID::DrawDataID(::live2d::DrawDataID* native)
    : Live2DSharp::ID((::live2d::ID*)native)
{
}

Live2DSharp::DrawDataID^ Live2DSharp::DrawDataID::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::DrawDataID((::live2d::DrawDataID*) native.ToPointer());
}

Live2DSharp::DrawDataID^ Live2DSharp::DrawDataID::getID(Live2DSharp::LDString^ str)
{
    auto &arg0 = *(::live2d::LDString*)str->NativePtr;
    auto __ret = ::live2d::DrawDataID::getID(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::DrawDataID((::live2d::DrawDataID*)__ret);
}

Live2DSharp::DrawDataID^ Live2DSharp::DrawDataID::getID(Live2DSharp::RefString^ refStr)
{
    auto &arg0 = *(::live2d::RefString*)refStr->NativePtr;
    auto __ret = ::live2d::DrawDataID::getID(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::DrawDataID((::live2d::DrawDataID*)__ret);
}

void Live2DSharp::DrawDataID::releaseStored_notForClientCall()
{
    ::live2d::DrawDataID::releaseStored_notForClientCall();
}

System::String^ Live2DSharp::DrawDataID::toChar()
{
    auto __ret = ((::live2d::DrawDataID*)NativePtr)->toChar();
    if (__ret == nullptr) return nullptr;
    return clix::marshalString<clix::E_UTF8>(__ret);
}

int Live2DSharp::DrawDataID::L2D_DRAWDATA_ID_INITIAL_CAPACITY::get()
{
    return ::live2d::DrawDataID::L2D_DRAWDATA_ID_INITIAL_CAPACITY;
}

