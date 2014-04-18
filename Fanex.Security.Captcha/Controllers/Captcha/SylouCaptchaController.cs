// ***********************************************************************
// Assembly         : Fanex.Security.Captcha
// Author           : Steven.Nguyen
// Created          : 04-18-2014
//
// Last Modified By : Steven.Nguyen
// Last Modified On : 04-18-2014
// ***********************************************************************
// <copyright file="SylouCaptchaController.cs" company="Nexcel">
//     Copyright (c) Nexcel. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Fanex.Security.Captcha.Controllers
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using CaptchaMvc.HtmlHelpers;
    using Fanex.Security.SpamFilter;

    /// <summary>
    /// Class Sylou Captcha Controller.
    /// </summary>
    public class SylouCaptchaController : BaseCaptchaController
    {
        /// <summary>
        /// Generates the captcha.
        /// </summary>
        /// <param name="rule">The customer rule.</param>
        /// <returns>Action Result.</returns>
        public override ActionResult GenerateCaptcha(string rule)
        {
            ViewBag.Success = false;
            ViewBag.Rule = rule;
            return View("~/Areas/SpamFilter/Views/Captcha/PageAccess/SylouCaptcha.cshtml");
        }

        /// <summary>
        /// Checks the captcha.
        /// </summary>
        /// <param name="rule">The customer rule.</param>
        /// <param name="context">The context.</param>
        /// <param name="routeData">The route data.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns>Action Result.</returns>
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
                ViewBag.Error = ex.Message;
                ViewBag.Success = false;
            }

            return View("~/Areas/SpamFilter/Views/Captcha/PageAccess/SylouCaptcha.cshtml");
        }
    }
}
