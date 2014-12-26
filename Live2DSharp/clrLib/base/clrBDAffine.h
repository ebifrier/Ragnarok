#pragma once

#include "CppSharp.h"
#include <base/BDAffine.h>
#include "base/clrIBaseContext.h"
#include "base/clrIBaseData.h"

namespace Live2DSharp
{
    ref class AffineEnt;
    ref class BDAffine;
    ref class BDAffineContext;
    ref class BReader;
    ref class MemoryParam;
    ref class ModelContext;
    ref class PivotManager;
}

namespace Live2DSharp
{
    public ref class BDAffine : Live2DSharp::IBaseData
    {
    public:

        BDAffine(::live2d::BDAffine* native);
        static BDAffine^ __CreateInstance(::System::IntPtr native);
        BDAffine();

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

    public ref class BDAffineContext : Live2DSharp::IBaseContext
    {
    public:

        BDAffineContext(::live2d::BDAffineContext* native);
        static BDAffineContext^ __CreateInstance(::System::IntPtr native);
        BDAffineContext(Live2DSharp::BDAffine^ src);

        property int tmpBaseDataIndex
        {
            int get();
            void set(int);
        }

        property Live2DSharp::AffineEnt^ interpolatedAffine
        {
            Live2DSharp::AffineEnt^ get();
            void set(Live2DSharp::AffineEnt^);
        }

        property Live2DSharp::AffineEnt^ transformedAffine
        {
            Live2DSharp::AffineEnt^ get();
            void set(Live2DSharp::AffineEnt^);
        }
    };
}
