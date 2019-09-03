using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teams.Integration.Fhir.Services.Data
{
    public abstract class BaseData
    {
        public string GetConnectionString()
        {
            Environment.GetEnvironmentVariable("enviroment");
            var env = Environment.GetEnvironmentVariable("enviroment");

            if (env == null) env = "Development";

            //return ConfigurationManager.ConnectionStrings[$"{env}_Conn"].ConnectionString;
            return ConfigurationManager.ConnectionStrings[string.Format("{0}_Conn", env)].ConnectionString;
        }
    }
}
