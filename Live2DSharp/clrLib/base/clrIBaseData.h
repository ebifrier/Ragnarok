#pragma once

#include "CppSharp.h"
#include <base/IBaseData.h>
#include "io/clrISerializableV2.h"

namespace Live2DSharp
{
    ref class BReader;
    ref class BaseDataID;
    ref class IBaseContext;
    ref class IBaseData;
    ref class MemoryParam;
    ref class ModelContext;
    ref class PivotManager;
}

namespace Live2DSharp
{
    public ref class IBaseData : Live2DSharp::ISerializableV2
    {
    public:

        IBaseData(::live2d::IBaseData* native);
        static IBaseData^ __CreateInstance(::System::IntPtr native);
        IBaseData();

        property Live2DSharp::BaseDataID^ TargetBaseDataID
        {
            Live2DSharp::BaseDataID^ get();
            void set(Live2DSharp::BaseDataID^);
        }

        property Live2DSharp::BaseDataID^ BaseDataID
        {
            Live2DSharp::BaseDataID^ get();
            void set(Live2DSharp::BaseDataID^);
        }

        property int Type
        {
            int get();
        }

        property bool needTransform
        {
            bool get();
        }

        virtual Live2DSharp::IBaseContext^ init(Live2DSharp::ModelContext^ mdc);

        virtual void readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam) override;

        virtual void setupInterpolate(Live2DSharp::ModelContext^ mdc, Live2DSharp::IBaseContext^ cdata);

        virtual void setupTransform(Live2DSharp::ModelContext^ mdc, Live2DSharp::IBaseContext^ cdata);

        virtual void transformPoints(Live2DSharp::ModelContext^ mdc, Live2DSharp::IBaseContext^ cdata, float* srcPoints, float* dstPoints, int numPoint, int pt_offset, int pt_step);

        static property int BASE_INDEX_NOT_INIT
        {
            int get();
        }

        static property int TYPE_BD_AFFINE
        {
            int get();
        }

        static property int TYPE_BD_BOX_GRID
        {
            int get();
        }
    };
}
