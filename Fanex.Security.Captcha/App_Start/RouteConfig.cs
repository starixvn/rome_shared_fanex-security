using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace Fanex.Security.Captcha.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(string areaName, AreaRegistrationContext context)
        {
            var controllerName = Assembly.GetExecutingAssembly().GetName().Name + ".Controllers";
            /*context.MapRoute(
               areaName  "_Default",
                "/{action}/{id}",
                new { controller = "Home", action = "Index", apiId = UrlParameter.Optional }, new[] { controllerName });
             * 
             * */

            context.MapRoute(
                areaName + "_Default",
                areaName + "/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { controllerName });
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}