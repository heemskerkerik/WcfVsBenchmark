using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;

using AutoFixture;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    public class WcfService<T>
        where T: class, new()
    {
        public void Start()
        {
            _host = new WebServiceHost(typeof(WcfServiceImpl), new Uri($"http://localhost:{_port}/"));
            _host.AddServiceEndpoint(typeof(IWcfService<T>), new BasicHttpBinding { MessageEncoding = _encoding, MaxReceivedMessageSize = 1024 * 1024 * 1024 }, "");

            _host.Open();
            _channelFactory = new ChannelFactory<IWcfService<T>>(new BasicHttpBinding { MessageEncoding = _encoding, MaxReceivedMessageSize = 1024 * 1024 * 1024 }, $"http://localhost:{_port}");
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

        public WcfService(int port, WSMessageEncoding encoding, int itemCount, bool sendItems, bool receiveItems)
        {
            _port = port;
            _encoding = encoding;

            var fixture = new Fixture();

            _itemsToSend = sendItems
                               ? Cache.Get<T>().Take(itemCount).ToArray()
                               : new T[0];
            _itemCountToRequest = receiveItems ? itemCount : 0;
        }

        private readonly int _port;
        private readonly WSMessageEncoding _encoding;
        private readonly T[] _itemsToSend;
        private readonly int _itemCountToRequest;
        private WebServiceHost _host;
        private ChannelFactory<IWcfService<T>> _channelFactory;
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
