using System;

using MessagePack;

namespace AspNetCoreWcfBenchmark
{
    [MessagePackObject]
    public class Item
    {
        [Key(0)]
        public Guid Id { get; set; }
    }
}
