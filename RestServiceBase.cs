using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCoreWcfBenchmark
{
    public class RestServiceBase
    {
        protected void InitializeClients()
        {
            _client.BaseAddress = new Uri($"http://localhost:{_port}");
        }

        public Task<IReadOnlyCollection<Item>> Invoke()
        {
            return _invokeFunc();
        }

        private async Task<IReadOnlyCollection<Item>> InvokeJsonNet()
        {
            var response = await _client.PostAsJsonAsync($"api/operation?itemCount={_itemCountToRequest}", _itemsToSend);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<Item[]>();
        }

        private async Task<IReadOnlyCollection<Item>> InvokeMessagePack()
        {
            var response = await _client.PostAsync($"api/operation?itemCount={_itemCountToRequest}", new ObjectContent(typeof(IReadOnlyCollection<Item>), _itemsToSend, _messagePackMediaTypeFormatter));
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<IReadOnlyCollection<Item>>(new[] { _messagePackMediaTypeFormatter });
        }

        protected RestServiceBase(int port, SerializerType format, int itemCount, bool sendItems, bool receiveItems)
        {
            _port = port;
            _format = format;

            _itemsToSend = sendItems
                               ? Enumerable.Range(0, itemCount).Select(_ => new Item { Id = Guid.NewGuid() }).ToArray()
                               : new Item[0];
            _itemCountToRequest = receiveItems ? itemCount : 0;

            _invokeFunc = GetInvokeFunction();
        }

        private Func<Task<IReadOnlyCollection<Item>>> GetInvokeFunction()
        {
            switch(_format)
            {
                case SerializerType.JsonNet:
                    return InvokeJsonNet;
                case SerializerType.MessagePack:
                    return InvokeMessagePack;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_format), _format, null);
            }
        }

        private readonly int _port;
        private readonly SerializerType _format;
        private readonly Item[] _itemsToSend;
        private readonly int _itemCountToRequest;
        private readonly MessagePackMediaTypeFormatter _messagePackMediaTypeFormatter = new MessagePackMediaTypeFormatter();

        private readonly HttpClient _client = new HttpClient();
        private readonly Func<Task<IReadOnlyCollection<Item>>> _invokeFunc;
    }
}
