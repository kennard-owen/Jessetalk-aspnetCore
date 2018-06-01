using System;
using IdentityModel;
using IdentityModel.Client;
using System.Net.Http;

namespace PwdClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var diso = DiscoveryClient.GetAsync("http://localhost:5001").Result;
           if (diso.IsError)
           {
               Console.WriteLine(diso.Error);
           }
           var tokenClient = new TokenClient(diso.TokenEndpoint, "pwdClient", "secret0");
            //var tokenResponse = tokenClient.RequestClientCredentialsAsync("api").Result;
            var tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync("Anker", "123456").Result;
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
            }
            else {
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
