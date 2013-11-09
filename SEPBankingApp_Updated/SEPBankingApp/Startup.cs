using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SEPBankingApp.Startup))]
namespace SEPBankingApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
