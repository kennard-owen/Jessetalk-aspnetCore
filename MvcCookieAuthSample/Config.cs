using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using System.Security.Claims;

namespace MvcCookieAuthSample
{
    public class Config
    {
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client{
                    ClientId="mvc",
                    ClientName="Mvc Client",
                    ClientUri="http://loacalhost:5003",
                    LogoUri="https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1518157900003&di=40591ce365b775a870e10444d2fad18f&imgtype=0&src=http%3A%2F%2Fawb.img.xmtbang.com%2Fimg%2Fuploadnew%2F201507%2F06%2F3b5d45fde92d4042ae6d59ff05aded0f.jpg",
                    //AllowedGrantTypes=GrantTypes.Implicit,//隐士匿名
                    AllowedGrantTypes=GrantTypes.Hybrid,//指定允许客户机使用的授予类型。使用granttypes类常见组合。
                    ClientSecrets=new List<Secret> {new Secret("secret".Sha256())},
                    AllowOfflineAccess=true,//这是否可以指定客户请求刷新tokens（be the requesting _脱机访问范围）
                    AllowAccessTokensViaBrowser=true,//指定此客户端是否允许通过浏览器接收访问令牌。这是哈登流动，
                                                    //允许多个响应类型有用（例如禁止混合流的客户端应该使用代码id_token添加令牌的响应类型，
                                                    //从而泄露令牌到浏览器
                    AllowRememberConsent=true,//指定用户是否可以选择存储同意决定。默认为true
                    //RequireConsent=true,//是否同意应该提醒A屏幕是必需的。缺省为True。
                    AlwaysIncludeUserClaimsInIdToken=false,//控制Id Token参数 当请求一个ID令牌和令牌，应用户的要求总是被添加到不需要客户端使用的端点身份令牌。默认为false。
                    AllowedScopes={
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api"//"api"
                    },//默认情况下，客户端无法访问任何资源——通过添加相应的范围名称来指定允许的资源
                    RedirectUris={ "http://localhost:5003/signin-oidc"},//客户端地址
                    PostLogoutRedirectUris={ "http://localhost:5003/signout-callback-oidc" },//退出的时候返回的地址
                    
                }
            };
        }
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>{
                new ApiResource("api","My Api")
            };
        }
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource> {
                   new IdentityResources.OpenId(),
                   //new IdentityResources.Email(),
                   new IdentityResources.Profile()
            };
        }
        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>{
                new TestUser{

                    SubjectId="10000",
                    Username="Anker",
                    
                    Password="123456",
                    Claims=new List<Claim>()
                    {
                       new Claim("name","Anker1"),
                       new Claim("name2","Anker2"),
                       new Claim("website","aaaaaaaaaaaa")
                    }
                }

            };
        }
    }
}
