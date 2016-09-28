using Microsoft.Owin;
using Owin;
using Legross;

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