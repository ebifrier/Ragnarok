#include "./clrExpressionMotion.h"
#include "./clrALive2DModel.h"
#include "motion/clrMotionQueueEnt.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::Framework::ExpressionParam::ExpressionParam(::live2d::framework::ExpressionParam* native)
{
    NativePtr = native;
}

Live2DSharp::Framework::ExpressionParam^ Live2DSharp::Framework::ExpressionParam::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::Framework::ExpressionParam((::live2d::framework::ExpressionParam*) native.ToPointer());
}

Live2DSharp::Framework::ExpressionParam::ExpressionParam()
{
    NativePtr = new ::live2d::framework::ExpressionParam();
}

System::IntPtr Live2DSharp::Framework::ExpressionParam::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::Framework::ExpressionParam::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::framework::ExpressionParam*)object.ToPointer();
}

System::String^ Live2DSharp::Framework::ExpressionParam::pid::get()
{
    return clix::marshalString<clix::E_UTF8>(((::live2d::framework::ExpressionParam*)NativePtr)->pid);
}

void Live2DSharp::Framework::ExpressionParam::pid::set(System::String^ value)
{
    ((::live2d::framework::ExpressionParam*)NativePtr)->pid = clix::marshalString<clix::E_UTF8>(value);
}

Live2DSharp::Framework::ExpressionType Live2DSharp::Framework::ExpressionParam::type::get()
{
    return (Live2DSharp::Framework::ExpressionType)((::live2d::framework::ExpressionParam*)NativePtr)->type;
}

void Live2DSharp::Framework::ExpressionParam::type::set(Live2DSharp::Framework::ExpressionType value)
{
    ((::live2d::framework::ExpressionParam*)NativePtr)->type = (::live2d::framework::ExpressionType)value;
}

float Live2DSharp::Framework::ExpressionParam::value::get()
{
    return ((::live2d::framework::ExpressionParam*)NativePtr)->value;
}

void Live2DSharp::Framework::ExpressionParam::value::set(float value)
{
    ((::live2d::framework::ExpressionParam*)NativePtr)->value = value;
}

Live2DSharp::Framework::ExpressionMotion::ExpressionMotion(::live2d::framework::ExpressionMotion* native)
    : Live2DSharp::AMotion((::live2d::AMotion*)native)
{
}

Live2DSharp::Framework::ExpressionMotion^ Live2DSharp::Framework::ExpressionMotion::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::Framework::ExpressionMotion((::live2d::framework::ExpressionMotion*) native.ToPointer());
}

Live2DSharp::Framework::ExpressionMotion::ExpressionMotion()
    : Live2DSharp::AMotion((::live2d::AMotion*)nullptr)
{
    NativePtr = new ::live2d::framework::ExpressionMotion();
}

System::Collections::Generic::List<Live2DSharp::Framework::ExpressionParam^>^ Live2DSharp::Framework::ExpressionMotion::ParamList::get()
{
    auto &__ret = ((::live2d::framework::ExpressionMotion*)NativePtr)->getParamList();
    auto _tmp__ret = gcnew System::Collections::Generic::List<Live2DSharp::Framework::ExpressionParam^>();
    for(auto _element : __ret)
    {
        auto ___element = new ::live2d::framework::ExpressionParam(_element);
        auto _marshalElement = (___element == nullptr) ? nullptr : gcnew Live2DSharp::Framework::ExpressionParam((::live2d::framework::ExpressionParam*)___element);
        _tmp__ret->Add(_marshalElement);
    }
    return (System::Collections::Generic::List<Live2DSharp::Framework::ExpressionParam^>^)(_tmp__ret);
}

void Live2DSharp::Framework::ExpressionMotion::ParamList::set(System::Collections::Generic::List<Live2DSharp::Framework::ExpressionParam^>^ other)
{
    auto _tmpother = std::vector<::live2d::framework::ExpressionParam>();
    for each(Live2DSharp::Framework::ExpressionParam^ _element in other)
    {
        auto _marshalElement = *(::live2d::framework::ExpressionParam*)_element->NativePtr;
        _tmpother.push_back(_marshalElement);
    }
    auto arg0 = _tmpother;
    ((::live2d::framework::ExpressionMotion*)NativePtr)->setParamList(arg0);
}

