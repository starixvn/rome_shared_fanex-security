using System;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Routing;
using Fanex.Security.Captcha.ViewModels;

namespace Fanex.Security.Captcha.Controllers
{
    public class BaseCaptchaController : BaseController
    {
       
        public enum CaptchaMode
        {
            Block = 1, Captcha = 2
        }


        public JavaScriptResult RenderCaptchaScript(Controller controller, string rule, CaptchaMode mode)
        {

            rule = rule ?? string.Empty;
            var viewName = mode.Equals(CaptchaMode.Captcha) ? "~/Areas/SpamFilter/Views/Captcha/PageAccess/CaptchaJS.cshtml" : "~/Areas/SpamFilter/Views/Captcha/PageAccess/Block.cshtml";


            var captchaScript = Render(controller, viewName, new PageInfo { PageName = rule });
            return JavaScript(captchaScript);
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult CheckCaptcha(string pageName, HttpContextBase context, RouteData routeData, string userName)
        {
            throw new NotImplementedException();
        }       

        public virtual ActionResult GenerateCaptcha(string rule)
        {
            throw new NotImplementedException();
        }


        public virtual bool Available()
        {
            return true; 
        }

        private string Render(Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

    }
}
