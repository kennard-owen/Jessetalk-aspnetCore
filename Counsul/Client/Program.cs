using DnsClient;
using System;
using System.Net;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var dnsQuery = new LookupClient(IPAddress.Parse("127.0.0.1"), 8600);
            var result = dnsQuery.ResolveService("service.consul", "servicename");
            
            Console.ReadLine();
        }
    }
}
