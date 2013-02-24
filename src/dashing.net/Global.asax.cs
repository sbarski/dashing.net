using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using dashing.net.App_Start;

namespace dashing.net
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            RerouteWidgetRequest();
        }

        private void RerouteWidgetRequest()
        {
            var url = Request.Path.ToLower();
            
            if (url.StartsWith("/views/") && url.EndsWith(".html"))
            {
                var widget = url.Split(new[] {'/'}).Last().Split(new[] {'.'}).First();

                //check if there is a widget under that name in the widgets directory
                if (File.Exists(Server.MapPath(string.Format("~/Widgets/{0}/{0}.html", widget))))
                {
                    var context = HttpContext.Current;
                    context.RewritePath(string.Format("~/Widgets/{0}/{0}.html", widget));
                }
            }
        }
    }
}