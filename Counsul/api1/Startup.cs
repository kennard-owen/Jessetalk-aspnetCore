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

namespace api1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env,IApplicationLifetime lifetime)
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
            client.Agent.ServiceDeregister("servicename:14295");
        }
        private void OnStart()
        {
            var client = new ConsulClient();
            var httpCheck = new AgentServiceCheck()
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                Interval = TimeSpan.FromSeconds(30),
                HTTP = "http://127.0.0.1:14295/HealthCheck" //api1 的端口
            };
            var agentReg = new AgentServiceRegistration()
            {
                ID = "servicename:14295",
                Check = httpCheck,
                Address = "127.0.0.1",
                Name = "servicename",
                Port = 14295
            };
            client.Agent.ServiceRegister(agentReg).ConfigureAwait(false);
        }
    }
}
