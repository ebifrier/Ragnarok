using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Live2DSharp;

using Ragnarok;

namespace Live2DSharp.Framework
{
    /// <summary>
    /// 髪を揺らすためのオブジェクトです。
    /// </summary>
    public sealed class L2DPhysics
    {
        private List<PhysicsHair> physicsObjects;
        private List<string> paramIDs;
        private long totalEllapsedMillis;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public L2DPhysics()
        {
            this.physicsObjects = new List<PhysicsHair>();
            this.paramIDs = new List<string>();
            this.totalEllapsedMillis = 0;
        }

        /// <summary>
        /// モデルのパラメータを更新します。
        /// </summary>
        public void UpdateParam(ALive2DModel model, TimeSpan elapsed)
        {
            this.totalEllapsedMillis += (long)elapsed.TotalMilliseconds;

            this.physicsObjects.ForEach(_ =>
                _.update(model, this.totalEllapsedMillis));
        }

        /// <summary>
        /// JSONファイルから読み込みます。
        /// </summary>
        /// <remarks>
        /// スキーマURL:
        /// http://sites.cybernoids.jp/cubism2/sdk_tutorial/framework/physics/json
        /// </remarks>
        public static L2DPhysics Load(string filepath)
        {
            var ret = new L2DPhysics();
            var json = L2DPhysicsData.Load(filepath);

            // 物理演算一覧			
            foreach (var hairData in json.PhysicsHair)
            {
                var setupData = hairData.Setup;

                var physics = new PhysicsHair();
                physics.setup(
                    (float)setupData.Length, // 長さ
                    (float)setupData.Regist, // 空気抵抗
                    (float)setupData.Mass);  // 質量

                // 元パラメータの設定
                foreach (var src in hairData.Srcs)
                {
                    var type = PhysicsHair.Src.SRC_TO_X;
                    var typeStr = src.PType;
                    if (typeStr == "x")
                    {
                        type = PhysicsHair.Src.SRC_TO_X;
                    }
                    else if (typeStr == "y")
                    {
                        type = PhysicsHair.Src.SRC_TO_Y;
                    }
                    else if (typeStr == "angle")
                    {
                        type = PhysicsHair.Src.SRC_TO_G_ANGLE;
                    }
                    else
                    {
                        Log.Error("Invalid parameter:PhysicsHair.Src");
                    }

                    var scale = (float)src.Scale;
                    var weight = (float)src.Weight;
                    ret.paramIDs.Add(src.Id);
                    physics.addSrcParam(type, src.Id, scale, weight);
                }

                // 対象パラメータの設定
                foreach (var target in hairData.Targets)
                {
                    var type = PhysicsHair.Target.TARGET_FROM_ANGLE;
                    var typeStr = target.PType;
                    if (typeStr == "angle")
                    {
                        type = PhysicsHair.Target.TARGET_FROM_ANGLE;
                    }
                    else if (typeStr == "angle_v")
                    {
                        type = PhysicsHair.Target.TARGET_FROM_ANGLE_V;
                    }
                    else
                    {
                        Log.Error("Invalid parameter:PhysicsHair.Target");
                    }

                    float scale = (float)target.Scale;
                    float weight = (float)target.Weight;
                    ret.paramIDs.Add(target.Id);
                    physics.addTargetParam(type, target.Id, scale, weight);
                }

                ret.physicsObjects.Add(physics);
            }

            return ret;
        }
    }
}
