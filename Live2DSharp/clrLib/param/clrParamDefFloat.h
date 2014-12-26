#pragma once

#include "CppSharp.h"
#include <param/ParamDefFloat.h>
#include "io/clrISerializableV2.h"

namespace Live2DSharp
{
    ref class BReader;
    ref class MemoryParam;
    ref class ParamDefFloat;
    ref class ParamID;
}

namespace Live2DSharp
{
    public ref class ParamDefFloat : Live2DSharp::ISerializableV2
    {
    public:

        ParamDefFloat(::live2d::ParamDefFloat* native);
        static ParamDefFloat^ __CreateInstance(::System::IntPtr native);
        ParamDefFloat();

        property float MinValue
        {
            float get();
        }

        property float MaxValue
        {
            float get();
        }

        property float DefaultValue
        {
            float get();
        }

        property Live2DSharp::ParamID^ ParamID
        {
            Live2DSharp::ParamID^ get();
        }

        virtual void readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam) override;
    };
}
