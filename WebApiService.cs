using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

using Microsoft.Owin.Hosting;

using Owin;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    public abstract class WebApiService: RestServiceBase
    {
        public override void Start()
        {
            _app = WebApp.Start($"http://localhost:{_port}", Configuration);
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

        public abstract MediaTypeFormatter GetMediaTypeFormatter();

        protected WebApiService(int port)
        {
            _port = port;
        }

        private readonly int _port;
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

        [HttpPost, Route("api/operation/MessagePackSmallItem")]
        public MessagePackSmallItem[] ItemOperation([FromBody] MessagePackSmallItem[] items, int itemCount)
        {
            return Cache.MessagePackSmallItems.Take(itemCount).ToArray();
        }

        [HttpPost, Route("api/operation/MessagePackLargeItem")]
        public MessagePackLargeItem[] LargeItemOperation([FromBody] MessagePackLargeItem[] items, int itemCount)
        {
            return Cache.MessagePackLargeItems.Take(itemCount).ToArray();
        }

        [HttpPost, Route("api/operation/ZeroFormatterSmallItem")]
        public ZeroFormatterSmallItem[] ItemOperation([FromBody] ZeroFormatterSmallItem[] items, int itemCount)
        {
            return Cache.ZeroFormatterSmallItems.Take(itemCount).ToArray();
        }

        [HttpPost, Route("api/operation/ZeroFormatterLargeItem")]
        public ZeroFormatterLargeItem[] LargeItemOperation([FromBody] ZeroFormatterLargeItem[] items, int itemCount)
        {
            return Cache.ZeroFormatterLargeItems.Take(itemCount).ToArray();
        }
    }

    public class JsonNetWebApiService<T>: WebApiService
        where T: class, new()
    {
        public JsonNetWebApiService(int port)
            : base(port)
        {
        }

        public override MediaTypeFormatter GetMediaTypeFormatter()
        {
            return new JsonMediaTypeFormatter();
        }
    }

    public class XmlWebApiService<T>: WebApiService
        where T: class, new()
    {
        public XmlWebApiService(int port)
            : base(port)
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

    public class MessagePackWebApiService<T>: WebApiService
        where T: class, new()
    {
        public MessagePackWebApiService(int port)
            : base(port)
        {
        }

        public override MediaTypeFormatter GetMediaTypeFormatter()
        {
            return new MessagePackMediaTypeFormatter<T>();
        }
    }

    public class Utf8JsonWebApiService<T>: WebApiService
        where T: class, new()
    {
        public Utf8JsonWebApiService(int port)
            : base(port)
        {
        }

        public override MediaTypeFormatter GetMediaTypeFormatter()
        {
            return new Utf8JsonMediaTypeFormatter<T>();
        }
    }

    public class ZeroFormatterWebApiService<T>: WebApiService
        where T: class, new()
    {
        public ZeroFormatterWebApiService(int port)
            : base(port)
        {
        }

        public override MediaTypeFormatter GetMediaTypeFormatter()
        {
            return new ZeroFormatterMediaTypeFormatter<T>();
        }
    }
}
