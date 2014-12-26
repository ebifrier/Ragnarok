#include "./clrLive2DModelWinGL.h"
#include "./clrModelContext.h"
#include "graphics/clrDrawParam.h"
#include "graphics/clrDrawParam_WinGL.h"
#include "type/clrLDString.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::Live2DModelWinGL::Live2DModelWinGL(::live2d::Live2DModelWinGL* native)
    : Live2DSharp::ALive2DModel((::live2d::ALive2DModel*)native)
{
}

Live2DSharp::Live2DModelWinGL^ Live2DSharp::Live2DModelWinGL::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::Live2DModelWinGL((::live2d::Live2DModelWinGL*) native.ToPointer());
}

Live2DSharp::Live2DModelWinGL::Live2DModelWinGL()
    : Live2DSharp::ALive2DModel((::live2d::ALive2DModel*)nullptr)
{
    NativePtr = new ::live2d::Live2DModelWinGL();
}

void Live2DSharp::Live2DModelWinGL::draw()
{
    ((::live2d::Live2DModelWinGL*)NativePtr)->draw();
}

void Live2DSharp::Live2DModelWinGL::setTexture(int textureNo, unsigned int openGLTextureNo)
{
    auto arg1 = (::GLuint)openGLTextureNo;
    ((::live2d::Live2DModelWinGL*)NativePtr)->setTexture(textureNo, arg1);
}

Live2DSharp::Live2DModelWinGL^ Live2DSharp::Live2DModelWinGL::loadModel(Live2DSharp::LDString^ filepath)
{
    auto &arg0 = *(::live2d::LDString*)filepath->NativePtr;
    auto __ret = ::live2d::Live2DModelWinGL::loadModel(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::Live2DModelWinGL((::live2d::Live2DModelWinGL*)__ret);
}

Live2DSharp::Live2DModelWinGL^ Live2DSharp::Live2DModelWinGL::loadModel(::System::IntPtr buf, int bufSize)
{
    auto arg0 = (const void*)buf;
    auto __ret = ::live2d::Live2DModelWinGL::loadModel(arg0, bufSize);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::Live2DModelWinGL((::live2d::Live2DModelWinGL*)__ret);
}

int Live2DSharp::Live2DModelWinGL::generateModelTextureNo()
{
    auto __ret = ((::live2d::Live2DModelWinGL*)NativePtr)->generateModelTextureNo();
    return __ret;
}

void Live2DSharp::Live2DModelWinGL::releaseModelTextureNo(int no)
{
    ((::live2d::Live2DModelWinGL*)NativePtr)->releaseModelTextureNo(no);
}

Live2DSharp::DrawParam^ Live2DSharp::Live2DModelWinGL::DrawParam::get()
{
    auto __ret = ((::live2d::Live2DModelWinGL*)NativePtr)->getDrawParam();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::DrawParam((::live2d::DrawParam*)__ret);
}

void Live2DSharp::Live2DModelWinGL::Matrix::set(float* matrix)
{
    auto arg0 = (float*)matrix;
    ((::live2d::Live2DModelWinGL*)NativePtr)->setMatrix(arg0);
}

