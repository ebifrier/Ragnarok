#include "io/clrByteBuffer.h"
#include "io/clrRefString.h"
#include "memory/clrMemoryParam.h"
#include "type/clrLDString.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::ByteBuffer::ByteBuffer(::live2d::ByteBuffer* native)
{
    NativePtr = native;
}

Live2DSharp::ByteBuffer^ Live2DSharp::ByteBuffer::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::ByteBuffer((::live2d::ByteBuffer*) native.ToPointer());
}

Live2DSharp::ByteBuffer::ByteBuffer(char* array, int length)
{
    auto arg0 = (char*)array;
    NativePtr = new ::live2d::ByteBuffer(arg0, length);
}

void Live2DSharp::ByteBuffer::rollback(int byteLen)
{
    ((::live2d::ByteBuffer*)NativePtr)->rollback(byteLen);
}

float Live2DSharp::ByteBuffer::readFloat()
{
    auto __ret = ((::live2d::ByteBuffer*)NativePtr)->readFloat();
    return __ret;
}

double Live2DSharp::ByteBuffer::readDouble()
{
    auto __ret = ((::live2d::ByteBuffer*)NativePtr)->readDouble();
    return __ret;
}

long long Live2DSharp::ByteBuffer::readLong()
{
    auto __ret = ((::live2d::ByteBuffer*)NativePtr)->readLong();
    return __ret;
}

int Live2DSharp::ByteBuffer::readNum()
{
    auto __ret = ((::live2d::ByteBuffer*)NativePtr)->readNum();
    return __ret;
}

int Live2DSharp::ByteBuffer::readInt()
{
    auto __ret = ((::live2d::ByteBuffer*)NativePtr)->readInt();
    return __ret;
}

bool Live2DSharp::ByteBuffer::readBoolean()
{
    auto __ret = ((::live2d::ByteBuffer*)NativePtr)->readBoolean();
    return __ret;
}

char Live2DSharp::ByteBuffer::readByte()
{
    auto __ret = ((::live2d::ByteBuffer*)NativePtr)->readByte();
    return __ret;
}

short Live2DSharp::ByteBuffer::readShort()
{
    auto __ret = ((::live2d::ByteBuffer*)NativePtr)->readShort();
    return __ret;
}

double* Live2DSharp::ByteBuffer::readArrayDouble(Live2DSharp::MemoryParam^ owner, int* ret_length)
{
    auto arg0 = (::live2d::MemoryParam*)owner->NativePtr;
    auto arg1 = (int*)ret_length;
    auto __ret = ((::live2d::ByteBuffer*)NativePtr)->readArrayDouble(arg0, arg1);
    return __ret;
}

float* Live2DSharp::ByteBuffer::readArrayFloat(Live2DSharp::MemoryParam^ owner, int* ret_length)
{
    auto arg0 = (::live2d::MemoryParam*)owner->NativePtr;
    auto arg1 = (int*)ret_length;
    auto __ret = ((::live2d::ByteBuffer*)NativePtr)->readArrayFloat(arg0, arg1);
    return __ret;
}

int* Live2DSharp::ByteBuffer::readArrayInt(Live2DSharp::MemoryParam^ owner, int* ret_length)
{
    auto arg0 = (::live2d::MemoryParam*)owner->NativePtr;
    auto arg1 = (int*)ret_length;
    auto __ret = ((::live2d::ByteBuffer*)NativePtr)->readArrayInt(arg0, arg1);
    return __ret;
}

unsigned short* Live2DSharp::ByteBuffer::readArrayIntAsUShort(Live2DSharp::MemoryParam^ owner, int* ret_length)
{
    auto arg0 = (::live2d::MemoryParam*)owner->NativePtr;
    auto arg1 = (int*)ret_length;
    auto __ret = ((::live2d::ByteBuffer*)NativePtr)->readArrayIntAsUShort(arg0, arg1);
    return __ret;
}

Live2DSharp::LDString^ Live2DSharp::ByteBuffer::readString(Live2DSharp::MemoryParam^ owner)
{
    auto arg0 = (::live2d::MemoryParam*)owner->NativePtr;
    auto __ret = ((::live2d::ByteBuffer*)NativePtr)->readString(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::LDString((::live2d::LDString*)__ret);
}

Live2DSharp::RefString^ Live2DSharp::ByteBuffer::readStringAsRef()
{
    auto &__ret = ((::live2d::ByteBuffer*)NativePtr)->readStringAsRef();
    return (Live2DSharp::RefString^)((&__ret == nullptr) ? nullptr : gcnew Live2DSharp::RefString((::live2d::RefString*)&__ret));
}

void Live2DSharp::ByteBuffer::skip(int bytes)
{
    ((::live2d::ByteBuffer*)NativePtr)->skip(bytes);
}

void Live2DSharp::ByteBuffer::staticInit_notForClientCall()
{
    ::live2d::ByteBuffer::staticInit_notForClientCall();
}

System::IntPtr Live2DSharp::ByteBuffer::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::ByteBuffer::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::ByteBuffer*)object.ToPointer();
}

char* Live2DSharp::ByteBuffer::CurPtr::get()
{
    auto __ret = ((::live2d::ByteBuffer*)NativePtr)->getCurPtr();
    return __ret;
}

void Live2DSharp::ByteBuffer::Endian::set(bool isBigEndian)
{
    ((::live2d::ByteBuffer*)NativePtr)->setEndian(isBigEndian);
}

void Live2DSharp::ByteBuffer::ChangeEndian::set(bool change)
{
    ((::live2d::ByteBuffer*)NativePtr)->setChangeEndian(change);
}

