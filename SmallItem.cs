using System;

using MessagePack;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    [MessagePackObject]
    public class SmallItem
    {
        [Key(0)]
        public Guid Id { get; set; }
    }
}
