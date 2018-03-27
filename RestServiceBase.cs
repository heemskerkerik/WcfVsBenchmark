﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using MessagePack;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

using ZeroFormatter;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    public abstract class RestServiceBase<T>
        where T: class, new()
    {
        protected void InitializeClients()
        {
            _client.BaseAddress = new Uri($"http://localhost:{_port}");
        }

        public abstract void Start();
        public abstract void Stop();

        public Task<IReadOnlyCollection<T>> InvokeAsync()
        {
            return _asyncInvokeFunc();
        }

        public IReadOnlyCollection<T> Invoke()
        {
            return _invokeFunc();
        }

        private async Task<IReadOnlyCollection<T>> InvokeJsonNet()
        {
            var response = await _client.PostAsJsonAsync($"api/operation/{typeof(T).Name}?itemCount={_itemCountToRequest}", _itemsToSend);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<T[]>();
        }

        private async Task<IReadOnlyCollection<T>> InvokeMessagePackHttpClient()
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

        private IReadOnlyCollection<T> InvokeMessagePackHttpWebRequest()
        {
            var request = WebRequest.CreateHttp($"http://localhost:{_port}/api/operation/{typeof(T).Name}?itemCount={_itemCountToRequest}");
            request.ContentType = "application/x-msgpack";
            request.Method = HttpMethods.Post;

            using(var requestStream = request.GetRequestStream())
            {
                MessagePackSerializer.Serialize(requestStream, _itemsToSend);
            }

            using(var response = (HttpWebResponse) request.GetResponse())
            using(var responseStream = response.GetResponseStream())
            {
                if(response.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Response status was not 200 OK but {response.StatusCode}.");

                return MessagePackSerializer.Deserialize<T[]>(responseStream);
            }
        }

        private IReadOnlyCollection<T> InvokeZeroFormatterHttpWebRequest()
        {
            var request = WebRequest.CreateHttp($"http://localhost:{_port}/api/operation/{typeof(T).Name}?itemCount={_itemCountToRequest}");
            request.ContentType = "application/x-zeroformatter";
            request.Method = HttpMethods.Post;

            using(var requestStream = request.GetRequestStream())
            {
                ZeroFormatterSerializer.Serialize(requestStream, _itemsToSend);
            }

            using(var response = (HttpWebResponse) request.GetResponse())
            using(var responseStream = response.GetResponseStream())
            {
                if(response.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Response status was not 200 OK but {response.StatusCode}.");

                return ZeroFormatterSerializer.Deserialize<T[]>(responseStream);
            }
        }

        protected RestServiceBase(int port, SerializerType format, bool useHttpClient, int itemCount)
        {
            _port = port;
            _format = format;

            _itemsToSend = Cache.Get<T>().Take(itemCount).ToArray();
            _itemCountToRequest = itemCount;

            if(useHttpClient)
            {
                _asyncInvokeFunc = GetAsyncInvokeFunction();
            }
            else
            {
                _invokeFunc = GetInvokeFunction();
            }
        }

        private Func<Task<IReadOnlyCollection<T>>> GetAsyncInvokeFunction()
        {
            switch(_format)
            {
                case SerializerType.Xml:
                    return InvokeXml;
                case SerializerType.JsonNet:
                    return InvokeJsonNet;
                case SerializerType.MessagePack:
                    return InvokeMessagePackHttpClient;
                case SerializerType.Utf8Json:
                    return InvokeUtf8Json;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_format), _format, null);
            }
        }

        private Func<IReadOnlyCollection<T>> GetInvokeFunction()
        {
            switch(_format)
            {
                case SerializerType.MessagePack:
                    return InvokeMessagePackHttpWebRequest;
                case SerializerType.ZeroFormatter:
                    return InvokeZeroFormatterHttpWebRequest;

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
        private readonly Func<Task<IReadOnlyCollection<T>>> _asyncInvokeFunc;
        private readonly Func<IReadOnlyCollection<T>> _invokeFunc;
    }

    public abstract class RestClientBase<T>
    {
        private readonly int _itemCount;

        protected int Port { get; }
        protected string TypeName { get; } = typeof(T).Name;
        protected T[] ItemsToSend { get; }
        protected string RelativeUri => $"api/operation/{TypeName}?itemCount={_itemCount}";
        protected string AbsoluteUri => $"http://localhost:{Port}/api/operation/{TypeName}?itemCount={_itemCount}";

        public virtual void Initialize()
        {
        }

        public abstract IReadOnlyCollection<T> Invoke();

        public RestClientBase(int port, int itemCount)
        {
            Port = port;
            _itemCount = itemCount;
            ItemsToSend = Cache.Get<T>().Take(itemCount).ToArray();
        }
    }

    public abstract class HttpClientRestClientBase<T>: RestClientBase<T>
    {
        private readonly HttpClient _client;
        private MediaTypeFormatter[] _mediaTypeFormatters;

        protected HttpClientRestClientBase(int port, int itemCount)
            : base(port, itemCount)
        {
            _client = new HttpClient
                      {
                          BaseAddress = new Uri($"http://localhost:{Port}"),
                      };
        }

        protected abstract MediaTypeFormatter MediaTypeFormatter { get; }

        public override void Initialize()
        {
            _mediaTypeFormatters = new[] { MediaTypeFormatter };
        }

        public override IReadOnlyCollection<T> Invoke()
        {
            var response = _client.PostAsync(RelativeUri, ItemsToSend, MediaTypeFormatter).Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<T[]>(_mediaTypeFormatters).Result;
        }

        public async Task<IReadOnlyCollection<T>> InvokeAsync()
        {
            var response = await _client.PostAsync(RelativeUri, ItemsToSend, MediaTypeFormatter);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<T[]>(_mediaTypeFormatters);
        }
    }

    public class JsonNetHttpClientClient<T>: HttpClientRestClientBase<T>
    {
        public JsonNetHttpClientClient(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override MediaTypeFormatter MediaTypeFormatter { get; } = new JsonMediaTypeFormatter();
    }

    public abstract class HttpWebRequestClientBase<T>: RestClientBase<T>
    {
        protected HttpWebRequestClientBase(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected abstract string RequestContentType { get; }

        public override IReadOnlyCollection<T> Invoke()
        {
            var request = WebRequest.CreateHttp(AbsoluteUri);
            request.Method = HttpMethods.Post;
            request.ContentType = RequestContentType;

            using(var requestStream = request.GetRequestStream())
            {
                SerializeItems(requestStream, ItemsToSend);
            }

            using(var response = (HttpWebResponse) request.GetResponse())
            using(var responseStream = response.GetResponseStream())
            {
                if(response.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Response status was not 200 OK but {response.StatusCode}.");

                return Deserialize(responseStream);
            }
        }

        protected abstract void SerializeItems(Stream stream, T[] items);
        protected abstract IReadOnlyCollection<T> Deserialize(Stream stream);
    }

    public class JsonNetHttpWebRequestClient<T>: HttpWebRequestClientBase<T>
    {
        private readonly JsonSerializer _serializer = new JsonSerializer();
        private readonly UTF8Encoding _encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        public JsonNetHttpWebRequestClient(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override string RequestContentType => "application/json";

        protected override void SerializeItems(Stream stream, T[] items)
        {
            using(var writer = new JsonTextWriter(new StreamWriter(stream, _encoding)))
                _serializer.Serialize(writer, items);
        }

        protected override IReadOnlyCollection<T> Deserialize(Stream stream)
        {
            using(var reader = new JsonTextReader(new StreamReader(stream)))
                return _serializer.Deserialize<T[]>(reader);
        }
    }

    public class MessagePackHttpClientClient<T>: HttpClientRestClientBase<T>
    {
        public MessagePackHttpClientClient(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override MediaTypeFormatter MediaTypeFormatter { get; } = new MessagePackMediaTypeFormatter<T>();
    }

    public class MessagePackHttpWebRequestClient<T>: HttpWebRequestClientBase<T>
    {
        public MessagePackHttpWebRequestClient(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override string RequestContentType => "application/x-msgpack";

        protected override void SerializeItems(Stream stream, T[] items)
        {
            MessagePackSerializer.Serialize(stream, items);
        }

        protected override IReadOnlyCollection<T> Deserialize(Stream stream)
        {
            return MessagePackSerializer.Deserialize<T[]>(stream);
        }
    }

    public abstract class PrecomputedRestClientBase<T>: RestClientBase<T>
    {
        private byte[] _dataToSend;

        protected abstract string RequestContentType { get; }

        protected abstract void SerializeItems(Stream stream);

        public override void Initialize()
        {
            using(var stream = new MemoryStream())
            {
                SerializeItems(stream);
                _dataToSend = stream.ToArray();
            }
        }

        protected Stream CreateStreamWithDataToSend()
        {
            return new MemoryStream(_dataToSend);
        }

        protected PrecomputedRestClientBase(int port, int itemCount)
            : base(port, itemCount)
        {
        }
    }

    public abstract class PrecomputedHttpClientRestClientBase<T>: PrecomputedRestClientBase<T>
    {
        private readonly HttpClient _client;
        private MediaTypeHeaderValue _contentTypeHeader;

        protected PrecomputedHttpClientRestClientBase(int port, int itemCount)
            : base(port, itemCount)
        {
            _client = new HttpClient
                      {
                          BaseAddress = new Uri($"http://localhost:{Port}"),
                      };
        }

        public override void Initialize()
        {
            base.Initialize();

            _contentTypeHeader = new MediaTypeHeaderValue(RequestContentType);
        }

        public override IReadOnlyCollection<T> Invoke()
        {
            var response = _client.PostAsync(RelativeUri, CreateHttpContent())
                                  .Result;
            response.EnsureSuccessStatusCode();

            response.Content.ReadAsByteArrayAsync().Wait();

            return new List<T>();
        }

        private HttpContent CreateHttpContent()
        {
            return new StreamContent(CreateStreamWithDataToSend())
                   {
                       Headers =
                       {
                           ContentType = _contentTypeHeader,
                       }
                   };
        }

        public async Task<IReadOnlyCollection<T>> InvokeAsync()
        {
            var response = await _client.PostAsync(RelativeUri, CreateHttpContent());
            response.EnsureSuccessStatusCode();

            await response.Content.ReadAsByteArrayAsync();

            return new List<T>();
        }
    }

    public class JsonNetPrecomputedHttpClientClient<T>: PrecomputedHttpClientRestClientBase<T>
    {
        public JsonNetPrecomputedHttpClientClient(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override string RequestContentType => "application/json";

        protected override void SerializeItems(Stream stream)
        {
            using(var writer = new JsonTextWriter(new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false))))
                new JsonSerializer().Serialize(writer, ItemsToSend);
        }
    }

    public class MessagePackPrecomputedHttpClientClient<T>: PrecomputedHttpClientRestClientBase<T>
    {
        public MessagePackPrecomputedHttpClientClient(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override string RequestContentType => "application/x-msgpack";

        protected override void SerializeItems(Stream stream)
        {
            MessagePackSerializer.Serialize(stream, ItemsToSend);
        }
    }

    public abstract class PrecomputedHttpWebRequestRestClientBase<T>: PrecomputedRestClientBase<T>
    {
        protected PrecomputedHttpWebRequestRestClientBase(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        public override IReadOnlyCollection<T> Invoke()
        {
            var request = WebRequest.CreateHttp(AbsoluteUri);
            request.Method = HttpMethods.Post;
            request.ContentType = RequestContentType;

            using(var requestStream = request.GetRequestStream())
            using(var sourceStream = CreateStreamWithDataToSend())
            {
                sourceStream.CopyTo(requestStream);
            }

            using(var response = (HttpWebResponse) request.GetResponse())
            using(var responseStream = response.GetResponseStream())
            {
                if(response.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Response status was not 200 OK but {response.StatusCode}.");

                responseStream.CopyTo(Stream.Null);

                return new List<T>();
            }
        }
    }

    public class JsonNetPrecomputedHttpWebRequestClient<T>: PrecomputedHttpWebRequestRestClientBase<T>
    {
        public JsonNetPrecomputedHttpWebRequestClient(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override string RequestContentType => "application/json";

        protected override void SerializeItems(Stream stream)
        {
            using(var writer = new JsonTextWriter(new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false))))
                new JsonSerializer().Serialize(writer, ItemsToSend);
        }
    }

    public class MessagePackPrecomputedHttpWebRequestClient<T>: PrecomputedHttpWebRequestRestClientBase<T>
    {
        public MessagePackPrecomputedHttpWebRequestClient(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override string RequestContentType => "application/x-msgpack";

        protected override void SerializeItems(Stream stream)
        {
            MessagePackSerializer.Serialize(stream, ItemsToSend);
        }
    }

}
