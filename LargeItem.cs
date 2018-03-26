using System;
using System.Collections.Generic;

using MessagePack;

using ZeroFormatter;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    [MessagePackObject]
    [ZeroFormattable]
    public class LargeItem
    {
        [Key(0)]
        [Index(0)]
        public virtual Guid OrderId { get; set; }

        [Key(1)]
        [Index(1)]
        public virtual ulong OrderNumber { get; set; }

        [Key(2)]
        [Index(2)]
        public virtual string EmailAddress { get; set; }

        [Key(3)]
        [Index(3)]
        public virtual Address ShippingAddress { get; set; } = new Address();

        [Key(4)]
        [Index(4)]
        public virtual Address InvoiceAddress { get; set; } = new Address();

        [Key(5)]
        [Index(5)]
        public virtual DateTimeOffset RequestedDeliveryDate { get; set; }

        [Key(6)]
        [Index(6)]
        public virtual decimal ShippingCosts { get; set; }

        [Key(7)]
        [Index(7)]
        public virtual DateTimeOffset LastModified { get; set; }

        [Key(8)]
        [Index(8)]
        public virtual Guid CreateNonce { get; set; }

        [Key(9)]
        [Index(9)]
        public virtual List<OrderLine> OrderLines { get; set; }
    }

    [MessagePackObject]
    [ZeroFormattable]
    public class OrderLine
    {
        [Key(0)]
        [Index(0)]
        public virtual string Sku { get; set; }

        [Key(1)]
        [Index(1)]
        public virtual int Quantity { get; set; }

        [Key(2)]
        [Index(2)]
        public virtual string Product { get; set; }

        [Key(3)]
        [Index(3)]
        public virtual decimal Price { get; set; }
    }

    [MessagePackObject]
    [ZeroFormattable]
    public class Address
    {
        [Key(0)]
        [Index(0)]
        public virtual string Name { get; set; }

        [Key(1)]
        [Index(1)]
        public virtual string Street { get; set; }

        [Key(2)]
        [Index(2)]
        public virtual string HouseNumber { get; set; }

        [Key(3)]
        [Index(3)]
        public virtual string PostalCode { get; set; }

        [Key(4)]
        [Index(4)]
        public virtual string City { get; set; }

        [Key(5)]
        [Index(5)]
        public virtual string Country { get; set; }
    }
}
