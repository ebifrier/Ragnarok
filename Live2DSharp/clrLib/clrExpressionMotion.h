#pragma once

#include "CppSharp.h"
#include <ExpressionMotion.h>
#include "motion/clrAMotion.h"

namespace Live2DSharp
{
    ref class ALive2DModel;
    ref class MotionQueueEnt;
    namespace Framework
    {
        enum struct ExpressionType;
        ref class ExpressionMotion;
        ref class ExpressionParam;
    }
}

namespace Live2DSharp
{
    namespace Framework
    {
        /// <summary>
        /// Expression�̌v�Z��ނ�����܂��B
        /// </summary>
        public enum struct ExpressionType
        {
            EXPRESSIONTYPE_SET = 0,
            EXPRESSIONTYPE_ADD = 1,
            EXPRESSIONTYPE_MULTIPLY = 2
        };

        /// <summary>
        /// L2DExpressionBase�œ������p�����[�^��ێ����܂��B
        /// </summary>
        public ref class ExpressionParam : ICppInstance
        {
        public:

            property ::live2d::framework::ExpressionParam* NativePtr;
            property System::IntPtr __Instance
            {
                virtual System::IntPtr get();
                virtual void set(System::IntPtr instance);
            }

            ExpressionParam(::live2d::framework::ExpressionParam* native);
            static ExpressionParam^ __CreateInstance(::System::IntPtr native);
            ExpressionParam();

            property System::String^ pid
            {
                System::String^ get();
                void set(System::String^);
            }

            property Live2DSharp::Framework::ExpressionType type
            {
                Live2DSharp::Framework::ExpressionType get();
                void set(Live2DSharp::Framework::ExpressionType);
            }

            property float value
            {
                float get();
                void set(float);
            }
        };

        public ref class ExpressionMotion : Live2DSharp::AMotion
        {
        public:

            ExpressionMotion(::live2d::framework::ExpressionMotion* native);
            static ExpressionMotion^ __CreateInstance(::System::IntPtr native);
            ExpressionMotion();

            property System::Collections::Generic::List<Live2DSharp::Framework::ExpressionParam^>^ ParamList
            {
                System::Collections::Generic::List<Live2DSharp::Framework::ExpressionParam^>^ get();
                void set(System::Collections::Generic::List<Live2DSharp::Framework::ExpressionParam^>^);
            }
        };
    }
}
