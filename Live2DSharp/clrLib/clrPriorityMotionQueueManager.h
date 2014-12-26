#pragma once

#include "CppSharp.h"
#include <PriorityMotionQueueManager.h>
#include "motion/clrMotionQueueManager.h"

namespace Live2DSharp
{
    ref class ALive2DModel;
    ref class AMotion;
    namespace Framework
    {
        ref class PriorityMotionQueueManager;
    }
}

namespace Live2DSharp
{
    namespace Framework
    {
        public ref class PriorityMotionQueueManager : Live2DSharp::MotionQueueManager
        {
        public:

            PriorityMotionQueueManager(::live2d::framework::PriorityMotionQueueManager* native);
            static PriorityMotionQueueManager^ __CreateInstance(::System::IntPtr native);
            PriorityMotionQueueManager();

            property int CurrentPriority
            {
                int get();
            }

            property int ReservePriority
            {
                int get();
                void set(int);
            }

            int startMotionPriority(Live2DSharp::AMotion^ motion, bool isDelete, int priority);

            bool updateParam(Live2DSharp::ALive2DModel^ model);

            bool reserveMotion(int priority);
        };
    }
}
