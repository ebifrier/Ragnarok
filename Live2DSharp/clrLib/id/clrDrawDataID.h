#pragma once

#include "CppSharp.h"
#include <id/DrawDataID.h>
#include "id/clrID.h"

namespace Live2DSharp
{
    ref class DrawDataID;
    ref class LDString;
    ref class RefString;
}

namespace Live2DSharp
{
    public ref class DrawDataID : Live2DSharp::ID
    {
    public:

        DrawDataID(::live2d::DrawDataID* native);
        static DrawDataID^ __CreateInstance(::System::IntPtr native);
        /// <summary>
        /// IDをC言語の文字列として取得
        /// </summary>
        System::String^ toChar();

        static Live2DSharp::DrawDataID^ getID(Live2DSharp::LDString^ str);

        static Live2DSharp::DrawDataID^ getID(Live2DSharp::RefString^ refStr);

        static void releaseStored_notForClientCall();

        static property int L2D_DRAWDATA_ID_INITIAL_CAPACITY
        {
            int get();
        }
    };
}
