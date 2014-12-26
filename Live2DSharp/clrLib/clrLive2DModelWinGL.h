#pragma once

#include "CppSharp.h"
#include <Live2DModelWinGL.h>
#include "./clrALive2DModel.h"

namespace Live2DSharp
{
    ref class DrawParam;
    ref class DrawParam_WinGL;
    ref class LDString;
    ref class Live2DModelWinGL;
    ref class ModelContext;
}

namespace Live2DSharp
{
    public ref class Live2DModelWinGL : Live2DSharp::ALive2DModel
    {
    public:

        Live2DModelWinGL(::live2d::Live2DModelWinGL* native);
        static Live2DModelWinGL^ __CreateInstance(::System::IntPtr native);
        Live2DModelWinGL();

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

        static Live2DSharp::Live2DModelWinGL^ loadModel(Live2DSharp::LDString^ filepath);

        static Live2DSharp::Live2DModelWinGL^ loadModel(::System::IntPtr buf, int bufSize);
    };
}
