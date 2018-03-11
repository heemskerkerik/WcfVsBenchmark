using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

using Microsoft.Owin.Hosting;

using Owin;

namespace AspNetCoreWcfBenchmark
{
    public class WebApiService<T>: RestServiceBase<T>
        where T: class, new()
    {
        public void Start()
        {
            var startup = new WebApiStartup<T>(_format);
            _app = WebApp.Start($"http://localhost:{_port}", startup.Configuration);
            InitializeClients();
        }

        public void Stop()
        {
            _app.Dispose();
        }

        public WebApiService(int port, SerializerType format, int itemCount, bool sendItems, bool receiveItems)
            : base(port, format, itemCount, sendItems, receiveItems)
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

    public class WebApiStartup<T>
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();
            config.Formatters.Clear();

            switch(_format)
            {
                case SerializerType.Xml:
                    config.Formatters.Add(new XmlMediaTypeFormatter
                                          {
                                              UseXmlSerializer = true,
                                          });
                    break;
                case SerializerType.JsonNet:
                    config.Formatters.Add(new JsonMediaTypeFormatter());
                    break;
                case SerializerType.MessagePack:
                    config.Formatters.Add(new MessagePackMediaTypeFormatter<T>());
                    break;
                case SerializerType.Utf8Json:
                    config.Formatters.Add(new Utf8JsonMediaTypeFormatter<T>());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            app.UseWebApi(config);
        }

        public WebApiStartup(SerializerType format)
        {
            _format = format;
        }

        private readonly SerializerType _format;
    }
}
