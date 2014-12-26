#pragma once

#include "CppSharp.h"
#include <param/PivotManager.h>
#include "io/clrISerializableV2.h"

namespace Live2DSharp
{
    ref class BReader;
    ref class MemoryParam;
    ref class ModelContext;
    ref class ParamID;
    ref class ParamPivots;
    ref class PivotManager;
}

namespace Live2DSharp
{
    public ref class PivotManager : Live2DSharp::ISerializableV2
    {
    public:

        PivotManager(::live2d::PivotManager* native);
        static PivotManager^ __CreateInstance(::System::IntPtr native);
        PivotManager();

        property int ParamCount
        {
            int get();
        }

        virtual void readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam) override;

        void initDirect(Live2DSharp::MemoryParam^ memParam);

        int calcPivotValue(Live2DSharp::ModelContext^ mdc, bool* ret_paramOutside);

        void calcPivotIndexies(unsigned short* array64, float* tmpT_array, int interpolateCount);

        bool checkParamUpdated(Live2DSharp::ModelContext^ mdc);
    };
}
