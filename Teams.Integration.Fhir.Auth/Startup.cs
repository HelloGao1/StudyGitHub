using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Teams.Integration.Fhir.Auth
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }
        private readonly IConfiguration _config ;

        public Startup(IHostingEnvironment environment, IConfiguration config)
        {
            Environment = environment;
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string sslCertificate = "7EC897BFB2700F8710646BC0674E59D58FDA211E";// set as default value
            var appSection = _config.GetSection("SSLCertificate");
            if (appSection != null && !string.IsNullOrEmpty(appSection.Value))
            {
                sslCertificate = appSection.Value;
            }
            var builder = services.AddIdentityServer()
                 .AddInMemoryIdentityResources(Config.GetIdentityResources())
                 .AddInMemoryApiResources(Config.GetApis())
                 .AddInMemoryClients(Config.GetClients());

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {

                X509Store computerCaStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);

                try
                {
                    computerCaStore.Open(OpenFlags.ReadOnly);
                    X509Certificate2Collection certificatesInStore = computerCaStore.Certificates;
                    X509Certificate2Collection certs = certificatesInStore.Find(X509FindType.FindByThumbprint, "7EC897BFB2700F8710646BC0674E59D58FDA211E", false);

                    if (certs.Count > 0)
                        builder.AddSigningCredential(certs[0]);
                }
                finally
                {
                    computerCaStore.Close();
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            if (env.IsDevelopment() || Environment.IsEnvironment("Test"))
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
        }
    }
}
