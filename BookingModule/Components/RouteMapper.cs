using DotNetNuke.Web.Api;
using System.Web.Http;

namespace Dnn.BookingModule.BookingModule.Components
{
    public class RouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute(
                moduleFolderName: "BookingModule",
                routeName: "default",
                url: "{controller}/{action}",
                namespaces: new[] { "Dnn.BookingModule.BookingModule.Controllers" }
            );
        }
    }
}