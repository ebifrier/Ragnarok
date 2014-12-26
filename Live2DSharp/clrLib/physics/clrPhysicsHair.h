#pragma once

#include "CppSharp.h"
#include <physics/PhysicsHair.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class ALive2DModel;
    ref class AMemoryHolder;
    ref class IPhysicsSrc;
    ref class IPhysicsTarget;
    ref class MemoryParam;
    ref class PhysicsHair;
    ref class PhysicsPoint;
}

namespace Live2DSharp
{
    public ref class PhysicsPoint : Live2DSharp::LDObject
    {
    public:

        PhysicsPoint(::live2d::PhysicsPoint* native);
        static PhysicsPoint^ __CreateInstance(::System::IntPtr native);
        PhysicsPoint();

        property float mass
        {
            float get();
            void set(float);
        }

        property float x
        {
            float get();
            void set(float);
        }

        property float y
        {
            float get();
            void set(float);
        }

        property float vx
        {
            float get();
            void set(float);
        }

        property float vy
        {
            float get();
            void set(float);
        }

        property float ax
        {
            float get();
            void set(float);
        }

        property float ay
        {
            float get();
            void set(float);
        }

        property float fx
        {
            float get();
            void set(float);
        }

        property float fy
        {
            float get();
            void set(float);
        }

        property float last_x
        {
            float get();
            void set(float);
        }

        property float last_y
        {
            float get();
            void set(float);
        }

        property float last_vx
        {
            float get();
            void set(float);
        }

        property float last_vy
        {
            float get();
            void set(float);
        }

        void setupLast();
    };

    public ref class PhysicsHair : Live2DSharp::LDObject
    {
    public:

        enum struct Src
        {
            SRC_TO_X = 0,
            SRC_TO_Y = 1,
            SRC_TO_G_ANGLE = 2
        };

        enum struct Target
        {
            TARGET_FROM_ANGLE = 0,
            TARGET_FROM_ANGLE_V = 1
        };

        PhysicsHair(::live2d::PhysicsHair* native);
        static PhysicsHair^ __CreateInstance(::System::IntPtr native);
        PhysicsHair();

        PhysicsHair(float baseLengthM, float airRegistance, float mass);

        property Live2DSharp::PhysicsPoint^ PhysicsPoint1
        {
            Live2DSharp::PhysicsPoint^ get();
        }

        property Live2DSharp::PhysicsPoint^ PhysicsPoint2
        {
            Live2DSharp::PhysicsPoint^ get();
        }

        property float GravityAngleDeg
        {
            float get();
            void set(float);
        }

        property float AngleP1toP2Deg
        {
            float get();
        }

        property float AngleP1toP2Deg_velocity
        {
            float get();
        }

        void setup(float baseLengthM, float airRegistance, float mass);

        void setup();

        void addSrcParam(Live2DSharp::PhysicsHair::Src srcType, System::String^ paramID, float scale, float weight);

        void addTargetParam(Live2DSharp::PhysicsHair::Target targetType, System::String^ paramID, float scale, float weight);

        void update(Live2DSharp::ALive2DModel^ model, long long time);
    };
}
