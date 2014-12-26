using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using Ragnarok.Utility;

namespace Live2DSharp.Framework
{
    [DataContract]
    public class L2DExpressionParamData
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "val")]
        public double Value { get; set; }

        [DataMember(Name = "calc")]
        public string Calc { get; set; }

        [DataMember(Name = "def")]
        public double? Default { get; set; }
    }

    [DataContract]
    public sealed class L2DExpressionData
    {
        /// <summary>
        /// 読み込みます。
        /// </summary>
        public static L2DExpressionData Load(string filepath)
        {
            return JsonUtil.DeserializeFromFile<L2DExpressionData>(filepath);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public L2DExpressionData()
        {
            Params = new List<L2DExpressionParamData>();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Params = Params ?? new List<L2DExpressionParamData>();
        }

        [DataMember(Name = "fade_in")]
        public int? FadeIn
        {
            get;
            set;
        }

        [DataMember(Name = "fade_out")]
        public int? FadeOut
        {
            get;
            set;
        }

        [DataMember(Name = "params")]
        public List<L2DExpressionParamData> Params
        {
            get;
            private set;
        }
    }
}
