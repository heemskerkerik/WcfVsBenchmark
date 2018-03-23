using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    public class WcfService<T>
        where T: class, new()
    {
        public void Start()
        {
            _host = new WebServiceHost(typeof(WcfServiceImpl), new Uri($"http://localhost:{_port}/"));
            AddServiceEndPoint();

            _channelFactory = CreateChannelFactory();
            _host.Open();
        }

        private void AddServiceEndPoint()
        {
            switch(_bindingType)
            {
                case WcfBindingType.BasicText:
                    AddServiceEndpoint(new BasicHttpBinding { MessageEncoding = WSMessageEncoding.Text, MaxReceivedMessageSize = 1024 * 1024 * 1024 });
                    break;
                case WcfBindingType.WebXml:
                {
                    var endpoint = AddServiceEndpoint(new WebHttpBinding { MaxReceivedMessageSize = 1024 * 1024 * 1024 });
                    endpoint.Behaviors.Add(new WebHttpBehavior { DefaultOutgoingRequestFormat = WebMessageFormat.Xml, DefaultOutgoingResponseFormat = WebMessageFormat.Xml, DefaultBodyStyle = WebMessageBodyStyle.Wrapped });
                    break;
                }
                case WcfBindingType.WebJson:
                {
                    var endpoint = AddServiceEndpoint(new WebHttpBinding { MaxReceivedMessageSize = 1024 * 1024 * 1024 });
                    endpoint.Behaviors.Add(new WebHttpBehavior { DefaultOutgoingRequestFormat = WebMessageFormat.Json, DefaultOutgoingResponseFormat = WebMessageFormat.Json, DefaultBodyStyle = WebMessageBodyStyle.Wrapped });
                    break;
                }
                case WcfBindingType.BinaryMessageEncoding:
                    var binding = new CustomBinding(
                        new BinaryMessageEncodingBindingElement(),
                        new HttpTransportBindingElement
                        {
                            MaxReceivedMessageSize = 1024 * 1024 * 1024,
                        }
                    );
                    AddServiceEndpoint(binding);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ServiceEndpoint AddServiceEndpoint(Binding binding)
            {
                return _host.AddServiceEndpoint(typeof(IWcfService<T>), binding, $"http://localhost:{_port}");
            }
        }

        private ChannelFactory<IWcfService<T>> CreateChannelFactory()
        {
            switch(_bindingType)
            {
                case WcfBindingType.BasicText:
                    return CreateChannelFactory(new BasicHttpBinding { MessageEncoding = WSMessageEncoding.Text, MaxReceivedMessageSize = 1024 * 1024 * 1024 });
                case WcfBindingType.WebXml:
                {
                    var factory = CreateChannelFactory(new WebHttpBinding { MaxReceivedMessageSize = 1024 * 1024 * 1024 });
                    factory.Endpoint.Behaviors.Add(new WebHttpBehavior { DefaultOutgoingRequestFormat = WebMessageFormat.Xml, DefaultOutgoingResponseFormat = WebMessageFormat.Xml, DefaultBodyStyle = WebMessageBodyStyle.Wrapped });

                    return factory;
                }
                case WcfBindingType.WebJson:
                {
                    var factory = CreateChannelFactory(new WebHttpBinding { MaxReceivedMessageSize = 1024 * 1024 * 1024 });
                    factory.Endpoint.Behaviors.Add(new WebHttpBehavior { DefaultOutgoingRequestFormat = WebMessageFormat.Json, DefaultOutgoingResponseFormat = WebMessageFormat.Json, DefaultBodyStyle = WebMessageBodyStyle.Wrapped });

                    return factory;
                }
                case WcfBindingType.BinaryMessageEncoding:
                    var binding = new CustomBinding(
                        new BinaryMessageEncodingBindingElement(),
                        new HttpTransportBindingElement
                        {
                            MaxReceivedMessageSize = 1024 * 1024 * 1024,
                        }
                    );

                    return CreateChannelFactory(binding);

                default:
                    throw new ArgumentOutOfRangeException();
            }

            ChannelFactory<IWcfService<T>> CreateChannelFactory(Binding binding)
            {
                return new ChannelFactory<IWcfService<T>>(binding, $"http://localhost:{_port}");
            }
        }

        public T[] Invoke()
        {
            var channel = _channelFactory.CreateChannel();

            return channel.Operation(_itemsToSend, _itemCountToRequest);
        }

        public void Stop()
        {
            _channelFactory?.Close();
            _host.Close();
        }

        public WcfService(int port, WcfBindingType bindingType, int itemCount)
        {
            _port = port;
            _bindingType = bindingType;

            _itemsToSend = Cache.Get<T>().Take(itemCount).ToArray();
            _itemCountToRequest = itemCount;
        }

        private readonly int _port;
        private readonly WcfBindingType _bindingType;
        private readonly T[] _itemsToSend;
        private readonly int _itemCountToRequest;
        private WebServiceHost _host;
        private ChannelFactory<IWcfService<T>> _channelFactory;
    }

    public enum WcfBindingType
    {
        BasicText,
        WebXml,
        WebJson,
        BinaryMessageEncoding,
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
}
