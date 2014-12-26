#pragma once

#include "CppSharp.h"
#include <id/PartsDataID.h>
#include "id/clrID.h"

namespace Live2DSharp
{
    ref class LDString;
    ref class PartsDataID;
    ref class RefString;
}

namespace Live2DSharp
{
    public ref class PartsDataID : Live2DSharp::ID
    {
    public:

        PartsDataID(::live2d::PartsDataID* native);
        static PartsDataID^ __CreateInstance(::System::IntPtr native);
        /// <summary>
        /// IDをC言語の文字列として取得
        /// </summary>
        System::String^ toChar();

        static Live2DSharp::PartsDataID^ getID(Live2DSharp::LDString^ str);

        static Live2DSharp::PartsDataID^ getID(Live2DSharp::RefString^ refStr);

        static void releaseStored_notForClientCall();

        static property int L2D_PARTS_ID_INITIAL_CAPACITY
        {
            int get();
        }
    };
}
