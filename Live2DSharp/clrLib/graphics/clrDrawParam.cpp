#include "graphics/clrDrawParam.h"
#include "id/clrDrawDataID.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::TextureInfo::TextureInfo(::live2d::TextureInfo* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::TextureInfo^ Live2DSharp::TextureInfo::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::TextureInfo((::live2d::TextureInfo*) native.ToPointer());
}

Live2DSharp::TextureInfo::TextureInfo()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::TextureInfo();
}

float Live2DSharp::TextureInfo::a::get()
{
    return ((::live2d::TextureInfo*)NativePtr)->a;
}

void Live2DSharp::TextureInfo::a::set(float value)
{
    ((::live2d::TextureInfo*)NativePtr)->a = value;
}

float Live2DSharp::TextureInfo::r::get()
{
    return ((::live2d::TextureInfo*)NativePtr)->r;
}

void Live2DSharp::TextureInfo::r::set(float value)
{
    ((::live2d::TextureInfo*)NativePtr)->r = value;
}

float Live2DSharp::TextureInfo::g::get()
{
    return ((::live2d::TextureInfo*)NativePtr)->g;
}

void Live2DSharp::TextureInfo::g::set(float value)
{
    ((::live2d::TextureInfo*)NativePtr)->g = value;
}

float Live2DSharp::TextureInfo::b::get()
{
    return ((::live2d::TextureInfo*)NativePtr)->b;
}

void Live2DSharp::TextureInfo::b::set(float value)
{
    ((::live2d::TextureInfo*)NativePtr)->b = value;
}

float Live2DSharp::TextureInfo::scale::get()
{
    return ((::live2d::TextureInfo*)NativePtr)->scale;
}

void Live2DSharp::TextureInfo::scale::set(float value)
{
    ((::live2d::TextureInfo*)NativePtr)->scale = value;
}

float Live2DSharp::TextureInfo::interpolate::get()
{
    return ((::live2d::TextureInfo*)NativePtr)->interpolate;
}

void Live2DSharp::TextureInfo::interpolate::set(float value)
{
    ((::live2d::TextureInfo*)NativePtr)->interpolate = value;
}

int Live2DSharp::TextureInfo::blendMode::get()
{
    return ((::live2d::TextureInfo*)NativePtr)->blendMode;
}

void Live2DSharp::TextureInfo::blendMode::set(int value)
{
    ((::live2d::TextureInfo*)NativePtr)->blendMode = value;
}

Live2DSharp::DrawParam::DrawParam(::live2d::DrawParam* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::DrawParam^ Live2DSharp::DrawParam::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::DrawParam((::live2d::DrawParam*) native.ToPointer());
}

Live2DSharp::DrawParam::DrawParam()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
}

void Live2DSharp::DrawParam::setupDraw()
{
    ((::live2d::DrawParam*)NativePtr)->setupDraw();
}

void Live2DSharp::DrawParam::drawTexture(int textureNo, int indexCount, int vertexCount, unsigned short* indexArray, float* vertexArray, float* uvArray, float opacity, int colorCompositionType)
{
    auto arg3 = (::l2d_index*)indexArray;
    auto arg4 = (::l2d_pointf*)vertexArray;
    auto arg5 = (::l2d_uvmapf*)uvArray;
    ((::live2d::DrawParam*)NativePtr)->drawTexture(textureNo, indexCount, vertexCount, arg3, arg4, arg5, opacity, colorCompositionType);
}

int Live2DSharp::DrawParam::generateModelTextureNo()
{
    auto __ret = ((::live2d::DrawParam*)NativePtr)->generateModelTextureNo();
    return __ret;
}

void Live2DSharp::DrawParam::releaseModelTextureNo(int no)
{
    ((::live2d::DrawParam*)NativePtr)->releaseModelTextureNo(no);
}

void Live2DSharp::DrawParam::setBaseColor(float alpha, float red, float green, float blue)
{
    ((::live2d::DrawParam*)NativePtr)->setBaseColor(alpha, red, green, blue);
}

void Live2DSharp::DrawParam::setTextureColor(int textureNo, float r, float g, float b, float a)
{
    ((::live2d::DrawParam*)NativePtr)->setTextureColor(textureNo, r, g, b, a);
}

void Live2DSharp::DrawParam::setTextureScale(int textureNo, float scale)
{
    ((::live2d::DrawParam*)NativePtr)->setTextureScale(textureNo, scale);
}

void Live2DSharp::DrawParam::setTextureInterpolate(int textureNo, float interpolate)
{
    ((::live2d::DrawParam*)NativePtr)->setTextureInterpolate(textureNo, interpolate);
}

void Live2DSharp::DrawParam::setTextureBlendMode(int textureNo, int mode)
{
    ((::live2d::DrawParam*)NativePtr)->setTextureBlendMode(textureNo, mode);
}

bool Live2DSharp::DrawParam::enabledTextureInfo(int textureNo)
{
    auto __ret = ((::live2d::DrawParam*)NativePtr)->enabledTextureInfo(textureNo);
    return __ret;
}

void Live2DSharp::DrawParam::Culling::set(bool culling)
{
    ((::live2d::DrawParam*)NativePtr)->setCulling(culling);
}

void Live2DSharp::DrawParam::Matrix::set(float* _matrix4x4)
{
    auto arg0 = (float*)_matrix4x4;
    ((::live2d::DrawParam*)NativePtr)->setMatrix(arg0);
}

void Live2DSharp::DrawParam::PremultipliedAlpha::set(bool enable)
{
    ((::live2d::DrawParam*)NativePtr)->setPremultipliedAlpha(enable);
}

int Live2DSharp::DrawParam::Anisotropy::get()
{
    auto __ret = ((::live2d::DrawParam*)NativePtr)->getAnisotropy();
    return __ret;
}

void Live2DSharp::DrawParam::Anisotropy::set(int n)
{
    ((::live2d::DrawParam*)NativePtr)->setAnisotropy(n);
}

Live2DSharp::DrawDataID^ Live2DSharp::DrawParam::pCurrentDrawDataID::get()
{
    auto __ret = ((::live2d::DrawParam*)NativePtr)->getpCurrentDrawDataID();
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::DrawDataID((::live2d::DrawDataID*)__ret);
}

void Live2DSharp::DrawParam::pCurrentDrawDataID::set(Live2DSharp::DrawDataID^ pDrawDataID)
{
    auto arg0 = (::live2d::DrawDataID*)pDrawDataID->NativePtr;
    ((::live2d::DrawParam*)NativePtr)->setpCurrentDrawDataID(arg0);
}

bool Live2DSharp::DrawParam::isPremultipliedAlpha::get()
{
    auto __ret = ((::live2d::DrawParam*)NativePtr)->isPremultipliedAlpha();
    return __ret;
}

int Live2DSharp::DrawParam::DEFAULT_FIXED_TEXTURE_COUNT::get()
{
    return ::live2d::DrawParam::DEFAULT_FIXED_TEXTURE_COUNT;
}

