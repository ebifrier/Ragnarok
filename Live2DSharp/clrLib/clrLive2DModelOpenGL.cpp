#include "./clrLive2DModelOpenGL.h"
#include "./clrModelContext.h"
#include "graphics/clrDrawParam.h"
#include "type/clrLDString.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::Live2DModelOpenGL::Live2DModelOpenGL(::live2d::Live2DModelOpenGL* native)
    : Live2DSharp::ALive2DModel((::live2d::ALive2DModel*)native)
{
}

Live2DSharp::Live2DModelOpenGL^ Live2DSharp::Live2DModelOpenGL::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::Live2DModelOpenGL((::live2d::Live2DModelOpenGL*) native.ToPointer());
}

Live2DSharp::Live2DModelOpenGL::Live2DModelOpenGL()
    : Live2DSharp::ALive2DModel((::live2d::ALive2DModel*)nullptr)
{
    NativePtr = new ::live2d::Live2DModelOpenGL();
}

void Live2DSharp::Live2DModelOpenGL::draw()
{
    ((::live2d::Live2DModelOpenGL*)NativePtr)->draw();
}

void Live2DSharp::Live2DModelOpenGL::setTexture(int textureNo, unsigned int openGLTextureNo)
{
    ((::live2d::Live2DModelOpenGL*)NativePtr)->setTexture(textureNo, openGLTextureNo);
}

Live2DSharp::Live2DModelOpenGL^ Live2DSharp::Live2DModelOpenGL::loadModel(Live2DSharp::LDString^ filepath)
{
    auto &arg0 = *(::live2d::LDString*)filepath->NativePtr;
    auto __ret = ::live2d::Live2DModelOpenGL::loadModel(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::Live2DModelOpenGL((::live2d::Live2DModelOpenGL*)__ret);
}

Live2DSharp::Live2DModelOpenGL^ Live2DSharp::Live2DModelOpenGL::loadModel(::System::IntPtr buf, int bufSize)
{
    auto arg0 = (const void*)buf;
    auto __ret = ::live2d::Live2DModelOpenGL::loadModel(arg0, bufSize);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::Live2DModelOpenGL((::live2d::Live2DModelOpenGL*)__ret);
}

int Live2DSharp::Live2DModelOpenGL::generateModelTextureNo()
{
    auto __ret = ((::live2d::Live2DModelOpenGL*)NativePtr)->generateModelTextureNo();
    return __ret;
}

void Live2DSharp::Live2DModelOpenGL::releaseModelTextureNo(int no)
{
    ((::live2d::Live2DModelOpenGL*)NativePtr)->releaseModelTextureNo(no);
}

void Live2DSharp::Live2DModelOpenGL::setTextureColor(int textureNo, float r, float g, float b, float scale)
{
    ((::live2d::Live2DModelOpenGL*)NativePtr)->setTextureColor(textureNo, r, g, b, scale);
}

Live2DSharp::DrawParam^ Live2DSharp::Live2DModelOpenGL::DrawParam::get()
{
    auto __ret = ((::live2d::Live2DModelOpenGL*)NativePtr)->getDrawParam();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::DrawParam((::live2d::DrawParam*)__ret);
}

void Live2DSharp::Live2DModelOpenGL::Matrix::set(float* matrix)
{
    auto arg0 = (float*)matrix;
    ((::live2d::Live2DModelOpenGL*)NativePtr)->setMatrix(arg0);
}

