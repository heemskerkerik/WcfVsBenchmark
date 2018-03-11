using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using MessagePack;

namespace AspNetCoreWcfBenchmark
{
    public class MessagePackMediaTypeFormatter<T>: MediaTypeFormatter
    {
        public override bool CanReadType(Type type)
        {
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var obj = MessagePackSerializer.NonGeneric.Deserialize(type, readStream);

            return Task.FromResult(obj);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            MessagePackSerializer.NonGeneric.Serialize(type, writeStream, value);
            return Task.CompletedTask;
        }

        public MessagePackMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/x-msgpack"));
        }
    }
}
