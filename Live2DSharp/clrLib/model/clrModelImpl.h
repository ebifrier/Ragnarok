#pragma once

#include "CppSharp.h"
#include <model/ModelImpl.h>
#include "io/clrISerializableV2.h"

namespace Live2DSharp
{
    ref class AMemoryHolder;
    ref class BReader;
    ref class IBaseData;
    ref class MemoryParam;
    ref class ModelImpl;
    ref class ParamDefSet;
    ref class PartsData;
}

namespace Live2DSharp
{
    public ref class ModelImpl : Live2DSharp::ISerializableV2
    {
    public:

        ModelImpl(::live2d::ModelImpl* native);
        static ModelImpl^ __CreateInstance(::System::IntPtr native);
        ModelImpl();

        property Live2DSharp::ParamDefSet^ ParamDefSet
        {
            Live2DSharp::ParamDefSet^ get();
        }

        property float CanvasWidth
        {
            float get();
        }

        property float CanvasHeight
        {
            float get();
        }

        void initDirect();

        virtual void readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam) override;

        void addPartsData(Live2DSharp::PartsData^ parts);

        static property int INSTANCE_COUNT
        {
            int get();
            void set(int);
        }
    };
}
