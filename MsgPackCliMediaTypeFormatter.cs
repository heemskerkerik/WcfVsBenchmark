using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    public class MsgPackCliMediaTypeFormatter<T>: MediaTypeFormatter
    {
        private readonly MsgPack.Serialization.MessagePackSerializer<T[]> _serializer =
            MsgPack.Serialization.MessagePackSerializer.Get<T[]>();

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
            return await _serializer.UnpackAsync(readStream);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            return _serializer.PackAsync(writeStream, (T[]) value);
        }

        public MsgPackCliMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/x-msgpack"));
        }
    }
}