using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

using AutoFixture;

namespace AspNetCoreWcfBenchmark
{
    public class RestServiceBase<T>
        where T: class, new()
    {
        protected void InitializeClients()
        {
            _client.BaseAddress = new Uri($"http://localhost:{_port}");
        }

        public Task<IReadOnlyCollection<T>> Invoke()
        {
            return _invokeFunc();
        }

        private async Task<IReadOnlyCollection<T>> InvokeJsonNet()
        {
            var response = await _client.PostAsJsonAsync($"api/operation/{typeof(T).Name}?itemCount={_itemCountToRequest}", _itemsToSend);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<T[]>();
        }

        private async Task<IReadOnlyCollection<T>> InvokeMessagePack()
        {
            var response = await _client.PostAsync($"api/operation/{typeof(T).Name}?itemCount={_itemCountToRequest}", new ObjectContent(typeof(IReadOnlyCollection<T>), _itemsToSend, _messagePackMediaTypeFormatter));
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<IReadOnlyCollection<T>>(new[] { _messagePackMediaTypeFormatter });
        }

        private async Task<IReadOnlyCollection<T>> InvokeXml()
        {
            var response = await _client.PostAsync($"api/operation/{typeof(T).Name}?itemCount={_itemCountToRequest}", new ObjectContent(typeof(IReadOnlyCollection<T>), _itemsToSend, _xmlMediaTypeFormatter));
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<T[]>(new[] { _xmlMediaTypeFormatter });
        }

        private async Task<IReadOnlyCollection<T>> InvokeUtf8Json()
        {
            var response = await _client.PostAsync($"api/operation/{typeof(T).Name}?itemCount={_itemCountToRequest}", new ObjectContent(typeof(IReadOnlyCollection<T>), _itemsToSend, _utf8JsonMediaTypeFormatter));
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<T[]>(new[] { _utf8JsonMediaTypeFormatter });
        }

        protected RestServiceBase(int port, SerializerType format, int itemCount, bool sendItems, bool receiveItems)
        {
            _port = port;
            _format = format;

            var fixture = new Fixture();

            _itemsToSend = sendItems
                               ? Cache.Get<T>().Take(itemCount).ToArray()
                               : new T[0];
            _itemCountToRequest = receiveItems ? itemCount : 0;

            _invokeFunc = GetInvokeFunction();
        }

        private Func<Task<IReadOnlyCollection<T>>> GetInvokeFunction()
        {
            switch(_format)
            {
                case SerializerType.Xml:
                    return InvokeXml;
                case SerializerType.JsonNet:
                    return InvokeJsonNet;
                case SerializerType.MessagePack:
                    return InvokeMessagePack;
                case SerializerType.Utf8Json:
                    return InvokeUtf8Json;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_format), _format, null);
            }
        }

        private readonly int _port;
        private readonly SerializerType _format;
        private readonly T[] _itemsToSend;
        private readonly int _itemCountToRequest;
        private readonly MessagePackMediaTypeFormatter<T> _messagePackMediaTypeFormatter = new MessagePackMediaTypeFormatter<T>();
        private readonly Utf8JsonMediaTypeFormatter<T> _utf8JsonMediaTypeFormatter = new Utf8JsonMediaTypeFormatter<T>();

        private readonly XmlMediaTypeFormatter _xmlMediaTypeFormatter = new XmlMediaTypeFormatter
                                                                        {
                                                                            UseXmlSerializer = true,
                                                                        };

        private readonly HttpClient _client = new HttpClient();
        private readonly Func<Task<IReadOnlyCollection<T>>> _invokeFunc;
    }
}
