#pragma once

#include "CppSharp.h"
#include <draw/IDrawData.h>
#include "io/clrISerializableV2.h"

namespace Live2DSharp
{
    ref class BReader;
    ref class BaseDataID;
    ref class DrawDataID;
    ref class DrawParam;
    ref class IDrawContext;
    ref class IDrawData;
    ref class MemoryParam;
    ref class ModelContext;
    ref class PivotManager;
}

namespace Live2DSharp
{
    public ref class IDrawData : Live2DSharp::ISerializableV2
    {
    public:

        IDrawData(::live2d::IDrawData* native);
        static IDrawData^ __CreateInstance(::System::IntPtr native);
        IDrawData();

        property Live2DSharp::BaseDataID^ TargetBaseDataID
        {
            Live2DSharp::BaseDataID^ get();
            void set(Live2DSharp::BaseDataID^);
        }

        property Live2DSharp::DrawDataID^ DrawDataID
        {
            Live2DSharp::DrawDataID^ get();
            void set(Live2DSharp::DrawDataID^);
        }

        static property int TotalMinOrder
        {
            int get();
        }

        static property int TotalMaxOrder
        {
            int get();
        }

        property int Type
        {
            int get();
        }

        property bool needTransform
        {
            bool get();
        }

        virtual void readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam) override;

        float getOpacity(Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ cdata);

        int getDrawOrder(Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ cdata);

        virtual Live2DSharp::IDrawContext^ init(Live2DSharp::ModelContext^ mdc);

        virtual void setupInterpolate(Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ cdata);

        virtual void setupTransform(Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ cdata);

        virtual void draw(Live2DSharp::DrawParam^ dp, Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ cdata);

        virtual void deviceLost(Live2DSharp::IDrawContext^ drawContext);

        virtual void setZ_TestImpl(Live2DSharp::ModelContext^ mdc, Live2DSharp::IDrawContext^ _cdata, float z);

        static property int BASE_INDEX_NOT_INIT
        {
            int get();
        }

        static property int DEFAULT_ORDER
        {
            int get();
        }

        static property int TYPE_DD_TEXTURE
        {
            int get();
        }
    };
}
