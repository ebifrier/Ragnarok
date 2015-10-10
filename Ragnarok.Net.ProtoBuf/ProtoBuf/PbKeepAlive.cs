using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Ragnarok.Net.ProtoBuf
{
    /// <summary>
    /// 相手の存在確認に使います。
    /// </summary>
    [DataContract()]
    [Serializable()]
    internal sealed class PbKeepAliveRequest
    {
    }

    /// <summary>
    /// 相手の存在確認に使います。
    /// </summary>
    [DataContract()]
    [Serializable()]
    internal sealed class PbKeepAliveResponse
    {
    }
}
