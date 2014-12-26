#pragma once

#include "CppSharp.h"
#include <id/BaseDataID.h>
#include "id/clrID.h"

namespace Live2DSharp
{
    ref class BaseDataID;
    ref class LDString;
    ref class RefString;
}

namespace Live2DSharp
{
    public ref class BaseDataID : Live2DSharp::ID
    {
    public:

        BaseDataID(::live2d::BaseDataID* native);
        static BaseDataID^ __CreateInstance(::System::IntPtr native);
        static property Live2DSharp::BaseDataID^ DST_BASE_ID
        {
            Live2DSharp::BaseDataID^ get();
        }

        System::String^ toChar();

        static Live2DSharp::BaseDataID^ getID(Live2DSharp::LDString^ str);

        static Live2DSharp::BaseDataID^ getID(Live2DSharp::RefString^ refStr);

        static void releaseStored_notForClientCall();

        static property int L2D_BASEDATA_ID_INITIAL_CAPACITY
        {
            int get();
        }
    };
}
