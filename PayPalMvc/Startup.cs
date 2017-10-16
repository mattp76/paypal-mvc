using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PayPalMvc.Startup))]
namespace PayPalMvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
