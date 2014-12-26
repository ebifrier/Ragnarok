#pragma once

#include "CppSharp.h"
#include <graphics/DrawParam_WinGL.h>
#include "graphics/clrDrawParam.h"

namespace Live2DSharp
{
    ref class DrawParam_WinGL;
}

namespace Live2DSharp
{
    /// <summary>
    /// WindowsのOpenGL用描画クラス
    /// ******************************************************************
    /// </summary>
    public ref class DrawParam_WinGL : Live2DSharp::DrawParam
    {
    public:

        DrawParam_WinGL(::live2d::DrawParam_WinGL* native);
        static DrawParam_WinGL^ __CreateInstance(::System::IntPtr native);
        DrawParam_WinGL();

        virtual void drawTexture(int textureNo, int indexCount, int vertexCount, unsigned short* indexArray, float* vertexArray, float* uvArray, float opacity, int colorCompositionType) override;

        virtual void setupDraw() override;

        void setTexture(int modelTextureNo, unsigned int textureNo);

        virtual int generateModelTextureNo() override;

        virtual void releaseModelTextureNo(int no) override;

        static void initGLFunc();

        static int ___checkError___(System::String^ str);

        static property bool initGLFuncSuccess
        {
            bool get();
            void set(bool);
        }
    };
}
