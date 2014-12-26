#pragma once

#include "CppSharp.h"
#include <type/LDString.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class LDString;
    ref class MemoryParam;
    ref class RefString;
}

namespace Live2DSharp
{
    public ref class LDString : Live2DSharp::LDObject
    {
    public:

        LDString(::live2d::LDString* native);
        static LDString^ __CreateInstance(::System::IntPtr native);
        LDString();

        LDString(System::String^ s, Live2DSharp::MemoryParam^ memParam);

        LDString(System::String^ s, int length, Live2DSharp::MemoryParam^ memParam);

        LDString(Live2DSharp::RefString^ refStr, Live2DSharp::MemoryParam^ memParam);

        LDString(System::String^ s, int length, bool useptr, Live2DSharp::MemoryParam^ memParam);

        property int Hashcode
        {
            int get();
        }

        property unsigned int length
        {
            unsigned int get();
        }

        property int size
        {
            int get();
        }

        property System::String^ c_str
        {
            System::String^ get();
        }

        static bool operator<(Live2DSharp::LDString^ __op, Live2DSharp::LDString^ s);

        static bool operator<(Live2DSharp::LDString^ __op, System::String^ c);

        static bool operator>(Live2DSharp::LDString^ __op, Live2DSharp::LDString^ s);

        static bool operator>(Live2DSharp::LDString^ __op, System::String^ c);

        bool equals(Live2DSharp::RefString^ refStr);

        static Live2DSharp::LDString^ operator+(Live2DSharp::LDString^ __op, Live2DSharp::LDString^ s);

        static Live2DSharp::LDString^ operator+(Live2DSharp::LDString^ __op, System::String^ s);

        Live2DSharp::LDString^ append(System::String^ p, int length);

        Live2DSharp::LDString^ append(int count, char p);

        void clear();
    };
}
