using System;

using MessagePack;

using ZeroFormatter;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    [MessagePackObject]
    [ZeroFormattable]
    public class SmallItem
    {
        [Key(0)]
        [Index(0)]
        public virtual Guid Id { get; set; }
    }
}
