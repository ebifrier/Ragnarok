#pragma once

#include "CppSharp.h"
#include <graphics/DrawParam.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class DrawDataID;
    ref class DrawParam;
    ref class TextureInfo;
}

namespace Live2DSharp
{
    public ref class TextureInfo : Live2DSharp::LDObject
    {
    public:

        TextureInfo(::live2d::TextureInfo* native);
        static TextureInfo^ __CreateInstance(::System::IntPtr native);
        TextureInfo();

        property float a
        {
            float get();
            void set(float);
        }

        property float r
        {
            float get();
            void set(float);
        }

        property float g
        {
            float get();
            void set(float);
        }

        property float b
        {
            float get();
            void set(float);
        }

        property float scale
        {
            float get();
            void set(float);
        }

        property float interpolate
        {
            float get();
            void set(float);
        }

        property int blendMode
        {
            int get();
            void set(int);
        }
    };

    public ref class DrawParam : Live2DSharp::LDObject
    {
    public:

        DrawParam(::live2d::DrawParam* native);
        static DrawParam^ __CreateInstance(::System::IntPtr native);
        DrawParam();

        property bool Culling
        {
            void set(bool);
        }

        property float* Matrix
        {
            void set(float*);
        }

        property bool PremultipliedAlpha
        {
            void set(bool);
        }

        property int Anisotropy
        {
            int get();
            void set(int);
        }

        property Live2DSharp::DrawDataID^ pCurrentDrawDataID
        {
            Live2DSharp::DrawDataID^ get();
            void set(Live2DSharp::DrawDataID^);
        }

        property bool isPremultipliedAlpha
        {
            bool get();
        }

        virtual void setupDraw();

        virtual void drawTexture(int textureNo, int indexCount, int vertexCount, unsigned short* indexArray, float* vertexArray, float* uvArray, float opacity, int colorCompositionType);

        virtual int generateModelTextureNo();

        virtual void releaseModelTextureNo(int no);

        virtual void setBaseColor(float alpha, float red, float green, float blue);

        void setTextureColor(int textureNo, float r, float g, float b, float a);

        void setTextureScale(int textureNo, float scale);

        void setTextureInterpolate(int textureNo, float interpolate);

        void setTextureBlendMode(int textureNo, int mode);

        bool enabledTextureInfo(int textureNo);

        static property int DEFAULT_FIXED_TEXTURE_COUNT
        {
            int get();
        }
    };
}
