﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using ZeroFormatter;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    public class ZeroFormatterMediaTypeFormatter<T>: MediaTypeFormatter
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
            using(var copyStream = readStream.CanSeek ? new MemoryStream((int) readStream.Length) : new MemoryStream())
            {
                readStream.CopyTo(copyStream);
                copyStream.Position = 0;

                var result = ZeroFormatterSerializer.Deserialize<T[]>(copyStream);
                return Task.FromResult<object>(result);
            }
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            ZeroFormatterSerializer.Serialize(writeStream, (T[]) value);
            return Task.CompletedTask;
        }

        public ZeroFormatterMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/x-zeroformatter"));
        }
    }
}
