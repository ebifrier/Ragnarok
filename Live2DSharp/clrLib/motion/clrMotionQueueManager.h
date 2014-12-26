#pragma once

#include "CppSharp.h"
#include <motion/MotionQueueManager.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class ALive2DModel;
    ref class AMemoryHolder;
    ref class AMotion;
    ref class Live2DMotion;
    ref class MemoryParam;
    ref class MotionQueueEnt;
    ref class MotionQueueManager;
}

namespace Live2DSharp
{
    public ref class MotionQueueManager : Live2DSharp::LDObject
    {
    public:

        MotionQueueManager(::live2d::MotionQueueManager* native);
        static MotionQueueManager^ __CreateInstance(::System::IntPtr native);
        MotionQueueManager();

        property bool MotionDebugMode
        {
            void set(bool);
        }

        int startMotion(Live2DSharp::AMotion^ motion, bool autoDelete);

        bool updateParam(Live2DSharp::ALive2DModel^ model);

        bool isFinished();

        bool isFinished(int motionQueueEntNo);

        void stopAllMotions();

        Live2DSharp::MotionQueueEnt^ getMotionQueueEnt(int entNo);

        void DUMP();
    };
}
