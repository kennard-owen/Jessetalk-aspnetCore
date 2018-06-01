using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Consul;

namespace api2
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            lifetime.ApplicationStarted.Register(OnStart);
            lifetime.ApplicationStarted.Register(OnStopped);
            app.UseMvc();
        }
        private void OnStopped()
        {
            var client = new ConsulClient();
            client.Agent.ServiceDeregister("servicename:14362");
        }
        private void OnStart()
        {
            var client = new ConsulClient();
            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                Interval = TimeSpan.FromSeconds(30),
                HTTP = "http://127.0.0.1:14362/HealthCheck" //api1 的端口
            };
            var agentReg = new AgentServiceRegistration()
            {
                ID = "servicename:14362",
                Check = httpCheck,
                Address = "127.0.0.1",
                Name = "servicename",
                Port = 14362
            };
            client.Agent.ServiceRegister(agentReg).ConfigureAwait(false);
        }
    }
}
