using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace MvcClient03
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddAuthentication(option => {
                option.DefaultScheme = "Cookies";
                option.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", option => {
                option.SignInScheme = "Cookies";
                option.Authority = "http://localhost:5002";
                option.RequireHttpsMetadata = false;
                option.ResponseType = OpenIdConnectResponseType.CodeIdTokenToken;//取access_token 再次请求
                option.ClientId = "mvc";
                option.ClientSecret = "secret";
                option.SaveTokens = true;
                ////方法一 两次请求
                {
                    option.GetClaimsFromUserInfoEndpoint = true;
                    option.ClaimActions.MapJsonKey("sub", "sub");
                    option.ClaimActions.MapJsonKey("preferred_username", "preferred_username");
                    option.ClaimActions.MapJsonKey("avatar", "avatar");
                    option.ClaimActions.MapCustomJson("role", jobj => jobj["role"].ToString());
                    //PostMan http://localhost:5002/connect/userinfo  [{"key":"Authorization","value":"Bearer "}]
                }
                option.Scope.Add("offline_access");
                option.Scope.Add("openid");
                option.Scope.Add("profile");

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
