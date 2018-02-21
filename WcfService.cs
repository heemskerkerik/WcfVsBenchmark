using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace AspNetCoreWcfBenchmark
{
    public class WcfService
    {
        public void Start()
        {
            _host = new WebServiceHost(typeof(WcfServiceImpl), new Uri($"http://localhost:{_port}/"));
            _host.AddServiceEndpoint(typeof(IWcfService), new BasicHttpBinding { MessageEncoding = _encoding, MaxReceivedMessageSize = 1024 * 1024 * 1024 }, "");

            _host.Open();
            _channelFactory = new ChannelFactory<IWcfService>(new BasicHttpBinding { MessageEncoding = _encoding, MaxReceivedMessageSize = 1024 * 1024 * 1024 }, $"http://localhost:{_port}");
        }

        public Item[] Invoke()
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

            _itemsToSend = sendItems
                               ? Enumerable.Range(0, itemCount).Select(_ => new Item { Id = Guid.NewGuid() }).ToArray()
                               : new Item[0];
            _itemCountToRequest = receiveItems ? itemCount : 0;
        }

        private readonly int _port;
        private readonly WSMessageEncoding _encoding;
        private readonly Item[] _itemsToSend;
        private readonly int _itemCountToRequest;
        private WebServiceHost _host;
        private ChannelFactory<IWcfService> _channelFactory;
    }

    [ServiceContract]
    public interface IWcfService
    {
        [OperationContract]
        Item[] Operation(Item[] items, int itemCount);
    }

    public class WcfServiceImpl: IWcfService
    {
        public Item[] Operation(Item[] items, int itemCount)
        {
            return Enumerable.Range(0, itemCount).Select(_ => new Item { Id = Guid.NewGuid() }).ToArray();
        }
    }
}
