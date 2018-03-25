﻿using System;
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
            string uri = $"http://localhost:{_port}/";

            if(_bindingType == WcfBindingType.NetTcp)
                uri = $"net.tcp://localhost:{_port}";

            _host = new WebServiceHost(typeof(WcfServiceImpl), new Uri(uri));
            AddServiceEndPoint();

            _clientFactory = CreateClientFactory();
            _host.Open();

            _client = _clientFactory();
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
                case WcfBindingType.NetTcp:
                    AddServiceEndpoint(new NetTcpBinding(SecurityMode.None) { MaxReceivedMessageSize = 1024 * 1024 * 1024 });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ServiceEndpoint AddServiceEndpoint(Binding binding)
            {
                string uri = $"http://localhost:{_port}";

                if(_bindingType == WcfBindingType.NetTcp)
                    uri = $"net.tcp://localhost:{_port}";

                return _host.AddServiceEndpoint(typeof(IWcfService<T>), binding, uri);
            }
        }

        private Func<WcfServiceClient<T>> CreateClientFactory()
        {
            string uri = $"http://localhost:{_port}";

            if(_bindingType == WcfBindingType.NetTcp)
                uri = $"net.tcp://localhost:{_port}";

            var endpointAddress = new EndpointAddress(uri);

            switch(_bindingType)
            {
                case WcfBindingType.BasicText:
                {
                    var binding = new BasicHttpBinding { MessageEncoding = WSMessageEncoding.Text, MaxReceivedMessageSize = 1024 * 1024 * 1024 };
                    return () => new WcfServiceClient<T>(binding, endpointAddress);
                }
                case WcfBindingType.WebXml:
                {
                    var binding = new WebHttpBinding { MaxReceivedMessageSize = 1024 * 1024 * 1024 };
                    var behavior = new WebHttpBehavior { DefaultOutgoingRequestFormat = WebMessageFormat.Xml, DefaultOutgoingResponseFormat = WebMessageFormat.Xml, DefaultBodyStyle = WebMessageBodyStyle.Wrapped };
                    return () =>
                           {
                               var client = new WcfServiceClient<T>(binding, endpointAddress);
                               client.ChannelFactory.Endpoint.Behaviors.Add(behavior);

                               return client;
                           };
                }
                case WcfBindingType.WebJson:
                {
                    var binding = new WebHttpBinding { MaxReceivedMessageSize = 1024 * 1024 * 1024 };
                    var behavior = new WebHttpBehavior { DefaultOutgoingRequestFormat = WebMessageFormat.Json, DefaultOutgoingResponseFormat = WebMessageFormat.Json, DefaultBodyStyle = WebMessageBodyStyle.Wrapped };
                    return () =>
                           {
                               var client = new WcfServiceClient<T>(binding, endpointAddress);
                               client.ChannelFactory.Endpoint.Behaviors.Add(behavior);

                               return client;
                           };
                }
                case WcfBindingType.BinaryMessageEncoding:
                {
                    var binding = new CustomBinding(
                        new BinaryMessageEncodingBindingElement(),
                        new HttpTransportBindingElement
                        {
                            MaxReceivedMessageSize = 1024 * 1024 * 1024,
                        }
                    );

                    return () => new WcfServiceClient<T>(binding, endpointAddress);
                }

                case WcfBindingType.NetTcp:
                {
                    var binding = new NetTcpBinding(SecurityMode.None) { MaxReceivedMessageSize = 1024 * 1024 * 1024 };
                    return () => new WcfServiceClient<T>(binding, endpointAddress);
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
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
        private Func<WcfServiceClient<T>> _clientFactory;
        private WcfServiceClient<T> _client;
    }

    public enum WcfBindingType
    {
        BasicText,
        WebXml,
        WebJson,
        BinaryMessageEncoding,
        NetTcp,
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
}
