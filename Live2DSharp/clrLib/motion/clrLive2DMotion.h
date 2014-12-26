#pragma once

#include "CppSharp.h"
#include <motion/Live2DMotion.h>
#include "memory/clrLDObject.h"
#include "motion/clrAMotion.h"

namespace Live2DSharp
{
    ref class ALive2DModel;
    ref class AMemoryHolder;
    ref class LDString;
    ref class Live2DMotion;
    ref class MemoryParam;
    ref class Motion;
    ref class MotionQueueEnt;
    ref class ParamID;
}

namespace Live2DSharp
{
    public ref class Live2DMotion : Live2DSharp::AMotion
    {
    public:

        Live2DMotion(::live2d::Live2DMotion* native);
        static Live2DMotion^ __CreateInstance(::System::IntPtr native);
        Live2DMotion();

        property bool Loop
        {
            void set(bool);
        }

        property bool LoopFadeIn
        {
            void set(bool);
        }

        property int DurationMSec
        {
            int get();
        }

        property int LoopDurationMSec
        {
            int get();
        }

        property bool isLoop
        {
            bool get();
        }

        property bool isLoopFadeIn
        {
            bool get();
        }

        void dump();

        void setParamFadeIn(System::String^ paramID, int value);

        void setParamFadeOut(System::String^ paramID, int value);

        int getParamFadeIn(System::String^ paramID);

        int getParamFadeOut(System::String^ paramID);

        static Live2DSharp::Live2DMotion^ loadMotion(Live2DSharp::LDString^ filepath);

        static Live2DSharp::Live2DMotion^ loadMotion(::System::IntPtr buf, int bufSize);
    };

    /// <summary>
    /// *************************************************************************
    /// 一つのパラメータについてのアクション定義
    /// *************************************************************************
    /// </summary>
    public ref class Motion : Live2DSharp::LDObject
    {
    public:

        Motion(::live2d::Motion* native);
        static Motion^ __CreateInstance(::System::IntPtr native);
        Motion(Live2DSharp::MemoryParam^ memParam);

        property Live2DSharp::LDString^ paramIDStr
        {
            Live2DSharp::LDString^ get();
            void set(Live2DSharp::LDString^);
        }

        property Live2DSharp::ParamID^ cached_paramID
        {
            Live2DSharp::ParamID^ get();
            void set(Live2DSharp::ParamID^);
        }

        property int cached_paramIndex
        {
            int get();
            void set(int);
        }

        property Live2DSharp::ALive2DModel^ cached_model_ofParamIndex
        {
            Live2DSharp::ALive2DModel^ get();
            void set(Live2DSharp::ALive2DModel^);
        }

        property int motionType
        {
            int get();
            void set(int);
        }

        property int fadeInMsec
        {
            int get();
            void set(int);
        }

        property int fadeOutMsec
        {
            int get();
            void set(int);
        }

        int getParamIndex(Live2DSharp::ALive2DModel^ model);

        static property int MOTION_TYPE_PARAM
        {
            int get();
        }

        static property int MOTION_TYPE_PARTS_VISIBLE
        {
            int get();
        }

        static property int MOTION_TYPE_PARAM_FADEIN
        {
            int get();
        }

        static property int MOTION_TYPE_PARAM_FADEOUT
        {
            int get();
        }

        static property int MOTION_TYPE_LAYOUT_X
        {
            int get();
        }

        static property int MOTION_TYPE_LAYOUT_Y
        {
            int get();
        }

        static property int MOTION_TYPE_LAYOUT_ANCHOR_X
        {
            int get();
        }

        static property int MOTION_TYPE_LAYOUT_ANCHOR_Y
        {
            int get();
        }

        static property int MOTION_TYPE_LAYOUT_SCALE_X
        {
            int get();
        }

        static property int MOTION_TYPE_LAYOUT_SCALE_Y
        {
            int get();
        }
    };
}
