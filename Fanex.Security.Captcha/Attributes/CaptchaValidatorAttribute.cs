// ***********************************************************************
// Assembly         : Fanex.Security.Captcha
// Author           : Steven.Nguyen
// Created          : 04-18-2014
//
// Last Modified By : Steven.Nguyen
// Last Modified On : 04-18-2014
// ***********************************************************************
// <copyright file="CaptchaValidatorAttribute.cs" company="Nexcel">
//     Copyright (c) Nexcel. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Fanex.Security.Captcha.Attributes
{
    using System.Web.Mvc;
    using Fanex.Security.Captcha.Configurations;

    /// <summary>
    /// Class CaptchaValidatorAttribute.
    /// </summary>
    public class CaptchaValidatorAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// The challeng e_ fiel d_ key.
        /// </summary>
        private const string CHALLENGEFIELDKEY = "recaptcha_challenge_field";

        /// <summary>
        /// The respons e_ fiel d_ key.
        /// </summary>
        private const string RESPONSEFIELDKEY = "recaptcha_response_field";

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var captchaChallengeValue = filterContext.HttpContext.Request.Form[CHALLENGEFIELDKEY];
            var captchaResponseValue = filterContext.HttpContext.Request.Form[RESPONSEFIELDKEY];

            var captchaValidtor = new Recaptcha.RecaptchaValidator
                                      {
                                          PrivateKey = AppSettings.RecaptchaPrivateKey,
                                          RemoteIP = filterContext.HttpContext.Request.UserHostAddress,
                                          Challenge = captchaChallengeValue,
                                          Response = captchaResponseValue
                                      };

            var recaptchaResponse = captchaValidtor.Validate();

            // this will push the result value into a parameter in our Action  
            filterContext.ActionParameters["captchaValid"] = recaptchaResponse.IsValid;

            base.OnActionExecuting(filterContext);
        }
    }
}
