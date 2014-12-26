#include "graphics/clrDrawProfileCocos2D.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::DrawProfileCocos2D::DrawProfileCocos2D(::live2d::DrawProfileCocos2D* native)
{
    NativePtr = native;
}

Live2DSharp::DrawProfileCocos2D^ Live2DSharp::DrawProfileCocos2D::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::DrawProfileCocos2D((::live2d::DrawProfileCocos2D*) native.ToPointer());
}

void Live2DSharp::DrawProfileCocos2D::preDraw()
{
    ::live2d::DrawProfileCocos2D::preDraw();
}

void Live2DSharp::DrawProfileCocos2D::postDraw()
{
    ::live2d::DrawProfileCocos2D::postDraw();
}

Live2DSharp::DrawProfileCocos2D::DrawProfileCocos2D()
{
    NativePtr = new ::live2d::DrawProfileCocos2D();
}

System::IntPtr Live2DSharp::DrawProfileCocos2D::__Instance::get()
{
    return System::IntPtr(NativePtr);
}

void Live2DSharp::DrawProfileCocos2D::__Instance::set(System::IntPtr object)
{
    NativePtr = (::live2d::DrawProfileCocos2D*)object.ToPointer();
}
