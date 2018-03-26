using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using MessagePack;

namespace WcfVsWebApiVsAspNetCoreBenchmark
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

        public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            return await MessagePackSerializer.DeserializeAsync<T[]>(readStream);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            return MessagePackSerializer.SerializeAsync(writeStream, (T[]) value);
        }

        public MessagePackMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/x-msgpack"));
        }
    }
}
