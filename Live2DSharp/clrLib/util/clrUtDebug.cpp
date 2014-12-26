#include "util/clrUtDebug.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::DebugTimerObj::DebugTimerObj(::live2d::DebugTimerObj* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::DebugTimerObj^ Live2DSharp::DebugTimerObj::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::DebugTimerObj((::live2d::DebugTimerObj*) native.ToPointer());
}

Live2DSharp::DebugTimerObj::DebugTimerObj()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::DebugTimerObj();
}

System::String^ Live2DSharp::DebugTimerObj::key::get()
{
    return clix::marshalString<clix::E_UTF8>(((::live2d::DebugTimerObj*)NativePtr)->key);
}

void Live2DSharp::DebugTimerObj::key::set(System::String^ value)
{
    auto _value = clix::marshalString<clix::E_UTF8>(value);
    ((::live2d::DebugTimerObj*)NativePtr)->key = _value.c_str();
}

long long Live2DSharp::DebugTimerObj::startTimeMs::get()
{
    return ((::live2d::DebugTimerObj*)NativePtr)->startTimeMs;
}

void Live2DSharp::DebugTimerObj::startTimeMs::set(long long value)
{
    ((::live2d::DebugTimerObj*)NativePtr)->startTimeMs = value;
}

Live2DSharp::UtDebug::UtDebug(::live2d::UtDebug* native)
{
    NativePtr = native;
}

Live2DSharp::UtDebug^ Live2DSharp::UtDebug::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::UtDebug((::live2d::UtDebug*) native.ToPointer());
}

void Live2DSharp::UtDebug::assertF(System::String^ file, int lineno, System::String^ format)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(file);
    auto arg0 = _arg0.c_str();
    auto _arg2 = clix::marshalString<clix::E_UTF8>(format);
    auto arg2 = _arg2.c_str();
    ::live2d::UtDebug::assertF(arg0, lineno, arg2);
}

void Live2DSharp::UtDebug::error(System::String^ msg)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(msg);
    auto arg0 = _arg0.c_str();
    ::live2d::UtDebug::error(arg0);
}

void Live2DSharp::UtDebug::dumpByte(System::String^ data, int length)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(data);
    auto arg0 = _arg0.c_str();
    ::live2d::UtDebug::dumpByte(arg0, length);
}

void Live2DSharp::UtDebug::print(System::String^ format)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(format);
    auto arg0 = _arg0.c_str();
    ::live2d::UtDebug::print(arg0);
}

void Live2DSharp::UtDebug::println(System::String^ format)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(format);
    auto arg0 = _arg0.c_str();
    ::live2d::UtDebug::println(arg0);
}

void Live2DSharp::UtDebug::debugBreak()
{
    ::live2d::UtDebug::debugBreak();
}

System::IntPtr Live2DSharp::UtDebug::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::UtDebug::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::UtDebug*)object.ToPointer();
}
unsigned int Live2DSharp::UtDebug::MEMORY_DEBUG_DUMP_ALLOCATOR::get()
{
    return ::live2d::UtDebug::MEMORY_DEBUG_DUMP_ALLOCATOR;
}

unsigned int Live2DSharp::UtDebug::MEMORY_DEBUG_DUMP_TMP::get()
{
    return ::live2d::UtDebug::MEMORY_DEBUG_DUMP_TMP;
}

unsigned int Live2DSharp::UtDebug::MEMORY_DEBUG_DUMP_FIXED::get()
{
    return ::live2d::UtDebug::MEMORY_DEBUG_DUMP_FIXED;
}

unsigned int Live2DSharp::UtDebug::MEMORY_DEBUG_DUMP_UNMANAGED::get()
{
    return ::live2d::UtDebug::MEMORY_DEBUG_DUMP_UNMANAGED;
}

unsigned int Live2DSharp::UtDebug::MEMORY_DEBUG_MEMORY_INFO_COUNT::get()
{
    return ::live2d::UtDebug::MEMORY_DEBUG_MEMORY_INFO_COUNT;
}

unsigned int Live2DSharp::UtDebug::MEMORY_DEBUG_MEMORY_INFO_DUMP::get()
{
    return ::live2d::UtDebug::MEMORY_DEBUG_MEMORY_INFO_DUMP;
}

unsigned int Live2DSharp::UtDebug::MEMORY_DEBUG_MEMORY_INFO_ALL::get()
{
    return ::live2d::UtDebug::MEMORY_DEBUG_MEMORY_INFO_ALL;
}

unsigned int Live2DSharp::UtDebug::MEMORY_DEBUG_MEMORY_INFO_KEEP_FREE::get()
{
    return ::live2d::UtDebug::MEMORY_DEBUG_MEMORY_INFO_KEEP_FREE;
}

unsigned int Live2DSharp::UtDebug::MEMORY_DEBUG_MEMORY_DUMP_PLACEMENT_NEW::get()
{
    return ::live2d::UtDebug::MEMORY_DEBUG_MEMORY_DUMP_PLACEMENT_NEW;
}

unsigned int Live2DSharp::UtDebug::MEMORY_DEBUG_DUMP_ALL::get()
{
    return ::live2d::UtDebug::MEMORY_DEBUG_DUMP_ALL;
}

unsigned int Live2DSharp::UtDebug::READ_OBJECT_DEBUG_DUMP::get()
{
    return ::live2d::UtDebug::READ_OBJECT_DEBUG_DUMP;
}

