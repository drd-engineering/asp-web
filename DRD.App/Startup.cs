using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DRD.App.Startup))]
namespace DRD.App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
