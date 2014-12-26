#include "id/clrParamID.h"
#include "io/clrRefString.h"
#include "type/clrLDString.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::ParamID::ParamID(::live2d::ParamID* native)
    : Live2DSharp::ID((::live2d::ID*)native)
{
}

Live2DSharp::ParamID^ Live2DSharp::ParamID::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::ParamID((::live2d::ParamID*) native.ToPointer());
}

System::String^ Live2DSharp::ParamID::toChar()
{
    auto __ret = ((::live2d::ParamID*)NativePtr)->toChar();
    if (__ret == nullptr) return nullptr;
    return clix::marshalString<clix::E_UTF8>(__ret);
}

Live2DSharp::ParamID^ Live2DSharp::ParamID::getID(Live2DSharp::LDString^ tmp_idstr)
{
    auto &arg0 = *(::live2d::LDString*)tmp_idstr->NativePtr;
    auto __ret = ::live2d::ParamID::getID(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::ParamID((::live2d::ParamID*)__ret);
}

Live2DSharp::ParamID^ Live2DSharp::ParamID::getID(System::String^ tmp_idstr)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(tmp_idstr);
    auto arg0 = _arg0.c_str();
    auto __ret = ::live2d::ParamID::getID(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::ParamID((::live2d::ParamID*)__ret);
}

Live2DSharp::ParamID^ Live2DSharp::ParamID::getID(Live2DSharp::RefString^ refStr)
{
    auto &arg0 = *(::live2d::RefString*)refStr->NativePtr;
    auto __ret = ::live2d::ParamID::getID(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::ParamID((::live2d::ParamID*)__ret);
}

void Live2DSharp::ParamID::releaseStored_notForClientCall()
{
    ::live2d::ParamID::releaseStored_notForClientCall();
}

int Live2DSharp::ParamID::L2D_PARAM_ID_INITIAL_CAPACITY::get()
{
    return ::live2d::ParamID::L2D_PARAM_ID_INITIAL_CAPACITY;
}

