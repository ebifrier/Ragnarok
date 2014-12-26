#pragma once

#include "CppSharp.h"
#include <type/LDAffineTransform.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class LDAffineTransform;
}

namespace Live2DSharp
{
    public ref class LDAffineTransform : Live2DSharp::LDObject
    {
    public:

        LDAffineTransform(::live2d::LDAffineTransform* native);
        static LDAffineTransform^ __CreateInstance(::System::IntPtr native);
        LDAffineTransform();

        LDAffineTransform(float m1, float m2, float m3, float m4, float m5, float m6);

        property float* Factor
        {
            void set(float*);
        }

        void factorize(float* ret);

        void getMatrix(float* ret);

        void transform(float* src, float* dst, int numPoint);

        static void interpolate(Live2DSharp::LDAffineTransform^ aa1, Live2DSharp::LDAffineTransform^ aa2, float t, Live2DSharp::LDAffineTransform^ ret);
    };
}
