#pragma once

#include "CppSharp.h"
#include <param/ParamPivots.h>
#include "io/clrISerializableV2.h"

namespace Live2DSharp
{
    ref class BReader;
    ref class MemoryParam;
    ref class ParamID;
    ref class ParamPivots;
}

namespace Live2DSharp
{
    /// <summary>
    /// パラメータのキーの管理クラス
    /// ***************************************************************************
    /// *****************
    /// </summary>
    public ref class ParamPivots : Live2DSharp::ISerializableV2
    {
    public:

        ParamPivots(::live2d::ParamPivots* native);
        static ParamPivots^ __CreateInstance(::System::IntPtr native);
        ParamPivots();

        property Live2DSharp::ParamID^ ParamID
        {
            Live2DSharp::ParamID^ get();
            void set(Live2DSharp::ParamID^);
        }

        property int PivotCount
        {
            int get();
        }

        property float* PivotValue
        {
            float* get();
        }

        property int TmpPivotIndex
        {
            int get();
            void set(int);
        }

        property float TmpT
        {
            float get();
            void set(float);
        }

        virtual void readV2(Live2DSharp::BReader^ br, Live2DSharp::MemoryParam^ memParam) override;

        int getParamIndex(int initVersion);

        void setParamIndex_(int index, int initVersion);

        void setPivotValue(int _pivotCount, float* _values);

        static property int PARAM_INDEX_NOT_INIT
        {
            int get();
        }
    };
}
