using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

using Ragnarok.Net;
using Ragnarok.Net.ProtoBuf;

namespace Ragnarok.Test.Net
{
    [TestFixture()]
    public class ProtoBufTest
    {
        [Test()]
        public void BigDataTest()
        {
            var roomInfo = new VoteRoomInfo
            {
                Id = 1,
                Name = "投票ルーム",
                OwnerNo = 10,
                VoteSpan = TimeSpan.FromSeconds(30),
                TotalVoteSpan = TimeSpan.MaxValue,
                BaseTimeNtp = DateTime.MinValue,
                Password = null,
            };

            var list = new List<VoteParticipantInfo>();
            for (var i = 0; i < 1000; ++i)
            {
                var participant = new VoteParticipantInfo
                {
                    Id = Guid.Empty.ToString(),
                    ImageUrl = "http://garnet-alice.net/test/common/image.jpg",
                    Name = "ニコ生ちゃん",
                    No = -i,
                    LiveDataList = new LiveData[]
                    {
                        new LiveData(LiveSite.NicoNama, "lv4556323"),
                    },
                };

                list.Add(participant);
            }
            roomInfo.ParticipantList = list.ToArray();

            Assert.DoesNotThrow(() =>
            {
                var data = PbUtil.Serialize(roomInfo);

                using (var stream = new FileStream("protobuf.dump", FileMode.Create))
                {
                    stream.Write(data, 0, data.Length);
                }

                PbUtil.Deserialize(data, typeof(VoteRoomInfo));
            });
        }
    }
}
