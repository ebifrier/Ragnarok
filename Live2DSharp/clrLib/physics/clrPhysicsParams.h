#pragma once

#include "CppSharp.h"
#include <physics/PhysicsParams.h>
#include "memory/clrLDObject.h"
#include "physics/clrPhysicsHair.h"

namespace Live2DSharp
{
    ref class ALive2DModel;
    ref class IPhysicsSrc;
    ref class IPhysicsTarget;
    ref class PhysicsHair;
    ref class PhysicsSrc;
    ref class PhysicsTarget;
}

namespace Live2DSharp
{
    public ref class IPhysicsSrc : Live2DSharp::LDObject
    {
    public:

        IPhysicsSrc(::live2d::IPhysicsSrc* native);
        static IPhysicsSrc^ __CreateInstance(::System::IntPtr native);
        IPhysicsSrc(System::String^ _paramID, float _scale, float _weight);

        virtual void updateSrc(Live2DSharp::ALive2DModel^ model, Live2DSharp::PhysicsHair^ hair);
    };

    public ref class PhysicsSrc : Live2DSharp::IPhysicsSrc
    {
    public:

        PhysicsSrc(::live2d::PhysicsSrc* native);
        static PhysicsSrc^ __CreateInstance(::System::IntPtr native);
        PhysicsSrc(Live2DSharp::PhysicsHair::Src srcType, System::String^ _paramID, float _scale, float _weight);

        virtual void updateSrc(Live2DSharp::ALive2DModel^ model, Live2DSharp::PhysicsHair^ hair) override;
    };

    public ref class IPhysicsTarget : Live2DSharp::LDObject
    {
    public:

        IPhysicsTarget(::live2d::IPhysicsTarget* native);
        static IPhysicsTarget^ __CreateInstance(::System::IntPtr native);
        IPhysicsTarget(System::String^ _paramID, float _scale, float _weight);

        virtual void updateTarget(Live2DSharp::ALive2DModel^ model, Live2DSharp::PhysicsHair^ hair);
    };

    public ref class PhysicsTarget : Live2DSharp::IPhysicsTarget
    {
    public:

        PhysicsTarget(::live2d::PhysicsTarget* native);
        static PhysicsTarget^ __CreateInstance(::System::IntPtr native);
        PhysicsTarget(Live2DSharp::PhysicsHair::Target targetType, System::String^ _paramID, float _scale, float _weight);

        virtual void updateTarget(Live2DSharp::ALive2DModel^ model, Live2DSharp::PhysicsHair^ hair) override;
    };
}
