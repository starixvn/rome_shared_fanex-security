// ***********************************************************************
// Assembly         : Fanex.Security.Captcha
// Author           : Steven.Nguyen
// Created          : 04-18-2014
//
// Last Modified By : Steven.Nguyen
// Last Modified On : 04-18-2014
// ***********************************************************************
// <copyright file="BaseCaptchaController.cs" company="Nexcel">
//     Copyright (c) Nexcel. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Fanex.Security.Captcha.Controllers
{
    using System;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Fanex.Security.Captcha.ViewModels;

    /// <summary>
    /// Class BaseCaptchaController.
    /// </summary>
    public class BaseCaptchaController : BaseController
    {
        /// <summary>
        /// Enum CaptchaMode.
        /// </summary>
        public enum CaptchaMode
        {
            /// <summary>
            /// The block.
            /// </summary>
            Block = 1, Captcha = 2
        }
        
        /// <summary>
        /// Renders the captcha script.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="rule">The customer rule.</param>
        /// <param name="mode">The captcha mode.</param>
        /// <returns>JavaScriptResult.</returns>
        public JavaScriptResult RenderCaptchaScript(Controller controller, string rule, CaptchaMode mode)
        {
            rule = rule ?? string.Empty;
            var captchaView = "~/Areas/SpamFilter/Views/Captcha/PageAccess/CaptchaJS.cshtml";
            var viewName = mode.Equals(CaptchaMode.Captcha) ? captchaView  : "~/Areas/SpamFilter/Views/Captcha/PageAccess/Block.cshtml";
            var captchaScript = Render(controller, viewName, new PageInfo { PageName = rule });
            return JavaScript(captchaScript);
        }

        /// <summary>
        /// Checks the captcha.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="context">The context.</param>
        /// <param name="routeData">The route data.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns>ActionResult.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult CheckCaptcha(string pageName, HttpContextBase context, RouteData routeData, string userName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates the captcha.
        /// </summary>
        /// <param name="rule">The customer rule.</param>
        /// <returns>Action Result.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual ActionResult GenerateCaptcha(string rule)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Availables this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool Available()
        {
            return true; 
        }

        /// <summary>
        /// Renders the specified controller.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="model">The model.</param>
        /// <returns>System.String.</returns>
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
