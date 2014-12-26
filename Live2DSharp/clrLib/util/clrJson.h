#pragma once

#include "CppSharp.h"
#include <util/Json.h>
#include "memory/clrLDObject.h"

namespace Live2DSharp
{
    ref class AMemoryHolder;
    ref class Array;
    ref class Boolean;
    ref class Double;
    ref class Error;
    ref class Json;
    ref class LDString;
    ref class Map;
    ref class MemoryParam;
    ref class NullValue;
    ref class String;
    ref class Value;
}

namespace Live2DSharp
{
    public ref class Value : Live2DSharp::LDObject
    {
    public:

        Value(::live2d::Value* native);
        static Value^ __CreateInstance(::System::IntPtr native);
        Value(Live2DSharp::MemoryParam^ memParam);

        property Live2DSharp::Value^ default[int]
        {
            Live2DSharp::Value^ get(int index);
            void set(int index, Live2DSharp::Value^ value);
        }

        property Live2DSharp::Value^ default[Live2DSharp::LDString^]
        {
            Live2DSharp::Value^ get(Live2DSharp::LDString^ s);
            void set(Live2DSharp::LDString^ s, Live2DSharp::Value^ value);
        }

        property int size
        {
            int get();
        }

        property bool isError
        {
            bool get();
        }

        property bool isNull
        {
            bool get();
        }

        property bool isBool
        {
            bool get();
        }

        property bool isDouble
        {
            bool get();
        }

        property bool isString
        {
            bool get();
        }

        property bool isArray
        {
            bool get();
        }

        property bool isMap
        {
            bool get();
        }

        property bool isStatic
        {
            bool get();
        }

        virtual Live2DSharp::LDString^ toString(Live2DSharp::LDString^ defaultV, Live2DSharp::LDString^ indent);

        virtual System::String^ c_str(Live2DSharp::LDString^ defaultV, Live2DSharp::LDString^ indent);

        virtual int toInt(int defaultV);

        virtual double toDouble(double defaultV);

        virtual bool toBoolean(bool defaultV);

        virtual bool equals(Live2DSharp::LDString^ v);

        virtual bool equals(System::String^ v);

        virtual bool equals(int v);

        virtual bool equals(double v);

        virtual bool equals(bool v);

        virtual Live2DSharp::Value^ setError_notForClientCall(System::String^ errorStr);

        static void staticInit_notForClientCall();

        static void staticRelease_notForClientCall();

        static property Live2DSharp::Value^ ERROR_VALUE
        {
            Live2DSharp::Value^ get();
            void set(Live2DSharp::Value^);
        }

        static property Live2DSharp::Value^ NULL_VALUE
        {
            Live2DSharp::Value^ get();
            void set(Live2DSharp::Value^);
        }
    };

    public ref class Json : Live2DSharp::LDObject
    {
    public:

        Json(::live2d::Json* native);
        static Json^ __CreateInstance(::System::IntPtr native);
        Json();

        Json(System::String^ buf, int length, int encoding);

        property Live2DSharp::Value^ Root
        {
            Live2DSharp::Value^ get();
        }

        property System::String^ Error
        {
            System::String^ get();
        }

        void release();

        bool parseFile(System::String^ filepath, int encoding);

        bool parseBytes(System::String^ buf, int length, int encoding);

        bool checkEOF();

        static Live2DSharp::Json^ parseFromBytes(System::String^ buf, int length, int encoding);

        static Live2DSharp::Json^ parseFromFile(System::String^ filepath, int encoding);

        static property int UTF8
        {
            int get();
        }

        static property int SJIS
        {
            int get();
        }
    };

    public ref class Double : Live2DSharp::Value
    {
    public:

        Double(::live2d::Double* native);
        static Double^ __CreateInstance(::System::IntPtr native);
        Double(double v);

        property bool isDouble
        {
            bool get();
        }

        virtual Live2DSharp::LDString^ toString(Live2DSharp::LDString^ defaultV, Live2DSharp::LDString^ indent) override;

        virtual int toInt(int defaultV) override;

        virtual double toDouble(double defaultV) override;

        virtual bool equals(double v) override;

        virtual bool equals(Live2DSharp::LDString^ v) override;

        virtual bool equals(System::String^ v) override;

        virtual bool equals(int v) override;

        virtual bool equals(bool v) override;
    };

    public ref class Boolean : Live2DSharp::Value
    {
    public:

        Boolean(::live2d::Boolean* native);
        static Boolean^ __CreateInstance(::System::IntPtr native);
        property bool isBool
        {
            bool get();
        }

        property bool isStatic
        {
            bool get();
        }

        virtual bool toBoolean(bool defaultV) override;

        virtual Live2DSharp::LDString^ toString(Live2DSharp::LDString^ defaultV, Live2DSharp::LDString^ indent) override;

        virtual bool equals(bool v) override;

        virtual bool equals(Live2DSharp::LDString^ v) override;

        virtual bool equals(System::String^ v) override;

        virtual bool equals(int v) override;

        virtual bool equals(double v) override;

        static property Live2DSharp::Boolean^ TRUE_VALUE
        {
            Live2DSharp::Boolean^ get();
            void set(Live2DSharp::Boolean^);
        }

        static property Live2DSharp::Boolean^ FALSE_VALUE
        {
            Live2DSharp::Boolean^ get();
            void set(Live2DSharp::Boolean^);
        }
    };

    public ref class String : Live2DSharp::Value
    {
    public:

        String(::live2d::String* native);
        static String^ __CreateInstance(::System::IntPtr native);
        String(Live2DSharp::MemoryParam^ memParam, Live2DSharp::LDString^ s);

        String(Live2DSharp::MemoryParam^ memParam, System::String^ s);

        property bool isString
        {
            bool get();
        }

        virtual Live2DSharp::LDString^ toString(Live2DSharp::LDString^ defaultV, Live2DSharp::LDString^ indent) override;

        virtual bool equals(Live2DSharp::LDString^ v) override;

        virtual bool equals(System::String^ v) override;

        virtual bool equals(int v) override;

        virtual bool equals(double v) override;

        virtual bool equals(bool v) override;
    };

    public ref class Error : Live2DSharp::String
    {
    public:

        Error(::live2d::Error* native);
        static Error^ __CreateInstance(::System::IntPtr native);
        property bool isStatic
        {
            bool get();
        }

        virtual Live2DSharp::Value^ setError_notForClientCall(System::String^ s) override;
    };

    public ref class NullValue : Live2DSharp::Value
    {
    public:

        NullValue(::live2d::NullValue* native);
        static NullValue^ __CreateInstance(::System::IntPtr native);
        property bool isNull
        {
            bool get();
        }

        property bool isStatic
        {
            bool get();
        }

        virtual Live2DSharp::LDString^ toString(Live2DSharp::LDString^ defaultV, Live2DSharp::LDString^ indent) override;
    };

    public ref class Array : Live2DSharp::Value
    {
    public:

        Array(::live2d::Array* native);
        static Array^ __CreateInstance(::System::IntPtr native);
        Array(Live2DSharp::MemoryParam^ memParam);

        property Live2DSharp::Value^ default[int]
        {
            Live2DSharp::Value^ get(int index);
            void set(int index, Live2DSharp::Value^ value);
        }

        property Live2DSharp::Value^ default[Live2DSharp::LDString^]
        {
            Live2DSharp::Value^ get(Live2DSharp::LDString^ s);
            void set(Live2DSharp::LDString^ s, Live2DSharp::Value^ value);
        }

        property bool isArray
        {
            bool get();
        }

        property int size
        {
            int get();
        }

        virtual Live2DSharp::LDString^ toString(Live2DSharp::LDString^ defaultV, Live2DSharp::LDString^ indent) override;

        void add(Live2DSharp::Value^ v);
    };

    public ref class Map : Live2DSharp::Value
    {
    public:

        Map(::live2d::Map* native);
        static Map^ __CreateInstance(::System::IntPtr native);
        Map(Live2DSharp::MemoryParam^ memParam);

        property Live2DSharp::Value^ default[Live2DSharp::LDString^]
        {
            Live2DSharp::Value^ get(Live2DSharp::LDString^ s);
            void set(Live2DSharp::LDString^ s, Live2DSharp::Value^ value);
        }

        property Live2DSharp::Value^ default[int]
        {
            Live2DSharp::Value^ get(int index);
            void set(int index, Live2DSharp::Value^ value);
        }

        property bool isMap
        {
            bool get();
        }

        property int size
        {
            int get();
        }

        virtual Live2DSharp::LDString^ toString(Live2DSharp::LDString^ defaultV, Live2DSharp::LDString^ indent) override;

        void put(Live2DSharp::LDString^ key, Live2DSharp::Value^ v);
    };
}
