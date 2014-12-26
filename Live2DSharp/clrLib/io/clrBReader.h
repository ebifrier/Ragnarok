#pragma once

#include "CppSharp.h"
#include <io/BReader.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class AMemoryHolder;
    ref class BReader;
    ref class ByteBuffer;
    ref class LDString;
    ref class MemoryParam;
    ref class RefString;
}

namespace Live2DSharp
{
    public ref class BReader : Live2DSharp::LDObject
    {
    public:

        BReader(::live2d::BReader* native);
        static BReader^ __CreateInstance(::System::IntPtr native);
        BReader(System::String^ buf, int length);

        property int FormatVersion
        {
            int get();
            void set(int);
        }

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

        ::System::IntPtr readObject(Live2DSharp::MemoryParam^ memParam, int cno, int flags);

        bool readBit();

        int readNum();

        float readFloat();

        double readDouble();

        long long readLong();

        int readInt();

        bool readBoolean();

        char readByte();

        short readShort();

        double* readArrayDouble(Live2DSharp::MemoryParam^ memParam, int* ret_length);

        float* readArrayFloat(Live2DSharp::MemoryParam^ memParam, int* ret_length);

        int* readArrayInt(Live2DSharp::MemoryParam^ memParam, int* ret_length);

        unsigned short* readArrayIntAsUShort(Live2DSharp::MemoryParam^ memParam, int* ret_length);

        Live2DSharp::LDString^ readString(Live2DSharp::MemoryParam^ memParam);

        Live2DSharp::RefString^ readStringAsRef();

        void skip(int bytes);

        static property int FLAG_READ_AS_USHORT_ARRAY
        {
            int get();
        }

        static property int FLAG_MALLOC_ON_GPU
        {
            int get();
        }

        static property int LOAD_OBJECT_INITIAL_CAPACITY
        {
            int get();
        }
    };
}
