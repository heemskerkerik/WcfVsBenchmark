using System;

using MessagePack;

using ZeroFormatter;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    [MessagePackObject]
    public class MessagePackSmallItem
    {
        [Key(0)]
        public Guid Id { get; set; }
    }
}
