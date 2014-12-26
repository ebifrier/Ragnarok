#include "io/clrFileFormat2.h"
#include "memory/clrMemoryParam.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::FileFormat2::FileFormat2(::live2d::FileFormat2* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::FileFormat2^ Live2DSharp::FileFormat2::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::FileFormat2((::live2d::FileFormat2*) native.ToPointer());
}

Live2DSharp::FileFormat2::FileFormat2()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::FileFormat2();
}

::System::IntPtr Live2DSharp::FileFormat2::newInstance(Live2DSharp::MemoryParam^ memParam, int classNo)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    auto __ret = ::live2d::FileFormat2::newInstance(arg0, classNo);
    if (__ret == nullptr) return System::IntPtr();
    return ::System::IntPtr(__ret);
}

bool Live2DSharp::FileFormat2::isPrimitive(int classNo)
{
    auto __ret = ::live2d::FileFormat2::isPrimitive(classNo);
    return __ret;
}

bool Live2DSharp::FileFormat2::isPrimitiveDouble(int classNo)
{
    auto __ret = ::live2d::FileFormat2::isPrimitiveDouble(classNo);
    return __ret;
}

bool Live2DSharp::FileFormat2::isPrimitiveFloat(int classNo)
{
    auto __ret = ::live2d::FileFormat2::isPrimitiveFloat(classNo);
    return __ret;
}

bool Live2DSharp::FileFormat2::isPrimitiveInt(int classNo)
{
    auto __ret = ::live2d::FileFormat2::isPrimitiveInt(classNo);
    return __ret;
}

int Live2DSharp::FileFormat2::LIVE2D_FORMAT_VERSION_V2_6_INTIAL::get()
{
    return ::live2d::FileFormat2::LIVE2D_FORMAT_VERSION_V2_6_INTIAL;
}

int Live2DSharp::FileFormat2::LIVE2D_FORMAT_VERSION_V2_7_OPACITY::get()
{
    return ::live2d::FileFormat2::LIVE2D_FORMAT_VERSION_V2_7_OPACITY;
}

int Live2DSharp::FileFormat2::LIVE2D_FORMAT_VERSION_V2_8_TEX_OPTION::get()
{
    return ::live2d::FileFormat2::LIVE2D_FORMAT_VERSION_V2_8_TEX_OPTION;
}

int Live2DSharp::FileFormat2::LIVE2D_FORMAT_VERSION_V2_9_AVATAR_PARTS::get()
{
    return ::live2d::FileFormat2::LIVE2D_FORMAT_VERSION_V2_9_AVATAR_PARTS;
}

int Live2DSharp::FileFormat2::LIVE2D_FORMAT_VERSION_V2_10_SDK2::get()
{
    return ::live2d::FileFormat2::LIVE2D_FORMAT_VERSION_V2_10_SDK2;
}

int Live2DSharp::FileFormat2::LIVE2D_FORMAT_VERSION_AVAILABLE::get()
{
    return ::live2d::FileFormat2::LIVE2D_FORMAT_VERSION_AVAILABLE;
}

int Live2DSharp::FileFormat2::LIVE2D_FORMAT_EOF_VALUE::get()
{
    return ::live2d::FileFormat2::LIVE2D_FORMAT_EOF_VALUE;
}

int Live2DSharp::FileFormat2::NULL_NO::get()
{
    return ::live2d::FileFormat2::NULL_NO;
}

int Live2DSharp::FileFormat2::ARRAY_NO::get()
{
    return ::live2d::FileFormat2::ARRAY_NO;
}

int Live2DSharp::FileFormat2::OBJECT_REF::get()
{
    return ::live2d::FileFormat2::OBJECT_REF;
}

