#pragma once

#include "CppSharp.h"
#include <memory/tmp/MHBin.h>

namespace Live2DSharp
{
    ref class MHBin;
    ref class MHPageHeaderTmp;
    ref class MemoryHolderTmp;
}

namespace Live2DSharp
{
    public ref class MHBin : ICppInstance
    {
    public:

        property ::live2d::MHBin* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        MHBin(::live2d::MHBin* native);
        static MHBin^ __CreateInstance(::System::IntPtr native);
        MHBin();

        property unsigned int chunkSize
        {
            unsigned int get();
            void set(unsigned int);
        }

        property unsigned int pageSize
        {
            unsigned int get();
            void set(unsigned int);
        }

        property unsigned short pageChunkCount
        {
            unsigned short get();
            void set(unsigned short);
        }

        property unsigned short binNo
        {
            unsigned short get();
            void set(unsigned short);
        }

        property cli::array<unsigned int>^ bitmask
        {
            cli::array<unsigned int>^ get();
            void set(cli::array<unsigned int>^);
        }

        void init(unsigned short binNo, unsigned int _chunkSize, unsigned int _pageSize);

        unsigned int getChunkSize(unsigned int malloc_size);
    };
}
