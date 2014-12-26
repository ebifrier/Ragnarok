using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ragnarok;

namespace Live2DSharp.Framework
{
    /// <summary>
    /// ポーズの各パーツを保持します。
    /// </summary>
    internal sealed class L2DPartsParam
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public L2DPartsParam(string id, IEnumerable<string> linkIds = null)
        {
            PartsID = id;
            LinkParts = (linkIds ?? new List<string>())
                .Select(_ => new L2DPartsParam(_))
                .ToList();
        }

        /// <summary>
        /// 対応するパーツＩＤを取得します。
        /// </summary>
        public string PartsID
        {
            get;
            private set;
        }

        /// <summary>
        /// 対応するパーツのインデックスを取得します。
        /// </summary>
        public int PartsIndex
        {
            get;
            private set;
        }

        public int ParamIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// 連動するパーツリストを取得します。
        /// </summary>
        public List<L2DPartsParam> LinkParts
        {
            get;
            private set;
        }

        /// <summary>
        /// パーツのインデックスなどを初期化します。
        /// </summary>
        public void InitIndex(ALive2DModel model)
        {
            var visibleParamID = "VISIBLE:" + PartsID;
            ParamIndex = model.getParamIndex(visibleParamID);

            PartsIndex = model.getPartsDataIndex(PartsID);
            model.setParamFloat(ParamIndex, 1, 1);
        }
    }

    /// <summary>
    /// Live2Dモデルのポーズを管理します。
    /// </summary>
    public sealed class L2DPose
    {
        /// <summary>
        /// この時間で不透明になります。
        /// </summary>
        public const double ClearTimeSec = 0.5;
        /// <summary>
        /// 背景が出にくいように、１＞０への変化を遅らせる場合は、
        /// 0.5よりも大きくする。ただし、あまり自然ではありません。
        /// </summary>
        public const double Phi = 0.5;
        public const double MaxBackOpacity = 0.15;

        private List<List<L2DPartsParam>> partsGroupList;
        private ALive2DModel lastModel;// パラメータインデックスが初期化されてるかどうかのチェック用。

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public L2DPose()
        {
            this.partsGroupList = new List<List<L2DPartsParam>>();
        }

        /// <summary>
        /// モデルのパラメータを更新。
        /// </summary>
        public void UpdateParam(ALive2DModel model, TimeSpan elapsed)
        {
            // 前回のモデルと同じではないときは初期化が必要
            if (model != lastModel)
            {
                InitParam(model);
            }
            lastModel = model;

            var elapsedSec = elapsed.TotalSeconds;
            foreach (var group in this.partsGroupList)
            {
                NormalizePartsOpacityGroup(model, elapsedSec, group);
            }

            CopyOpacityOtherParts(model);
        }

        /// <summary>
        /// 表示を初期化。
        /// </summary>
        /// <remarks>
        /// αの初期値が0でないパラメータは、αを1に設定します。
        /// </remarks>
        private void InitParam(ALive2DModel model)
        {
            foreach (var group in this.partsGroupList)
            {
                for (int j = 0; j < group.Count(); ++j)
                {
                    var param = group[j];
                    param.InitIndex(model);

                    // indexがない場合は処理をスキップします。
                    if (param.PartsIndex < 0)
                    {
                        continue;
                    }

                    var value = (j == 0 ? 1.0f : 0.0f);
                    model.setPartsOpacity(param.PartsIndex, value);
                    model.setParamFloat(param.ParamIndex, value, 1.0f);

                    // リンクパーツの初期化も行います。
                    foreach (var link in param.LinkParts)
                    {
                        link.InitIndex(model);
                    }
                }
            }
        }

        /// <summary>
        /// パーツのフェードイン、フェードアウトを設定します。
        /// </summary>
        private void NormalizePartsOpacityGroup(ALive2DModel model, double elapsedSec,
                                                List<L2DPartsParam> group)
        {
            var visibleParts = -1;
            var visibleOpacity = 1.0;

            //  現在、表示状態になっているパーツを取得
            for (int i = 0; i < group.Count(); ++i)
            {
                int partsIndex = group[i].PartsIndex;
                int paramIndex = group[i].ParamIndex;

                if (model.getParamFloat(paramIndex) != 0)
                {
                    visibleParts = i;
                    visibleOpacity = model.getPartsOpacity(partsIndex);

                    //  新しいOpacityを計算
                    visibleOpacity += elapsedSec / ClearTimeSec;
                    visibleOpacity = Math.Min(1.0, visibleOpacity);
                    break;
                }
            }

            if (visibleParts < 0)
            {
                visibleParts = 0;
                visibleOpacity = 1;
            }

            //  表示パーツ、非表示パーツの透明度を設定する
            for (int i = 0; i < group.Count(); ++i)
            {
                int partsIndex = group[i].PartsIndex;

                //  表示パーツの設定
                if (visibleParts == i)
                {
                    model.setPartsOpacity(partsIndex, (float)visibleOpacity);
                }
                //  非表示パーツの設定
                else
                {
                    double a1; // 計算によって求められる透明度

                    if (visibleOpacity < Phi)
                    {
                        // (0,1)-(Phi,Phi) を通る直線式
                        a1 = visibleOpacity * (Phi - 1) / Phi + 1;
                    }
                    else
                    {
                        // (1,0)-(Phi,Phi)を通る直線式
                        a1 = (1 - visibleOpacity) * Phi / (1 - Phi);
                    }

                    // 背景の見える割合を制限する場合
                    var backOpacity = (1 - a1) * (1 - visibleOpacity);
                    if (backOpacity > MaxBackOpacity)
                    {
                        a1 = 1 - MaxBackOpacity / (1 - visibleOpacity);
                    }

                    // 計算の透明度よりも大きければ（濃ければ）透明度を上げる
                    var opacity = Math.Min(
                        model.getPartsOpacity(partsIndex),
                        (float)a1);

                    model.setPartsOpacity(partsIndex, opacity);
                }
            }
        }

        /// <summary>
        /// パーツのαを連動させます。
        /// </summary>
        private void CopyOpacityOtherParts(ALive2DModel model)
        {
            var list = this.partsGroupList.SelectMany(_ => _);
            foreach (var parts in list)
            {
                var partsIndex = parts.PartsIndex;
                var opacity = model.getPartsOpacity(partsIndex);

                // リンクするパーツのαを設定します。
                parts.LinkParts
                    .Select(_ => _.PartsIndex)
                    .Where(_ => _ >= 0)
                    .ForEach(_ => model.setPartsOpacity(_, opacity));
            }
        }

        /// <summary>
        /// JSONファイルからパーツを読み込みます。
        /// </summary>
        /// <remarks>
        /// スキーマURL:
        /// http://sites.cybernoids.jp/cubism2/sdk_tutorial/framework/pose/json
        /// </remarks>
        public static L2DPose Load(string filepath)
        {
            var json = L2DPoseData.Load(filepath);
            var ret = new L2DPose();

            // パーツ切替一覧
            foreach (var poseData in json.VisibleParts)
            {
                // IDリストの作成
                var list = poseData.Groups
                    .Select(_ => new L2DPartsParam(_.Id, _.Link))
                    .ToList();

                ret.partsGroupList.Add(list);
            }

            return ret;
        }
    }
}
