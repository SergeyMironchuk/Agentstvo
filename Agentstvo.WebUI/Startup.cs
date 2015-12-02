using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Agentstvo.WebUI.Startup))]
namespace Agentstvo.WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
