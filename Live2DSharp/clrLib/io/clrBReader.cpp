#include "io/clrBReader.h"
#include "io/clrByteBuffer.h"
#include "io/clrRefString.h"
#include "memory/clrAMemoryHolder.h"
#include "memory/clrMemoryParam.h"
#include "type/clrLDString.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::BReader::BReader(::live2d::BReader* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::BReader^ Live2DSharp::BReader::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::BReader((::live2d::BReader*) native.ToPointer());
}

Live2DSharp::BReader::BReader(System::String^ buf, int length)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(buf);
    auto arg0 = _arg0.c_str();
    NativePtr = new ::live2d::BReader(arg0, length);
}

void Live2DSharp::BReader::rollback(int byteLen)
{
    ((::live2d::BReader*)NativePtr)->rollback(byteLen);
}

::System::IntPtr Live2DSharp::BReader::readObject(Live2DSharp::MemoryParam^ memParam, int cno, int flags)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    auto __ret = ((::live2d::BReader*)NativePtr)->readObject(arg0, cno, flags);
    if (__ret == nullptr) return System::IntPtr();
    return ::System::IntPtr(__ret);
}

bool Live2DSharp::BReader::readBit()
{
    auto __ret = ((::live2d::BReader*)NativePtr)->readBit();
    return __ret;
}

int Live2DSharp::BReader::readNum()
{
    auto __ret = ((::live2d::BReader*)NativePtr)->readNum();
    return __ret;
}

float Live2DSharp::BReader::readFloat()
{
    auto __ret = ((::live2d::BReader*)NativePtr)->readFloat();
    return __ret;
}

double Live2DSharp::BReader::readDouble()
{
    auto __ret = ((::live2d::BReader*)NativePtr)->readDouble();
    return __ret;
}

long long Live2DSharp::BReader::readLong()
{
    auto __ret = ((::live2d::BReader*)NativePtr)->readLong();
    return __ret;
}

int Live2DSharp::BReader::readInt()
{
    auto __ret = ((::live2d::BReader*)NativePtr)->readInt();
    return __ret;
}

bool Live2DSharp::BReader::readBoolean()
{
    auto __ret = ((::live2d::BReader*)NativePtr)->readBoolean();
    return __ret;
}

char Live2DSharp::BReader::readByte()
{
    auto __ret = ((::live2d::BReader*)NativePtr)->readByte();
    return __ret;
}

short Live2DSharp::BReader::readShort()
{
    auto __ret = ((::live2d::BReader*)NativePtr)->readShort();
    return __ret;
}

double* Live2DSharp::BReader::readArrayDouble(Live2DSharp::MemoryParam^ memParam, int* ret_length)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    auto arg1 = (int*)ret_length;
    auto __ret = ((::live2d::BReader*)NativePtr)->readArrayDouble(arg0, arg1);
    return __ret;
}

float* Live2DSharp::BReader::readArrayFloat(Live2DSharp::MemoryParam^ memParam, int* ret_length)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    auto arg1 = (int*)ret_length;
    auto __ret = ((::live2d::BReader*)NativePtr)->readArrayFloat(arg0, arg1);
    return __ret;
}

int* Live2DSharp::BReader::readArrayInt(Live2DSharp::MemoryParam^ memParam, int* ret_length)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    auto arg1 = (int*)ret_length;
    auto __ret = ((::live2d::BReader*)NativePtr)->readArrayInt(arg0, arg1);
    return __ret;
}

unsigned short* Live2DSharp::BReader::readArrayIntAsUShort(Live2DSharp::MemoryParam^ memParam, int* ret_length)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    auto arg1 = (int*)ret_length;
    auto __ret = ((::live2d::BReader*)NativePtr)->readArrayIntAsUShort(arg0, arg1);
    return __ret;
}

Live2DSharp::LDString^ Live2DSharp::BReader::readString(Live2DSharp::MemoryParam^ memParam)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    auto __ret = ((::live2d::BReader*)NativePtr)->readString(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::LDString((::live2d::LDString*)__ret);
}

Live2DSharp::RefString^ Live2DSharp::BReader::readStringAsRef()
{
    auto &__ret = ((::live2d::BReader*)NativePtr)->readStringAsRef();
    return (Live2DSharp::RefString^)((&__ret == nullptr) ? nullptr : gcnew Live2DSharp::RefString((::live2d::RefString*)&__ret));
}

void Live2DSharp::BReader::skip(int bytes)
{
    ((::live2d::BReader*)NativePtr)->skip(bytes);
}

int Live2DSharp::BReader::FormatVersion::get()
{
    auto __ret = ((::live2d::BReader*)NativePtr)->getFormatVersion();
    return __ret;
}

void Live2DSharp::BReader::FormatVersion::set(int version)
{
    ((::live2d::BReader*)NativePtr)->setFormatVersion(version);
}

char* Live2DSharp::BReader::CurPtr::get()
{
    auto __ret = ((::live2d::BReader*)NativePtr)->getCurPtr();
    return __ret;
}

void Live2DSharp::BReader::Endian::set(bool isBigEndian)
{
    ((::live2d::BReader*)NativePtr)->setEndian(isBigEndian);
}

void Live2DSharp::BReader::ChangeEndian::set(bool change)
{
    ((::live2d::BReader*)NativePtr)->setChangeEndian(change);
}

int Live2DSharp::BReader::FLAG_READ_AS_USHORT_ARRAY::get()
{
    return ::live2d::BReader::FLAG_READ_AS_USHORT_ARRAY;
}

int Live2DSharp::BReader::FLAG_MALLOC_ON_GPU::get()
{
    return ::live2d::BReader::FLAG_MALLOC_ON_GPU;
}

int Live2DSharp::BReader::LOAD_OBJECT_INITIAL_CAPACITY::get()
{
    return ::live2d::BReader::LOAD_OBJECT_INITIAL_CAPACITY;
}

