using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.HtmlHelpers;
using System.Web.Routing;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Interface;
using Fanex.Security.Captcha.ViewModels;
using Fanex.Security.SpamFilter;
using System;


namespace Fanex.Security.Captcha.Controllers
{
    public class SylouCaptchaController : BaseCaptchaController
    {

        public override ActionResult GenerateCaptcha(string rule)
        {
            ViewBag.Success = false;

            ViewBag.Rule = rule;

          

            return View("~/Areas/SpamFilter/Views/Captcha/PageAccess/SylouCaptcha.cshtml");
        }

        //
        // GET: /CaptchaMVC/
        //[CaptchaVerify("Captcha is not valid.")]
        public override ActionResult CheckCaptcha(string rule, HttpContextBase context, RouteData routeData, string userName)
        {
            try
            {
                rule = !string.IsNullOrEmpty(rule) ? rule : context.Request["rule"];
                ViewBag.Rule = rule;
                this.ControllerContext = new ControllerContext(context, routeData, this);
                if (!this.IsCaptchaValid("Captcha is not valid"))
                {
                    ViewBag.Error = "Not valid";
                    ViewBag.Success = false;
                    
                }
                else
                {
                    ViewBag.Success = true;

                    System.Web.HttpContext.Current.SpamFilterResetRule(rule);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Not valid";
                ViewBag.Success = false;
            }

            return View("~/Areas/SpamFilter/Views/Captcha/PageAccess/SylouCaptcha.cshtml");
        }

        

    }
}
