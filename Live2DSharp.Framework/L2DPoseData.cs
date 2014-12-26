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
    public class PoseGroupData
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PoseGroupData()
        {
            Link = new List<string>();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Link = Link ?? new List<string>();
        }

        [DataMember(Name = "id")]
        public string Id
        {
            get;
            set;
        }

        [DataMember(Name = "link")]
        public List<string> Link
        {
            get;
            private set;
        }
    }

    [DataContract]
    public class PosePartsVisibleData
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PosePartsVisibleData()
        {
            Groups = new List<PoseGroupData>();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Groups = Groups ?? new List<PoseGroupData>();
        }

        [DataMember(Name = "group")]
        public List<PoseGroupData> Groups
        {
            get;
            private set;
        }
    }

    [DataContract]
    public class L2DPoseData
    {
        /// <summary>
        /// 
        /// </summary>
        public static L2DPoseData Load(string filepath)
        {
            return JsonUtil.DeserializeFromFile<L2DPoseData>(filepath);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public L2DPoseData()
        {
            VisibleParts = new List<PosePartsVisibleData>();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            VisibleParts = VisibleParts ?? new List<PosePartsVisibleData>();
        }

        [DataMember(Name = "parts_visible")]
        public List<PosePartsVisibleData> VisibleParts
        {
            get;
            private set;
        }
    }
}
