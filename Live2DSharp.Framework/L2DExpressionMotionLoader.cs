using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Live2DSharp.Framework
{
    public static class L2DExpressionMotionLoader
    {
        /// <summary>
        /// JSONファイルから読み込み。
        /// </summary>
        /// <remarks>
        /// スキーマURL:
        /// http://sites.cybernoids.jp/cubism2/sdk_tutorial/framework/02-moshonnorodokara-dongkasumade-zhun-bei-zhong/expression/json
        /// </remarks>
        public static ExpressionMotion Load(string filepath)
        {
            var ret = new ExpressionMotion();
            var json = L2DExpressionData.Load(filepath);

            ret.FadeIn = json.FadeIn ?? 1000; // フェードイン
            ret.FadeOut = json.FadeOut ?? 1000; // フェードアウト

            // 各パラメータについて
            foreach (var param in json.Params)
            {
                var paramID = param.Id; // パラメータID
                var value = param.Value;// 値
                var defaultValue = 0.0;

                // 計算方法の設定
                ExpressionType calcTypeInt;
                switch (param.Calc)
                {
                    case "set":
                        calcTypeInt = ExpressionType.EXPRESSIONTYPE_SET;
                        break;
                    case "mult":
                        calcTypeInt = ExpressionType.EXPRESSIONTYPE_MULTIPLY;
                        defaultValue = param.Default ?? 1.0;
                        if (defaultValue == 0) defaultValue = 1;// 0(不正値)を指定した場合は1(標準)にする
                        value = value / defaultValue;
                        break;
                    default:
                        // 仕様にない値を設定したときも加算モードにすることで復旧
                        calcTypeInt = ExpressionType.EXPRESSIONTYPE_ADD;
                        defaultValue = param.Default ?? 0.0;
                        value = value - defaultValue;
                        break;
                }

                // 設定オブジェクトを作成してリストに追加する
                var item = new ExpressionParam
                {
                    pid = param.Id,
                    type = calcTypeInt,
                    value = (float)value,
                };
                ret.ParamList.Add(item);
            }

            return ret;
        }
    }
}
