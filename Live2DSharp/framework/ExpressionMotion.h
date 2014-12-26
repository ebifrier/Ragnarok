/*
 *
 */
#pragma once

// Live2D lib
#include "motion/AMotion.h"
#include "ALive2DModel.h"
#include <string>
#include <vector>

namespace live2d
{
	namespace framework
	{
		/**
		 * Expressionの計算種類を示します。
		 */
		enum ExpressionType
		{
			EXPRESSIONTYPE_SET,
			EXPRESSIONTYPE_ADD,
			EXPRESSIONTYPE_MULTIPLY,
		};

		/**
		 * L2DExpressionBaseで動かすパラメータを保持します。
		 */
		struct ExpressionParam
		{
			std::string pid;
            ExpressionType type;
			float value;
		};

		class ExpressionMotion : public AMotion
		{
		private:
            std::vector<ExpressionParam> paramList;

		public:
            virtual ~ExpressionMotion() {}

			/**
			 * 動かすパラメータリストを取得します。
			 */
            const std::vector<ExpressionParam> &getParamList() const
			{
				return paramList;
			}

			/**
			 * 動かすパラメータリストを設定します。
			 */
			void setParamList(const std::vector<ExpressionParam> &other)
			{
				paramList = other;
			}

        protected:
			virtual void updateParamExe(live2d::ALive2DModel *model, l2d_int64 timeMSec,
										float weight, MotionQueueEnt *motionQueueEnt);
		};
	}
}