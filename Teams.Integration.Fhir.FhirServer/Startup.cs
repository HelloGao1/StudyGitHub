using Hl7.Fhir.WebApi;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Teams.Integration.Fhir.FhirServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host.
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config, new CdrService()); // this is from the actual WebAPI Project
            appBuilder.UseWebApi(config);
        }
    }
}
