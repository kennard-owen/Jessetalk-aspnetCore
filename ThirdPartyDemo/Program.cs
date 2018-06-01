using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;


namespace ThirdPartyDemo
{
    class Program
    {
        static  void Main(string[] args)
        {
            ///密码模式IdentityServer4
            var diso = DiscoveryClient.GetAsync("http://localhost:5001").Result;
            if (diso.IsError)
            {
                Console.WriteLine(diso.Error);
            }
            var tokenClient = new TokenClient(diso.TokenEndpoint, "client0", "secret0");
            var tokenResponse = tokenClient.RequestClientCredentialsAsync("793087382").Result;
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
            }
            else
            {
                Console.WriteLine(tokenResponse.Json);
            }
            var httpClicnt = new HttpClient();
            httpClicnt.SetBearerToken(tokenResponse.AccessToken);
            var response = httpClicnt.GetAsync("http://localhost:5000/api/values").Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            }
            Console.ReadLine();
        }
    }
}
