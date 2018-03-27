using System;
using System.Collections.Generic;

using MessagePack;

using ZeroFormatter;

namespace WcfVsWebApiVsAspNetCoreBenchmark
{
    public class LargeItem
    {
        public Guid OrderId { get; set; }

        public ulong OrderNumber { get; set; }

        public string EmailAddress { get; set; }

        public Address ShippingAddress { get; set; } = new Address();

        public Address InvoiceAddress { get; set; } = new Address();

        public DateTimeOffset RequestedDeliveryDate { get; set; }

        public decimal ShippingCosts { get; set; }

        public DateTimeOffset LastModified { get; set; }

        public Guid CreateNonce { get; set; }

        public List<OrderLine> OrderLines { get; set; }
    }

    public class OrderLine
    {
        public string Sku { get; set; }

        public int Quantity { get; set; }

        public string Product { get; set; }

        public decimal Price { get; set; }
    }

    public class Address
    {
        public string Name { get; set; }

        public string Street { get; set; }

        public string HouseNumber { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }

        public string Country { get; set; }
    }
}
