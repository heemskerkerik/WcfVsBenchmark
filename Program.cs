﻿using System.Collections.Generic;
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
using BenchmarkDotNet.Toolchains.InProcess;

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
                Add(Job.Clr.With(InProcessToolchain.Instance));
                Add(ConsoleLogger.Default);
                Add(TargetMethodColumn.Method, new ParamColumn(nameof(ItemCount)), StatisticColumn.Mean, StatisticColumn.P95);
                Add(MemoryDiagnoser.Default.GetColumnProvider());
                Add(EnvironmentAnalyser.Default);
                Add(MemoryDiagnoser.Default);
                Add(MarkdownExporter.Default);
                UnionRule = ConfigUnionRule.AlwaysUseLocal;
            }
        }

        [Params(0, 10, 100, 1000)]
        public int ItemCount { get; set; }

        [GlobalSetup]
        public void Initialize()
        {
            int port = 9001;

            _smallTextWcfService = new WcfService<SmallItem>(
                port: port++,
                bindingType: WcfBindingType.BasicText,
                itemCount: ItemCount
            );
            _smallTextWcfService.Start();

            _smallWebXmlWcfService = new WcfService<SmallItem>(
                port: port++,
                bindingType: WcfBindingType.WebXml,
                itemCount: ItemCount
            );
            _smallWebXmlWcfService.Start();

            _smallWebJsonWcfService = new WcfService<SmallItem>(
                port: port++,
                bindingType: WcfBindingType.WebJson,
                itemCount: ItemCount
            );
            _smallWebJsonWcfService.Start();

            _smallBinaryWcfService = new WcfService<SmallItem>(
                port: port++,
                bindingType: WcfBindingType.BinaryMessageEncoding,
                itemCount: ItemCount
            );
            _smallBinaryWcfService.Start();

            _smallNetTcpWcfService = new WcfService<SmallItem>(
                port: port++,
                bindingType: WcfBindingType.NetTcp,
                itemCount: ItemCount
            );
            _smallNetTcpWcfService.Start();

            _smallWebApiJsonNetService = new WebApiService<SmallItem>(
                port: port++,
                format: SerializerType.JsonNet,
                useHttpClient: true,
                itemCount: ItemCount
            );
            _smallWebApiJsonNetService.Start();

            _smallWebApiMessagePackService = new WebApiService<SmallItem>(
                port: port++,
                format: SerializerType.MessagePack,
                useHttpClient: true,
                itemCount: ItemCount
            );
            _smallWebApiMessagePackService.Start();

            _smallWebApiXmlService = new WebApiService<SmallItem>(
                port: port++,
                format: SerializerType.Xml,
                useHttpClient: true,
                itemCount: ItemCount
            );
            _smallWebApiXmlService.Start();

            _smallWebApiUtf8JsonService = new WebApiService<SmallItem>(
                port: port++,
                format: SerializerType.Utf8Json,
                useHttpClient: true,
                itemCount: ItemCount
            );
            _smallWebApiUtf8JsonService.Start();

            _smallWebApiMessagePackHttpWebRequestService = new WebApiService<SmallItem>(
                port: port++,
                format: SerializerType.MessagePack,
                useHttpClient: false,
                itemCount: ItemCount
            );
            _smallWebApiMessagePackHttpWebRequestService.Start();

            _smallAspNetCoreJsonNetService = new AspNetCoreService<SmallItem>(
                port: port++,
                format: SerializerType.JsonNet,
                useHttpClient: true,
                itemCount: ItemCount
            );
            _smallAspNetCoreJsonNetService.Start();

            _smallAspNetCoreMessagePackService = new AspNetCoreService<SmallItem>(
                port: port++,
                format: SerializerType.MessagePack,
                useHttpClient: true,
                itemCount: ItemCount
            );
            _smallAspNetCoreMessagePackService.Start();

            _smallAspNetCoreXmlService = new AspNetCoreService<SmallItem>(
                port: port++,
                format: SerializerType.Xml,
                useHttpClient: true,
                itemCount: ItemCount
            );
            _smallAspNetCoreXmlService.Start();

            _smallAspNetCoreUtf8JsonService = new AspNetCoreService<SmallItem>(
                port: port++,
                format: SerializerType.Utf8Json,
                useHttpClient: true,
                itemCount: ItemCount
            );
            _smallAspNetCoreUtf8JsonService.Start();

            _smallAspNetCoreMessagePackHttpWebRequestService = new AspNetCoreService<SmallItem>(
                port: port++,
                format: SerializerType.MessagePack,
                useHttpClient: false,
                itemCount: ItemCount
            );
            _smallAspNetCoreMessagePackHttpWebRequestService.Start();

            _largeTextWcfService = new WcfService<LargeItem>(
                port: port++,
                bindingType: WcfBindingType.BasicText,
                itemCount: ItemCount
            );
            _largeTextWcfService.Start();

            _largeWebXmlWcfService = new WcfService<LargeItem>(
                port: port++,
                bindingType: WcfBindingType.WebXml,
                itemCount: ItemCount
            );
            _largeWebXmlWcfService.Start();

            _largeWebJsonWcfService = new WcfService<LargeItem>(
                port: port++,
                bindingType: WcfBindingType.WebJson,
                itemCount: ItemCount
            );
            _largeWebJsonWcfService.Start();

            _largeBinaryWcfService = new WcfService<LargeItem>(
                port: port++,
                bindingType: WcfBindingType.BinaryMessageEncoding,
                itemCount: ItemCount
            );
            _largeBinaryWcfService.Start();

            _largeNetTcpWcfService = new WcfService<LargeItem>(
                port: port++,
                bindingType: WcfBindingType.NetTcp,
                itemCount: ItemCount
            );
            _largeNetTcpWcfService.Start();

            _largeWebApiJsonNetService = new WebApiService<LargeItem>(
                port: port++,
                format: SerializerType.JsonNet,
                useHttpClient: true,
                itemCount: ItemCount
            );
            _largeWebApiJsonNetService.Start();

            _largeWebApiMessagePackService = new WebApiService<LargeItem>(
                port: port++,
                format: SerializerType.MessagePack,
                useHttpClient: true,
                itemCount: ItemCount
            );
            _largeWebApiMessagePackService.Start();

            _largeWebApiXmlService = new WebApiService<LargeItem>(
                port: port++,
                format: SerializerType.Xml,
                useHttpClient: true,
                itemCount: ItemCount
            );
            _largeWebApiXmlService.Start();

            _largeWebApiUtf8JsonService = new WebApiService<LargeItem>(
                port: port++,
                format: SerializerType.Utf8Json,
                useHttpClient: true,
                itemCount: ItemCount
            );
            _largeWebApiUtf8JsonService.Start();

            _largeWebApiMessagePackHttpWebRequestService = new WebApiService<LargeItem>(
                port: port++,
                format: SerializerType.MessagePack,
                useHttpClient: false,
                itemCount: ItemCount
            );
            _largeWebApiMessagePackHttpWebRequestService.Start();

            _largeAspNetCoreJsonNetService = new AspNetCoreService<LargeItem>(
                port: port++,
                format: SerializerType.JsonNet,
                useHttpClient: true,
                itemCount: ItemCount
            );
            _largeAspNetCoreJsonNetService.Start();

            _largeAspNetCoreMessagePackService = new AspNetCoreService<LargeItem>(
                port: port++,
                format: SerializerType.MessagePack,
                useHttpClient: true,
                itemCount: ItemCount
            );
            _largeAspNetCoreMessagePackService.Start();

            _largeAspNetCoreXmlService = new AspNetCoreService<LargeItem>(
                port: port++,
                format: SerializerType.Xml,
                useHttpClient: true,
                itemCount: ItemCount
            );
            _largeAspNetCoreXmlService.Start();

            _largeAspNetCoreUtf8JsonService = new AspNetCoreService<LargeItem>(
                port: port++,
                format: SerializerType.Utf8Json,
                useHttpClient: true,
                itemCount: ItemCount
            );
            _largeAspNetCoreUtf8JsonService.Start();

            _largeAspNetCoreMessagePackHttpWebRequestService = new AspNetCoreService<LargeItem>(
                port: port++,
                format: SerializerType.MessagePack,
                useHttpClient: false,
                itemCount: ItemCount
            );
            _largeAspNetCoreMessagePackHttpWebRequestService.Start();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _smallTextWcfService?.Stop();
            _smallWebXmlWcfService?.Stop();
            _smallWebJsonWcfService?.Stop();
            _smallBinaryWcfService?.Stop();
            _smallNetTcpWcfService?.Stop();
            _smallWebApiJsonNetService?.Stop();
            _smallWebApiMessagePackService?.Stop();
            _smallWebApiXmlService?.Stop();
            _smallWebApiUtf8JsonService?.Stop();
            _smallWebApiMessagePackHttpWebRequestService?.Stop();
            _smallAspNetCoreJsonNetService?.Stop();
            _smallAspNetCoreMessagePackService?.Stop();
            _smallAspNetCoreXmlService?.Stop();
            _smallAspNetCoreUtf8JsonService?.Stop();
            _smallAspNetCoreMessagePackHttpWebRequestService?.Stop();
            _largeTextWcfService?.Stop();
            _largeWebXmlWcfService?.Stop();
            _largeWebJsonWcfService?.Stop();
            _largeBinaryWcfService?.Stop();
            _largeNetTcpWcfService?.Stop();
            _largeWebApiJsonNetService?.Stop();
            _largeWebApiMessagePackService?.Stop();
            _largeWebApiXmlService?.Stop();
            _largeWebApiUtf8JsonService?.Stop();
            _largeWebApiMessagePackHttpWebRequestService?.Stop();
            _largeAspNetCoreJsonNetService?.Stop();
            _largeAspNetCoreMessagePackService?.Stop();
            _largeAspNetCoreXmlService?.Stop();
            _largeAspNetCoreUtf8JsonService?.Stop();
            _largeAspNetCoreMessagePackHttpWebRequestService?.Stop();
        }

        [Benchmark]
        public IReadOnlyCollection<SmallItem> SmallWcfText()
        {
            return _smallTextWcfService.Invoke();
        }

        //[Benchmark]
        public IReadOnlyCollection<SmallItem> SmallWcfWebXml()
        {
            return _smallWebXmlWcfService.Invoke();
        }

        //[Benchmark]
        public IReadOnlyCollection<SmallItem> SmallWcfWebJson()
        {
            return _smallWebJsonWcfService.Invoke();
        }

        [Benchmark]
        public IReadOnlyCollection<SmallItem> SmallWcfBinary()
        {
            return _smallBinaryWcfService.Invoke();
        }

        [Benchmark]
        public IReadOnlyCollection<SmallItem> SmallWcfNetTcp()
        {
            return _smallNetTcpWcfService.Invoke();
        }

        //[Benchmark]
        public Task<IReadOnlyCollection<SmallItem>> SmallWebApiJsonNet()
        {
            return _smallWebApiJsonNetService.InvokeAsync();
        }

        //[Benchmark]
        public Task<IReadOnlyCollection<SmallItem>> SmallWebApiMessagePack()
        {
            return _smallWebApiMessagePackService.InvokeAsync();
        }

        //[Benchmark]
        public Task<IReadOnlyCollection<SmallItem>> SmallWebApiXml()
        {
            return _smallWebApiXmlService.InvokeAsync();
        }

        //[Benchmark]
        public Task<IReadOnlyCollection<SmallItem>> SmallWebApiUtf8Json()
        {
            return _smallWebApiUtf8JsonService.InvokeAsync();
        }

        //[Benchmark]
        public IReadOnlyCollection<SmallItem> SmallWebApiMessagePackHttpWebRequest()
        {
            return _smallWebApiMessagePackHttpWebRequestService.Invoke();
        }

        //[Benchmark]
        public Task<IReadOnlyCollection<SmallItem>> SmallAspNetCoreJsonNet()
        {
            return _smallAspNetCoreJsonNetService.InvokeAsync();
        }

        //[Benchmark]
        public Task<IReadOnlyCollection<SmallItem>> SmallAspNetCoreMessagePack()
        {
            return _smallAspNetCoreMessagePackService.InvokeAsync();
        }

        //[Benchmark]
        public Task<IReadOnlyCollection<SmallItem>> SmallAspNetCoreXml()
        {
            return _smallAspNetCoreXmlService.InvokeAsync();
        }

        //[Benchmark]
        public Task<IReadOnlyCollection<SmallItem>> SmallAspNetCoreUtf8Json()
        {
            return _smallAspNetCoreUtf8JsonService.InvokeAsync();
        }

        [Benchmark]
        public IReadOnlyCollection<SmallItem> SmallAspNetCoreMessagePackHttpWebRequest()
        {
            return _smallAspNetCoreMessagePackHttpWebRequestService.Invoke();
        }

        //[Benchmark]
        public IReadOnlyCollection<LargeItem> LargeWcfText()
        {
            return _largeTextWcfService.Invoke();
        }

        //[Benchmark]
        public IReadOnlyCollection<LargeItem> LargeWcfWebXml()
        {
            return _largeWebXmlWcfService.Invoke();
        }

        //[Benchmark]
        public IReadOnlyCollection<LargeItem> LargeWcfWebJson()
        {
            return _largeWebJsonWcfService.Invoke();
        }

        //[Benchmark]
        public IReadOnlyCollection<LargeItem> LargeWcfBinary()
        {
            return _largeBinaryWcfService.Invoke();
        }

        //[Benchmark]
        public IReadOnlyCollection<LargeItem> LargeWcfNetTcp()
        {
            return _largeNetTcpWcfService.Invoke();
        }

        //[Benchmark]
        public Task<IReadOnlyCollection<LargeItem>> LargeWebApiJsonNet()
        {
            return _largeWebApiJsonNetService.InvokeAsync();
        }

        //[Benchmark]
        public Task<IReadOnlyCollection<LargeItem>> LargeWebApiMessagePack()
        {
            return _largeWebApiMessagePackService.InvokeAsync();
        }

        //[Benchmark]
        public Task<IReadOnlyCollection<LargeItem>> LargeWebApiXml()
        {
            return _largeWebApiXmlService.InvokeAsync();
        }

        //[Benchmark]
        public Task<IReadOnlyCollection<LargeItem>> LargeWebApiUtf8Json()
        {
            return _largeWebApiUtf8JsonService.InvokeAsync();
        }

        //[Benchmark]
        public IReadOnlyCollection<LargeItem> LargeWebApiMessagePackHttpWebRequest()
        {
            return _largeWebApiMessagePackHttpWebRequestService.Invoke();
        }

        //[Benchmark]
        public Task<IReadOnlyCollection<LargeItem>> LargeAspNetCoreJsonNet()
        {
            return _largeAspNetCoreJsonNetService.InvokeAsync();
        }
        
        //[Benchmark]
        public Task<IReadOnlyCollection<LargeItem>> LargeAspNetCoreMessagePack()
        {
            return _largeAspNetCoreMessagePackService.InvokeAsync();
        }

        //[Benchmark]
        public Task<IReadOnlyCollection<LargeItem>> LargeAspNetCoreXml()
        {
            return _largeAspNetCoreXmlService.InvokeAsync();
        }

        //[Benchmark]
        public Task<IReadOnlyCollection<LargeItem>> LargeAspNetCoreUtf8Json()
        {
            return _largeAspNetCoreUtf8JsonService.InvokeAsync();
        }

        //[Benchmark]
        public IReadOnlyCollection<LargeItem> LargeAspNetCoreMessagePackHttpWebRequest()
        {
            return _largeAspNetCoreMessagePackHttpWebRequestService.Invoke();
        }

        private WcfService<SmallItem> _smallTextWcfService;
        private WcfService<SmallItem> _smallWebXmlWcfService;
        private WcfService<SmallItem> _smallWebJsonWcfService;
        private WcfService<SmallItem> _smallBinaryWcfService;
        private WcfService<SmallItem> _smallNetTcpWcfService;
        private WebApiService<SmallItem> _smallWebApiJsonNetService;
        private WebApiService<SmallItem> _smallWebApiMessagePackService;
        private WebApiService<SmallItem> _smallWebApiXmlService;
        private WebApiService<SmallItem> _smallWebApiUtf8JsonService;
        private WebApiService<SmallItem> _smallWebApiMessagePackHttpWebRequestService;
        private AspNetCoreService<SmallItem> _smallAspNetCoreJsonNetService;
        private AspNetCoreService<SmallItem> _smallAspNetCoreMessagePackService;
        private AspNetCoreService<SmallItem> _smallAspNetCoreXmlService;
        private AspNetCoreService<SmallItem> _smallAspNetCoreUtf8JsonService;
        private AspNetCoreService<SmallItem> _smallAspNetCoreMessagePackHttpWebRequestService;
        private WcfService<LargeItem> _largeTextWcfService;
        private WcfService<LargeItem> _largeWebXmlWcfService;
        private WcfService<LargeItem> _largeWebJsonWcfService;
        private WcfService<LargeItem> _largeBinaryWcfService;
        private WcfService<LargeItem> _largeNetTcpWcfService;
        private WebApiService<LargeItem> _largeWebApiJsonNetService;
        private WebApiService<LargeItem> _largeWebApiMessagePackService;
        private WebApiService<LargeItem> _largeWebApiXmlService;
        private WebApiService<LargeItem> _largeWebApiUtf8JsonService;
        private WebApiService<LargeItem> _largeWebApiMessagePackHttpWebRequestService;
        private AspNetCoreService<LargeItem> _largeAspNetCoreJsonNetService;
        private AspNetCoreService<LargeItem> _largeAspNetCoreMessagePackService;
        private AspNetCoreService<LargeItem> _largeAspNetCoreXmlService;
        private AspNetCoreService<LargeItem> _largeAspNetCoreUtf8JsonService;
        private AspNetCoreService<LargeItem> _largeAspNetCoreMessagePackHttpWebRequestService;
    }
}
