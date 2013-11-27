using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fanex.Security.Captcha.Controllers
{    
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var language = Session["culture"];

            language = language ?? "en-US";

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language.ToString());
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(language.ToString());
            base.OnActionExecuting(filterContext);
        }

    }
}
