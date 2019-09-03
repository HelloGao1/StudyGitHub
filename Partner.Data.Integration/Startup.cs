using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Partner.Data.Integration.Startup))]
namespace Partner.Data.Integration
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //
            // Any connection or hub wire up and configuration should go here.
            //
            app.MapSignalR();
        }
    }
}
