#pragma once

#include "CppSharp.h"
#include <base/BDBoxGrid.h>
#include "base/clrIBaseContext.h"
#include "base/clrIBaseData.h"

namespace Live2DSharp
{
    ref class BDBoxGrid;
    ref class BDBoxGridContext;
    ref class BReader;
    ref class MemoryParam;
    ref class ModelContext;
    ref class PivotManager;
}

namespace Live2DSharp
{
    public ref class BDBoxGrid : Live2DSharp::IBaseData
    {
    public:

        BDBoxGrid(::live2d::BDBoxGrid* native);
        static BDBoxGrid^ __CreateInstance(::System::IntPtr native);
        BDBoxGrid();

        property int NumPts
        {
            int get();
        }

        property int Type
        {
            int get();
        }

        void initDirect(Live2DSharp::MemoryParam^ memParam);

        virtual void readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam) override;

        virtual Live2DSharp::IBaseContext^ init(Live2DSharp::ModelContext^ mdc) override;

        virtual void setupInterpolate(Live2DSharp::ModelContext^ mdc, Live2DSharp::IBaseContext^ cdata) override;

        virtual void setupTransform(Live2DSharp::ModelContext^ mdc, Live2DSharp::IBaseContext^ cdata) override;

        virtual void transformPoints(Live2DSharp::ModelContext^ mdc, Live2DSharp::IBaseContext^ cdata, float* srcPoints, float* dstPoints, int numPoint, int pt_offset, int pt_step) override;
    };

    public ref class BDBoxGridContext : Live2DSharp::IBaseContext
    {
    public:

        BDBoxGridContext(::live2d::BDBoxGridContext* native);
        static BDBoxGridContext^ __CreateInstance(::System::IntPtr native);
        BDBoxGridContext(Live2DSharp::BDBoxGrid^ src);

        property int tmpBaseDataIndex
        {
            int get();
            void set(int);
        }

        property float* interpolatedPoints
        {
            float* get();
            void set(float*);
        }

        property float* transformedPoints
        {
            float* get();
            void set(float*);
        }
    };
}
