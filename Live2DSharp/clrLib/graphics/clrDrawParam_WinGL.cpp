#include "graphics/clrDrawParam_WinGL.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::DrawParam_WinGL::DrawParam_WinGL(::live2d::DrawParam_WinGL* native)
    : Live2DSharp::DrawParam((::live2d::DrawParam*)native)
{
}

Live2DSharp::DrawParam_WinGL^ Live2DSharp::DrawParam_WinGL::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::DrawParam_WinGL((::live2d::DrawParam_WinGL*) native.ToPointer());
}

Live2DSharp::DrawParam_WinGL::DrawParam_WinGL()
    : Live2DSharp::DrawParam((::live2d::DrawParam*)nullptr)
{
    NativePtr = new ::live2d::DrawParam_WinGL();
}

void Live2DSharp::DrawParam_WinGL::drawTexture(int textureNo, int indexCount, int vertexCount, unsigned short* indexArray, float* vertexArray, float* uvArray, float opacity, int colorCompositionType)
{
    auto arg3 = (::l2d_index*)indexArray;
    auto arg4 = (::l2d_pointf*)vertexArray;
    auto arg5 = (::l2d_uvmapf*)uvArray;
    ((::live2d::DrawParam_WinGL*)NativePtr)->drawTexture(textureNo, indexCount, vertexCount, arg3, arg4, arg5, opacity, colorCompositionType);
}

void Live2DSharp::DrawParam_WinGL::initGLFunc()
{
    ::live2d::DrawParam_WinGL::initGLFunc();
}

void Live2DSharp::DrawParam_WinGL::setupDraw()
{
    ((::live2d::DrawParam_WinGL*)NativePtr)->setupDraw();
}

void Live2DSharp::DrawParam_WinGL::setTexture(int modelTextureNo, unsigned int textureNo)
{
    auto arg1 = (::GLuint)textureNo;
    ((::live2d::DrawParam_WinGL*)NativePtr)->setTexture(modelTextureNo, arg1);
}

int Live2DSharp::DrawParam_WinGL::generateModelTextureNo()
{
    auto __ret = ((::live2d::DrawParam_WinGL*)NativePtr)->generateModelTextureNo();
    return __ret;
}

void Live2DSharp::DrawParam_WinGL::releaseModelTextureNo(int no)
{
    ((::live2d::DrawParam_WinGL*)NativePtr)->releaseModelTextureNo(no);
}

int Live2DSharp::DrawParam_WinGL::___checkError___(System::String^ str)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(str);
    auto arg0 = _arg0.c_str();
    auto __ret = ::live2d::DrawParam_WinGL::___checkError___(arg0);
    return __ret;
}

bool Live2DSharp::DrawParam_WinGL::initGLFuncSuccess::get()
{
    return ::live2d::DrawParam_WinGL::initGLFuncSuccess;
}

void Live2DSharp::DrawParam_WinGL::initGLFuncSuccess::set(bool value)
{
    ::live2d::DrawParam_WinGL::initGLFuncSuccess = value;
}

