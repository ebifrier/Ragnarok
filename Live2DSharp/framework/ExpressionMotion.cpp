/*
 *
 */

#include "ExpressionMotion.h"

using namespace std;

namespace live2d
{
	namespace framework
	{
		/**
		 * モーションの更新処理を行います。
		 */
        void ExpressionMotion::updateParamExe(ALive2DModel *model, l2d_int64 timeMSec,
										   			 float weight, MotionQueueEnt *motionQueueEnt)
		{
			int len = paramList.size();

			for (int i = 0; i < len; i++)
			{
				ExpressionParam& param = paramList[i];

				switch (param.type)
				{
				case EXPRESSIONTYPE_ADD:
					model->addToParamFloat(param.pid.c_str(), param.value, weight);
					break;
				case EXPRESSIONTYPE_MULTIPLY:
					model->multParamFloat(param.pid.c_str(), param.value, weight);
					break;
				case EXPRESSIONTYPE_SET:
					model->setParamFloat(param.pid.c_str(), param.value, weight);
					break;
				}
			}
		}
	}
}