using System;
using System.Collections.Generic;

using MessagePack;

using ZeroFormatter;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    [ZeroFormattable]
    public class ZeroFormatterLargeItem
    {
        [Index(0)]
        public virtual Guid OrderId { get; set; }

        [Index(1)]
        public virtual ulong OrderNumber { get; set; }

        [Index(2)]
        public virtual string EmailAddress { get; set; }

        [Index(3)]
        public virtual ZeroFormatterAddress ShippingAddress { get; set; } = new ZeroFormatterAddress();

        [Index(4)]
        public virtual ZeroFormatterAddress InvoiceAddress { get; set; } = new ZeroFormatterAddress();

        [Index(5)]
        public virtual DateTimeOffset RequestedDeliveryDate { get; set; }

        [Index(6)]
        public virtual decimal ShippingCosts { get; set; }

        [Index(7)]
        public virtual DateTimeOffset LastModified { get; set; }

        [Index(8)]
        public virtual Guid CreateNonce { get; set; }

        [Index(9)]
        public virtual List<ZeroFormatterOrderLine> OrderLines { get; set; }
    }
    
    [ZeroFormattable]
    public class ZeroFormatterOrderLine
    {
        [Index(0)]
        public virtual string Sku { get; set; }

        [Index(1)]
        public virtual int Quantity { get; set; }

        [Index(2)]
        public virtual string Product { get; set; }

        [Index(3)]
        public virtual decimal Price { get; set; }
    }

    [ZeroFormattable]
    public class ZeroFormatterAddress
    {
        [Index(0)]
        public virtual string Name { get; set; }

        [Index(1)]
        public virtual string Street { get; set; }

        [Index(2)]
        public virtual string HouseNumber { get; set; }

        [Index(3)]
        public virtual string PostalCode { get; set; }

        [Index(4)]
        public virtual string City { get; set; }

        [Index(5)]
        public virtual string Country { get; set; }
    }
}
