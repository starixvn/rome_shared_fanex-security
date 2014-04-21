// ***********************************************************************
// Assembly         : Fanex.Security.Captcha
// Author           : Steven.Nguyen
// Created          : 04-18-2014
//
// Last Modified By : Steven.Nguyen
// Last Modified On : 04-18-2014
// ***********************************************************************
// <copyright file="CaptchaController.cs" company="Nexcel">
//     Copyright (c) Nexcel. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Fanex.Security.Captcha.Controllers
{
    using System.Web.Mvc;

    /// <summary>
    /// Class CaptchaController.
    /// </summary>
    public class CaptchaController : BaseController
    {
        /// <summary>
        /// The _captcha controller.
        /// </summary>
        private readonly BaseCaptchaController _captchaController;

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptchaController"/> class.
        /// </summary>
        public CaptchaController()
        {
            _captchaController = new SylouCaptchaController();
        }

        /// <summary>
        /// Pages the access captcha.
        /// </summary>
        /// <param name="rule">The customer rule.</param>
        /// <returns>Action Result.</returns>
        public ActionResult PageAccessCaptcha(string rule)
        {
            return _captchaController.GenerateCaptcha(rule);
        }

        /// <summary>
        /// Renders the captcha script.
        /// </summary>
        /// <param name="rule">The customer rule.</param>
        /// <param name="mode">The captcha mode.</param>
        /// <returns>JavaScript Result.</returns>
        public JavaScriptResult RenderCaptchaScript(string rule, BaseCaptchaController.CaptchaMode mode)
        {
            return _captchaController.RenderCaptchaScript(this, rule, mode);
        }

        /// <summary>
        /// Checks the captcha.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <returns>Action Result.</returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult CheckCaptcha(string pageName)
        {
            return _captchaController.CheckCaptcha(pageName, HttpContext, RouteData, User.Identity.Name);
        }
    }
}
