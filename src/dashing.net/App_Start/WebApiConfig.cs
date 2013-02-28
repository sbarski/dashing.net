using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace dashing.net
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "EventsApi",
                routeTemplate: "api/{controller}"
            );

            config.Routes.MapHttpRoute(
                name: "Widget",
                routeTemplate: "widgets/{id}",
                defaults: new { controller = "Widgets" });
        }
    }
}
