#pragma once

#include "CppSharp.h"
#include <ModelContext.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class ALive2DModel;
    ref class AMemoryHolder;
    ref class BaseDataID;
    ref class DrawDataID;
    ref class DrawParam;
    ref class IBaseContext;
    ref class IBaseData;
    ref class IDrawContext;
    ref class IDrawData;
    ref class MemoryParam;
    ref class ModelContext;
    ref class ParamID;
    ref class PartsData;
    ref class PartsDataContext;
    ref class PartsDataID;
}

namespace Live2DSharp
{
    public ref class ModelContext : Live2DSharp::LDObject
    {
    public:

        ModelContext(::live2d::ModelContext* native);
        static ModelContext^ __CreateInstance(::System::IntPtr native);
        ModelContext(Live2DSharp::ALive2DModel^ model);

        property Live2DSharp::MemoryParam^ MemoryParam
        {
            Live2DSharp::MemoryParam^ get();
        }

        property int InitVersion
        {
            int get();
        }

        property unsigned short* TmpPivotTableIndicesRef
        {
            unsigned short* get();
        }

        property float* TmpT_ArrayRef
        {
            float* get();
        }

        property int BaseDataCount
        {
            int get();
        }

        property int DrawDataCount
        {
            int get();
        }

        property int PartsDataCount
        {
            int get();
        }

        property bool update
        {
            bool get();
        }

        void release();

        void init();

        bool requireSetup();

        void draw(Live2DSharp::DrawParam^ dp);

        bool isParamUpdated(int paramIndex);

        int getParamIndex(Live2DSharp::ParamID^ paramID);

        int getBaseDataIndex(Live2DSharp::BaseDataID^ baseID);

        int getPartsDataIndex(Live2DSharp::PartsDataID^ partsID);

        int getDrawDataIndex(Live2DSharp::DrawDataID^ drawDataID);

        int addFloatParam(Live2DSharp::ParamID^ id, float value, float min, float max);

        void setBaseData(unsigned int baseDataIndex, Live2DSharp::IBaseData^ baseData);

        void setParamFloat(unsigned int paramIndex, float value);

        float getParamMax(unsigned int paramIndex);

        float getParamMin(unsigned int paramIndex);

        void loadParam();

        void saveParam();

        void setPartsOpacity(int partIndex, float opacity);

        float getPartsOpacity(int partIndex);

        Live2DSharp::IBaseData^ getBaseData(unsigned int baseDataIndex);

        Live2DSharp::IDrawData^ getDrawData(unsigned int drawDataIndex);

        Live2DSharp::IBaseContext^ getBaseContext(unsigned int baseDataIndex);

        Live2DSharp::IDrawContext^ getDrawContext(unsigned int drawDataIndex);

        Live2DSharp::PartsDataContext^ getPartsContext(unsigned int partsDataIndex);

        float getParamFloat(unsigned int paramIndex);

        void deviceLost();

        void updateZBuffer_TestImpl(float startZ, float stepZ);

        static property unsigned short NOT_USED_ORDER
        {
            unsigned short get();
        }

        static property unsigned short NO_NEXT
        {
            unsigned short get();
        }
    };
}
