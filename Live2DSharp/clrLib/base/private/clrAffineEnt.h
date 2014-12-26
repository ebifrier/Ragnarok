#pragma once

#include "CppSharp.h"
#include <base/private/AffineEnt.h>
#include "io/clrISerializableV2.h"

namespace Live2DSharp
{
    ref class AffineEnt;
    ref class BReader;
    ref class MemoryParam;
}

namespace Live2DSharp
{
    public ref class AffineEnt : Live2DSharp::ISerializableV2
    {
    public:

        AffineEnt(::live2d::AffineEnt* native);
        static AffineEnt^ __CreateInstance(::System::IntPtr native);
        AffineEnt();

        property float originX
        {
            float get();
            void set(float);
        }

        property float originY
        {
            float get();
            void set(float);
        }

        property float scaleX
        {
            float get();
            void set(float);
        }

        property float scaleY
        {
            float get();
            void set(float);
        }

        property float rotateDeg
        {
            float get();
            void set(float);
        }

        property bool reflectX
        {
            bool get();
            void set(bool);
        }

        property bool reflectY
        {
            bool get();
            void set(bool);
        }

        void init(Live2DSharp::AffineEnt^ ent);

        virtual void readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam) override;

        void DUMP();
    };
}
