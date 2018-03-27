using System;

using MessagePack;

using ZeroFormatter;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    [ZeroFormattable]
    public class ZeroFormatterSmallItem
    {
        [Index(0)]
        public virtual Guid Id { get; set; }
    }
}
