using Microsoft.Owin;
using Owin;

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