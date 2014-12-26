#pragma once

#include "CppSharp.h"
#include <model/PartsData.h>
#include "io/clrISerializableV2.h"
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class BReader;
    ref class IBaseData;
    ref class IDrawData;
    ref class MemoryParam;
    ref class ModelContext;
    ref class PartsData;
    ref class PartsDataContext;
    ref class PartsDataID;
}

namespace Live2DSharp
{
    public ref class PartsData : Live2DSharp::ISerializableV2
    {
    public:

        PartsData(::live2d::PartsData* native);
        static PartsData^ __CreateInstance(::System::IntPtr native);
        PartsData();

        property bool Visible
        {
            void set(bool);
        }

        property bool Locked
        {
            void set(bool);
        }

        property Live2DSharp::PartsDataID^ PartsDataID
        {
            Live2DSharp::PartsDataID^ get();
            void set(Live2DSharp::PartsDataID^);
        }

        property Live2DSharp::PartsDataID^ PartsID
        {
            Live2DSharp::PartsDataID^ get();
            void set(Live2DSharp::PartsDataID^);
        }

        property bool isVisible
        {
            bool get();
        }

        property bool isLocked
        {
            bool get();
        }

        void initDirect(Live2DSharp::MemoryParam^ memParam);

        Live2DSharp::PartsDataContext^ init(Live2DSharp::ModelContext^ mdc);

        void addBaseData(Live2DSharp::IBaseData^ baseData);

        void addDrawData(Live2DSharp::IDrawData^ drawData);

        virtual void readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam) override;

        static property int INSTANCE_COUNT
        {
            int get();
            void set(int);
        }
    };

    public ref class PartsDataContext : Live2DSharp::LDObject
    {
    public:

        PartsDataContext(::live2d::PartsDataContext* native);
        static PartsDataContext^ __CreateInstance(::System::IntPtr native);
        PartsDataContext(Live2DSharp::PartsData^ src);

        property float PartsOpacity
        {
            float get();
            void set(float);
        }
    };
}
