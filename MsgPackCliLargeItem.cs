using System;
using System.Collections.Generic;

using MessagePack;

using MsgPack.Serialization;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    public class MsgPackCliLargeItem
    {
        [MessagePackMember(0)]
        public Guid OrderId { get; set; }

        [MessagePackMember(1)]
        public ulong OrderNumber { get; set; }

        [MessagePackMember(2)]
        public string EmailAddress { get; set; }

        [MessagePackMember(3)]
        public MessagePackAddress ShippingAddress { get; set; } = new MessagePackAddress();

        [MessagePackMember(4)]
        public MessagePackAddress InvoiceAddress { get; set; } = new MessagePackAddress();

        [MessagePackMember(5)]
        public DateTimeOffset RequestedDeliveryDate { get; set; }

        [MessagePackMember(6)]
        public decimal ShippingCosts { get; set; }

        [MessagePackMember(7)]
        public DateTimeOffset LastModified { get; set; }

        [MessagePackMember(8)]
        public Guid CreateNonce { get; set; }

        [MessagePackMember(9)]
        public List<MessagePackOrderLine> OrderLines { get; set; }
    }

    [MessagePackObject]
    public class MsgPackCliAddress
    {
        [MessagePackMember(0)]
        public string Name { get; set; }

        [MessagePackMember(1)]
        public string Street { get; set; }

        [MessagePackMember(2)]
        public string HouseNumber { get; set; }

        [MessagePackMember(3)]
        public string PostalCode { get; set; }

        [MessagePackMember(4)]
        public string City { get; set; }

        [MessagePackMember(5)]
        public string Country { get; set; }
    }

    [MessagePackObject]
    public class MsgPackCliOrderLine
    {
        [MessagePackMember(0)]
        public string Sku { get; set; }

        [MessagePackMember(1)]
        public int Quantity { get; set; }

        [MessagePackMember(2)]
        public string Product { get; set; }

        [MessagePackMember(3)]
        public decimal Price { get; set; }
    }
}
