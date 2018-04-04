using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    public class RestBenchmarkSuite<T>
        where T: class, new()
    {
        public IReadOnlyCollection<T> InvokeHttpClient()
        {
            return _httpClientClient.Invoke();
        }

        public Task<IReadOnlyCollection<T>> InvokeHttpClientAsync()
        {
            return _httpClientClient.InvokeAsync();
        }

        public IReadOnlyCollection<T> InvokeHttpWebRequest()
        {
            return _httpWebRequestClient.Invoke();
        }

        public void Start()
        {
            _service.Start();
            _httpClientClient.Initialize();
            _httpWebRequestClient.Initialize();
        }

        public void Stop()
        {
            _service.Stop();
        }

        public RestBenchmarkSuite(int port, int itemCount, string format, string host)
        {
            _service = CreateService(port, Type.GetType($"{nameof(WcfVsWebApiVsAspNetCoreBenchmark)}.{format}{host}Service`1").MakeGenericType(typeof(T)));
            _httpClientClient = CreateHttpClientClient(port, itemCount, Type.GetType($"{nameof(WcfVsWebApiVsAspNetCoreBenchmark)}.{format}HttpClientClient`1").MakeGenericType(typeof(T)));
            _httpWebRequestClient = CreateHttpWebRequestClient(port, itemCount, Type.GetType($"{nameof(WcfVsWebApiVsAspNetCoreBenchmark)}.{format}HttpWebRequestClient`1").MakeGenericType(typeof(T)));
        }

        private RestServiceBase CreateService(int port, Type serviceType)
        {
            return (RestServiceBase) Activator.CreateInstance(
                serviceType,
                port
            );
        }

        private HttpClientRestClientBase<T> CreateHttpClientClient(int port, int itemCount, Type clientType)
        {
            return (HttpClientRestClientBase<T>) Activator.CreateInstance(
                clientType,
                port,
                itemCount
            );
        }

        private HttpWebRequestClientBase<T> CreateHttpWebRequestClient(int port, int itemCount, Type clientType)
        {
            return (HttpWebRequestClientBase<T>) Activator.CreateInstance(
                clientType,
                port,
                itemCount
            );
        }

        private readonly RestServiceBase _service;
        private readonly HttpClientRestClientBase<T> _httpClientClient;
        private readonly HttpWebRequestClientBase<T> _httpWebRequestClient;
    }
}
