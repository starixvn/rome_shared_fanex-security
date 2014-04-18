// ***********************************************************************
// Assembly         : Fanex.Security.Captcha
// Author           : Steven.Nguyen
// Created          : 04-18-2014
//
// Last Modified By : Steven.Nguyen
// Last Modified On : 04-18-2014
// ***********************************************************************
// <copyright file="AreaRegistrator.cs" company="Nexcel">
//     Copyright (c) Nexcel. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Fanex.Security.Captcha.App_Start
{
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;
    using CaptchaMvc.Infrastructure;
    using CaptchaMvc.Interface;

    /// <summary>
    /// Class AreaRegistrator.
    /// </summary>
    public class AreaRegistrator : AreaRegistration
    {
        /// <summary>
        /// Gets the name of the area to register.
        /// </summary>
        /// <value>The name of the area.</value>
        /// <returns>The name of the area to register.</returns>
        public override string AreaName
        {
            get
            {
                return "SpamFilter";
            }
        }

        /// <summary>
        /// Registers an area in an ASP.NET MVC application using the specified area's context information.
        /// </summary>
        /// <param name="context">Encapsulates the information that is required in order to register the area.</param>
        public override void RegisterArea(AreaRegistrationContext context)
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(AreaName, context);

            InitSylouCaptcha();
        }
        
        /// <summary>
        /// Initializes the sylou captcha.
        /// </summary>
        private static void InitSylouCaptcha()
        {
            var defaultCaptchaManager = (DefaultCaptchaManager)CaptchaMvc.Infrastructure.CaptchaUtils.CaptchaManager;
            defaultCaptchaManager.ImageUrlFactory = (helper, pair) => ImageUrlFactory(defaultCaptchaManager, helper, pair);
            defaultCaptchaManager.RefreshUrlFactory = RefreshUrlFactory;
        }

        /// <summary>
        /// Refreshes the URL factory.
        /// </summary>
        /// <param name="urlHelper">The URL helper.</param>
        /// <param name="keyValuePair">The key value pair.</param>
        /// <returns>System String.</returns>
        private static string RefreshUrlFactory(UrlHelper urlHelper, KeyValuePair<string, ICaptchaValue> keyValuePair)
        {
            return urlHelper.Action("Refresh", "DefaultCaptcha", new { area = string.Empty });
        }

        /// <summary>
        /// Images the URL factory.
        /// </summary>
        /// <param name="captchaManager">The captcha manager.</param>
        /// <param name="url">The URL helper.</param>
        /// <param name="keyValuePair">The key value pair.</param>
        /// <returns>System.String.</returns>
        private static string ImageUrlFactory(DefaultCaptchaManager captchaManager, UrlHelper url, KeyValuePair<string, ICaptchaValue> keyValuePair)
        {
            var routeDictionary = new RouteValueDictionary
                {
                    { "area", string.Empty },
                    { captchaManager.TokenParameterName, keyValuePair.Key }
                };
            return url.Action("Generate", "DefaultCaptcha", routeDictionary);
        }
    }
}