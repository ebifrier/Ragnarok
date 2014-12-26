#pragma once

#include "CppSharp.h"
#include <motion/Live2DMotionBin.h>
#include "memory/clrLDObject.h"
#include "motion/clrAMotion.h"

namespace Live2DSharp
{
    ref class ALive2DModel;
    ref class AMemoryHolder;
    ref class LDString;
    ref class Live2DMotionBin;
    ref class MemoryParam;
    ref class MotionBin;
    ref class MotionQueueEnt;
    ref class ParamID;
}

namespace Live2DSharp
{
    public ref class Live2DMotionBin : Live2DSharp::AMotion
    {
    public:

        enum struct BufType
        {
            DO_NOTHING_ON_DESTRUCT = 0,
            DUPLICATE_AND_DESTRUCT = 1,
            FREE_ON_DESTRUCT = 2,
            DELETE_ARRAY_ON_DESTRUCT = 3,
            DELETE_NORMAL_ON_DESTRUCT = 4,
            L2D_FREE_ON_DESTRUCT = 5,
            UTFILE_RELEASE_ON_DESTRUCT = 6
        };

        Live2DMotionBin(::live2d::Live2DMotionBin* native);
        static Live2DMotionBin^ __CreateInstance(::System::IntPtr native);
        Live2DMotionBin();

        property bool Loop
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

        void dump();

        static Live2DSharp::Live2DMotionBin^ loadMotion(Live2DSharp::LDString^ filepath);

        static Live2DSharp::Live2DMotionBin^ loadMotion(::System::IntPtr buf, int bufSize, Live2DSharp::Live2DMotionBin::BufType bufType);
    };

    public ref class MotionBin : Live2DSharp::LDObject
    {
    public:

        MotionBin(::live2d::MotionBin* native);
        static MotionBin^ __CreateInstance(::System::IntPtr native);
        MotionBin();

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

        property bool isShortArray
        {
            bool get();
            void set(bool);
        }

        property ::System::IntPtr valuePtr
        {
            ::System::IntPtr get();
            void set(::System::IntPtr);
        }

        property float minValue
        {
            float get();
            void set(float);
        }

        property float maxValue
        {
            float get();
            void set(float);
        }

        property int valueCount
        {
            int get();
            void set(int);
        }

        property int motionType
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
