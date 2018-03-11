using System;
using System.Linq;

using AutoFixture;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    internal static class Cache
    {
        public static readonly SmallItem[] SmallItems = Enumerable.Range(0, 1000).Select(_ => new SmallItem { Id = Guid.NewGuid() }).ToArray();
        public static readonly LargeItem[] LargeItems = new Fixture().CreateMany<LargeItem>(count: 1000).ToArray();

        public static T[] Get<T>()
        {
            if(typeof(T) == typeof(SmallItem))
                return (T[]) (object) SmallItems;
            if(typeof(T) == typeof(LargeItem))
                return (T[]) (object) LargeItems;

            throw new InvalidOperationException();
        }
    }
}
