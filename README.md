# WcfVsBenchmark
A benchmark between WCF and other .NET web frameworks

## Comparisons
This benchmark compares the response times of [WCF](https://docs.microsoft.com/en-us/dotnet/framework/wcf/whats-wcf), ASP.NET Web API and ASP.NET Core under various workloads.

It spins up a number of web servers, and measures the duration of a round-trip where a number of items of varying complexity are sent to and received from the web server.

The items are serialized and deserialized using different frameworks, namely [Newtonsoft.Json](https://www.newtonsoft.com/json), [MessagePack](https://github.com/neuecc/MessagePack-CSharp/), [XmlSerializer](https://docs.microsoft.com/en-us/dotnet/api/system.xml.serialization.xmlserializer?view=netframework-4.7.1), and [Utf8Json](https://github.com/neuecc/Utf8Json).

## Running
To run this benchmark, you might have to start it running as Administrator, because the WCF bits use HTTP.sys.