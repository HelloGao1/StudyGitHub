using Hl7.Fhir.WebApi;
using IdentityServer3.AccessTokenValidation;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


[assembly: OwinStartup(typeof(Teams.Integration.Fhir.FhirServerWeb.Startup))]
namespace Teams.Integration.Fhir.FhirServerWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host.
            HttpConfiguration config = new HttpConfiguration();

            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            // add Authentication, define the Identity Server as the Authority
            appBuilder.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = "https://teams-fhir-auth.dapasoft.com/",
                RequiredScopes = new[] { "msteams-scope" },       
            });

            //config.MapHttpAttributeRoutes();

            Hl7.Fhir.WebApi.WebApiConfig.Register(config, new CdrService()); // this is from the actual WebAPI Project
            appBuilder.UseWebApi(config);
        }
    }
}