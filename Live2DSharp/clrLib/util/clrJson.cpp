#include "util/clrJson.h"
#include "memory/clrAMemoryHolder.h"
#include "memory/clrMemoryParam.h"
#include "type/clrLDMap.h"
#include "type/clrLDString.h"
#include "type/clrLDVector.h"

using namespace System;
using namespace System::Runtime::InteropServices;

Live2DSharp::Value::Value(::live2d::Value* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::Value^ Live2DSharp::Value::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::Value((::live2d::Value*) native.ToPointer());
}

Live2DSharp::Value::Value(Live2DSharp::MemoryParam^ memParam)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
}

Live2DSharp::LDString^ Live2DSharp::Value::toString(Live2DSharp::LDString^ defaultV, Live2DSharp::LDString^ indent)
{
    auto &arg0 = *(::live2d::LDString*)defaultV->NativePtr;
    auto &arg1 = *(::live2d::LDString*)indent->NativePtr;
    auto __ret = ((::live2d::Value*)NativePtr)->toString(arg0, arg1);
    auto ____ret = new ::live2d::LDString(__ret);
    return (____ret == nullptr) ? nullptr : gcnew Live2DSharp::LDString((::live2d::LDString*)____ret);
}

System::String^ Live2DSharp::Value::c_str(Live2DSharp::LDString^ defaultV, Live2DSharp::LDString^ indent)
{
    auto &arg0 = *(::live2d::LDString*)defaultV->NativePtr;
    auto &arg1 = *(::live2d::LDString*)indent->NativePtr;
    auto __ret = ((::live2d::Value*)NativePtr)->c_str(arg0, arg1);
    if (__ret == nullptr) return nullptr;
    return clix::marshalString<clix::E_UTF8>(__ret);
}

int Live2DSharp::Value::toInt(int defaultV)
{
    auto __ret = ((::live2d::Value*)NativePtr)->toInt(defaultV);
    return __ret;
}

double Live2DSharp::Value::toDouble(double defaultV)
{
    auto __ret = ((::live2d::Value*)NativePtr)->toDouble(defaultV);
    return __ret;
}

bool Live2DSharp::Value::toBoolean(bool defaultV)
{
    auto __ret = ((::live2d::Value*)NativePtr)->toBoolean(defaultV);
    return __ret;
}

bool Live2DSharp::Value::equals(Live2DSharp::LDString^ v)
{
    auto &arg0 = *(::live2d::LDString*)v->NativePtr;
    auto __ret = ((::live2d::Value*)NativePtr)->equals(arg0);
    return __ret;
}

bool Live2DSharp::Value::equals(System::String^ v)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(v);
    auto arg0 = _arg0.c_str();
    auto __ret = ((::live2d::Value*)NativePtr)->equals(arg0);
    return __ret;
}

bool Live2DSharp::Value::equals(int v)
{
    auto __ret = ((::live2d::Value*)NativePtr)->equals(v);
    return __ret;
}

bool Live2DSharp::Value::equals(double v)
{
    auto __ret = ((::live2d::Value*)NativePtr)->equals(v);
    return __ret;
}

bool Live2DSharp::Value::equals(bool v)
{
    auto __ret = ((::live2d::Value*)NativePtr)->equals(v);
    return __ret;
}

Live2DSharp::Value^ Live2DSharp::Value::setError_notForClientCall(System::String^ errorStr)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(errorStr);
    auto arg0 = _arg0.c_str();
    auto __ret = ((::live2d::Value*)NativePtr)->setError_notForClientCall(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::Value((::live2d::Value*)__ret);
}

void Live2DSharp::Value::staticInit_notForClientCall()
{
    ::live2d::Value::staticInit_notForClientCall();
}

void Live2DSharp::Value::staticRelease_notForClientCall()
{
    ::live2d::Value::staticRelease_notForClientCall();
}

Live2DSharp::Value^ Live2DSharp::Value::default::get(int index)
{
    auto &__ret = ((::live2d::Value*)NativePtr)->operator[](index);
    return (&__ret == nullptr) ? nullptr : gcnew Live2DSharp::Value((::live2d::Value*)&__ret);
}

void Live2DSharp::Value::default::set(int index, Live2DSharp::Value^ value)
{
    ((::live2d::Value*)NativePtr)->operator[](index) = *(::live2d::Value*)value->NativePtr;
}

Live2DSharp::Value^ Live2DSharp::Value::default::get(Live2DSharp::LDString^ s)
{
    auto &arg0 = *(::live2d::LDString*)s->NativePtr;
    auto &__ret = ((::live2d::Value*)NativePtr)->operator[](arg0);
    return (&__ret == nullptr) ? nullptr : gcnew Live2DSharp::Value((::live2d::Value*)&__ret);
}

void Live2DSharp::Value::default::set(Live2DSharp::LDString^ s, Live2DSharp::Value^ value)
{
    ((::live2d::Value*)NativePtr)->operator[](*(::live2d::LDString*)s->NativePtr) = *(::live2d::Value*)value->NativePtr;
}

int Live2DSharp::Value::size::get()
{
    auto __ret = ((::live2d::Value*)NativePtr)->size();
    return __ret;
}

bool Live2DSharp::Value::isError::get()
{
    auto __ret = ((::live2d::Value*)NativePtr)->isError();
    return __ret;
}

bool Live2DSharp::Value::isNull::get()
{
    auto __ret = ((::live2d::Value*)NativePtr)->isNull();
    return __ret;
}

bool Live2DSharp::Value::isBool::get()
{
    auto __ret = ((::live2d::Value*)NativePtr)->isBool();
    return __ret;
}

bool Live2DSharp::Value::isDouble::get()
{
    auto __ret = ((::live2d::Value*)NativePtr)->isDouble();
    return __ret;
}

bool Live2DSharp::Value::isString::get()
{
    auto __ret = ((::live2d::Value*)NativePtr)->isString();
    return __ret;
}

bool Live2DSharp::Value::isArray::get()
{
    auto __ret = ((::live2d::Value*)NativePtr)->isArray();
    return __ret;
}

bool Live2DSharp::Value::isMap::get()
{
    auto __ret = ((::live2d::Value*)NativePtr)->isMap();
    return __ret;
}

bool Live2DSharp::Value::isStatic::get()
{
    auto __ret = ((::live2d::Value*)NativePtr)->isStatic();
    return __ret;
}

Live2DSharp::Value^ Live2DSharp::Value::ERROR_VALUE::get()
{
    return (::live2d::Value::ERROR_VALUE == nullptr) ? nullptr : gcnew Live2DSharp::Value((::live2d::Value*)::live2d::Value::ERROR_VALUE);
}

void Live2DSharp::Value::ERROR_VALUE::set(Live2DSharp::Value^ value)
{
    ::live2d::Value::ERROR_VALUE = (::live2d::Value*)value->NativePtr;
}

Live2DSharp::Value^ Live2DSharp::Value::NULL_VALUE::get()
{
    return (::live2d::Value::NULL_VALUE == nullptr) ? nullptr : gcnew Live2DSharp::Value((::live2d::Value*)::live2d::Value::NULL_VALUE);
}

void Live2DSharp::Value::NULL_VALUE::set(Live2DSharp::Value^ value)
{
    ::live2d::Value::NULL_VALUE = (::live2d::Value*)value->NativePtr;
}

Live2DSharp::Json::Json(::live2d::Json* native)
    : Live2DSharp::LDObject((::live2d::LDObject*)native)
{
}

Live2DSharp::Json^ Live2DSharp::Json::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::Json((::live2d::Json*) native.ToPointer());
}

Live2DSharp::Json::Json()
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    NativePtr = new ::live2d::Json();
}

Live2DSharp::Json::Json(System::String^ buf, int length, int encoding)
    : Live2DSharp::LDObject((::live2d::LDObject*)nullptr)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(buf);
    auto arg0 = _arg0.c_str();
    NativePtr = new ::live2d::Json(arg0, length, encoding);
}

void Live2DSharp::Json::release()
{
    ((::live2d::Json*)NativePtr)->release();
}

bool Live2DSharp::Json::parseFile(System::String^ filepath, int encoding)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(filepath);
    auto arg0 = _arg0.c_str();
    auto __ret = ((::live2d::Json*)NativePtr)->parseFile(arg0, encoding);
    return __ret;
}

bool Live2DSharp::Json::parseBytes(System::String^ buf, int length, int encoding)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(buf);
    auto arg0 = _arg0.c_str();
    auto __ret = ((::live2d::Json*)NativePtr)->parseBytes(arg0, length, encoding);
    return __ret;
}

bool Live2DSharp::Json::checkEOF()
{
    auto __ret = ((::live2d::Json*)NativePtr)->checkEOF();
    return __ret;
}

Live2DSharp::Json^ Live2DSharp::Json::parseFromBytes(System::String^ buf, int length, int encoding)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(buf);
    auto arg0 = _arg0.c_str();
    auto __ret = ::live2d::Json::parseFromBytes(arg0, length, encoding);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::Json((::live2d::Json*)__ret);
}

Live2DSharp::Json^ Live2DSharp::Json::parseFromFile(System::String^ filepath, int encoding)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(filepath);
    auto arg0 = _arg0.c_str();
    auto __ret = ::live2d::Json::parseFromFile(arg0, encoding);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::Json((::live2d::Json*)__ret);
}

Live2DSharp::Value^ Live2DSharp::Json::Root::get()
{
    auto &__ret = ((::live2d::Json*)NativePtr)->getRoot();
    return (Live2DSharp::Value^)((&__ret == nullptr) ? nullptr : gcnew Live2DSharp::Value((::live2d::Value*)&__ret));
}

System::String^ Live2DSharp::Json::Error::get()
{
    auto __ret = ((::live2d::Json*)NativePtr)->getError();
    if (__ret == nullptr) return nullptr;
    return clix::marshalString<clix::E_UTF8>(__ret);
}

int Live2DSharp::Json::UTF8::get()
{
    return ::live2d::Json::UTF8;
}

int Live2DSharp::Json::SJIS::get()
{
    return ::live2d::Json::SJIS;
}

Live2DSharp::Double::Double(::live2d::Double* native)
    : Live2DSharp::Value((::live2d::Value*)native)
{
}

Live2DSharp::Double^ Live2DSharp::Double::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::Double((::live2d::Double*) native.ToPointer());
}

Live2DSharp::Double::Double(double v)
    : Live2DSharp::Value((::live2d::Value*)nullptr)
{
    NativePtr = new ::live2d::Double(v);
}

Live2DSharp::LDString^ Live2DSharp::Double::toString(Live2DSharp::LDString^ defaultV, Live2DSharp::LDString^ indent)
{
    auto &arg0 = *(::live2d::LDString*)defaultV->NativePtr;
    auto &arg1 = *(::live2d::LDString*)indent->NativePtr;
    auto __ret = ((::live2d::Double*)NativePtr)->toString(arg0, arg1);
    auto ____ret = new ::live2d::LDString(__ret);
    return (____ret == nullptr) ? nullptr : gcnew Live2DSharp::LDString((::live2d::LDString*)____ret);
}

int Live2DSharp::Double::toInt(int defaultV)
{
    auto __ret = ((::live2d::Double*)NativePtr)->toInt(defaultV);
    return __ret;
}

double Live2DSharp::Double::toDouble(double defaultV)
{
    auto __ret = ((::live2d::Double*)NativePtr)->toDouble(defaultV);
    return __ret;
}

bool Live2DSharp::Double::equals(double v)
{
    auto __ret = ((::live2d::Double*)NativePtr)->equals(v);
    return __ret;
}

bool Live2DSharp::Double::equals(Live2DSharp::LDString^ v)
{
    auto &arg0 = *(::live2d::LDString*)v->NativePtr;
    auto __ret = ((::live2d::Double*)NativePtr)->equals(arg0);
    return __ret;
}

bool Live2DSharp::Double::equals(System::String^ v)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(v);
    auto arg0 = _arg0.c_str();
    auto __ret = ((::live2d::Double*)NativePtr)->equals(arg0);
    return __ret;
}

bool Live2DSharp::Double::equals(int v)
{
    auto __ret = ((::live2d::Double*)NativePtr)->equals(v);
    return __ret;
}

bool Live2DSharp::Double::equals(bool v)
{
    auto __ret = ((::live2d::Double*)NativePtr)->equals(v);
    return __ret;
}

bool Live2DSharp::Double::isDouble::get()
{
    auto __ret = ((::live2d::Double*)NativePtr)->isDouble();
    return __ret;
}

Live2DSharp::Boolean::Boolean(::live2d::Boolean* native)
    : Live2DSharp::Value((::live2d::Value*)native)
{
}

Live2DSharp::Boolean^ Live2DSharp::Boolean::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::Boolean((::live2d::Boolean*) native.ToPointer());
}

bool Live2DSharp::Boolean::toBoolean(bool defaultV)
{
    auto __ret = ((::live2d::Boolean*)NativePtr)->toBoolean(defaultV);
    return __ret;
}

Live2DSharp::LDString^ Live2DSharp::Boolean::toString(Live2DSharp::LDString^ defaultV, Live2DSharp::LDString^ indent)
{
    auto &arg0 = *(::live2d::LDString*)defaultV->NativePtr;
    auto &arg1 = *(::live2d::LDString*)indent->NativePtr;
    auto __ret = ((::live2d::Boolean*)NativePtr)->toString(arg0, arg1);
    auto ____ret = new ::live2d::LDString(__ret);
    return (____ret == nullptr) ? nullptr : gcnew Live2DSharp::LDString((::live2d::LDString*)____ret);
}

bool Live2DSharp::Boolean::equals(bool v)
{
    auto __ret = ((::live2d::Boolean*)NativePtr)->equals(v);
    return __ret;
}

bool Live2DSharp::Boolean::equals(Live2DSharp::LDString^ v)
{
    auto &arg0 = *(::live2d::LDString*)v->NativePtr;
    auto __ret = ((::live2d::Boolean*)NativePtr)->equals(arg0);
    return __ret;
}

bool Live2DSharp::Boolean::equals(System::String^ v)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(v);
    auto arg0 = _arg0.c_str();
    auto __ret = ((::live2d::Boolean*)NativePtr)->equals(arg0);
    return __ret;
}

bool Live2DSharp::Boolean::equals(int v)
{
    auto __ret = ((::live2d::Boolean*)NativePtr)->equals(v);
    return __ret;
}

bool Live2DSharp::Boolean::equals(double v)
{
    auto __ret = ((::live2d::Boolean*)NativePtr)->equals(v);
    return __ret;
}

bool Live2DSharp::Boolean::isBool::get()
{
    auto __ret = ((::live2d::Boolean*)NativePtr)->isBool();
    return __ret;
}

bool Live2DSharp::Boolean::isStatic::get()
{
    auto __ret = ((::live2d::Boolean*)NativePtr)->isStatic();
    return __ret;
}

Live2DSharp::Boolean^ Live2DSharp::Boolean::TRUE_VALUE::get()
{
    return (::live2d::Boolean::TRUE_VALUE == nullptr) ? nullptr : gcnew Live2DSharp::Boolean((::live2d::Boolean*)::live2d::Boolean::TRUE_VALUE);
}

void Live2DSharp::Boolean::TRUE_VALUE::set(Live2DSharp::Boolean^ value)
{
    ::live2d::Boolean::TRUE_VALUE = (::live2d::Boolean*)value->NativePtr;
}

Live2DSharp::Boolean^ Live2DSharp::Boolean::FALSE_VALUE::get()
{
    return (::live2d::Boolean::FALSE_VALUE == nullptr) ? nullptr : gcnew Live2DSharp::Boolean((::live2d::Boolean*)::live2d::Boolean::FALSE_VALUE);
}

void Live2DSharp::Boolean::FALSE_VALUE::set(Live2DSharp::Boolean^ value)
{
    ::live2d::Boolean::FALSE_VALUE = (::live2d::Boolean*)value->NativePtr;
}

Live2DSharp::String::String(::live2d::String* native)
    : Live2DSharp::Value((::live2d::Value*)native)
{
}

Live2DSharp::String^ Live2DSharp::String::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::String((::live2d::String*) native.ToPointer());
}

Live2DSharp::String::String(Live2DSharp::MemoryParam^ memParam, Live2DSharp::LDString^ s)
    : Live2DSharp::Value((::live2d::Value*)nullptr)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    auto &arg1 = *(::live2d::LDString*)s->NativePtr;
    NativePtr = new ::live2d::String(arg0, arg1);
}

Live2DSharp::String::String(Live2DSharp::MemoryParam^ memParam, System::String^ s)
    : Live2DSharp::Value((::live2d::Value*)nullptr)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    auto _arg1 = clix::marshalString<clix::E_UTF8>(s);
    auto arg1 = _arg1.c_str();
    NativePtr = new ::live2d::String(arg0, arg1);
}

Live2DSharp::LDString^ Live2DSharp::String::toString(Live2DSharp::LDString^ defaultV, Live2DSharp::LDString^ indent)
{
    auto &arg0 = *(::live2d::LDString*)defaultV->NativePtr;
    auto &arg1 = *(::live2d::LDString*)indent->NativePtr;
    auto __ret = ((::live2d::String*)NativePtr)->toString(arg0, arg1);
    auto ____ret = new ::live2d::LDString(__ret);
    return (____ret == nullptr) ? nullptr : gcnew Live2DSharp::LDString((::live2d::LDString*)____ret);
}

bool Live2DSharp::String::equals(Live2DSharp::LDString^ v)
{
    auto &arg0 = *(::live2d::LDString*)v->NativePtr;
    auto __ret = ((::live2d::String*)NativePtr)->equals(arg0);
    return __ret;
}

bool Live2DSharp::String::equals(System::String^ v)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(v);
    auto arg0 = _arg0.c_str();
    auto __ret = ((::live2d::String*)NativePtr)->equals(arg0);
    return __ret;
}

bool Live2DSharp::String::equals(int v)
{
    auto __ret = ((::live2d::String*)NativePtr)->equals(v);
    return __ret;
}

bool Live2DSharp::String::equals(double v)
{
    auto __ret = ((::live2d::String*)NativePtr)->equals(v);
    return __ret;
}

bool Live2DSharp::String::equals(bool v)
{
    auto __ret = ((::live2d::String*)NativePtr)->equals(v);
    return __ret;
}

bool Live2DSharp::String::isString::get()
{
    auto __ret = ((::live2d::String*)NativePtr)->isString();
    return __ret;
}

Live2DSharp::Error::Error(::live2d::Error* native)
    : Live2DSharp::String((::live2d::String*)native)
{
}

Live2DSharp::Error^ Live2DSharp::Error::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::Error((::live2d::Error*) native.ToPointer());
}

Live2DSharp::Value^ Live2DSharp::Error::setError_notForClientCall(System::String^ s)
{
    auto _arg0 = clix::marshalString<clix::E_UTF8>(s);
    auto arg0 = _arg0.c_str();
    auto __ret = ((::live2d::Error*)NativePtr)->setError_notForClientCall(arg0);
    if (__ret == nullptr) return nullptr;
    return (__ret == nullptr) ? nullptr : gcnew Live2DSharp::Value((::live2d::Value*)__ret);
}

bool Live2DSharp::Error::isStatic::get()
{
    auto __ret = ((::live2d::Error*)NativePtr)->isStatic();
    return __ret;
}

Live2DSharp::NullValue::NullValue(::live2d::NullValue* native)
    : Live2DSharp::Value((::live2d::Value*)native)
{
}

Live2DSharp::NullValue^ Live2DSharp::NullValue::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::NullValue((::live2d::NullValue*) native.ToPointer());
}

Live2DSharp::LDString^ Live2DSharp::NullValue::toString(Live2DSharp::LDString^ defaultV, Live2DSharp::LDString^ indent)
{
    auto &arg0 = *(::live2d::LDString*)defaultV->NativePtr;
    auto &arg1 = *(::live2d::LDString*)indent->NativePtr;
    auto __ret = ((::live2d::NullValue*)NativePtr)->toString(arg0, arg1);
    auto ____ret = new ::live2d::LDString(__ret);
    return (____ret == nullptr) ? nullptr : gcnew Live2DSharp::LDString((::live2d::LDString*)____ret);
}

bool Live2DSharp::NullValue::isNull::get()
{
    auto __ret = ((::live2d::NullValue*)NativePtr)->isNull();
    return __ret;
}

bool Live2DSharp::NullValue::isStatic::get()
{
    auto __ret = ((::live2d::NullValue*)NativePtr)->isStatic();
    return __ret;
}

Live2DSharp::Array::Array(::live2d::Array* native)
    : Live2DSharp::Value((::live2d::Value*)native)
{
}

Live2DSharp::Array^ Live2DSharp::Array::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::Array((::live2d::Array*) native.ToPointer());
}

Live2DSharp::Array::Array(Live2DSharp::MemoryParam^ memParam)
    : Live2DSharp::Value((::live2d::Value*)nullptr)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    NativePtr = new ::live2d::Array(arg0);
}

Live2DSharp::LDString^ Live2DSharp::Array::toString(Live2DSharp::LDString^ defaultV, Live2DSharp::LDString^ indent)
{
    auto &arg0 = *(::live2d::LDString*)defaultV->NativePtr;
    auto &arg1 = *(::live2d::LDString*)indent->NativePtr;
    auto __ret = ((::live2d::Array*)NativePtr)->toString(arg0, arg1);
    auto ____ret = new ::live2d::LDString(__ret);
    return (____ret == nullptr) ? nullptr : gcnew Live2DSharp::LDString((::live2d::LDString*)____ret);
}

void Live2DSharp::Array::add(Live2DSharp::Value^ v)
{
    auto arg0 = (::live2d::Value*)v->NativePtr;
    ((::live2d::Array*)NativePtr)->add(arg0);
}

Live2DSharp::Value^ Live2DSharp::Array::default::get(int index)
{
    auto &__ret = ((::live2d::Array*)NativePtr)->operator[](index);
    return (&__ret == nullptr) ? nullptr : gcnew Live2DSharp::Value((::live2d::Value*)&__ret);
}

void Live2DSharp::Array::default::set(int index, Live2DSharp::Value^ value)
{
    ((::live2d::Array*)NativePtr)->operator[](index) = *(::live2d::Value*)value->NativePtr;
}

Live2DSharp::Value^ Live2DSharp::Array::default::get(Live2DSharp::LDString^ s)
{
    auto &arg0 = *(::live2d::LDString*)s->NativePtr;
    auto &__ret = ((::live2d::Array*)NativePtr)->operator[](arg0);
    return (&__ret == nullptr) ? nullptr : gcnew Live2DSharp::Value((::live2d::Value*)&__ret);
}

void Live2DSharp::Array::default::set(Live2DSharp::LDString^ s, Live2DSharp::Value^ value)
{
    ((::live2d::Array*)NativePtr)->operator[](*(::live2d::LDString*)s->NativePtr) = *(::live2d::Value*)value->NativePtr;
}

bool Live2DSharp::Array::isArray::get()
{
    auto __ret = ((::live2d::Array*)NativePtr)->isArray();
    return __ret;
}

int Live2DSharp::Array::size::get()
{
    auto __ret = ((::live2d::Array*)NativePtr)->size();
    return __ret;
}

Live2DSharp::Map::Map(::live2d::Map* native)
    : Live2DSharp::Value((::live2d::Value*)native)
{
}

Live2DSharp::Map^ Live2DSharp::Map::__CreateInstance(::System::IntPtr native)
{
    return gcnew Live2DSharp::Map((::live2d::Map*) native.ToPointer());
}

Live2DSharp::Map::Map(Live2DSharp::MemoryParam^ memParam)
    : Live2DSharp::Value((::live2d::Value*)nullptr)
{
    auto arg0 = (::live2d::MemoryParam*)memParam->NativePtr;
    NativePtr = new ::live2d::Map(arg0);
}

Live2DSharp::LDString^ Live2DSharp::Map::toString(Live2DSharp::LDString^ defaultV, Live2DSharp::LDString^ indent)
{
    auto &arg0 = *(::live2d::LDString*)defaultV->NativePtr;
    auto &arg1 = *(::live2d::LDString*)indent->NativePtr;
    auto __ret = ((::live2d::Map*)NativePtr)->toString(arg0, arg1);
    auto ____ret = new ::live2d::LDString(__ret);
    return (____ret == nullptr) ? nullptr : gcnew Live2DSharp::LDString((::live2d::LDString*)____ret);
}

void Live2DSharp::Map::put(Live2DSharp::LDString^ key, Live2DSharp::Value^ v)
{
    auto &arg0 = *(::live2d::LDString*)key->NativePtr;
    auto arg1 = (::live2d::Value*)v->NativePtr;
    ((::live2d::Map*)NativePtr)->put(arg0, arg1);
}

Live2DSharp::Value^ Live2DSharp::Map::default::get(Live2DSharp::LDString^ s)
{
    auto &arg0 = *(::live2d::LDString*)s->NativePtr;
    auto &__ret = ((::live2d::Map*)NativePtr)->operator[](arg0);
    return (&__ret == nullptr) ? nullptr : gcnew Live2DSharp::Value((::live2d::Value*)&__ret);
}

void Live2DSharp::Map::default::set(Live2DSharp::LDString^ s, Live2DSharp::Value^ value)
{
    ((::live2d::Map*)NativePtr)->operator[](*(::live2d::LDString*)s->NativePtr) = *(::live2d::Value*)value->NativePtr;
}

Live2DSharp::Value^ Live2DSharp::Map::default::get(int index)
{
    auto &__ret = ((::live2d::Map*)NativePtr)->operator[](index);
    return (&__ret == nullptr) ? nullptr : gcnew Live2DSharp::Value((::live2d::Value*)&__ret);
}

void Live2DSharp::Map::default::set(int index, Live2DSharp::Value^ value)
{
    ((::live2d::Map*)NativePtr)->operator[](index) = *(::live2d::Value*)value->NativePtr;
}

bool Live2DSharp::Map::isMap::get()
{
    auto __ret = ((::live2d::Map*)NativePtr)->isMap();
    return __ret;
}

int Live2DSharp::Map::size::get()
{
    auto __ret = ((::live2d::Map*)NativePtr)->size();
    return __ret;
}

