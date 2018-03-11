using System;
using System.Collections.Generic;

using MessagePack;

namespace AspNetCoreWcfBenchmark
{
    [MessagePackObject]
    public class LargeItem
    {
        [Key(0)]
        public Guid OrderId { get; set; }

        [Key(1)]
        public ulong OrderNumber { get; set; }

        [Key(2)]
        public string EmailAddress { get; set; }

        [Key(3)]
        public Address ShippingAddress { get; set; } = new Address();

        [Key(4)]
        public Address InvoiceAddress { get; set; } = new Address();

        [Key(5)]
        public DateTimeOffset RequestedDeliveryDate { get; set; }

        [Key(6)]
        public decimal ShippingCosts { get; set; }

        [Key(7)]
        public DateTimeOffset LastModified { get; set; }

        [Key(8)]
        public Guid CreateNonce { get; set; }

        [Key(9)]
        public List<OrderLine> OrderLines { get; set; }
    }

    [MessagePackObject]
    public class OrderLine
    {
        [Key(0)]
        public string Sku { get; set; }

        [Key(1)]
        public int Quantity { get; set; }

        [Key(2)]
        public string Product { get; set; }

        [Key(3)]
        public decimal Price { get; set; }
    }

    [MessagePackObject]
    public class Address
    {
        [Key(0)]
        public string Name { get; set; }

        [Key(1)]
        public string Street { get; set; }

        [Key(2)]
        public string HouseNumber { get; set; }

        [Key(3)]
        public string PostalCode { get; set; }

        [Key(4)]
        public string City { get; set; }

        [Key(5)]
        public string Country { get; set; }
    }
}
