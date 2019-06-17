using Microsoft.Owin;
using Owin;

using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SampleWebApp.Infrastructure;

[assembly: OwinStartupAttribute(typeof(SampleWebApp.Startup))]
namespace SampleWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);           
        }
    }
}
