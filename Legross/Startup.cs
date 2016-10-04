using Microsoft.Owin;
using Owin;
using Legross;
[assembly: OwinStartup(typeof(Legross.Startup))]
namespace Legross
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}