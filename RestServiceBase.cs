using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using MessagePack;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

using ZeroFormatter;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    public abstract class RestServiceBase
    {
        public abstract void Start();
        public abstract void Stop();
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
                          DefaultRequestHeaders =
                          {
                              ConnectionClose = false,
                              ExpectContinue = false,
                          },
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
            request.KeepAlive = true;
            request.Expect = null;

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
        protected byte[] DataToSend { get; private set; }

        protected abstract string RequestContentType { get; }

        protected abstract void SerializeItems(Stream stream);

        public override void Initialize()
        {
            using(var stream = new MemoryStream())
            {
                SerializeItems(stream);
                DataToSend = stream.ToArray();
            }
        }

        protected Stream CreateStreamWithDataToSend()
        {
            return new MemoryStream(DataToSend);
        }

        protected PrecomputedRestClientBase(int port, int itemCount)
            : base(port, itemCount)
        {
        }
    }

    public class XmlHttpClientClient<T>: HttpClientRestClientBase<T>
    {
        public XmlHttpClientClient(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override MediaTypeFormatter MediaTypeFormatter { get; } = new XmlMediaTypeFormatter { UseXmlSerializer = true };
    }

    public class XmlHttpWebRequestClient<T>: HttpWebRequestClientBase<T>
    {
        private XmlSerializer _serializer = new XmlSerializer(typeof(T[]));

        public XmlHttpWebRequestClient(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override string RequestContentType => "application/xml";

        protected override void SerializeItems(Stream stream, T[] items)
        {
            using(var writer = XmlWriter.Create(stream))
                _serializer.Serialize(writer, items);
        }

        protected override IReadOnlyCollection<T> Deserialize(Stream stream)
        {
            using(var reader = XmlReader.Create(stream))
                return (T[]) _serializer.Deserialize(reader);
        }
    }

    public class Utf8JsonHttpClientClient<T>: HttpClientRestClientBase<T>
    {
        public Utf8JsonHttpClientClient(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override MediaTypeFormatter MediaTypeFormatter { get; } = new Utf8JsonMediaTypeFormatter<T>();
    }

    public class Utf8JsonHttpWebRequestClient<T>: HttpWebRequestClientBase<T>
    {
        public Utf8JsonHttpWebRequestClient(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override string RequestContentType => "application/json";

        protected override void SerializeItems(Stream stream, T[] items)
        {
            Utf8Json.JsonSerializer.Serialize(stream, items);
        }

        protected override IReadOnlyCollection<T> Deserialize(Stream stream)
        {
            return Utf8Json.JsonSerializer.Deserialize<T[]>(stream);
        }
    }

    public class ZeroFormatterHttpClientClient<T>: HttpClientRestClientBase<T>
    {
        public ZeroFormatterHttpClientClient(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override MediaTypeFormatter MediaTypeFormatter { get; } = new ZeroFormatterMediaTypeFormatter<T>();
    }

    public class ZeroFormatterHttpWebRequestClient<T>: HttpWebRequestClientBase<T>
    {
        public ZeroFormatterHttpWebRequestClient(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override string RequestContentType => "application/x-zeroformatter";

        protected override void SerializeItems(Stream stream, T[] items)
        {
            ZeroFormatterSerializer.Serialize(stream, items);
        }

        protected override IReadOnlyCollection<T> Deserialize(Stream stream)
        {
            return ZeroFormatterSerializer.Deserialize<T[]>(stream);
        }
    }

    public class MsgPackCliHttpClientClient<T>: HttpClientRestClientBase<T>
    {
        public MsgPackCliHttpClientClient(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override MediaTypeFormatter MediaTypeFormatter { get; } = new MsgPackCliMediaTypeFormatter<T>();
    }

    public class MsgPackCliHttpWebRequestClient<T>: HttpWebRequestClientBase<T>
    {
        private readonly MsgPack.Serialization.MessagePackSerializer<T[]> _serializer =
            MsgPack.Serialization.MessagePackSerializer.Get<T[]>();

        public MsgPackCliHttpWebRequestClient(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override string RequestContentType => "application/x-msgpack";

        protected override void SerializeItems(Stream stream, T[] items)
        {
            _serializer.Pack(stream, items);
        }

        protected override IReadOnlyCollection<T> Deserialize(Stream stream)
        {
            return _serializer.Unpack(stream);
        }
    }
}
