using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QaProject.Startup))]
namespace QaProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
