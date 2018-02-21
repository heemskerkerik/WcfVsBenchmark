using System;
using System.Linq;
using System.Threading.Tasks;

using MessagePack;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreWcfBenchmark
{
    public class AspNetCoreService: RestServiceBase
    {
        public void Start()
        {
            var startup = new AspNetCoreStartup(_format);

            _host = new WebHostBuilder()
                   .UseKestrel()
                   .ConfigureServices(startup.ConfigureServices)
                   .Configure(startup.Configure)
                   .UseUrls($"http://localhost:{_port}")
                   .Build();
            _host.Start();

            InitializeClients();
        }

        public void Stop()
        {
            _host?.StopAsync().Wait();
        }

        public AspNetCoreService(int port, SerializerType format, int itemCount, bool sendItems, bool receiveItems)
            : base(port, format, itemCount, sendItems, receiveItems)
        {
            _port = port;
            _format = format;
        }

        private readonly int _port;
        private readonly SerializerType _format;
        private IWebHost _host;
    }

    public class AspNetCoreStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(opt =>
                            {
                                opt.InputFormatters.RemoveType<JsonPatchInputFormatter>();

                                var jsonInputFormatter = opt.InputFormatters.OfType<JsonInputFormatter>().First();
                                var jsonOutputFormatter = opt.OutputFormatters.OfType<JsonOutputFormatter>().First();

                                opt.InputFormatters.Clear();
                                opt.OutputFormatters.Clear();

                                switch(_format)
                                {
                                    case SerializerType.Xml:
                                        opt.InputFormatters.Add(new XmlSerializerInputFormatter());
                                        opt.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                                        break;
                                    case SerializerType.JsonNet:
                                        opt.InputFormatters.Add(jsonInputFormatter);
                                        opt.OutputFormatters.Add(jsonOutputFormatter);
                                        break;
                                    case SerializerType.MessagePack:
                                        opt.InputFormatters.Add(new MessagePackInputFormatter());
                                        opt.OutputFormatters.Add(new MessagePackOutputFormatter());
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }

        public AspNetCoreStartup(SerializerType format)
        {
            _format = format;
        }

        private readonly SerializerType _format;
    }

    public class AspNetCoreController: Controller
    {
        [HttpPost("api/operation")]
        public Item[] Operation([FromBody] Item[] items, int itemCount)
        {
            return Enumerable.Range(0, itemCount).Select(_ => new Item { Id = Guid.NewGuid() }).ToArray();
        }
    }

    public class MessagePackInputFormatter: InputFormatter
    {
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var items = await MessagePackSerializer.DeserializeAsync<Item[]>(context.HttpContext.Request.Body);

            return InputFormatterResult.Success(items);
        }

        public MessagePackInputFormatter()
        {
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add("application/x-msgpack");
        }
    }

    public class MessagePackOutputFormatter: OutputFormatter
    {
        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            return MessagePackSerializer.SerializeAsync(context.HttpContext.Response.Body, context.Object);
        }

        public MessagePackOutputFormatter()
        {
            SupportedMediaTypes.Clear();
            SupportedMediaTypes.Add("application/x-msgpack");
        }
    }
}
