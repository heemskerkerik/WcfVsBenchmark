using System;

using MsgPack.Serialization;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    public class MsgPackCliSmallItem
    {
        [MessagePackMember(0)]
        public Guid Id { get; set; }
    }
}
