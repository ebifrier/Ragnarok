#pragma once

#include "CppSharp.h"
#include <ALive2DModel.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class ALive2DModel;
    ref class DrawParam;
    ref class IDrawData;
    ref class LDString;
    ref class ModelContext;
    ref class ModelImpl;
    ref class ParamID;
    ref class PartsDataID;
}

namespace Live2DSharp
{
    public ref class ALive2DModel : Live2DSharp::LDObject
    {
    public:

        ALive2DModel(::live2d::ALive2DModel* native);
        static ALive2DModel^ __CreateInstance(::System::IntPtr native);
        ALive2DModel();

        property Live2DSharp::ModelImpl^ ModelImpl
        {
            Live2DSharp::ModelImpl^ get();
            void set(Live2DSharp::ModelImpl^);
        }

        property Live2DSharp::ModelContext^ ModelContext
        {
            Live2DSharp::ModelContext^ get();
        }

        property int ErrorFlags
        {
            int get();
        }

        property float CanvasWidth
        {
            float get();
        }

        property float CanvasHeight
        {
            float get();
        }

        property Live2DSharp::DrawParam^ DrawParam
        {
            Live2DSharp::DrawParam^ get();
        }

        property bool PremultipliedAlpha
        {
            void set(bool);
        }

        property int Anisotropy
        {
            int get();
            void set(int);
        }

        property bool isPremultipliedAlpha
        {
            bool get();
        }

        float getParamFloat(System::String^ paramID);

        void setParamFloat(System::String^ paramID, float value, float weight);

        void addToParamFloat(System::String^ paramID, float value, float weight);

        void multParamFloat(System::String^ paramID, float mult, float weight);

        int getParamIndex(System::String^ paramID);

        int getParamIndex(Live2DSharp::ParamID^ paramID);

        float getParamFloat(int paramIndex);

        void setParamFloat(int paramIndex, float value, float weight);

        void addToParamFloat(int paramIndex, float value, float weight);

        void multParamFloat(int paramIndex, float mult, float weight);

        void loadParam();

        void saveParam();

        virtual void init();

        virtual void update();

        virtual void draw();

        void setPartsOpacity(System::String^ partsID, float opacity);

        void setPartsOpacity(int partsIndex, float opacity);

        float getPartsOpacity(System::String^ partsID);

        float getPartsOpacity(int partsIndex);

        virtual int generateModelTextureNo();

        virtual void releaseModelTextureNo(int no);

        virtual int getDrawDataIndex(System::String^ drawDataID);

        virtual Live2DSharp::IDrawData^ getDrawData(int drawIndex);

        virtual float* getTransformedPoints(int drawIndex, int* pointCount);

        virtual unsigned short* getIndexArray(int drawIndex, int* polygonCount);

        void updateZBuffer_TestImpl(float startZ, float stepZ);

        int getPartsDataIndex(System::String^ partsID);

        int getPartsDataIndex(Live2DSharp::PartsDataID^ partsID);

        static property int FILE_LOAD_EOF_ERROR
        {
            int get();
        }

        static property int FILE_LOAD_VERSION_ERROR
        {
            int get();
        }

        static property int INSTANCE_COUNT
        {
            int get();
            void set(int);
        }
    };
}
