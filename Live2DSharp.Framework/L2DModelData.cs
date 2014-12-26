using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

using Ragnarok.Utility;

namespace Live2DSharp.Framework
{
    [DataContract()]
    public class L2DModelExpressionData
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "file")]
        public string File { get; set; }
    }

    [DataContract()]
    public class L2DLayoutData
    {
        [DataMember(Name = "width")]
        public double? Width { get; set; }

        [DataMember(Name = "height")]
        public double? Height { get; set; }

        [DataMember(Name = "center_x")]
        public double? CenterX { get; set; }

        [DataMember(Name = "center_y")]
        public double? CenterY { get; set; }

        [DataMember(Name = "x")]
        public double? X { get; set; }

        [DataMember(Name = "y")]
        public double? Y { get; set; }

        [DataMember(Name = "left")]
        public double? Left { get; set; }

        [DataMember(Name = "top")]
        public double? Top { get; set; }

        [DataMember(Name = "right")]
        public double? Right { get; set; }

        [DataMember(Name = "bottom")]
        public double? Bottom { get; set; }
    }

    [DataContract()]
    public class L2DHitAreaData
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }
    }

    [DataContract()]
    public class L2DMotionData
    {
        [DataMember(Name = "file")]
        public string File { get; set; }

        [DataMember(Name = "sound")]
        public string Sound { get; set; }

        [DataMember(Name = "fade_in")]
        public int FadeIn { get; set; }

        [DataMember(Name = "fade_out")]
        public int FadeOut { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// スキーマURL:
    /// http://sites.cybernoids.jp/cubism2/sdk_tutorial/framework/loadmodel/json
    /// </remarks>
    [DataContract()]
    public class L2DModelData
    {
        /// <summary>
        /// ファイルから読み込みます。
        /// </summary>
        public static L2DModelData Load(string filepath)
        {
            return JsonUtil.DeserializeFromFile<L2DModelData>(filepath);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public L2DModelData()
        {
            Textures = new List<string>();
            Expressions = new List<L2DModelExpressionData>();
            HitAreas = new List<L2DHitAreaData>();
            Motions = new Dictionary<string, List<L2DMotionData>>();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Textures = Textures ?? new List<string>();
            Expressions = Expressions ?? new List<L2DModelExpressionData>();
            HitAreas = HitAreas ?? new List<L2DHitAreaData>();
            Motions = Motions ?? new Dictionary<string, List<L2DMotionData>>();
        }

        [DataMember(Name = "version")]
        public string Version { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "model")]
        public string ModelFile { get; set; }

        [DataMember(Name = "textures")]
        public List<string> Textures
        {
            get;
            private set;
        }

        [DataMember(Name = "physics")]
        public string PhysicsFile { get; set; }

        [DataMember(Name = "pose")]
        public string PoseFile { get; set; }

        [DataMember(Name = "expressions")]
        public List<L2DModelExpressionData> Expressions
        {
            get;
            private set;
        }

        [DataMember(Name = "layout")]
        public L2DLayoutData Layout
        {
            get;
            set;
        }

        [DataMember(Name = "hit_areas")]
        public List<L2DHitAreaData> HitAreas
        {
            get;
            private set;
        }

        [DataMember(Name = "motions")]
        public Dictionary<string, List<L2DMotionData>> Motions
        {
            get;
            private set;
        }
    }
}
