#pragma once

#include "CppSharp.h"
#include <memory/fixed/MHPageHeaderFixed.h>
#include "memory/clrAPageHeader.h"

namespace Live2DSharp
{
    ref class MHPageHeaderFixed;
}

namespace Live2DSharp
{
    public ref class MHPageHeaderFixed : Live2DSharp::APageHeader
    {
    public:

        MHPageHeaderFixed(::live2d::MHPageHeaderFixed* native);
        static MHPageHeaderFixed^ __CreateInstance(::System::IntPtr native);
        MHPageHeaderFixed();

        property Live2DSharp::MHPageHeaderFixed^ nextPage
        {
            Live2DSharp::MHPageHeaderFixed^ get();
            void set(Live2DSharp::MHPageHeaderFixed^);
        }

        property char* endPtr
        {
            char* get();
            void set(char*);
        }

        property unsigned int size
        {
            unsigned int get();
            void set(unsigned int);
        }

        property char* curPtr
        {
            char* get();
            void set(char*);
        }

        property unsigned int rest
        {
            unsigned int get();
            void set(unsigned int);
        }

        property unsigned int pageNo
        {
            unsigned int get();
            void set(unsigned int);
        }

        property int PageNo
        {
            int get();
        }

        property int PageAmount
        {
            int get();
        }

        property ::System::IntPtr StartPtr
        {
            ::System::IntPtr get();
        }

        virtual void free_exe(::System::IntPtr ptr) override;

        static property int pageAmount
        {
            int get();
            void set(int);
        }
    };
}
