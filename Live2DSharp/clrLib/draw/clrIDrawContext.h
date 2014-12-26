#pragma once

#include "CppSharp.h"
#include <draw/IDrawContext.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class IDrawContext;
    ref class IDrawData;
}

namespace Live2DSharp
{
    public ref class IDrawContext : Live2DSharp::LDObject
    {
    public:

        IDrawContext(::live2d::IDrawContext* native);
        static IDrawContext^ __CreateInstance(::System::IntPtr native);
        IDrawContext(Live2DSharp::IDrawData^ src);

        property int interpolatedDrawOrder
        {
            int get();
            void set(int);
        }

        property float interpolatedOpacity
        {
            float get();
            void set(float);
        }

        property bool paramOutside
        {
            bool get();
            void set(bool);
        }

        property float partsOpacity
        {
            float get();
            void set(float);
        }

        property bool available
        {
            bool get();
            void set(bool);
        }

        property float baseOpacity
        {
            float get();
            void set(float);
        }

        property ::System::IntPtr SrcPtr
        {
            ::System::IntPtr get();
        }

        property int PartsIndex
        {
            int get();
            void set(int);
        }

        property bool isAvailable
        {
            bool get();
        }
    };
}
