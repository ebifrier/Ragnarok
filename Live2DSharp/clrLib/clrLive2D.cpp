#include "./clrLive2D.h"
#include "memory/clrLDAllocator.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::Live2D::Live2D(::live2d::Live2D* native)
{
    NativePtr = native;
}

Live2DSharp::Live2D^ Live2DSharp::Live2D::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::Live2D((::live2d::Live2D*) native.ToPointer());
}

void Live2DSharp::Live2D::init(Live2DSharp::LDAllocator^ allocator)
{
    auto arg0 = (::live2d::LDAllocator*)allocator->NativePtr;
    ::live2d::Live2D::init(arg0);
}

void Live2DSharp::Live2D::dispose()
{
    ::live2d::Live2D::dispose();
}

System::IntPtr Live2DSharp::Live2D::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::Live2D::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::Live2D*)object.ToPointer();
}

System::String^ Live2DSharp::Live2D::VersionStr::get()
{
    auto __ret = ::live2d::Live2D::getVersionStr();
    if (__ret == nullptr) return nullptr;
    return clix::marshalString<clix::E_UTF8>(__ret);
}

unsigned int Live2DSharp::Live2D::VersionNo::get()
{
    auto __ret = ::live2d::Live2D::getVersionNo();
    return __ret;
}

bool Live2DSharp::Live2D::BuildOption_RANGE_CHECK_POINT::get()
{
    auto __ret = ::live2d::Live2D::getBuildOption_RANGE_CHECK_POINT();
    return __ret;
}

bool Live2DSharp::Live2D::BuildOption_AVATAR_OPTION_A::get()
{
    auto __ret = ::live2d::Live2D::getBuildOption_AVATAR_OPTION_A();
    return __ret;
}

void Live2DSharp::Live2D::VertexDoubleBufferEnabled::set(bool enabled)
{
    auto arg0 = (::l2d_bool)enabled;
    ::live2d::Live2D::setVertexDoubleBufferEnabled(arg0);
}

unsigned int Live2DSharp::Live2D::Error::get()
{
    auto __ret = ::live2d::Live2D::getError();
    return __ret;
}

void Live2DSharp::Live2D::Error::set(unsigned int errorNo)
{
    auto arg0 = (::l2d_uint32)errorNo;
    ::live2d::Live2D::setError(arg0);
}

bool Live2DSharp::Live2D::isVertexDoubleBufferEnabled::get()
{
    auto __ret = ::live2d::Live2D::isVertexDoubleBufferEnabled();
    return __ret;
}

int Live2DSharp::Live2D::L2D_NO_ERROR::get()
{
    return ::live2d::Live2D::L2D_NO_ERROR;
}

int Live2DSharp::Live2D::L2D_ERROR_LIVE2D_INIT_FAILED::get()
{
    return ::live2d::Live2D::L2D_ERROR_LIVE2D_INIT_FAILED;
}

int Live2DSharp::Live2D::L2D_ERROR_FILE_LOAD_FAILED::get()
{
    return ::live2d::Live2D::L2D_ERROR_FILE_LOAD_FAILED;
}

int Live2DSharp::Live2D::L2D_ERROR_MEMORY_ERROR::get()
{
    return ::live2d::Live2D::L2D_ERROR_MEMORY_ERROR;
}

int Live2DSharp::Live2D::L2D_ERROR_MODEL_DATA_VERSION_MISMATCH::get()
{
    return ::live2d::Live2D::L2D_ERROR_MODEL_DATA_VERSION_MISMATCH;
}

int Live2DSharp::Live2D::L2D_ERROR_MODEL_DATA_EOF_ERROR::get()
{
    return ::live2d::Live2D::L2D_ERROR_MODEL_DATA_EOF_ERROR;
}

int Live2DSharp::Live2D::L2D_COLOR_BLEND_MODE_MULT::get()
{
    return ::live2d::Live2D::L2D_COLOR_BLEND_MODE_MULT;
}

int Live2DSharp::Live2D::L2D_COLOR_BLEND_MODE_ADD::get()
{
    return ::live2d::Live2D::L2D_COLOR_BLEND_MODE_ADD;
}

int Live2DSharp::Live2D::L2D_COLOR_BLEND_MODE_INTERPOLATE::get()
{
    return ::live2d::Live2D::L2D_COLOR_BLEND_MODE_INTERPOLATE;
}

