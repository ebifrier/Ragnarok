#pragma once

#include "CppSharp.h"
#include <param/ParamDefSet.h>
#include "io/clrISerializableV2.h"

namespace Live2DSharp
{
    ref class BReader;
    ref class MemoryParam;
    ref class ParamDefFloat;
    ref class ParamDefSet;
}

namespace Live2DSharp
{
    public ref class ParamDefSet : Live2DSharp::ISerializableV2
    {
    public:

        ParamDefSet(::live2d::ParamDefSet* native);
        static ParamDefSet^ __CreateInstance(::System::IntPtr native);
        ParamDefSet();

        void initDirect(Live2DSharp::MemoryParam^ memParam);

        virtual void readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam) override;
    };
}
