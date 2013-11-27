using System;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Web.Routing;
using Fanex.Security.Captcha.Configurations;

namespace Fanex.Security.Captcha.Controllers
{
    public class GoogleCaptchaController : BaseCaptchaController
    {

        public override bool Available()
        {
            
            try
            {
                var request = WebRequest.Create("http://www.google.com");
                var response = (HttpWebResponse)request.GetResponse();
                //if (response == null || response.StatusCode != HttpStatusCode.OK || request.RequestUri != response.ResponseUri)
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public override ActionResult GenerateCaptcha(string rule)
        {
            return View("~/Areas/SpamFilter/Views/Captcha/PageAccess/GoogleCaptcha.cshtml");            
        }



        public override ActionResult CheckCaptcha(string pageName, HttpContextBase context, RouteData routeData, string userName)
        {
            bool validCaptcha = false;
            try
            {
                validCaptcha = ValidCaptcha(context);

                if (validCaptcha)
                {
                    var captCha = context.Request.Form["recaptcha_response_field"];
                }
            }
            catch (Exception ex)
            {
                validCaptcha = false;
            }

            return Json(validCaptcha);
        }


        private const string CHALLENGE_FIELD_KEY = "recaptcha_challenge_field";
        private const string RESPONSE_FIELD_KEY = "recaptcha_response_field";

        

        private bool ValidCaptcha(HttpContextBase context)
        {
            var captchaChallengeValue = context.Request.Form[CHALLENGE_FIELD_KEY];
            var captchaResponseValue = context.Request.Form[RESPONSE_FIELD_KEY];

        
       
            var captchaValidtor = new Recaptcha.RecaptchaValidator
                                      {
                                          PrivateKey = AppSettings.RecaptchaPrivateKey,
                                          RemoteIP = context.Request.UserHostAddress,
                                          Challenge = captchaChallengeValue,
                                          Response = captchaResponseValue
                                      };

            var recaptchaResponse = captchaValidtor.Validate();

            return recaptchaResponse.IsValid;
        }



    }
}
