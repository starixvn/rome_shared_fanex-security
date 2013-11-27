using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Http;
using System.Web.Routing;
using CaptchaMvc.Infrastructure;
using CaptchaMvc.Interface;
using Fanex.Security.Captcha.Controllers;

namespace Fanex.Security.Captcha.App_Start
{
    public class AreaRegistrator : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SpamFilter";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(AreaName, context);

            Register(GlobalConfiguration.Configuration);

            InitSylouCaptcha();
            
        }

        public static void Register(HttpConfiguration config)
        {
           
        }

        private static void InitSylouCaptcha()
        {
            var defaultCaptchaManager = (DefaultCaptchaManager)CaptchaMvc.Infrastructure.CaptchaUtils.CaptchaManager;
            defaultCaptchaManager.ImageUrlFactory = (helper, pair) => ImageUrlFactory(defaultCaptchaManager, helper, pair);
            defaultCaptchaManager.RefreshUrlFactory = RefreshUrlFactory;
        }

        private static string RefreshUrlFactory(UrlHelper urlHelper, KeyValuePair<string, ICaptchaValue> keyValuePair)
        {
            return urlHelper.Action("Refresh", "DefaultCaptcha", new { area = "" });
        }

        private static string ImageUrlFactory(DefaultCaptchaManager captchaManager, UrlHelper urlHelper, KeyValuePair<string, ICaptchaValue> keyValuePair)
        {
            return urlHelper.Action("Generate", "DefaultCaptcha",
                                    new RouteValueDictionary
                                        {
                                            {"area", ""},
                                            {captchaManager.TokenParameterName, keyValuePair.Key}
                                        });
        }
    }
}