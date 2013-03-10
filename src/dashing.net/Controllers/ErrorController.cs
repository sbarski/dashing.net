using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace dashing.net.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index(HttpStatusCode statusCode, Exception exception)
        {
            Response.StatusCode = (int)statusCode;

            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                {
                    return new FilePathResult("~/Views/Error/404.html", "text/html");
                }
            }

            return View();
        }
    }
}
