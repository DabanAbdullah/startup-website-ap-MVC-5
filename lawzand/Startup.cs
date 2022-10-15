using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(lawzand.Startup))]
namespace lawzand
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();

            ConfigureAuth(app);
        }
    }
}
