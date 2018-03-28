using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

using Microsoft.Owin.Hosting;

using Owin;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    public class WebApiService<T>: RestServiceBase<T>
        where T: class, new()
    {
        public override void Start()
        {
            _app = WebApp.Start($"http://localhost:{_port}", Configuration);
            InitializeClients();
        }

        public override void Stop()
        {
            _app.Dispose();
        }

        public virtual void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();
            config.Formatters.Clear();

            config.Formatters.Add(GetMediaTypeFormatter());

            app.UseWebApi(config);
        }

        public virtual MediaTypeFormatter GetMediaTypeFormatter()
        {
            switch(_format)
            {
                case SerializerType.Xml:
                    return new XmlMediaTypeFormatter
                           {
                               UseXmlSerializer = true,
                           };
                case SerializerType.JsonNet:
                    return new JsonMediaTypeFormatter();
                case SerializerType.MessagePack:
                    return new MessagePackMediaTypeFormatter<T>();
                case SerializerType.Utf8Json:
                    return new Utf8JsonMediaTypeFormatter<T>();
                case SerializerType.ZeroFormatter:
                    return new ZeroFormatterMediaTypeFormatter<T>();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public WebApiService(int port, SerializerType format, bool useHttpClient, int itemCount)
            : base(port, format, useHttpClient, itemCount)
        {
            _port = port;
            _format = format;
        }

        private readonly int _port;
        private readonly SerializerType _format;
        private IDisposable _app;
    }

    public class WebApiController: ApiController
    {
        [HttpPost, Route("api/operation/SmallItem")]
        public SmallItem[] ItemOperation([FromBody] SmallItem[] items, int itemCount)
        {
            return Cache.SmallItems.Take(itemCount).ToArray();
        }

        [HttpPost, Route("api/operation/LargeItem")]
        public LargeItem[] LargeItemOperation([FromBody] LargeItem[] items, int itemCount)
        {
            return Cache.LargeItems.Take(itemCount).ToArray();
        }
    }

    public class JsonNetWebApiService<T>: WebApiService<T>
        where T: class, new()
    {
        public JsonNetWebApiService(int port, int itemCount)
            : base(port, SerializerType.JsonNet, true, itemCount)
        {
        }

        public override MediaTypeFormatter GetMediaTypeFormatter()
        {
            return new JsonMediaTypeFormatter();
        }
    }

    public class XmlWebApiService<T>: WebApiService<T>
        where T: class, new()
    {
        public XmlWebApiService(int port, int itemCount)
            : base(port, SerializerType.Xml, true, itemCount)
        {
        }

        public override MediaTypeFormatter GetMediaTypeFormatter()
        {
            return new XmlMediaTypeFormatter
                   {
                       UseXmlSerializer = true,
                   };
        }
    }

    public class MessagePackWebApiService<T>: WebApiService<T>
        where T: class, new()
    {
        public MessagePackWebApiService(int port, int itemCount)
            : base(port, SerializerType.JsonNet, true, itemCount)
        {
        }

        public override MediaTypeFormatter GetMediaTypeFormatter()
        {
            return new MessagePackMediaTypeFormatter<T>();
        }
    }

    public class Utf8JsonWebApiService<T>: WebApiService<T>
        where T: class, new()
    {
        public Utf8JsonWebApiService(int port, int itemCount)
            : base(port, SerializerType.Utf8Json, true, itemCount)
        {
        }

        public override MediaTypeFormatter GetMediaTypeFormatter()
        {
            return new Utf8JsonMediaTypeFormatter<T>();
        }
    }

    public class ZeroFormatterWebApiService<T>: WebApiService<T>
        where T: class, new()
    {
        public ZeroFormatterWebApiService(int port, int itemCount)
            : base(port, SerializerType.ZeroFormatter, true, itemCount)
        {
        }

        public override MediaTypeFormatter GetMediaTypeFormatter()
        {
            return new ZeroFormatterMediaTypeFormatter<T>();
        }
    }
}
