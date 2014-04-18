// ***********************************************************************
// Assembly         : Fanex.Security.Captcha
// Author           : Steven.Nguyen
// Created          : 04-18-2014
//
// Last Modified By : Steven.Nguyen
// Last Modified On : 04-18-2014
// ***********************************************************************
// <copyright file="BaseController.cs" company="Nexcel">
//     Copyright (c) Nexcel. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Fanex.Security.Captcha.Controllers
{
    using System.Web.Mvc;

    /// <summary>
    /// Class BaseController.
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// Called before the action method is invoked.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var language = Session["culture"];

            language = language ?? "en-US";

            System.Threading.Thread.CurrentThread.CurrentUICulture =
                new System.Globalization.CultureInfo(language.ToString());
            System.Threading.Thread.CurrentThread.CurrentCulture =
                System.Globalization.CultureInfo.CreateSpecificCulture(language.ToString());
            base.OnActionExecuting(filterContext);
        }
    }
}
