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
    public class L2DPhysicsSetupData
    {
        [DataMember(Name = "length")]
        public double Length
        {
            get;
            set;
        }

        [DataMember(Name = "regist")]
        public double Regist
        {
            get;
            set;
        }

        [DataMember(Name = "mass")]
        public double Mass
        {
            get;
            set;
        }
    }

    [DataContract]
    public class PhysicsParameterData
    {
        [DataMember(Name = "id")]
        public string Id
        {
            get;
            set;
        }

        [DataMember(Name = "ptype")]
        public string PType
        {
            get;
            set;
        }

        [DataMember(Name = "scale")]
        public double Scale
        {
            get;
            set;
        }

        [DataMember(Name = "weight")]
        public double Weight
        {
            get;
            set;
        }
    }

    [DataContract]
    public class L2DPhysicsHairData
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public L2DPhysicsHairData()
        {
            Srcs = new List<PhysicsParameterData>();
            Targets =  new List<PhysicsParameterData>();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Srcs = Srcs ?? new List<PhysicsParameterData>();
            Targets = Targets ?? new List<PhysicsParameterData>();
        }

        [DataMember(Name = "setup")]
        public L2DPhysicsSetupData Setup
        {
            get;
            set;
        }

        [DataMember(Name = "src")]
        public List<PhysicsParameterData> Srcs
        {
            get;
            private set;
        }

        [DataMember(Name = "targets")]
        public List<PhysicsParameterData> Targets
        {
            get;
            private set;
        }
    }

    [DataContract]
    public class L2DPhysicsData
    {
        /// <summary>
        /// Jsonファイルから読み込みます。
        /// </summary>
        public static L2DPhysicsData Load(string filepath)
        {
            return JsonUtil.DeserializeFromFile<L2DPhysicsData>(filepath);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public L2DPhysicsData()
        {
            PhysicsHair = new List<L2DPhysicsHairData>();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            PhysicsHair = PhysicsHair ?? new List<L2DPhysicsHairData>();
        }

        [DataMember(Name = "physics_hair")]
        public List<L2DPhysicsHairData> PhysicsHair
        {
            get;
            private set;
        }
    }
}
