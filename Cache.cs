using System;
using System.Linq;

using AutoFixture;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    internal static class Cache
    {
        public static readonly SmallItem[] SmallItems = Enumerable.Range(0, 1000).Select(_ => new SmallItem { Id = Guid.NewGuid() }).ToArray();
        public static readonly LargeItem[] LargeItems = new Fixture().CreateMany<LargeItem>(count: 1000).ToArray();
        public static readonly MessagePackSmallItem[] MessagePackSmallItems = Enumerable.Range(0, 1000).Select(_ => new MessagePackSmallItem { Id = Guid.NewGuid() }).ToArray();
        public static readonly MessagePackLargeItem[] MessagePackLargeItems = new Fixture().CreateMany<MessagePackLargeItem>(count: 1000).ToArray();
        public static readonly ZeroFormatterSmallItem[] ZeroFormatterSmallItems = Enumerable.Range(0, 1000).Select(_ => new ZeroFormatterSmallItem { Id = Guid.NewGuid() }).ToArray();
        public static readonly ZeroFormatterLargeItem[] ZeroFormatterLargeItems = new Fixture().CreateMany<ZeroFormatterLargeItem>(count: 1000).ToArray();

        public static T[] Get<T>()
        {
            if(typeof(T) == typeof(SmallItem))
                return (T[]) (object) SmallItems;
            if(typeof(T) == typeof(LargeItem))
                return (T[]) (object) LargeItems;
            if(typeof(T) == typeof(MessagePackSmallItem))
                return (T[]) (object) MessagePackSmallItems;
            if(typeof(T) == typeof(MessagePackLargeItem))
                return (T[]) (object) MessagePackLargeItems;
            if(typeof(T) == typeof(ZeroFormatterSmallItem))
                return (T[]) (object) ZeroFormatterSmallItems;
            if(typeof(T) == typeof(ZeroFormatterLargeItem))
                return (T[]) (object) ZeroFormatterLargeItems;

            throw new InvalidOperationException();
        }
    }
}
