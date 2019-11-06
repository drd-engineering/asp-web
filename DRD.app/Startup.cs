using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DRD.Web.Startup))]
namespace DRD.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
