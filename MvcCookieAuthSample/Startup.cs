using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MvcCookieAuthSample.Data;
using MvcCookieAuthSample.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;

using IdentityServer4.EntityFramework;
using MvcCookieAuthSample.Services;
using IdentityServer4.Services;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

namespace MvcCookieAuthSample
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
            const string connectionString = @"Data Source=192.168.208.135;database=IdentityServer4.EntityFramework-2.0.0;User ID=sa; Password=2+2=si;";
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            {
                services.AddDbContext<ApplicationDbContext>(optins =>
                {
                    optins.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                });
                services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();
                services.AddIdentityServer()//添加AddIdentityServer
                    .AddDeveloperSigningCredential()//添加一个开发的证书
                                                    //在启动时创建临时密钥材料。这是在没有证书的情况下仅用于开发的场景。
                                                    //生成的密钥将被保存到文件系统，以便在服务器重新启动时保持稳定
                                                    //（可以通过传递false禁用）。当客户机/ API元数据缓存在开发过程中不同步时，
                    //.AddInMemoryClients(Config.GetClients())//添加客户端
                    //.AddInMemoryApiResources(Config.GetrResources())//基于在apiresource配置对象内存回收iresourcestore实现寄存器。
                    //.AddInMemoryIdentityResources(Config.GetIdentityResources())//基于在identityresource配置对象内存回收iresourcestore实现寄存器。
                    .AddConfigurationStore(options => {
                        options.ConfigureDbContext = builder =>
                        {
                            builder.UseSqlServer(connectionString, sql =>
                            {
                                sql.MigrationsAssembly(migrationsAssembly);
                            }
                            );
                        };
                    })
                    .AddOperationalStore(options => {
                        options.ConfigureDbContext = builder =>
                        {
                            builder.UseSqlServer(
                                connectionString,
                                sql =>
                                {
                                    sql.MigrationsAssembly(migrationsAssembly);
                                });
                        };
                    })
                    .AddAspNetIdentity<ApplicationUser>()
                    .Services.AddScoped<IProfileService, ProfileService>();
                //.AddTestUsers(Config.GetTestUsers());//添加TestUer
                //IdentityServer4.EntityFramework.DbContexts.PersistedGrantDbContext
                //Add-Migration init -Context ConfigurationDbContext -OutputDir Data/Migrations//IdentityServer/ConfigurationDb
            }

            //services.AddDbContext<ApplicationDbContext>(optins =>
            //{
            //    optins.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            //});
            //services.AddIdentity<ApplicationUser, ApplicationRole>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;

            });
            ////services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            ////Cookie授权
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //        .AddCookie(options =>
            //        {
            //            options.LoginPath = "/Account/Login";
            //        });
            services.AddScoped<ConsentServices>();
            services.AddMvc();
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
            InitIdentityServerDataBase(app);
            app.UseStaticFiles();
            app.UseIdentityServer();
            //app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        public void InitIdentityServerDataBase(IApplicationBuilder app)
        {
            using (var scope= app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                if (!configurationDbContext.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        configurationDbContext.Clients.Add(client.ToEntity());
                    }
                    configurationDbContext.SaveChanges();
                }
                if (!configurationDbContext.ApiResources.Any())
                {
                    foreach (var api in Config.GetApiResources())
                    {
                        configurationDbContext.ApiResources.Add(api.ToEntity());
                    }
                    configurationDbContext.SaveChanges();
                }
                if (!configurationDbContext.IdentityResources.Any())
                {
                    foreach (var identity in Config.GetIdentityResources())
                    {
                        configurationDbContext.IdentityResources.Add(identity.ToEntity());
                    }
                    configurationDbContext.SaveChanges();
                }
            }
        }
    }
}
