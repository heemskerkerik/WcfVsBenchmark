using System;

using MessagePack;

namespace AspNetCoreWcfBenchmark
{
    [MessagePackObject]
    public class SmallItem
    {
        [Key(0)]
        public Guid Id { get; set; }
    }
}
