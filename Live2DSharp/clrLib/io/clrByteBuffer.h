#pragma once

#include "CppSharp.h"
#include <io/ByteBuffer.h>

namespace Live2DSharp
{
    ref class ByteBuffer;
    ref class LDString;
    ref class MemoryParam;
    ref class RefString;
}

namespace Live2DSharp
{
    public ref class ByteBuffer : ICppInstance
    {
    public:

        property ::live2d::ByteBuffer* NativePtr;
        property System::IntPtr __Instance
        {
            virtual System::IntPtr get();
            virtual void set(System::IntPtr instance);
        }

        ByteBuffer(::live2d::ByteBuffer* native);
        static ByteBuffer^ __CreateInstance(::System::IntPtr native);
        ByteBuffer(char* array, int length);

        property char* CurPtr
        {
            char* get();
        }

        property bool Endian
        {
            void set(bool);
        }

        property bool ChangeEndian
        {
            void set(bool);
        }

        void rollback(int byteLen);

        float readFloat();

        double readDouble();

        long long readLong();

        int readNum();

        int readInt();

        bool readBoolean();

        char readByte();

        short readShort();

        double* readArrayDouble(Live2DSharp::MemoryParam^ owner, int* ret_length);

        float* readArrayFloat(Live2DSharp::MemoryParam^ owner, int* ret_length);

        int* readArrayInt(Live2DSharp::MemoryParam^ owner, int* ret_length);

        unsigned short* readArrayIntAsUShort(Live2DSharp::MemoryParam^ owner, int* ret_length);

        Live2DSharp::LDString^ readString(Live2DSharp::MemoryParam^ owner);

        Live2DSharp::RefString^ readStringAsRef();

        void skip(int bytes);

        static void staticInit_notForClientCall();
    };
}
