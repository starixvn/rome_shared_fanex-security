// ***********************************************************************
// Assembly         : Fanex.Security.Captcha
// Author           : Steven.Nguyen
// Created          : 04-18-2014
//
// Last Modified By : Steven.Nguyen
// Last Modified On : 04-18-2014
// ***********************************************************************
// <copyright file="GoogleCaptchaController.cs" company="Nexcel">
//     Copyright (c) Nexcel. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Fanex.Security.Captcha.Controllers
{
    using System;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Fanex.Security.Captcha.Configurations;

    /// <summary>
    /// Class GoogleCaptchaController.
    /// </summary>
    public class GoogleCaptchaController : BaseCaptchaController
    {
        /// <summary>
        /// The challenge  key.
        /// </summary>
        private const string CHALLENGEFIELDKEY = "recaptcha_challenge_field";

        /// <summary>
        /// The responds key.
        /// </summary>
        private const string RESPONSEFIELDKEY = "recaptcha_response_field";
        
        /// <summary>
        /// Available this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool Available()
        {
            try
            {
                var request = WebRequest.Create("http://www.google.com");
                var response = (HttpWebResponse)request.GetResponse();
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Generates the captcha.
        /// </summary>
        /// <param name="rule">The customer rule.</param>
        /// <returns>Action Result.</returns>
        public override ActionResult GenerateCaptcha(string rule)
        {
            return View("~/Areas/SpamFilter/Views/Captcha/PageAccess/GoogleCaptcha.cshtml");            
        }
        
        /// <summary>
        /// Checks the captcha.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="context">The context.</param>
        /// <param name="routeData">The route data.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns>Action Result.</returns>
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
                var message = ex.Message;
                validCaptcha = false;
            }

            return Json(validCaptcha);
        }
       
        /// <summary>
        /// Valid the captcha.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool ValidCaptcha(HttpContextBase context)
        {
            var captchaChallengeValue = context.Request.Form[CHALLENGEFIELDKEY];
            var captchaResponseValue = context.Request.Form[RESPONSEFIELDKEY];
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
