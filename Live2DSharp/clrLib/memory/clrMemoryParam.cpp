#include "memory/clrMemoryParam.h"
#include "memory/clrAMemoryHolder.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::MemoryStackEnt::MemoryStackEnt(::live2d::MemoryStackEnt* native)
    : Live2DSharp::LDUnmanagedObject((::live2d::LDUnmanagedObject*)native)
{
}

Live2DSharp::MemoryStackEnt^ Live2DSharp::MemoryStackEnt::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::MemoryStackEnt((::live2d::MemoryStackEnt*) native.ToPointer());
}

Live2DSharp::MemoryStackEnt::MemoryStackEnt()
    : Live2DSharp::LDUnmanagedObject((::live2d::LDUnmanagedObject*)nullptr)
{
    NativePtr = new ::live2d::MemoryStackEnt();
}

Live2DSharp::MemoryParam::MemoryParam(::live2d::MemoryParam* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::MemoryParam^ Live2DSharp::MemoryParam::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::MemoryParam((::live2d::MemoryParam*) native.ToPointer());
}

Live2DSharp::MemoryParam::MemoryParam(Live2DSharp::AMemoryHolder^ main, Live2DSharp::AMemoryHolder^ gpu)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    auto arg0 = (::live2d::AMemoryHolder*)main->NativePtr;
    auto arg1 = (::live2d::AMemoryHolder*)gpu->NativePtr;
    NativePtr = new ::live2d::MemoryParam(arg0, arg1);
}

Live2DSharp::LDAllocator::Type Live2DSharp::MemoryParam::setAllocType(Live2DSharp::LDAllocator::Type allocType)
{
    auto arg0 = (::live2d::LDAllocator::Type)allocType;
    auto __ret = ((::live2d::MemoryParam*)NativePtr)->setAllocType(arg0);
    return (Live2DSharp::LDAllocator::Type)__ret;
}

int Live2DSharp::MemoryParam::setAllocAlign(int align)
{
    auto __ret = ((::live2d::MemoryParam*)NativePtr)->setAllocAlign(align);
    return __ret;
}

::System::IntPtr Live2DSharp::MemoryParam::malloc_exe(unsigned int size)
{
    auto arg0 = (::l2d_size_t)(::size_t)size;
    auto __ret = ((::live2d::MemoryParam*)NativePtr)->malloc_exe(arg0);
    if (__ret == nullptr) return System::IntPtr();
    return ::System::IntPtr(__ret);
}

Live2DSharp::AMemoryHolder^ Live2DSharp::MemoryParam::getMemoryHolder(Live2DSharp::LDAllocator::Type allocType)
{
    auto arg0 = (::live2d::LDAllocator::Type)allocType;
    auto __ret = ((::live2d::MemoryParam*)NativePtr)->getMemoryHolder(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::AMemoryHolder((::live2d::AMemoryHolder*)__ret);
}

void Live2DSharp::MemoryParam::clear()
{
    ((::live2d::MemoryParam*)NativePtr)->clear();
}

void Live2DSharp::MemoryParam::checkMemory()
{
    ((::live2d::MemoryParam*)NativePtr)->checkMemory();
}

Live2DSharp::LDAllocator::Type Live2DSharp::MemoryParam::AllocType::get()
{
    auto __ret = ((::live2d::MemoryParam*)NativePtr)->getAllocType();
    return (Live2DSharp::LDAllocator::Type)__ret;
}

int Live2DSharp::MemoryParam::AllocAlign::get()
{
    auto __ret = ((::live2d::MemoryParam*)NativePtr)->getAllocAlign();
    return __ret;
}

Live2DSharp::AMemoryHolder^ Live2DSharp::MemoryParam::CurMemoryHolder::get()
{
    auto __ret = ((::live2d::MemoryParam*)NativePtr)->getCurMemoryHolder();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::AMemoryHolder((::live2d::AMemoryHolder*)__ret);
}

void Live2DSharp::MemoryParam::MemoryHolderMain::set(Live2DSharp::AMemoryHolder^ h)
{
    auto arg0 = (::live2d::AMemoryHolder*)h->NativePtr;
    ((::live2d::MemoryParam*)NativePtr)->setMemoryHolderMain(arg0);
}

void Live2DSharp::MemoryParam::MemoryHolderGPU::set(Live2DSharp::AMemoryHolder^ h)
{
    auto arg0 = (::live2d::AMemoryHolder*)h->NativePtr;
    ((::live2d::MemoryParam*)NativePtr)->setMemoryHolderGPU(arg0);
}

