using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IdentityServerCenter
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetrResources()
        {
            return new List<ApiResource>{
                new ApiResource("793087380","My Api"),
                new ApiResource("793087381","My Api"),
                new ApiResource("793087382","My Api")
            };
        }
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client{
                    ClientId="client0",
                    AllowedGrantTypes=GrantTypes.ClientCredentials,
                    ClientSecrets=new List<Secret> {new Secret("secret0".Sha256())},
                    RequireClientSecret=false,
                    AllowedScopes={"793087382"}
                },
                new Client{
                    ClientId="pwdClient",
                    AllowedGrantTypes=GrantTypes.ResourceOwnerPassword,
                    ClientSecrets=new List<Secret> {new Secret("secret0".Sha256())},
                    RequireClientSecret=false,
                    AllowedScopes={"793087381"}
                }
            };

        }
        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>{
                new TestUser{
                    SubjectId="1",
                    Username="Anker",
                    Password="123456",
                    
                }

            };
        }
    }

}