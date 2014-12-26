#pragma once

#include "CppSharp.h"
#include <id/ParamID.h>
#include "id/clrID.h"

namespace Live2DSharp
{
    ref class LDString;
    ref class ParamID;
    ref class RefString;
}

namespace Live2DSharp
{
    public ref class ParamID : Live2DSharp::ID
    {
    public:

        ParamID(::live2d::ParamID* native);
        static ParamID^ __CreateInstance(::System::IntPtr native);
        System::String^ toChar();

        static Live2DSharp::ParamID^ getID(Live2DSharp::LDString^ tmp_idstr);

        static Live2DSharp::ParamID^ getID(System::String^ tmp_idstr);

        static Live2DSharp::ParamID^ getID(Live2DSharp::RefString^ refStr);

        static void releaseStored_notForClientCall();

        static property int L2D_PARAM_ID_INITIAL_CAPACITY
        {
            int get();
        }
    };
}
