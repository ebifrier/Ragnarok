#pragma once

#include "CppSharp.h"
#include <Live2DModelOpenGL.h>
#include "./clrALive2DModel.h"

namespace Live2DSharp
{
    ref class DrawParam;
    ref class LDString;
    ref class Live2DModelOpenGL;
    ref class ModelContext;
}

namespace Live2DSharp
{
    public ref class Live2DModelOpenGL : Live2DSharp::ALive2DModel
    {
    public:

        Live2DModelOpenGL(::live2d::Live2DModelOpenGL* native);
        static Live2DModelOpenGL^ __CreateInstance(::System::IntPtr native);
        Live2DModelOpenGL();

        property Live2DSharp::DrawParam^ DrawParam
        {
            Live2DSharp::DrawParam^ get();
        }

        property float* Matrix
        {
            void set(float*);
        }

        virtual void draw() override;

        void setTexture(int textureNo, unsigned int openGLTextureNo);

        virtual int generateModelTextureNo() override;

        virtual void releaseModelTextureNo(int no) override;

        void setTextureColor(int textureNo, float r, float g, float b, float scale);

        static Live2DSharp::Live2DModelOpenGL^ loadModel(Live2DSharp::LDString^ filepath);

        static Live2DSharp::Live2DModelOpenGL^ loadModel(::System::IntPtr buf, int bufSize);
    };
}
