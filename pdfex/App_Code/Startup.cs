using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(pdfex.Startup))]
namespace pdfex
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
