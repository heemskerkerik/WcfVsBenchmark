using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    public abstract class WcfService<T>
        where T: class, new()
    {
        public virtual void Start()
        {
            _host = new WebServiceHost(typeof(WcfServiceImpl), new Uri(Uri));
            AddServiceEndPoint();

            _clientFactory = CreateClientFactory();
            _host.Open();

            _client = _clientFactory();
        }

        protected virtual string Uri => $"http://localhost:{Port}/";

        private void AddServiceEndPoint()
        {
            var binding = CreateBinding();
            var endpoint = AddServiceEndpoint();
            ConfigureEndpoint(endpoint);

            ServiceEndpoint AddServiceEndpoint()
            {
                return _host.AddServiceEndpoint(typeof(IWcfService<T>), binding, Uri);
            }
        }

        protected abstract Binding CreateBinding();

        protected virtual void ConfigureEndpoint(ServiceEndpoint endpoint)
        {
        }

        private Func<WcfServiceClient<T>> CreateClientFactory()
        {
            var endpointAddress = new EndpointAddress(Uri);
            var binding = CreateBinding();

            return () =>
                   {
                       var client = new WcfServiceClient<T>(binding, endpointAddress);
                       ConfigureClient(client);

                       return client;
                   };
        }

        protected virtual void ConfigureClient(WcfServiceClient<T> client)
        {
        }

        public T[] Invoke()
        {
            return _client.Operation(_itemsToSend, _itemCountToRequest);
        }

        public void Stop()
        {
            _client?.Close();
            _host.Close();
        }

        protected WcfService(int port, int itemCount)
        {
            Port = port;

            _itemsToSend = Cache.Get<T>().Take(itemCount).ToArray();
            _itemCountToRequest = itemCount;
        }

        protected int Port { get; }

        private readonly T[] _itemsToSend;
        private readonly int _itemCountToRequest;
        private WebServiceHost _host;
        private Func<WcfServiceClient<T>> _clientFactory;
        private WcfServiceClient<T> _client;
    }

    public class WcfServiceClient<T>: ClientBase<IWcfService<T>>, IWcfService<T>
    {
        public WcfServiceClient(Binding binding, EndpointAddress endpointAddress)
            : base(binding, endpointAddress)
        {
        }

        public T[] Operation(T[] items, int itemCount)
        {
            return Channel.Operation(items, itemCount);
        }
    }

    [ServiceContract]
    public interface IWcfService<T>
    {
        [OperationContract]
        T[] Operation(T[] items, int itemCount);
    }

    public class WcfServiceImpl: IWcfService<SmallItem>, IWcfService<LargeItem>
    {
        public SmallItem[] Operation(SmallItem[] items, int itemCount)
        {
            return Cache.SmallItems.Take(itemCount).ToArray();
        }

        public LargeItem[] Operation(LargeItem[] items, int itemCount)
        {
            return Cache.LargeItems.Take(itemCount).ToArray();
        }
    }

    public class TextWcfService<T>: WcfService<T>
        where T: class, new()
    {
        public TextWcfService(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override Binding CreateBinding()
        {
            return new BasicHttpBinding
                   {
                       MessageEncoding = WSMessageEncoding.Text,
                       MaxReceivedMessageSize = 1024 * 1024 * 1024,
                   };
        }
    }

    public abstract class WebWcfServiceBase<T>: WcfService<T>
        where T: class, new()
    {
        private WebHttpBehavior _behavior;

        protected WebWcfServiceBase(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected abstract WebMessageFormat MessageFormat { get; }

        public override void Start()
        {
            _behavior = new WebHttpBehavior
                        {
                            DefaultOutgoingRequestFormat = MessageFormat,
                            DefaultOutgoingResponseFormat = MessageFormat,
                            DefaultBodyStyle = WebMessageBodyStyle.Wrapped,
                        };

            base.Start();
        }

        protected override Binding CreateBinding()
        {
            return new WebHttpBinding { MaxReceivedMessageSize = 1024 * 1024 * 1024, };
        }

        protected override void ConfigureEndpoint(ServiceEndpoint endpoint)
        {
            endpoint.Behaviors.Add(_behavior);
        }

        protected override void ConfigureClient(WcfServiceClient<T> client)
        {
            client.ChannelFactory.Endpoint.Behaviors.Add(_behavior);
        }
    }

    public class WebJsonWcfService<T>: WebWcfServiceBase<T>
        where T: class, new()
    {
        public WebJsonWcfService(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override WebMessageFormat MessageFormat => WebMessageFormat.Json;
    }

    public class WebXmlWcfService<T>: WebWcfServiceBase<T>
        where T: class, new()
    {
        public WebXmlWcfService(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override WebMessageFormat MessageFormat => WebMessageFormat.Xml;
    }

    public class BinaryMessageEncodingWcfService<T>: WcfService<T>
        where T: class, new()
    {
        public BinaryMessageEncodingWcfService(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override Binding CreateBinding()
        {
            return new CustomBinding(
                new BinaryMessageEncodingBindingElement(),
                new HttpTransportBindingElement
                {
                    MaxReceivedMessageSize = 1024 * 1024 * 1024,
                }
            );
        }
    }

    public class NetTcpWcfService<T>: WcfService<T>
        where T: class, new()
    {
        public NetTcpWcfService(int port, int itemCount)
            : base(port, itemCount)
        {
        }

        protected override Binding CreateBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { MaxReceivedMessageSize = 1024 * 1024 * 1024, };
        }

        protected override string Uri => $"net.tcp://localhost:{Port}";
    }
}
