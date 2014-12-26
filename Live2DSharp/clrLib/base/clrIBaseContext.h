#pragma once

#include "CppSharp.h"
#include <base/IBaseContext.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class IBaseContext;
    ref class IBaseData;
}

namespace Live2DSharp
{
    public ref class IBaseContext : Live2DSharp::LDObject
    {
    public:

        IBaseContext(::live2d::IBaseContext* native);
        static IBaseContext^ __CreateInstance(::System::IntPtr native);
        IBaseContext(Live2DSharp::IBaseData^ src);

        property ::System::IntPtr SrcPtr
        {
            ::System::IntPtr get();
        }

        property int PartsIndex
        {
            int get();
            void set(int);
        }

        property bool OutsideParam
        {
            void set(bool);
        }

        property bool Available
        {
            void set(bool);
        }

        property float TotalScale
        {
            float get();
        }

        property float TotalScale_notForClient
        {
            void set(float);
        }

        property float InterpolatedOpacity
        {
            float get();
            void set(float);
        }

        property float TotalOpacity
        {
            float get();
            void set(float);
        }

        property bool isOutsideParam
        {
            bool get();
        }

        property bool isAvailable
        {
            bool get();
        }
    };
}
