using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<WcfVsWebApiVsAspNetCoreMvc>();
        }
    }

    [Config(typeof(Config))]
    public class WcfVsWebApiVsAspNetCoreMvc
    {
        private class Config: ManualConfig
        {
            public Config()
            {
                Add(Job.Clr);
                Add(ConsoleLogger.Default);
                Add(TargetMethodColumn.Method, new ParamColumn(nameof(ItemCount)), new ParamColumn(nameof(SendItems)), new ParamColumn(nameof(ReceiveItems)), StatisticColumn.Mean, StatisticColumn.P95);
                Add(MemoryDiagnoser.Default.GetColumnProvider());
                Add(EnvironmentAnalyser.Default);
                Add(MemoryDiagnoser.Default);
                Add(MarkdownExporter.Default);
                UnionRule = ConfigUnionRule.AlwaysUseLocal;
            }
        }

        [Params(0, 10, 100, 1000)]
        public int ItemCount { get; set; }

        public bool SendItems { get; set; } = true;
        public bool ReceiveItems { get; set; } = true;

        [GlobalSetup]
        public void Initialize()
        {
            int port = 9001;

            _smallTextWcfService = new WcfService<SmallItem>(
                port: port++,
                encoding: WSMessageEncoding.Text,
                itemCount: ItemCount,
                sendItems: SendItems,
                receiveItems: ReceiveItems
            );
            _smallTextWcfService.Start();

            _smallWebApiJsonNetService = new WebApiService<SmallItem>(
                port: port++,
                format: SerializerType.JsonNet,
                itemCount: ItemCount,
                sendItems: SendItems,
                receiveItems: ReceiveItems
            );
            _smallWebApiJsonNetService.Start();

            _smallWebApiMessagePackService = new WebApiService<SmallItem>(
                port: port++,
                format: SerializerType.MessagePack,
                itemCount: ItemCount,
                sendItems: SendItems,
                receiveItems: ReceiveItems
            );
            _smallWebApiMessagePackService.Start();

            _smallWebApiXmlService = new WebApiService<SmallItem>(
                port: port++,
                format: SerializerType.Xml,
                itemCount: ItemCount,
                sendItems: SendItems,
                receiveItems: ReceiveItems
            );
            _smallWebApiXmlService.Start();

            _smallWebApiUtf8JsonService = new WebApiService<SmallItem>(
                port: port++,
                format: SerializerType.Utf8Json,
                itemCount: ItemCount,
                sendItems: SendItems,
                receiveItems: ReceiveItems
            );
            _smallWebApiUtf8JsonService.Start();

            _smallAspNetCoreJsonNetService = new AspNetCoreService<SmallItem>(
                port: port++,
                format:SerializerType.JsonNet,
                itemCount: ItemCount,
                sendItems: SendItems,
                receiveItems: ReceiveItems
            );
            _smallAspNetCoreJsonNetService.Start();

            _smallAspNetCoreMessagePackService = new AspNetCoreService<SmallItem>(
                port: port++,
                format: SerializerType.MessagePack,
                itemCount: ItemCount,
                sendItems: SendItems,
                receiveItems: ReceiveItems
            );
            _smallAspNetCoreMessagePackService.Start();

            _smallAspNetCoreXmlService = new AspNetCoreService<SmallItem>(
                port: port++,
                format: SerializerType.Xml,
                itemCount: ItemCount,
                sendItems: SendItems,
                receiveItems: ReceiveItems
            );
            _smallAspNetCoreXmlService.Start();

            _smallAspNetCoreUtf8JsonService = new AspNetCoreService<SmallItem>(
                port: port++,
                format: SerializerType.Utf8Json,
                itemCount: ItemCount,
                sendItems: SendItems,
                receiveItems: ReceiveItems
            );
            _smallAspNetCoreUtf8JsonService.Start();

            _largeTextWcfService = new WcfService<LargeItem>(
                port: port++,
                encoding: WSMessageEncoding.Text,
                itemCount: ItemCount,
                sendItems: SendItems,
                receiveItems: ReceiveItems
            );
            _largeTextWcfService.Start();

            _largeWebApiJsonNetService = new WebApiService<LargeItem>(
                port: port++,
                format: SerializerType.JsonNet,
                itemCount: ItemCount,
                sendItems: SendItems,
                receiveItems: ReceiveItems
            );
            _largeWebApiJsonNetService.Start();

            _largeWebApiMessagePackService = new WebApiService<LargeItem>(
                port: port++,
                format: SerializerType.MessagePack,
                itemCount: ItemCount,
                sendItems: SendItems,
                receiveItems: ReceiveItems
            );
            _largeWebApiMessagePackService.Start();

            _largeWebApiXmlService = new WebApiService<LargeItem>(
                port: port++,
                format: SerializerType.Xml,
                itemCount: ItemCount,
                sendItems: SendItems,
                receiveItems: ReceiveItems
            );
            _largeWebApiXmlService.Start();

            _largeWebApiUtf8JsonService = new WebApiService<LargeItem>(
                port: port++,
                format: SerializerType.Utf8Json,
                itemCount: ItemCount,
                sendItems: SendItems,
                receiveItems: ReceiveItems
            );
            _largeWebApiUtf8JsonService.Start();

            _largeAspNetCoreJsonNetService = new AspNetCoreService<LargeItem>(
                port: port++,
                format:SerializerType.JsonNet,
                itemCount: ItemCount,
                sendItems: SendItems,
                receiveItems: ReceiveItems
            );
            _largeAspNetCoreJsonNetService.Start();

            _largeAspNetCoreMessagePackService = new AspNetCoreService<LargeItem>(
                port: port++,
                format: SerializerType.MessagePack,
                itemCount: ItemCount,
                sendItems: SendItems,
                receiveItems: ReceiveItems
            );
            _largeAspNetCoreMessagePackService.Start();

            _largeAspNetCoreXmlService = new AspNetCoreService<LargeItem>(
                port: port++,
                format: SerializerType.Xml,
                itemCount: ItemCount,
                sendItems: SendItems,
                receiveItems: ReceiveItems
            );
            _largeAspNetCoreXmlService.Start();

            _largeAspNetCoreUtf8JsonService = new AspNetCoreService<LargeItem>(
                port: port++,
                format: SerializerType.Utf8Json,
                itemCount: ItemCount,
                sendItems: SendItems,
                receiveItems: ReceiveItems
            );
            _largeAspNetCoreUtf8JsonService.Start();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _smallTextWcfService?.Stop();
            _smallWebApiJsonNetService?.Stop();
            _smallWebApiMessagePackService?.Stop();
            _smallWebApiXmlService?.Stop();
            _smallWebApiUtf8JsonService?.Stop();
            _smallAspNetCoreJsonNetService?.Stop();
            _smallAspNetCoreMessagePackService?.Stop();
            _smallAspNetCoreXmlService?.Stop();
            _smallAspNetCoreUtf8JsonService?.Stop();
            _largeTextWcfService?.Stop();
            _largeWebApiJsonNetService?.Stop();
            _largeWebApiMessagePackService?.Stop();
            _largeWebApiXmlService?.Stop();
            _largeWebApiUtf8JsonService?.Stop();
            _largeAspNetCoreJsonNetService?.Stop();
            _largeAspNetCoreMessagePackService?.Stop();
            _largeAspNetCoreXmlService?.Stop();
            _largeAspNetCoreUtf8JsonService?.Stop();
        }

        [Benchmark]
        public IReadOnlyCollection<SmallItem> SmallWcfText()
        {
            return _smallTextWcfService.Invoke();
        }

        [Benchmark]
        public Task<IReadOnlyCollection<SmallItem>> SmallWebApiJsonNet()
        {
            return _smallWebApiJsonNetService.Invoke();
        }

        [Benchmark]
        public Task<IReadOnlyCollection<SmallItem>> SmallWebApiMessagePack()
        {
            return _smallWebApiMessagePackService.Invoke();
        }

        [Benchmark]
        public Task<IReadOnlyCollection<SmallItem>> SmallWebApiXml()
        {
            return _smallWebApiXmlService.Invoke();
        }

        [Benchmark]
        public Task<IReadOnlyCollection<SmallItem>> SmallWebApiUtf8Json()
        {
            return _smallWebApiUtf8JsonService.Invoke();
        }

        [Benchmark]
        public Task<IReadOnlyCollection<SmallItem>> SmallAspNetCoreJsonNet()
        {
            return _smallAspNetCoreJsonNetService.Invoke();
        }

        [Benchmark]
        public Task<IReadOnlyCollection<SmallItem>> SmallAspNetCoreMessagePack()
        {
            return _smallAspNetCoreMessagePackService.Invoke();
        }

        [Benchmark]
        public Task<IReadOnlyCollection<SmallItem>> SmallAspNetCoreXml()
        {
            return _smallAspNetCoreXmlService.Invoke();
        }

        [Benchmark]
        public Task<IReadOnlyCollection<SmallItem>> SmallAspNetCoreUtf8Json()
        {
            return _smallAspNetCoreUtf8JsonService.Invoke();
        }

        [Benchmark]
        public IReadOnlyCollection<LargeItem> LargeWcfText()
        {
            return _largeTextWcfService.Invoke();
        }

        [Benchmark]
        public Task<IReadOnlyCollection<LargeItem>> LargeWebApiJsonNet()
        {
            return _largeWebApiJsonNetService.Invoke();
        }

        [Benchmark]
        public Task<IReadOnlyCollection<LargeItem>> LargeWebApiMessagePack()
        {
            return _largeWebApiMessagePackService.Invoke();
        }

        [Benchmark]
        public Task<IReadOnlyCollection<LargeItem>> LargeWebApiXml()
        {
            return _largeWebApiXmlService.Invoke();
        }

        [Benchmark]
        public Task<IReadOnlyCollection<LargeItem>> LargeWebApiUtf8Json()
        {
            return _largeWebApiUtf8JsonService.Invoke();
        }

        [Benchmark]
        public Task<IReadOnlyCollection<LargeItem>> LargeAspNetCoreJsonNet()
        {
            return _largeAspNetCoreJsonNetService.Invoke();
        }

        [Benchmark]
        public Task<IReadOnlyCollection<LargeItem>> LargeAspNetCoreMessagePack()
        {
            return _largeAspNetCoreMessagePackService.Invoke();
        }

        [Benchmark]
        public Task<IReadOnlyCollection<LargeItem>> LargeAspNetCoreXml()
        {
            return _largeAspNetCoreXmlService.Invoke();
        }

        [Benchmark]
        public Task<IReadOnlyCollection<LargeItem>> LargeAspNetCoreUtf8Json()
        {
            return _largeAspNetCoreUtf8JsonService.Invoke();
        }

        private WcfService<SmallItem> _smallTextWcfService;
        private WebApiService<SmallItem> _smallWebApiJsonNetService;
        private WebApiService<SmallItem> _smallWebApiMessagePackService;
        private WebApiService<SmallItem> _smallWebApiXmlService;
        private WebApiService<SmallItem> _smallWebApiUtf8JsonService;
        private AspNetCoreService<SmallItem> _smallAspNetCoreJsonNetService;
        private AspNetCoreService<SmallItem> _smallAspNetCoreMessagePackService;
        private AspNetCoreService<SmallItem> _smallAspNetCoreXmlService;
        private AspNetCoreService<SmallItem> _smallAspNetCoreUtf8JsonService;
        private WcfService<LargeItem> _largeTextWcfService;
        private WebApiService<LargeItem> _largeWebApiJsonNetService;
        private WebApiService<LargeItem> _largeWebApiMessagePackService;
        private WebApiService<LargeItem> _largeWebApiXmlService;
        private WebApiService<LargeItem> _largeWebApiUtf8JsonService;
        private AspNetCoreService<LargeItem> _largeAspNetCoreJsonNetService;
        private AspNetCoreService<LargeItem> _largeAspNetCoreMessagePackService;
        private AspNetCoreService<LargeItem> _largeAspNetCoreXmlService;
        private AspNetCoreService<LargeItem> _largeAspNetCoreUtf8JsonService;
    }
}
